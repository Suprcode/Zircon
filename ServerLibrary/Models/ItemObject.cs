using Library;
using Library.Network;
using Server.DBModels;
using Server.Envir;
using Server.Models.Monsters;
using System;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    public sealed class ItemObject : MapObject
    {
        public override ObjectType Race => ObjectType.Item;
        public override bool Blocking => false;

        public DateTime ExpireTime { get; set; }

        public UserItem Item { get; set; }
        public AccountInfo Account { get; set; } //Use account instead of playerobject incase disconnection
        public HashSet<AccountInfo> Owners { get; set; }
        public bool GuildTaxPaid { get; set; }

        public bool MonsterDrop { get; set; }

        public override void Process()
        {
            base.Process();

            if (SEnvir.Now > ExpireTime)
            {
                Despawn();
                return;
            }

        }

        public override void OnDespawned()
        {
            base.OnDespawned();

            if (Item?.UserTask != null)
            {
                Item.UserTask.Objects.Remove(this);
                Item.UserTask = null;
                Item.Flags &= ~UserItemFlags.QuestItem;
            }

            if (Item?.IsTemporary == true)
                Item.Delete();

            Item = null;
            Account = null;
            Owners = null;
        }

        public bool CanPickUpItem(PlayerObject ob)
        {
            if (!CanAccessQuestItem(ob)) return false;

            if (Account != null && !IsOwner(ob.Character.Account))
            {
                if (CanShareGroupDrop(ob)) return true;

                if (Config.DropVisibleOtherPlayers)
                {
                    var isSameGuild = Account.GuildMember != null
                        && ob.Character.Account.GuildMember != null
                        && Account.GuildMember.Guild == ob.Character.Account.GuildMember.Guild;

                    var isSameGroup = ob.GroupMembers != null
                        && Account.Connection?.Player.GroupMembers == ob.GroupMembers;

                    var spawnElapsed = (int)Math.Floor((SEnvir.Now - SpawnTime).TotalMinutes);

                    if (spawnElapsed >= 10)
                        return true;
                    else if (isSameGuild && spawnElapsed >= 5)
                        return true;
                    else if (isSameGroup && spawnElapsed >= 2)
                        return true;
                }

                return false;
            }

            return true;
        }

        public bool CanPickUpItem(Companion ob)
        {
            if (ob == null || !CanPickUpItem(ob.CompanionOwner)) return false;

            long taxableAmount = GetGuildTax();
            ItemCheck check = new ItemCheck(Item, Item.Count - taxableAmount, Item.Flags, Item.ExpireTime);

            if (ob.CompanionOwner.UsesGroupLoot(Item))
                return ob.FilterCompanionPicks(check) && ob.CompanionOwner.CanAddGroupLoot(Item, check.Count);

            return ob.CanGainItems(true, check);
        }

        private UserItem TakeItem()
        {
            UserItem item = Item;
            item.UserTask?.Objects.Remove(this);
            Item = null;
            Despawn();
            return item;
        }

        public bool PickUpItem(PlayerObject ob)
        {
            if (!CanPickUpItem(ob))
                return false;

            long taxableAmount = GetGuildTax();

            if (ob.UsesGroupCurrencySharing(Item))
            {
                if (taxableAmount > 0)
                {
                    Item.Count -= taxableAmount;
                    Account.GuildMember.Contribute(taxableAmount);
                }

                ob.DistributeGroupCurrency(TakeItem());
                return true;
            }

            if (ob.UsesGroupLoot(Item))
            {
                if (!ob.CanAddGroupLoot(Item, Item.Count - taxableAmount)) return false;

                if (taxableAmount > 0)
                {
                    Item.Count -= taxableAmount;
                    Account.GuildMember.Contribute(taxableAmount);
                }

                ob.AddGroupLoot(TakeItem());
                return true;
            }

            ItemCheck check = new ItemCheck(Item, Item.Count - taxableAmount, Item.Flags, Item.ExpireTime);

            if (ob.CanGainItems(false, check))
            {
                if (taxableAmount > 0)
                {
                    Item.Count -= taxableAmount;

                    Account.GuildMember.Contribute(taxableAmount);
                }

                ob.GainItem(TakeItem());
                return true;
            }

            //Get Max Carry of type
            //Reduce Amount by type.
            //Send updated floor counts
            //Gain New / partial items
            return false;
        }

        public void PickUpItem(Companion ob)
        {
            if (!CanPickUpItem(ob))
                return;

            long taxableAmount = GetGuildTax();
            ItemCheck check = new ItemCheck(Item, Item.Count - taxableAmount, Item.Flags, Item.ExpireTime);

            if (ob.CompanionOwner.UsesGroupCurrencySharing(Item))
            {
                if (taxableAmount > 0)
                {
                    Item.Count -= taxableAmount;
                    Account.GuildMember.Contribute(taxableAmount);
                }

                ob.CompanionOwner.DistributeGroupCurrency(TakeItem());
                return;
            }

            if (ob.CompanionOwner.UsesGroupLoot(Item))
            {
                if (taxableAmount > 0)
                {
                    Item.Count -= taxableAmount;
                    Account.GuildMember.Contribute(taxableAmount);
                }

                ob.CompanionOwner.AddGroupLoot(TakeItem());
                return;
            }

            if (ob.CanGainItems(false, check))
            {
                if (taxableAmount > 0)
                {
                    Item.Count -= taxableAmount;

                    Account.GuildMember.Contribute(taxableAmount);
                }

                ob.GainItem(TakeItem());
                return;
            }

            //Get Max Carry of type
            //Reduce Amount by type.
            //Send updated floor counts
            //Gain New / partial items
            return;
        }


        public override void ProcessHPMP()
        {
        }
        public override void ProcessNameColour()
        {
        }
        public override void ProcessBuff()
        {
        }
        public override void ProcessPoison()
        {
        }

        public override bool CanBeSeenBy(PlayerObject ob)
        {
            if (!CanAccessQuestItem(ob)) return false;

            if (!Config.DropVisibleOtherPlayers)
            {
                if (Account != null && !IsOwner(ob.Character.Account) && !CanBeSeenByGroupMember(ob)) return false;
            }

            return base.CanBeSeenBy(ob);
        }

        private bool CanAccessQuestItem(PlayerObject ob)
        {
            if (Item?.UserTask != null &&
                ((Item.UserTask.Quest.Character != null && Item.UserTask.Quest.Character != ob.Character) ||
                 (Item.UserTask.Quest.Account != null && Item.UserTask.Quest.Account != ob.Character.Account))) return false;

            return Item == null || (Item.Flags & UserItemFlags.QuestItem) != UserItemFlags.QuestItem || Account == ob.Character.Account;
        }

        private bool IsOwner(AccountInfo account)
        {
            return Owners?.Contains(account) ?? Account == account;
        }

        private long GetGuildTax()
        {
            return GuildTaxPaid ? 0 : Account?.GuildMember?.Guild?.CalculateGuildTax(Item) ?? 0;
        }

        private bool CanShareGroupDrop(PlayerObject ob)
        {
            if (Owners != null) return false;

            PlayerObject owner = Account?.Connection?.Player;
            if (owner?.GroupMembers == null || owner.GroupMembers != ob.GroupMembers) return false;
            if (ob.UsesGroupCurrencySharing(Item)) return true;

            long taxableAmount = GetGuildTax();
            return ob.CanAddGroupLoot(Item, Item.Count - taxableAmount);
        }

        private bool CanBeSeenByGroupMember(PlayerObject ob)
        {
            if (Owners != null) return Owners.Contains(ob.Character.Account);
            if (!Config.EnableGroupLoot || Item == null || Item.UserTask != null || (Item.Flags & UserItemFlags.QuestItem) == UserItemFlags.QuestItem) return false;

            PlayerObject owner = Account?.Connection?.Player;
            return owner?.GroupMembers != null && owner.GroupMembers == ob.GroupMembers;
        }

        public override void Activate()
        {
            if (Activated) return;

            Activated = true;
            SEnvir.ActiveObjects.Add(this);
        }
        public override void DeActivate()
        {
            return;
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();

            ExpireTime = SEnvir.Now + Config.DropDuration;

            AddAllObjects();

            Activate();
        }
        public override Packet GetInfoPacket(PlayerObject ob)
        {
            return new S.ObjectItem
            {
                ObjectID = ObjectID,
                Item = Item.ToClientInfo(),
                Location = CurrentLocation,
            };
        }
        public override Packet GetDataPacket(PlayerObject ob)
        {
            return new S.DataObjectItem
            {
                ObjectID = ObjectID,

                MapIndex = CurrentMap.Info.Index,
                CurrentLocation = CurrentLocation,

                ItemIndex = Item.Info.Index,
            };
        }
    }
}
