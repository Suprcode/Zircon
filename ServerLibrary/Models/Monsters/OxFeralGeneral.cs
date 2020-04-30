using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class OxFeralGeneral : MonsterObject
    {
        public int Cast;
        private DateTime CastTime;

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if ((Cast <= 0 || SEnvir.Now > CastTime) && CanAttack && Functions.InRange(Target.CurrentLocation, CurrentLocation, 8))
            {
                Cast = 3;
                CastTime = SEnvir.Now.AddSeconds(5);

                switch (SEnvir.Random.Next(4))
                {
                    case 0:
                        LineAoE(12, -2, 2, MagicType.GreaterFrozenEarth, Element.Ice);
                        break;
                    case 1:
                        LineAoE(12, -2, 2, MagicType.MonsterScortchedEarth, Element.Fire);
                        break;
                    case 2:
                        LineAoE(12, -2, 2, MagicType.LightningBeam, Element.Lightning);
                        break;
                    case 3:
                        LineAoE(12, -2, 2, MagicType.BlowEarth, Element.Wind);
                        break;
                }
            }

            base.ProcessTarget();
        }
        protected override void Attack()
        {
            Cast--;

            base.Attack();
        }
    }
}
