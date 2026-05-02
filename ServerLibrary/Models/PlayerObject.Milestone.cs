using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    //TODO - Add to quest tracker
    //TODO - Add account based reward claim
    //TODO - Task description getter
    public partial class PlayerObject
    {
        private readonly Dictionary<int, ClientUserMilestone> _clientMilestoneCache = new();

        public bool ReceiveMilestoneUpdates;

        public void LogMilestone(MilestoneType type, long amount = 1, bool setAmount = false, CharacterInfo player = null, ItemInfo item = null, MonsterInfo monster = null, CurrencyInfo currency = null, MapRegion region = null, InstanceInfo instance = null, QuestInfo quest = null, MagicInfo magic = null)
        {
            SEnvir.LogMilestone(Character, type, amount, setAmount, player, item, monster, currency, region, instance, quest, magic);
        }

        public void CheckMilestones(UserMilestoneLog log, MilestoneType type)
        {
            // Update client milestone cache for all MilestoneInfo entries of this type.
            foreach (var info in SEnvir.MilestoneInfoList.Binding)
            {
                if (!info.Tasks.Any(x => x.Type == type)) continue;

                if (_clientMilestoneCache.TryGetValue(info.Index, out var cached) && cached.DateEarned > DateTime.Now)
                    continue;

                UpdateClientMilestoneCacheForInfo(info);
            }

            foreach (MilestoneInfo info in SEnvir.MilestoneInfoList.Binding)
            {
                if (Character.Milestones.Any(x => x.Info == info)) continue;

                if (!MatchesRequiredClass(Class, info.RequiredClass)) continue;

                // Only milestones touched by this log type can have changed, but all tasks must
                // still be complete before the milestone is awarded.
                if (!info.Tasks.Any(x => x.Type == log.Type)) continue;

                bool meetsAllTasks = true;

                foreach (var task in info.Tasks)
                {
                    var taskCount = Character.MilestoneLogs.Where(x => x.Type == task.Type &&
                                                            (task.Class == RequiredClass.None || task.Class == RequiredClass.All || MatchesRequiredClass(x.Character?.Class, task.Class)) &&
                                                            (task.Item == null || x.Item == task.Item) &&
                                                            (task.Monster == null || x.Monster == task.Monster) &&
                                                            (task.Currency == null || x.Currency == task.Currency) &&
                                                            (task.Region == null || x.Region == task.Region) &&
                                                            (task.Instance == null || x.Instance == task.Instance) &&
                                                            (task.Quest == null || x.Quest == task.Quest) &&
                                                            (task.Magic == null || x.Magic == task.Magic))
                                                            .Sum(x => x.Count);

                    switch (task.Type)
                    {      
                        case MilestoneType.Ranking: //Ranking needs to reverse the count
                            if (task.Amount > 0 && taskCount > task.Amount)
                            {
                                meetsAllTasks = false;
                            }
                            break;
                        default:
                            if (task.Amount > 0 && taskCount < task.Amount)
                            {
                                meetsAllTasks = false;
                            }
                            break;
                    }

                    if (!meetsAllTasks) break;
                }

                if (!meetsAllTasks) continue;

                UserMilestone milestone = SEnvir.UserMilestoneList.CreateNewObject();
                milestone.Character = Character;
                milestone.Info = info;
                milestone.DateEarned = Time.Now;
                Character.Milestones.Add(milestone);

                Enqueue(new S.MilestoneEarned { Index = info.Index });

                // Update cache for the earned milestone info.
                UpdateClientMilestoneCacheForInfo(info);
            }
        }

        private bool MatchesRequiredClass(MirClass? @class, RequiredClass requiredClass)
        {
            if (@class == null) return false;

            switch (@class)
            {
                case MirClass.Warrior:
                    if ((requiredClass & RequiredClass.Warrior) != RequiredClass.Warrior) return false;
                    break;
                case MirClass.Wizard:
                    if ((requiredClass & RequiredClass.Wizard) != RequiredClass.Wizard) return false;
                    break;
                case MirClass.Taoist:
                    if ((requiredClass & RequiredClass.Taoist) != RequiredClass.Taoist) return false;
                    break;
                case MirClass.Assassin:
                    if ((requiredClass & RequiredClass.Assassin) != RequiredClass.Assassin) return false;
                    break;
            }

            return true;
        }

        public void MilestoneActive(C.MilestoneActive p)
        {
            var userMilestone = Character.Milestones.FirstOrDefault(x => x.Index == p.Index);

            if (userMilestone == null) return;

            var activeMilestone = Character.Milestones.FirstOrDefault(x => x.Active == true);

            if (activeMilestone != null)
            {
                activeMilestone.Active = false;

                UpdateClientMilestoneCacheForInfo(activeMilestone.Info);
            }

            userMilestone.Active = p.Active;

            UpdateClientMilestoneCacheForInfo(userMilestone.Info);

            var items = GetClientUserMilestones(true);

            Enqueue(new S.UserMilestones { Milestones = items });

            SendChangeUpdate();
        }

        public void MilestoneClaim(C.MilestoneClaim p)
        {
            var userMilestone = Character.Milestones.FirstOrDefault(x => x.Index == p.Index);
            if (userMilestone == null) return;

            if (userMilestone.Info.Reward == null || userMilestone.Info.RewardAmount <= 0) return;

            if (userMilestone.Claimed) return;

            ItemCheck check = new ItemCheck(userMilestone.Info.Reward, userMilestone.Info.RewardAmount, UserItemFlags.Bound | UserItemFlags.Worthless, TimeSpan.Zero);

            if (CanGainItems(false, check))
            {
                UserItem item = SEnvir.CreateFreshItem(check);

                GainItem(item);

                userMilestone.Claimed = true;

                UpdateClientMilestoneCacheForInfo(userMilestone.Info);

                var items = GetClientUserMilestones(true);
                Enqueue(new S.UserMilestones { Milestones = items });
                SendChangeUpdate();

                Connection.ReceiveChatWithObservers(con => $"Milestone reward claimed: {userMilestone.Info.Reward.ItemName} x{userMilestone.Info.RewardAmount}", MessageType.System);
            }
            else
            {
                Connection.ReceiveChatWithObservers(con => string.Format("Not enough inventory space to claim milestone reward."), MessageType.System);
            }
        }

        public List<ClientUserMilestone> GetClientUserMilestones(bool updatedOnly = false)
        {
            var list = new List<ClientUserMilestone>();

            foreach (var info in SEnvir.MilestoneInfoList.Binding)
            {
                var item = GetClientUserMilestone(info);

                if (updatedOnly && item.LastSent > item.LastUpdated)
                    continue;

                item.LastSent = Time.Now;

                list.Add(item);
            }

            return list;
        }

        private void UpdateClientMilestoneCacheForInfo(MilestoneInfo info)
        {
            var userMilestone = Character.Milestones.FirstOrDefault(x => x.Info == info);

            if (userMilestone != null)
            {
                var item = new ClientUserMilestone
                {
                    Index = userMilestone.Index,
                    InfoIndex = info.Index,
                    DateEarned = userMilestone.DateEarned,
                    Active = userMilestone.Active,
                    Claimed = userMilestone.Claimed,
                    LastUpdated = Time.Now,
                    Tasks = []
                };

                foreach (var task in info.Tasks)
                {
                    var taskItem = new ClientUserMilestoneTask
                    {
                        InfoTaskIndex = task.Index,
                        Count = task.Amount
                    };

                    item.Tasks.Add(taskItem);
                }

                _clientMilestoneCache[info.Index] = item;
            }
            else
            {
                List<UserMilestoneLog> logs = new List<UserMilestoneLog>();

                var item = new ClientUserMilestone
                {
                    InfoIndex = info.Index,
                    DateEarned = DateTime.MinValue,
                    Claimed = false,
                    Active = false,
                    Tasks = [],
                    LastUpdated = Time.Now
                };

                foreach (var task in info.Tasks)
                {
                    var taskLogs = Character.MilestoneLogs.Where(x => x.Type == task.Type &&
                                                            (task.Class == RequiredClass.None || task.Class == RequiredClass.All || MatchesRequiredClass(x.Character?.Class, task.Class)) &&
                                                            (task.Item == null || x.Item == task.Item) &&
                                                            (task.Monster == null || x.Monster == task.Monster) &&
                                                            (task.Currency == null || x.Currency == task.Currency) &&
                                                            (task.Region == null || x.Region == task.Region) &&
                                                            (task.Instance == null || x.Instance == task.Instance) &&
                                                            (task.Quest == null || x.Quest == task.Quest) &&
                                                            (task.Magic == null || x.Magic == task.Magic));
                    logs.AddRange(taskLogs);

                    var count = task.Type switch
                    {
                        MilestoneType.Ranking => taskLogs.Sum(x => x.Count), //Ranking needs to show actual rank
                        _ => Math.Min(task.Amount, taskLogs.Sum(x => x.Count)),
                    };

                    var taskItem = new ClientUserMilestoneTask
                    {
                        InfoTaskIndex = task.Index,
                        Count = count
                    };

                    item.Tasks.Add(taskItem);
                }

                _clientMilestoneCache[info.Index] = item;
            }
        }

        private ClientUserMilestone GetClientUserMilestone(MilestoneInfo info)
        {
            if (_clientMilestoneCache.TryGetValue(info.Index, out var cached))
                return cached;

            UpdateClientMilestoneCacheForInfo(info);

            return _clientMilestoneCache[info.Index];
        }
    }
}
