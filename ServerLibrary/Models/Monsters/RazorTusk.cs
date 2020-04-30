using System;
using System.Collections.Generic;
using Library;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class RazorTusk : MonsterObject
    {
        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;
            if (Target.CurrentLocation == CurrentLocation) return false;

            return Functions.InRange(CurrentLocation, Target.CurrentLocation, 3);
        }
        public override void ProcessAction(DelayedAction action)
        {
            switch (action.Type)
            {
                case ActionType.DelayAttack:
                    Attack((MapObject)action.Data[0], (bool)action.Data[1]);
                    return;
            }

            base.ProcessAction(action);
        }

        public void Attack(MapObject ob, bool melee)
        {
            if (melee)
            {
                base.Attack(ob, GetDC(), AttackElement);
                return;
            }

            base.Attack(ob, GetDC(), AttackElement);
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (InAttackRange() && CanAttack && SEnvir.Random.Next(3) > 0)
                Attack();

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
                                   true));
            }
            else
            {
                Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Targets = new List<uint> { Target.ObjectID } });

                ActionList.Add(new DelayedAction(
                                   SEnvir.Now.AddMilliseconds(400),
                                   ActionType.DelayAttack,
                                   Target,
                                   false));
            }

        }
    }
}
