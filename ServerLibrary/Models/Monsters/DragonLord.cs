using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class DragonLord : MonsterObject
    {
        public DateTime SlaveTime;

        public int AttackRange = 12;

        protected override bool InAttackRange()
        {
            return CurrentMap == Target.CurrentMap && Functions.InRange(CurrentLocation, Target.CurrentLocation, AttackRange);
        }

        public override void Process()
        {
            base.Process();

            if (Dead || SEnvir.Now < SlaveTime) return;

            SlaveTime = SEnvir.Now.AddSeconds(15);

            for (int i = MinionList.Count - 1; i >= 0; i--)
            {
                MonsterObject mob = MinionList[i];
                if (mob.CurrentMap == CurrentMap && Functions.InRange(CurrentLocation, mob.CurrentLocation, 15)) continue;

                if (mob.MonsterInfo.IsBoss) continue;

                mob.EXPOwner = null;
                mob.Die();
                mob.Despawn();
            }

            SpawnMinions(10 - MinionList.Count, 0, Target);
        }

        public override bool SpawnMinion(MonsterObject mob)
        {
            return mob.Spawn(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, 10));
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;
            
            if (!InAttackRange())
            {
                if (CurrentLocation == Target.CurrentLocation)
                {
                    MirDirection direction = (MirDirection)SEnvir.Random.Next(8);
                    int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                    for (int d = 0; d < 8; d++)
                    {
                        if (Walk(direction)) break;

                        direction = Functions.ShiftDirection(direction, rotation);
                    }
                }
                else
                    MoveTo(Target.CurrentLocation);

                return;
            }

            if (CanAttack)
            Attack();
        }

        protected override void Attack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            S.ObjectRangeAttack packet = new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation};

            Broadcast(packet);

            UpdateAttackTime();


            foreach (MapObject target in GetTargets(CurrentMap, CurrentLocation, AttackRange))
            {
                if (SEnvir.Random.Next(10) > 3) continue;

                packet.Targets.Add(target.ObjectID);

                foreach (MapObject attackTarget in GetTargets(CurrentMap, target.CurrentLocation, 2))
                {
                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(1000),
                        ActionType.DelayAttack,
                        attackTarget,
                        GetDC(),
                        AttackElement));
                }
            }
            
            
        }
    }
}
