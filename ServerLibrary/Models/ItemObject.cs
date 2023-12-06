using System;
using System.Linq;
using Library;
using Library.Network;
using S = Library.Network.ServerPackets;
using Server.DBModels;
using Server.Envir;
using Server.Models.Monsters;

namespace Server.Models
{
    public sealed class ItemObject : MapObject
    {
        public override ObjectType Race => ObjectType.Item;
        public override bool Blocking => false;

        public DateTime ExpireTime { get; set; }

        public UserItem Item { get; set; }
        public AccountInfo Account { get; set; } //Use account instead of playerobject incase disconnection

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

            if (Item.UserTask != null)
            {
                Item.UserTask.Objects.Remove(this);
                Item.UserTask = null;
                Item.Flags &= ~UserItemFlags.QuestItem;
            }

            Item = null;
            Account = null;
        }

        public override void OnSafeDespawn()
        {
            base.OnSafeDespawn();


            if (Item.UserTask != null)
            {
                Item.UserTask.Objects.Remove(this);
                Item.UserTask = null;
                Item.Flags &= ~UserItemFlags.QuestItem;
            }

            Item = null;
            Account = null;
        }

        public bool CanPickUpItem(PlayerObject ob)
        {
            if (Account != null && Account != ob.Character.Account)
            {
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

        public bool PickUpItem(PlayerObject ob)
        {
            if (!CanPickUpItem(ob))
                return false;

            long taxableAmount = Account?.GuildMember?.Guild?.CalculateGuildTax(Item) ?? 0;

            ItemCheck check = new ItemCheck(Item, Item.Count - taxableAmount, Item.Flags, Item.ExpireTime);

            if (ob.CanGainItems(false, check))
            {
                if (taxableAmount > 0)
                {
                    Item.Count -= taxableAmount;

                    Account.GuildMember.Contribute(taxableAmount);
                }

                Item.UserTask?.Objects.Remove(this);

                ob.GainItem(Item);
                Despawn();
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
            if (!CanPickUpItem(ob.CompanionOwner))
                return;

            long taxableAmount = Account?.GuildMember?.Guild?.CalculateGuildTax(Item) ?? 0;

            ItemCheck check = new ItemCheck(Item, Item.Count - taxableAmount, Item.Flags, Item.ExpireTime);

            if (ob.CanGainItems(false, check))
            {
                if (taxableAmount > 0)
                {
                    Item.Count -= taxableAmount;

                    Account.GuildMember.Contribute(taxableAmount);
                }

                Item.UserTask?.Objects.Remove(this);

                ob.GainItem(Item);
                Despawn();
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
            if (!Config.DropVisibleOtherPlayers)
            {
                if (Account != null && ob.Character.Account != Account) return false;
                if (Item.UserTask != null && 
                    ((Item.UserTask.Quest.Character != null && Item.UserTask.Quest.Character != ob.Character) || 
                    (Item.UserTask.Quest.Account != null && Item.UserTask.Quest.Account != ob.Character.Account))) return false;
            }

            return base.CanBeSeenBy(ob);
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
