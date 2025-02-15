using Library;
using Library.SystemModels;
using Server.Envir;
using Server.Envir.Commands.Command.Player;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class CastleGuard : CastleObject
    {
        public CastleGuardInfo GuardInfo { get; set; }

        public override bool CanMove => false;

        public int AttackRange = 15;

        public bool Spawn(CastleInfo castle, CastleGuardInfo guardInfo)
        {
            Castle = castle;
            GuardInfo = guardInfo;

            if (castle == null || guardInfo == null)
            {
                return false;
            }

            Direction = guardInfo.Direction;

            var map = SEnvir.Maps.First(x => x.Key == castle.Map).Value;

            if (!base.Spawn(map, new Point(guardInfo.X, guardInfo.Y)))
            {
                return false;
            }

            map.CastleGuards.Add(this);

            return true;
        }

        protected override bool InAttackRange()
        {
            return CurrentMap == Target.CurrentMap && Functions.InRange(CurrentLocation, Target.CurrentLocation, AttackRange);
        }

        protected override void MoveTo(Point target)
        {
            throw new NotSupportedException();
        }

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

        protected override void Attack()
        {
            var target = (PlayerObject)Target;

            if (target.Character.Account.GuildMember?.Guild.Castle != null) return;

            if (War.Participants.Count > 0 && !War.Participants.Contains(target.Character.Account.GuildMember.Guild)) return;

            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Targets = new List<uint> { Target.ObjectID } });

            UpdateAttackTime();

            ActionList.Add(new DelayedAction(
                               SEnvir.Now.AddMilliseconds(400 + Functions.Distance(CurrentLocation, Target.CurrentLocation) * Globals.ProjectileSpeed),
                               ActionType.DelayAttack,
                               Target,
                               GetDC(),
                               AttackElement));
        }

        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            if (War == null) return 0;

            if (attacker == null || attacker.Race != ObjectType.Player) return 0;

            var playerAttacker = (PlayerObject)attacker;

            var attackerGuild = playerAttacker.Character.Account.GuildMember?.Guild;

            if (attackerGuild == Guild) return 0;

            if (War.Participants.Count == 0 || !War.Participants.Contains(attackerGuild)) return 0;

            return base.Attacked(attacker, power, element, canReflect, ignoreShield, canCrit, canStruck);
        }

        public void RepairGuard()
        {
            if (CurrentHP <= 0)
            {
                Dead = false;
                SetHP(Stats[Stat.Health]);
                Broadcast(new S.ObjectRevive { ObjectID = ObjectID, Location = CurrentLocation, Effect = false });
            }
            else
                SetHP(Stats[Stat.Health]);
        }

        protected int GetDamageLevel()
        {
            int level = (int)Math.Round((double)(3 * CurrentHP) / Stats[Stat.Health]);

            if (level < 1) level = 1;

            return level;
        }
    }
}
