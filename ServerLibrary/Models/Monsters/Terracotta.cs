using Library;
using Server.Envir;
using System;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class Terracotta : MonsterObject
    {
        public bool CanPhase;

        public override bool CanAttack => Visible && base.CanAttack;
        public override bool Blocking => Visible && base.Blocking;
        private DateTime PhaseTime;

        public Terracotta()
        {
            Visible = false;
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

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
            else if (CanPhase && Visible && !Functions.InRange(CurrentLocation, Target.CurrentLocation, 4) && SEnvir.Random.Next(45) == 0)
                PhaseOut();
            else
                MoveTo(Target.CurrentLocation);

            if (Functions.InRange(CurrentLocation, Target.CurrentLocation, 2))
            {
                if (!Visible)
                {
                    PhaseIn();
                    return;
                }
            }

            if (InAttackRange() && CanAttack)
                Attack();
        }

        public override void ProcessAction(DelayedAction action)
        {
            switch (action.Type)
            {
                case ActionType.Function:
                    Visible = false;
                    return;
            }

            base.ProcessAction(action);
        }

        public void PhaseIn()
        {
            Visible = true;
            CellTime = SEnvir.Now.AddMilliseconds(500);
            ActionTime = SEnvir.Now.AddSeconds(1);

            AddAllObjects();
            Broadcast(new S.ObjectShow { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
        }

        public void PhaseOut()
        {
            if (PhaseTime > SEnvir.Now)
            {
                return;
            }

            PhaseTime = SEnvir.Now.AddSeconds(5);

            Broadcast(new S.ObjectHide { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
            ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(900), ActionType.Function));

            MoveTime = SEnvir.Now.AddMilliseconds(1200);
            ActionTime = SEnvir.Now.AddMilliseconds(1200);

            //SetHP(MaximumHP);
            //PoisonList.Clear();
        }

        public override bool CanBeSeenBy(PlayerObject ob)
        {
            return Visible && base.CanBeSeenBy(ob);
        }

        public override void UpdateMoveTime()
        {
            if (!Visible)
            {
                MoveTime = SEnvir.Now.AddMilliseconds(20);
                ActionTime = SEnvir.Now.AddMilliseconds(20);
            }
            else
            {
                base.UpdateMoveTime();
            }
        }
    }
}
