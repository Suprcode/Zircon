using System;
using Library;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class EvilElephant : MonsterObject
    {

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

        public void Attack(MapObject ob, bool applyPoison)
        {
            if (!applyPoison)
            {
                Attack(ob, GetDC(), AttackElement);
                return;
            }

            if (Attack(ob, GetDC(), AttackElement) <= 0) return;
                
            ob.ApplyPoison(new Poison
            {
                Value = GetSC(),
                Type = PoisonType.Green,
                Owner = this,
                TickFrequency = TimeSpan.FromSeconds(2),
                TickCount = 10,
            });
        }


        protected override void Attack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            UpdateAttackTime();

            if (SEnvir.Random.Next(6) > 0)
            {
                Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                ActionList.Add(new DelayedAction(
                                   SEnvir.Now.AddMilliseconds(400),
                                   ActionType.DelayAttack,
                                   Target,
                                   false));
            }
            else
            {
                Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                foreach (MapObject ob in GetTargets(CurrentMap, CurrentLocation, 1))
                {
                    ActionList.Add(new DelayedAction(
                                       SEnvir.Now.AddMilliseconds(400),
                                       ActionType.DelayAttack,
                                       ob,
                                       true));
                }
            }

        }
    }
}
