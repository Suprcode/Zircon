using System;
using Server.Envir;
using Library;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class BanyoCaptain : MonsterObject
    {
        public DateTime CastTime { get; set; }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (CanAttack && SEnvir.Now > CastTime)
            {
                CastTime = SEnvir.Now.AddSeconds(3);

                if (SEnvir.Random.Next(5) == 0)
                    Purification();
                else
                {
                    int distance = Functions.Distance(CurrentLocation, Target.CurrentLocation);

                    int damage = 100;

                    if (distance > 2)
                        damage += 50;


                    if (distance > 4)
                        damage += 50;

                    AttackMagic(MagicType.ThunderBolt, Element.Lightning, false, damage);
                }
            }

            base.ProcessTarget();
        }

        protected override void Attack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            UpdateAttackTime();

            int damage = GetDC();

            if (SEnvir.Random.Next(10) == 0)
                damage *= 2;

            ActionList.Add(new DelayedAction(
                               SEnvir.Now.AddMilliseconds(400),
                               ActionType.DelayAttack,
                               Target,
                               damage,
                               AttackElement));
        }
    }
}

