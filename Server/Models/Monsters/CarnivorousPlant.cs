using System;
using Library;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class CarnivorousPlant : MonsterObject
    {
        public override bool CanAttack => Visible && base.CanAttack;
        public override bool CanMove => Visible && base.CanMove;
        public override bool Blocking => Visible && base.Blocking;
        
        public DateTime VisibleTime;
        public int FindRange = 5;
        public int HideRange = 5;

        public CarnivorousPlant()
        {
            Visible = false;
        }

        public override void ProcessAction(DelayedAction action)
        {

            switch (action.Type)
            {
                case ActionType.Function:
                    RemoveAllObjects();
                    return;
            }

            base.ProcessAction(action);
        }

        public override void Process()
        {
            base.Process();

            if (Dead || SEnvir.Now < ShockTime) return;

            if (SEnvir.Now <= VisibleTime) return;

            VisibleTime = SEnvir.Now.AddSeconds(3);

            bool visible = Target != null && (Functions.InRange(Target.CurrentLocation, CurrentLocation, Visible ? HideRange : FindRange) || (Visible && InAttackRange()));
            

            if (!Visible && visible)
            {
                Visible = true;
                CellTime = SEnvir.Now.AddMilliseconds(500);
                ActionTime = SEnvir.Now.AddSeconds(1);

                AddAllObjects();
                Broadcast(new S.ObjectShow { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
            }

            if (Visible && !visible && CanAttack)
            {
                Visible = false;
                VisibleTime = SEnvir.Now.AddSeconds(4);

                Broadcast(new S.ObjectHide { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(3000), ActionType.Function));

                SetHP(Stats[Stat.Health]);
                PoisonList.Clear();
            }
        }
        
        public override bool CanBeSeenBy(PlayerObject ob)
        {
            return Visible && base.CanBeSeenBy(ob);
        }
    }
}
