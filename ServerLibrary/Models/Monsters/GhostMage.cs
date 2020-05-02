using System;
using Library;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class GhostMage : MonsterObject
    {
        public bool SpectralHit;

        public DateTime VisibleTime;

        public override bool CanAttack => Visible && base.CanAttack;
        public override bool CanMove => Visible && base.CanMove;
        public override bool Blocking => Visible && base.Blocking;

        public GhostMage()
        {
            Visible = false;
        }
        
        public override void Process()
        {
            base.Process();

            if (Dead || SEnvir.Now < ShockTime || Visible) return;

            if (SEnvir.Now <= VisibleTime) return;

            VisibleTime = SEnvir.Now.AddSeconds(3);
            
            bool visible = Target != null && Functions.InRange(Target.CurrentLocation, CurrentLocation, 5);

            if (!visible) return;

            Visible = true;
            CellTime = SEnvir.Now.AddMilliseconds(500);
            ActionTime = SEnvir.Now.AddSeconds(1);

            AddAllObjects();
            Broadcast(new S.ObjectShow { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
        }

        public override bool CanBeSeenBy(PlayerObject ob)
        {
            return Visible && base.CanBeSeenBy(ob);
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
                               SpectralHit ? (Element)SEnvir.Random.Next(8) : AttackElement));
        }
    }
}
