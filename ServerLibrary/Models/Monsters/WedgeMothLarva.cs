using System;
using Library;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class WedgeMothLarva : MonsterObject
    {
        public override bool CanMove => false;

        public WedgeMothLarva()
        {
            Direction = MirDirection.Up;
        }

        protected override bool InAttackRange()
        {
            return Target.CurrentMap == CurrentMap;
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
            return mob.Spawn(CurrentMap, CurrentLocation);
        }
    }
}
