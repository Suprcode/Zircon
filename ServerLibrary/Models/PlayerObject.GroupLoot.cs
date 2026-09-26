using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    sealed class GroupLootState
    {
        public GroupLootMode Mode;
        public bool BagEnabled;
        public bool NeedRestrictions;
        public bool AllowManualTaking;
        public readonly HashSet<ItemType> ItemTypes = new();
        public readonly HashSet<Rarity> Rarities = new();
        public readonly List<UserItem> Items = new();
        public readonly Queue<UserItem> PendingItems = new();
        public uint RoundRobinObjectID;
        public GroupLootShareState Share;
    }

    sealed class GroupLootShareState
    {
        public readonly List<UserItem> Items = new();
        public readonly List<uint> Members = new();
        public readonly Dictionary<uint, long> AwardedWeight = new();
        public readonly Dictionary<uint, GroupLootVote> Votes = new();
        public bool Instant;
        public int Position;
        public DateTime VoteExpiry;
        public UserItem CurrentItem => Position < Items.Count ? Items[Position] : null;
    }

    public partial class PlayerObject
    {
        private GroupLootState GroupLoot;

        public bool UsesGroupCurrencySharing(UserItem item)
        {
            return GroupMembers?.Count > 1 && item != null && item.UserTask == null &&
                   (item.Flags & UserItemFlags.QuestItem) != UserItemFlags.QuestItem && SEnvir.IsCurrencyItem(item.Info);
        }

        public void DistributeGroupCurrency(UserItem item)
        {
            List<PlayerObject> members = GroupMembers.ToList();
            long share = item.Count / members.Count;
            long remainder = item.Count % members.Count;
            int start = Math.Max(0, members.IndexOf(this));

            for (int i = 0; i < members.Count; i++)
            {
                long amount = share + (i < remainder ? 1 : 0);
                if (amount <= 0) continue;

                PlayerObject member = members[(start + i) % members.Count];
                UserCurrency currency = member.GetCurrency(item.Info);
                currency.Amount += amount;
                member.CurrencyChanged(currency);
                member.LogMilestone(MilestoneType.CurrencyGain, amount, currency: currency.Info);
            }

            item.SetTemporary(true);
            item.Delete();
        }

        private GroupLootState CreateGroupLootState()
        {
            GroupLootState state = new GroupLootState
            {
                Mode = Enum.IsDefined(typeof(GroupLootMode), Character.GroupLootMode) ? Character.GroupLootMode : GroupLootMode.Random,
                BagEnabled = Character.GroupLootBagEnabled,
                NeedRestrictions = Character.GroupLootNeedRestrictions,
                AllowManualTaking = Character.GroupLootAllowManualTaking,
            };

            foreach (string value in (Character.GroupLootItemTypes ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries))
                if (Enum.TryParse(value, out ItemType type) && Enum.IsDefined(typeof(ItemType), type) && type != ItemType.Nothing)
                    state.ItemTypes.Add(type);

            foreach (string value in (Character.GroupLootRarities ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries))
                if (Enum.TryParse(value, out Rarity rarity) && Enum.IsDefined(typeof(Rarity), rarity))
                    state.Rarities.Add(rarity);

            return state;
        }

        public bool UsesGroupLoot(UserItem item)
        {
            return MatchesGroupLootFilters(item) && (GroupLoot.BagEnabled || GroupLoot.Mode != GroupLootMode.FreeForAll);
        }

        private bool MatchesGroupLootFilters(UserItem item)
        {
            if (!Config.EnableGroupLoot || GroupMembers == null || GroupLoot == null || item == null) return false;
            if (item.UserTask != null || (item.Flags & UserItemFlags.QuestItem) == UserItemFlags.QuestItem) return false;
            if (item.Info.ItemEffect == ItemEffect.Experience || SEnvir.IsCurrencyItem(item.Info)) return false;

            return GroupLoot.ItemTypes.Contains(item.Info.ItemType) && GroupLoot.Rarities.Contains(item.Info.Rarity);
        }

        public bool CanAddGroupLoot(UserItem item, long count)
        {
            if (!UsesGroupLoot(item)) return false;

            if (!GroupLoot.BagEnabled)
                return GroupMembers.Any(x => CanReceiveGroupLoot(x, item));

            long weight = GroupLoot.Items.Sum(x => (long)x.Weight);
            long itemWeight = item.Info.ItemType == ItemType.Poison || item.Info.ItemType == ItemType.Amulet ? item.Info.Weight : item.Info.Weight * count;
            return weight + itemWeight <= GetGroupLootCapacity();
        }

        public void AddGroupLoot(UserItem item)
        {
            if (GroupLoot.BagEnabled)
            {
                GroupLoot.Items.Add(item);
                BroadcastGroupLootUpdate();
                return;
            }

            GroupLoot.PendingItems.Enqueue(item);
            if (GroupLoot.Share == null)
                GroupMembers[0].StartInstantGroupLootShare();
        }

        public int GetGroupLootCapacity()
        {
            if (GroupMembers == null) return 0;

            long capacity = GroupMembers.Sum(x => Math.Max(0L, (long)x.Stats[Stat.BagWeight] - x.GetInventoryBagWeight()));
            return (int)Math.Min(int.MaxValue, capacity);
        }

        private int GetInventoryBagWeight()
        {
            long weight = Inventory.Where(x => x != null).Sum(x => (long)x.Weight);
            return (int)Math.Min(int.MaxValue, weight);
        }

        private bool CanCarryInventoryWeight(long additionalWeight)
        {
            if (GroupMembers == null || GroupLoot == null)
                return BagWeight + additionalWeight <= Stats[Stat.BagWeight];

            long personalWeight = GetInventoryBagWeight() + additionalWeight;
            if (personalWeight > Stats[Stat.BagWeight]) return false;

            long capacity = 0;
            foreach (PlayerObject member in GroupMembers)
            {
                long memberWeight = member == this ? personalWeight : member.GetInventoryBagWeight();
                capacity += Math.Max(0L, (long)member.Stats[Stat.BagWeight] - memberWeight);
            }

            long groupWeight = GroupLoot.Items.Sum(x => (long)x.Weight);
            return groupWeight <= capacity;
        }

        private void RebalanceGroupLootWeight()
        {
            if (GroupMembers == null || GroupLoot == null) return;

            List<PlayerObject> members = GroupMembers.ToList();
            if (members.Count == 0) return;

            Dictionary<PlayerObject, long> inventoryWeights = members.ToDictionary(x => x, x => (long)x.GetInventoryBagWeight());
            Dictionary<PlayerObject, long> allocations = members.ToDictionary(x => x, x => 0L);
            Dictionary<PlayerObject, long> capacities = members.ToDictionary(x => x, x => Math.Max(0L, (long)x.Stats[Stat.BagWeight] - inventoryWeights[x]));
            List<PlayerObject> available = members.Where(x => capacities[x] > 0).ToList();
            long remaining = GroupLoot.Items.Sum(x => (long)x.Weight);

            while (remaining > 0 && available.Count > 0)
            {
                long share = remaining / available.Count;
                List<PlayerObject> full = available.Where(x => capacities[x] <= share).ToList();

                if (full.Count > 0)
                {
                    foreach (PlayerObject member in full)
                    {
                        allocations[member] += capacities[member];
                        remaining -= capacities[member];
                        available.Remove(member);
                    }

                    continue;
                }

                foreach (PlayerObject member in available)
                {
                    allocations[member] += share;
                    capacities[member] -= share;
                    remaining -= share;
                }

                foreach (PlayerObject member in available)
                {
                    if (remaining == 0) break;

                    allocations[member]++;
                    capacities[member]--;
                    remaining--;
                }
            }

            // Membership or stat changes can leave existing contents over capacity. Keep all of
            // that weight assigned so the shared bag can never become free carry space.
            if (remaining > 0)
            {
                long share = remaining / members.Count;
                int extra = (int)(remaining % members.Count);

                for (int i = 0; i < members.Count; i++)
                    allocations[members[i]] += share + (i < extra ? 1 : 0);
            }

            foreach (PlayerObject member in members)
            {
                member.BagWeight = (int)Math.Min(int.MaxValue, inventoryWeights[member] + allocations[member]);
                member.Enqueue(new S.WeightUpdate { BagWeight = member.BagWeight, WearWeight = member.WearWeight, HandWeight = member.HandWeight });
            }
        }

        public void SendGroupLootUpdate()
        {
            Enqueue(new S.GroupLootUpdate { Loot = GetClientGroupLoot() });
        }

        public void BroadcastGroupLootUpdate()
        {
            if (GroupMembers == null || GroupLoot == null) return;

            RebalanceGroupLootWeight();

            foreach (PlayerObject member in GroupMembers)
                member.SendGroupLootUpdate();
        }

        private ClientGroupLootInfo GetClientGroupLoot()
        {
            GroupLootState state = GroupLoot ?? CreateGroupLootState();

            return new ClientGroupLootInfo
            {
                Mode = state.Mode,
                BagEnabled = state.BagEnabled,
                NeedRestrictions = state.NeedRestrictions,
                AllowManualTaking = state.AllowManualTaking,
                ItemTypes = state.ItemTypes.OrderBy(x => x).ToList(),
                Rarities = state.Rarities.OrderBy(x => x).ToList(),
                Items = state.Items.Select(x => x.ToClientInfo()).ToList(),
                Weight = (int)Math.Min(int.MaxValue, state.Items.Sum(x => (long)x.Weight)),
                Capacity = GetGroupLootCapacity(),
                Sharing = state.Share != null,
            };
        }

        public void SetGroupLootSettings(C.GroupLootSettings packet)
        {
            if (!Config.EnableGroupLoot || !Enum.IsDefined(typeof(GroupLootMode), packet.Mode)) return;

            bool solo = GroupMembers == null;
            GroupLootState state;

            if (solo)
                state = CreateGroupLootState();
            else
            {
                if (GroupLoot == null || GroupMembers[0] != this || GroupLoot.Share != null) return;
                state = GroupLoot;
            }

            state.Mode = packet.Mode;
            state.BagEnabled = packet.BagEnabled;
            state.NeedRestrictions = packet.NeedRestrictions;
            state.AllowManualTaking = packet.AllowManualTaking;
            state.ItemTypes.Clear();
            state.Rarities.Clear();

            if (packet.ItemTypes != null)
                foreach (ItemType type in packet.ItemTypes.Distinct())
                    if (Enum.IsDefined(typeof(ItemType), type) && type != ItemType.Nothing)
                        state.ItemTypes.Add(type);

            if (packet.Rarities != null)
                foreach (Rarity rarity in packet.Rarities.Distinct())
                    if (Enum.IsDefined(typeof(Rarity), rarity))
                        state.Rarities.Add(rarity);

            Character.GroupLootMode = state.Mode;
            Character.GroupLootBagEnabled = state.BagEnabled;
            Character.GroupLootNeedRestrictions = state.NeedRestrictions;
            Character.GroupLootAllowManualTaking = state.AllowManualTaking;
            Character.GroupLootItemTypes = string.Join(",", state.ItemTypes.OrderBy(x => x));
            Character.GroupLootRarities = string.Join(",", state.Rarities.OrderBy(x => x));

            if (solo)
                SendGroupLootUpdate();
            else
            {
                BroadcastGroupLootUpdate();

                if (!state.BagEnabled && state.Mode != GroupLootMode.FreeForAll && state.Items.Count > 0)
                    StartGroupLootShare();
            }
        }

        public void StartGroupLootShare()
        {
            if (!Config.EnableGroupLoot || GroupMembers == null || GroupLoot == null || GroupLoot.Mode == GroupLootMode.FreeForAll || GroupMembers[0] != this || GroupLoot.Share != null || GroupLoot.Items.Count == 0) return;

            BeginGroupLootShare(GroupLoot.Items, false);
        }

        private void StartInstantGroupLootShare()
        {
            if (!Config.EnableGroupLoot || GroupMembers == null || GroupLoot == null || GroupLoot.BagEnabled || GroupLoot.Mode == GroupLootMode.FreeForAll || GroupMembers[0] != this || GroupLoot.Share != null || GroupLoot.PendingItems.Count == 0) return;

            List<UserItem> items = GroupLoot.PendingItems.ToList();
            GroupLoot.PendingItems.Clear();
            BeginGroupLootShare(items, true);
        }

        private void BeginGroupLootShare(IEnumerable<UserItem> items, bool instant)
        {
            GroupLoot.Share = new GroupLootShareState { Instant = instant };
            GroupLoot.Share.Items.AddRange(items);
            GroupLoot.Share.Members.AddRange(GroupMembers.Select(x => x.ObjectID));
            foreach (uint objectID in GroupLoot.Share.Members)
                GroupLoot.Share.AwardedWeight[objectID] = 0;

            BroadcastGroupLootUpdate();

            if (GroupLoot.Mode == GroupLootMode.NeedGreed)
                BeginGroupLootVote();
            else
            {
                while (GroupLoot.Share != null && GroupLoot.Share.CurrentItem != null)
                {
                    AwardAutomaticGroupLoot(GroupLoot.Share.CurrentItem);
                    GroupLoot.Share.Position++;
                }

                FinishGroupLootShare();
            }
        }

        private List<PlayerObject> GetShareMembers()
        {
            if (GroupMembers == null || GroupLoot?.Share == null) return new List<PlayerObject>();
            return GroupMembers.Where(x => GroupLoot.Share.Members.Contains(x.ObjectID)).ToList();
        }

        private bool CanReceiveGroupLoot(PlayerObject member, UserItem item)
        {
            if (member == null || item == null || member.GetInventoryBagWeight() + (long)item.Weight > member.Stats[Stat.BagWeight]) return false;

            return member.CanGainItems(false, new ItemCheck(item, item.Count, item.Flags, item.ExpireTime));
        }

        private bool CanNeedGroupLoot(PlayerObject member, UserItem item)
        {
            if (GroupLoot?.NeedRestrictions != true) return true;

            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
                if (member.CanWearItem(item, slot))
                    return true;

            return false;
        }

        public void TakeGroupLoot(int itemIndex)
        {
            if (!Config.EnableGroupLoot || GroupMembers == null || GroupLoot == null || GroupLoot.Share != null) return;
            if (!GroupLoot.AllowManualTaking && GroupLoot.Mode != GroupLootMode.FreeForAll) return;

            UserItem item = GroupLoot.Items.FirstOrDefault(x => x.Index == itemIndex);
            if (!CanReceiveGroupLoot(this, item) || !GroupLoot.Items.Remove(item)) return;

            GainItem(item);
            BroadcastGroupLootResult(item, this, GroupLootVote.Pass);
        }

        private void AwardAutomaticGroupLoot(UserItem item)
        {
            List<PlayerObject> candidates = GetShareMembers().Where(x => CanReceiveGroupLoot(x, item)).ToList();
            if (candidates.Count == 0)
            {
                if (GroupLoot.Share.Instant)
                {
                    ReturnInstantGroupLoot(item);
                    BroadcastGroupLootResult(item, null, GroupLootVote.Pass);
                }

                return;
            }

            long minimum = candidates.Min(x => GroupLoot.Share.AwardedWeight[x.ObjectID]);
            candidates = candidates.Where(x => GroupLoot.Share.AwardedWeight[x.ObjectID] == minimum).ToList();

            PlayerObject winner;
            if (GroupLoot.Mode == GroupLootMode.Random)
                winner = candidates[SEnvir.Random.Next(candidates.Count)];
            else
            {
                List<PlayerObject> members = GetShareMembers();
                int start = Math.Max(0, members.FindIndex(x => x.ObjectID == GroupLoot.RoundRobinObjectID));
                winner = Enumerable.Range(0, members.Count).Select(i => members[(start + i) % members.Count]).First(candidates.Contains);
                GroupLoot.RoundRobinObjectID = members[(members.IndexOf(winner) + 1) % members.Count].ObjectID;
            }

            AwardGroupLoot(item, winner, GroupLootVote.Pass);
        }

        private void BeginGroupLootVote()
        {
            if (GroupLoot?.Share?.CurrentItem == null)
            {
                FinishGroupLootShare();
                return;
            }

            GroupLoot.Share.Votes.Clear();
            GroupLoot.Share.VoteExpiry = SEnvir.Now.AddSeconds(30);
            UserItem item = GroupLoot.Share.CurrentItem;

            foreach (PlayerObject member in GetShareMembers())
                member.Enqueue(new S.GroupLootVotePrompt { Item = item.ToClientInfo(), Duration = TimeSpan.FromSeconds(30), CanNeed = CanNeedGroupLoot(member, item) });
        }

        public void SubmitGroupLootVote(int itemIndex, GroupLootVote vote)
        {
            if (!Config.EnableGroupLoot || GroupMembers == null || GroupLoot?.Share?.CurrentItem == null || GroupLoot.Mode != GroupLootMode.NeedGreed) return;
            if (!GroupLoot.Share.Members.Contains(ObjectID) || GroupLoot.Share.CurrentItem.Index != itemIndex) return;
            if (!Enum.IsDefined(typeof(GroupLootVote), vote) || GroupLoot.Share.Votes.ContainsKey(ObjectID)) return;

            if (vote == GroupLootVote.Need && !CanNeedGroupLoot(this, GroupLoot.Share.CurrentItem))
                vote = GroupLootVote.Pass;

            GroupLoot.Share.Votes[ObjectID] = vote;

            PlayerObject leader = GroupMembers[0];
            if (leader.GetShareMembers().All(x => GroupLoot.Share.Votes.ContainsKey(x.ObjectID)))
                leader.ResolveGroupLootVote();
        }

        public void ProcessGroupLoot()
        {
            if (GroupMembers == null || GroupMembers[0] != this || GroupLoot?.Share == null || GroupLoot.Mode != GroupLootMode.NeedGreed) return;
            if (SEnvir.Now >= GroupLoot.Share.VoteExpiry)
                ResolveGroupLootVote();
        }

        private void ResolveGroupLootVote()
        {
            UserItem item = GroupLoot?.Share?.CurrentItem;
            if (item == null) return;

            List<PlayerObject> members = GetShareMembers();
            List<PlayerObject> candidates = members.Where(x => GroupLoot.Share.Votes.TryGetValue(x.ObjectID, out GroupLootVote vote) && vote == GroupLootVote.Need && CanNeedGroupLoot(x, item) && CanReceiveGroupLoot(x, item)).ToList();
            GroupLootVote winningVote = GroupLootVote.Need;

            if (candidates.Count == 0)
            {
                candidates = members.Where(x => GroupLoot.Share.Votes.TryGetValue(x.ObjectID, out GroupLootVote vote) && vote == GroupLootVote.Greed && CanReceiveGroupLoot(x, item)).ToList();
                winningVote = GroupLootVote.Greed;
            }

            if (candidates.Count > 0)
                AwardGroupLoot(item, candidates[SEnvir.Random.Next(candidates.Count)], winningVote);
            else
            {
                if (GroupLoot.Share.Instant)
                    ReturnInstantGroupLoot(item);

                BroadcastGroupLootResult(item, null, GroupLootVote.Pass);
            }

            GroupLoot.Share.Position++;
            BeginGroupLootVote();
        }

        private void AwardGroupLoot(UserItem item, PlayerObject winner, GroupLootVote vote)
        {
            if (!GroupLoot.Share.Instant && !GroupLoot.Items.Remove(item)) return;

            GroupLoot.Share.AwardedWeight[winner.ObjectID] += item.Weight;
            winner.GainItem(item);
            BroadcastGroupLootResult(item, winner, vote);
        }

        private void BroadcastGroupLootResult(UserItem item, PlayerObject winner, GroupLootVote vote)
        {
            foreach (PlayerObject member in GroupMembers)
                member.Enqueue(new S.GroupLootResult { ItemIndex = item.Index, ItemName = item.Info.ItemName, Winner = winner?.Name, Vote = vote });

            BroadcastGroupLootUpdate();
        }

        private void FinishGroupLootShare()
        {
            if (GroupLoot == null) return;
            GroupLoot.Share = null;
            BroadcastGroupLootUpdate();

            if (!GroupLoot.BagEnabled && GroupLoot.PendingItems.Count > 0)
                StartInstantGroupLootShare();
        }

        private void ReturnInstantGroupLoot(UserItem item)
        {
            Cell cell = GetDropLocation(Config.DropDistance, null) ?? CurrentCell;
            if (cell == null)
            {
                item.Delete();
                return;
            }

            item.SetTemporary(true);
            new ItemObject
            {
                Item = item,
                Account = Character.Account,
                Owners = GroupMembers.Where(x => GroupLoot.Share.Members.Contains(x.ObjectID)).Select(x => x.Character.Account).ToHashSet(),
                GuildTaxPaid = true,
                MonsterDrop = true,
            }.Spawn(CurrentMap, cell.Location);
        }

        private void DropGroupLoot(GroupLootState state)
        {
            if (state == null) return;

            List<UserItem> items = state.Items.Concat(state.PendingItems).ToList();
            if (state.Share?.Instant == true)
                items.AddRange(state.Share.Items.Skip(state.Share.Position));

            state.Share = null;
            state.PendingItems.Clear();
            foreach (UserItem item in items.Distinct())
            {
                Cell cell = GetDropLocation(Config.DropDistance, null) ?? CurrentCell;
                if (cell == null)
                {
                    item.Delete();
                    state.Items.Remove(item);
                    continue;
                }

                item.SetTemporary(true);
                new ItemObject { Item = item, Account = Character.Account, GuildTaxPaid = true }.Spawn(CurrentMap, cell.Location);
                state.Items.Remove(item);
            }
        }
    }
}
