using System;
using System.Drawing;
using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class GhostSorcerer : MonsterObject
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
            Broadcast(new Library.Network.ServerPackets.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            UpdateAttackTime();


            LineAttack(AttackRange);
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
