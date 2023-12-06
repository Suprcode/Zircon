using Library;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class TerracottaSub : Terracotta
    {
        public override bool CanAttack => Visible && base.CanAttack;
        public override bool Blocking => Visible && base.Blocking;

        public TerracottaSub()
        {
            Visible = false;
            CanPhase = true;
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
            {
                if (SEnvir.Random.Next(2) == 0)
                    Attack();
                else
                    Attack2();
            }
        }

        protected override void Attack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            UpdateAttackTime();

            for (int radius = 1; radius <= 2; radius++)
            {
                for (int dis = -1; dis <= 1; dis++)
                {
                    foreach (MapObject ob in GetTargets(CurrentMap, Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, dis), radius), 0))
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(400),
                            ActionType.DelayAttack,
                            ob,
                            GetDC(),
                            AttackElement));
                    }
                }
            }
        }

        protected virtual void Attack2()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.None, AttackElement = Element.None });

            UpdateAttackTime();

            foreach (MapObject ob in GetTargets(CurrentMap, Functions.Move(CurrentLocation, Direction), 1))
            {
                ActionList.Add(new DelayedAction(
                    SEnvir.Now.AddMilliseconds(400),
                    ActionType.DelayAttack,
                    ob,
                    GetDC(),
                    AttackElement));
            }
        }
    }
}
