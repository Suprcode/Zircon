using Library;
using Library.Network;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class CastleObjective : MonsterObject
    {
        public ConquestWar War;
    }

    public class CastleFlag : CastleObjective
    {
        public override bool CanMove => false;

        private static int _takeDuration = 30;
        public TimeSpan ContesterDelay = TimeSpan.FromSeconds(_takeDuration);

        private GuildInfo Contester = null;
        private DateTime ContesterTime = DateTime.MaxValue;

        public GuildInfo CurrentGuild = null;

        private int _flag = 0;
        private Color _colour = Color.White;

        private TimeSpan _messageDuration = TimeSpan.FromSeconds(5);

        public CastleFlag()
        {
            Direction = MirDirection.Up;
        }

        public override void Process()
        {
            base.Process();

            CastleInfo castle;

            if (War == null)
            {
                //find castle based on map
                castle = SEnvir.CastleInfoList.Binding.FirstOrDefault(x => x.CastleRegion != null && x.CastleRegion.Map == CurrentMap.Info);
            }
            else
            {
                //find castle from war
                castle = War.Castle;
            }

            GuildInfo ownerGuild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Castle == castle);

            if (CurrentGuild != ownerGuild)
            {
                _flag = ownerGuild?.Flag ?? 0;
                _colour = Color.FromArgb(ownerGuild?.Colour.R ?? 0, ownerGuild?.Colour.G ?? 0, ownerGuild?.Colour.B ?? 0);

                Visible = false;
                RemoveAllObjects();

                Visible = true;
                AddAllObjects();

                CurrentGuild = ownerGuild;
            }

            if (Target != null && !InAttackRange())
                Target = null;

            if (Target == null && Contester != null)
            {
                foreach (SConnection con in SEnvir.Connections)
                    con.ReceiveChat(string.Format(con.Language.ConquestNotTakingFlag, Contester.GuildName, War.Castle.Name), MessageType.System);

                Contester = null;
                ContesterTime = DateTime.MaxValue;
            }
        }

        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;

            return Target.CurrentLocation == CurrentLocation || Functions.InRange(CurrentLocation, Target.CurrentLocation, ViewRange);
        }

        public override void ProcessRoam()
        {
        }

        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            return 0;
        }

        protected override void Attack()
        {
            UpdateAttackTime();

            var target = (PlayerObject)Target;

            if (target.Character.Account.GuildMember == null) return;

            if (target.Character.Account.GuildMember.Guild.Castle != null) return;

            if (War.Participants.Count > 0 && !War.Participants.Contains(target.Character.Account.GuildMember.Guild)) return;

            if (Contester == null)
            {
                Contester = target.Character.Account.GuildMember.Guild;

                //Start 30 seconds timer
                ContesterTime = SEnvir.Now.Add(ContesterDelay);

                foreach (SConnection con in SEnvir.Connections)
                    con.ReceiveChat(string.Format(con.Language.ConquestTakingFlag, Contester.GuildName, War.Castle.Name, _takeDuration), MessageType.System);

                return;
            }
            else
            {
                bool contestGuildNear = false;

                //check if any other guild nearby
                foreach (PlayerObject player in CurrentMap.Players)
                {
                    int distance;

                    distance = Functions.Distance(player.CurrentLocation, CurrentLocation);

                    if (distance > ViewRange) continue;

                    if (player.Character.Account.GuildMember == null) continue;
                    if (War.Participants.Count > 0 && !War.Participants.Contains(player.Character.Account.GuildMember.Guild)) return;

                    //if guild near, add to timer
                    if (player.Character.Account.GuildMember.Guild != Contester)
                    {
                        //Another guild near flag - reset contest time
                        ContesterTime = SEnvir.Now.Add(ContesterDelay);

                        foreach (SConnection con in SEnvir.Connections)
                            con.ReceiveChat(string.Format(con.Language.ConquestPreventingFlag, player.Character.Account.GuildMember.Guild.GuildName, Contester.GuildName, War.Castle.Name), MessageType.System);

                        return;
                    }
                    else
                    {
                        contestGuildNear = true;
                    }
                }

                if (!contestGuildNear)
                {
                    foreach (SConnection con in SEnvir.Connections)
                        con.ReceiveChat(string.Format(con.Language.ConquestNotTakingFlag, Contester.GuildName, War.Castle.Name), MessageType.System);

                    Contester = null;
                    ContesterTime = DateTime.MaxValue;

                    return;
                }
                else
                {
                    var difference = (ContesterTime - SEnvir.Now).Seconds;
                    foreach (SConnection con in SEnvir.Connections)
                        con.ReceiveChat(string.Format(con.Language.ConquestTakingFlag, Contester.GuildName, War.Castle.Name, difference), MessageType.System);

                }
            }

            if (ContesterTime > SEnvir.Now) return;

            //Remove current guild from castle
            GuildInfo ownerGuild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Castle == War.Castle);

            if (ownerGuild != null)
                ownerGuild.Castle = null;

            //Update new guild with castle
            Contester.Castle = War.Castle;

            foreach (SConnection con in SEnvir.Connections)
                con.ReceiveChat(string.Format(con.Language.ConquestCapture, Contester.GuildName, War.Castle.Name), MessageType.System);

            SEnvir.Broadcast(new S.GuildCastleInfo { Index = War.Castle.Index, Owner = Contester.GuildName });

            Contester = null;
        }

        public override bool ApplyPoison(Poison p)
        {
            return false;
        }

        public override void ProcessSearch()
        {
            base.ProcessSearch();
        }
        public override void ProcessTarget()
        {
            base.ProcessTarget();
        }

        public override void ProcessRegen() { }
        public override bool ShouldAttackTarget(MapObject ob)
        {
            if (ob == this || ob?.Node == null || ob.Dead || !ob.Visible || War == null) return false;

            switch (ob.Race)
            {
                case ObjectType.Item:
                case ObjectType.NPC:
                case ObjectType.Spell:
                case ObjectType.Monster:
                    return false;
            }

            if (ob.Buffs.Any(x => x.Type == BuffType.Invisibility) && !CoolEye) return false;
            if (ob.Buffs.Any(x => x.Type == BuffType.Cloak))
            {
                if (!CoolEye) return false;
                if (!Functions.InRange(ob.CurrentLocation, CurrentLocation, 2)) return false;
                if (ob.Level >= Level) return false;
            }
            if (ob.Buffs.Any(x => x.Type == BuffType.Transparency)) return false;

            switch (ob.Race)
            {
                case ObjectType.Player:
                    PlayerObject player = (PlayerObject)ob;

                    return player.Character.Account.GuildMember?.Guild.Castle != War.Castle;
                default:
                    throw new NotImplementedException();
            }
        }
        public override bool CanAttackTarget(MapObject ob)
        {
            if (ob == this || ob?.Node == null || ob.Dead || !ob.Visible || War == null) return false;

            switch (ob.Race)
            {
                case ObjectType.Item:
                case ObjectType.NPC:
                case ObjectType.Spell:
                case ObjectType.Monster:
                    return false;
            }

            switch (ob.Race)
            {
                case ObjectType.Player:
                    PlayerObject player = (PlayerObject)ob;

                    return player.Character.Account.GuildMember?.Guild.Castle != War.Castle;
                default:
                    throw new NotImplementedException();
            }
        }

        public override bool CanBeSeenBy(PlayerObject ob)
        {
            return Visible && base.CanBeSeenBy(ob);
        }

        public override void Die()
        {
            base.Die();
        }

        public override Packet GetInfoPacket(PlayerObject ob)
        {
            return new S.ObjectMonster
            {
                ObjectID = ObjectID,
                MonsterIndex = MonsterInfo.Index,

                Location = CurrentLocation,

                NameColour = NameColour,
                Direction = Direction,

                Poison = Poison,

                Buffs = Buffs.Where(x => x.Visible).Select(x => x.Type).ToList(),

                Extra1 = _flag,
                Colour = _colour
            };
        }
    }
}
