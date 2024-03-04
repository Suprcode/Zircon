using Library;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public sealed class Tornado : MonsterObject
    {
        public override bool Blocking => false;

        public DateTime VisibleTime;

        public Tornado()
        {
            RoamDelay = TimeSpan.FromSeconds(1);
        }

        public override void Die()
        {
            base.Die();

            DeadTime = SEnvir.Now.AddMilliseconds(700);
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();

            ActionTime = SEnvir.Now.AddSeconds(2);

            Broadcast(new S.ObjectShow { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
        }

        public override int Pushed(MirDirection direction, int distance)
        {
            return 0;
        }

        public override void Process()
        {
            base.Process();

            if (SEnvir.Now <= VisibleTime || Dead) return;

            Die();
        }

        public override void ProcessRoam()
        {
            if (!CanMove) return;

            if (SEnvir.Now < RoamTime || SeenByPlayers.Count == 0) return;

            RoamTime = SEnvir.Now + RoamDelay;

            if (CurrentCell.Objects.Any(x => x != this && x.Blocking))
            {
                if (CanAttack)
                    Attack();
            }

            if (SEnvir.Random.Next(5) > 0)
                Walk(Direction);
            else
                Turn((MirDirection)SEnvir.Random.Next(8));
        }

        protected override void Attack()
        {
            List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, 1);

            foreach (MapObject target in targets)
            {
                if (target == this || !CanAttackTarget(target)) continue;

                ActionList.Add(new DelayedAction(
                                   SEnvir.Now.AddMilliseconds(100),
                                   ActionType.DelayAttack,
                                   target,
                                   GetDC(),
                                   AttackElement));
            }

            UpdateAttackTime();
        }

        public override bool CanAttackTarget(MapObject ob)
        {
            if (ob == this || ob?.Node == null || ob.Dead || !ob.Visible || ob is Guard || ob is CastleLord) return false;

            switch (ob.Race)
            {
                case ObjectType.Item:
                case ObjectType.NPC:
                case ObjectType.Spell:
                    return false;
            }

            switch (ob.Race)
            {
                case ObjectType.Player:
                    PlayerObject player = (PlayerObject)ob;

                    if (player.GameMaster) return false;

                    if (InSafeZone || player.InSafeZone) return false;

                    return true;
                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject)ob;

                    return true;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
