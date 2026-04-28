using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    public partial class PlayerObject
    {
        private readonly Dictionary<(MilestoneType, int), UserMilestoneLog> _milestoneLogCache = new();

        private readonly Dictionary<int, ClientUserMilestone> _clientMilestoneCache = new();

        public bool ReceiveMilestoneUpdates;

        private static int GetSecondaryId(MilestoneType type, CharacterInfo player = null, ItemInfo item = null, MonsterInfo monster = null, CurrencyInfo currency = null, MapRegion region = null, InstanceInfo instance = null)
        {
            int index = -1;

            switch (type)
            {
                case MilestoneType.Region:
                    index = region?.Index ?? 0;
                    break;
                case MilestoneType.InstanceJoin:
                    index = instance?.Index ?? 0;
                    break;
                case MilestoneType.MonsterKilled:
                case MilestoneType.MonsterDeath:
                case MilestoneType.MonsterDamage:
                case MilestoneType.MonsterSeen:
                    index = monster?.Index ?? 0;
                    break;
                case MilestoneType.PlayerKilled:
                case MilestoneType.PlayerDeath:
                case MilestoneType.PlayerDamage:
                    index = player?.Index ?? 0;
                    break;
                case MilestoneType.ItemGained:
                case MilestoneType.ItemUsed:
                    index = item?.Index ?? 0;
                    break;
                case MilestoneType.CurrencyGain:
                    index = currency?.Index ?? 0;
                    break;
            }

            return index;
        }

        public void LogMilestone(MilestoneType type, int amount = 1, CharacterInfo player = null, ItemInfo item = null, MonsterInfo monster = null, CurrencyInfo currency = null, MapRegion region = null, InstanceInfo instance = null)
        {
            int secondaryId = GetSecondaryId(type, player, item, monster, currency, region, instance);
            var key = (type, secondaryId);

            if (!_milestoneLogCache.TryGetValue(key, out var log))
            {
                switch (type)
                {
                    case MilestoneType.Region:
                        log = Character.MilestoneLogs.FirstOrDefault(x => x.Type == type && x.Region == region);
                        break;
                    case MilestoneType.InstanceJoin:
                        log = Character.MilestoneLogs.FirstOrDefault(x => x.Type == type && x.Instance == instance);
                        break;
                    case MilestoneType.MonsterKilled:
                    case MilestoneType.MonsterDeath:
                    case MilestoneType.MonsterDamage:
                    case MilestoneType.MonsterSeen:
                        log = Character.MilestoneLogs.FirstOrDefault(x => x.Type == type && x.Monster == monster);
                        break;
                    case MilestoneType.PlayerKilled:
                    case MilestoneType.PlayerDeath:
                    case MilestoneType.PlayerDamage:
                        log = Character.MilestoneLogs.FirstOrDefault(x => x.Type == type && x.Player == player);
                        break;
                    case MilestoneType.ItemGained:
                    case MilestoneType.ItemUsed:
                        log = Character.MilestoneLogs.FirstOrDefault(x => x.Type == type && x.Item == item);
                        break;
                    case MilestoneType.CurrencyGain:
                        log = Character.MilestoneLogs.FirstOrDefault(x => x.Type == type && x.Currency == currency);
                        break;
                    default:
                        log = Character.MilestoneLogs.FirstOrDefault(x => x.Type == type);
                        break;
                }

                if (log == null)
                {
                    log = SEnvir.UserMilestoneLogList.CreateNewObject();
                    log.Character = Character;
                    log.Type = type;
                    log.Item = item;
                    log.Monster = monster;
                    log.Currency = currency;
                    log.Region = region;
                    log.Instance = instance;
                    log.Player = player;
                    log.Count = 0;
                    Character.MilestoneLogs.Add(log);
                }
                _milestoneLogCache[key] = log;
            }

            log.Count += amount;
            CheckMilestones(log);

            // Update client milestone cache for all MilestoneInfo entries of this type.
            foreach (var info in SEnvir.MilestoneInfoList.Binding.Where(i => i.Type == type))
            {
                if (_clientMilestoneCache.TryGetValue(info.Index, out var cached) && cached.DateEarned > DateTime.Now)
                    continue;

                UpdateClientMilestoneCacheForInfo(info);
            }

            //if (ReceiveMilestoneUpdates)
            //{
            //    SendMilestones(type);
            //}
        }

        private void CheckMilestones(UserMilestoneLog log)
        {
            foreach (MilestoneInfo info in SEnvir.MilestoneInfoList.Binding)
            {
                if (info.Type != log.Type) continue;
                if (Character.Milestones.Any(x => x.Info == info)) continue;

                if (!MatchesRequiredClass(info.RequiredClass)) continue;

                List<UserMilestoneLog> logs = new List<UserMilestoneLog>();
                
                bool meetsAllTasks = true;

                foreach (var task in info.Tasks)
                {
                    var taskLogs = Character.MilestoneLogs.Where(x => x.Type == info.Type &&
                                                            (task.Item == null || x.Item == task.Item) &&
                                                            (task.Monster == null || x.Monster == task.Monster) &&
                                                            (task.Currency == null || x.Currency == task.Currency) &&
                                                            (task.Region == null || x.Region == task.Region) &&
                                                            (task.Instance == null || x.Instance == task.Instance));

                    if (task.Amount > 0 && taskLogs.Sum(x => x.Count) < task.Amount)
                    {
                        meetsAllTasks = false;
                        break;
                    }

                    logs.AddRange(taskLogs);
                }

                if (!meetsAllTasks || !logs.Any()) continue;

                UserMilestone milestone = SEnvir.UserMilestoneList.CreateNewObject();
                milestone.Character = Character;
                milestone.Info = info;
                milestone.DateEarned = Time.Now;
                Character.Milestones.Add(milestone);
                Connection.ReceiveChatWithObservers(con => $"{info.Title} has been earned!", MessageType.System);

                // Update cache for the earned milestone info.
                UpdateClientMilestoneCacheForInfo(info);
            }
        }

        private bool MatchesRequiredClass(RequiredClass requiredClass)
        {
            switch (Class)
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

                foreach (var task in info.Tasks)
                {
                    var taskLogs = Character.MilestoneLogs.Where(x => x.Type == info.Type &&
                                                            (task.Item == null || x.Item == task.Item) &&
                                                            (task.Monster == null || x.Monster == task.Monster) &&
                                                            (task.Currency == null || x.Currency == task.Currency) &&
                                                            (task.Region == null || x.Region == task.Region) &&
                                                            (task.Instance == null || x.Instance == task.Instance));
                    logs.AddRange(taskLogs);
                }

                var count = logs.Sum(x => x.Count);

                var item = new ClientUserMilestone
                {
                    InfoIndex = info.Index,
                    Count = count,
                    DateEarned = DateTime.MinValue,
                    Active = false,
                    Tasks = [],
                    LastUpdated = Time.Now
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
        }

        public ClientUserMilestone GetClientUserMilestone(MilestoneInfo info)
        {
            if (_clientMilestoneCache.TryGetValue(info.Index, out var cached))
                return cached;

            UpdateClientMilestoneCacheForInfo(info);

            return _clientMilestoneCache[info.Index];
        }
    }
}
