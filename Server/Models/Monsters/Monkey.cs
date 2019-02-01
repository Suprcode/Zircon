using System;
using System.Collections.Generic;
using Library;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class Monkey : MonsterObject
    {
        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;
            if (Target.CurrentLocation == CurrentLocation) return false;

            return Functions.InRange(CurrentLocation, Target.CurrentLocation, 2);
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (InAttackRange() && CanAttack && SEnvir.Random.Next(2) == 0)
                Attack();

            if (CurrentLocation == Target.CurrentLocation)
            {
                MirDirection direction = (MirDirection) SEnvir.Random.Next(8);
                int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                for (int d = 0; d < 8; d++)
                {
                    if (Walk(direction)) break;

                    direction = Functions.ShiftDirection(direction, rotation);
                }
            }
            else
                MoveTo(Target.CurrentLocation);

            if (InAttackRange() && CanAttack)
                Attack();
        }


        protected override void Attack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            UpdateAttackTime();

            if (Functions.InRange(CurrentLocation, Target.CurrentLocation, 1))
            {
                Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                
                ActionList.Add(new DelayedAction(
                                   SEnvir.Now.AddMilliseconds(400),
                                   ActionType.DelayAttack,
                                   Target,
                                   GetDC(),
                                   AttackElement));
            }
            else
            {
                Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Targets = new List<uint> { Target.ObjectID } });
                
                ActionList.Add(new DelayedAction(
                                   SEnvir.Now.AddMilliseconds(400),
                                   ActionType.DelayAttack,
                                   Target,
                                   GetDC(),
                                   AttackElement));
            }

        }
    }
}
