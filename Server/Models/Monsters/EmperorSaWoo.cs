using System;
using System.Collections.Generic;
using Library;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class EmperorSaWoo : MonsterObject
    {

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (!InAttackRange())
            {
                if (CanAttack)
                {
                    if (SEnvir.Random.Next(3) == 0)
                        RangeAttack();
                }


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
            }

            if (!CanAttack) return;

            if (SEnvir.Random.Next(5) > 0)
            {
                if (InAttackRange())
                    Attack();
            }
            else RangeAttack();
        }

        protected override void Attack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            UpdateAttackTime();


            ActionList.Add(new DelayedAction(
                               SEnvir.Now.AddMilliseconds(400),
                               ActionType.DelayAttack,
                               Target,
                               GetDC(),
                               AttackElement));
        }


        public void RangeAttack()
        {
            switch (SEnvir.Random.Next(3))
            {
                case 0:
                    MassCyclone();
                    break;
                default:
                    Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

                    UpdateAttackTime();

                    Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Targets = new List<uint> { Target.ObjectID } });

                    foreach (MapObject ob in GetTargets(CurrentMap, Target.CurrentLocation, 2))
                        ActionList.Add(new DelayedAction(
                                       SEnvir.Now.AddMilliseconds(400),
                                       ActionType.DelayAttack,
                                       ob,
                                       GetDC(),
                                       AttackElement));
                    break;
            }
        }
    }
}
