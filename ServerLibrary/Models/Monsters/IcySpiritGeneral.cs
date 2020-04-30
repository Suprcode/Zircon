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
    public class IcySpiritGeneral : MonsterObject
    {
        public int AttackRange = 8;

        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;
            if (Target.CurrentLocation == CurrentLocation) return false;

            return Functions.InRange(CurrentLocation, Target.CurrentLocation, AttackRange);
        }

        public override void ProcessAction(DelayedAction action)
        {

            switch (action.Type)
            {
                case ActionType.Function:
                    Attack((MapObject)action.Data[0]);
                    return;
            }

            base.ProcessAction(action);
        }

        public int Attack(MapObject ob)
        {
            int result = base.Attack(ob, GetDC(), Element.Dark);

            if (result > 0)
                ob.ChangeMP(-Math.Min(ob.CurrentMP, result));

            return result;
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (InAttackRange() && CanAttack) //random 3
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


            if (SEnvir.Random.Next(2) == 0)
            {
                Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Targets = new List<uint> { Target.ObjectID } });

                UpdateAttackTime();

                ActionList.Add(new DelayedAction(
                                   SEnvir.Now.AddMilliseconds(400),
                                   ActionType.Function,
                                   Target));
            }
            else
            {
                AttackAoE(1, MagicType.MonsterIceStorm, Element.Ice);
            }
        }

    }
}
