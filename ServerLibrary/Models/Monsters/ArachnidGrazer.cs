using System;
using System.Linq;
using Library;
using Library.SystemModels;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class ArachnidGrazer : MonsterObject
    {
        public override bool CanMove => false;
        
        protected override bool InAttackRange()
        {
            return Target.CurrentMap == CurrentMap;
        }


        public ArachnidGrazer()
        {
            Direction = (MirDirection) SEnvir.Random.Next(3);
        }
        
        public override void ProcessAction(DelayedAction action)
        {
            switch (action.Type)
            {
                case ActionType.Function:
                    SpawnMinions(1, 0, Target);
                    break;
            }

            base.ProcessAction(action);
        }
        
        protected override void Attack()
        {
            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            UpdateAttackTime();

            ActionList.Add(new DelayedAction(
                               SEnvir.Now.AddMilliseconds(600),
                               ActionType.Function));
        }
        

        public override bool SpawnMinion(MonsterObject mob)
        {
            return mob.Spawn(CurrentMap, Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, 3)));
        }
    }
}
