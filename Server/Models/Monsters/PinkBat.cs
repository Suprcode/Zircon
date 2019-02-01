using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    class PinkBat : MonsterObject
    {
        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (!InAttackRange())
            {
                if (CanAttack)
                {
                    if (SEnvir.Random.Next(2) == 0)
                        RangeAttack();
                }


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
            }

            if (!CanAttack) return;

            if (SEnvir.Random.Next(5) > 0)
            {
                if (InAttackRange())
                    Attack();
            }
            else RangeAttack();
        }

        public virtual void RangeAttack()
        {
            if (Functions.InRange(Target.CurrentLocation, CurrentLocation, Globals.MagicRange))
                AttackMagic(MagicType.PinkFireBall, Element.Phantom, true);
        }
    }
}
