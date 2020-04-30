using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class NumaHighMage : Monkey
    {
        public int AttackRange = 6;

        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;
            if (Target.CurrentLocation == CurrentLocation) return false;

            int x = Math.Abs(Target.CurrentLocation.X - CurrentLocation.X);
            int y = Math.Abs(Target.CurrentLocation.Y - CurrentLocation.Y);

            if (x > AttackRange || y > AttackRange) return false;


            return x == 0 || x == y || y == 0;
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

                LineAttack(AttackRange);
            }

        }


        private void LineAttack(int distance)
        {
            for (int i = 1; i <= distance; i++)
            {
                Point target = Functions.Move(CurrentLocation, Direction, i);

                if (target == Target.CurrentLocation)
                {
                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(400),
                        ActionType.DelayAttack,
                        Target,
                        GetDC(),
                        AttackElement));
                }
                else
                {
                    Cell cell = CurrentMap.GetCell(target);
                    if (cell?.Objects == null) continue;

                    foreach (MapObject ob in cell.Objects)
                    {
                        if (!CanAttackTarget(ob)) continue;

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
    }
}
