using Library;
using Server.Envir;
using System;

namespace Server.Models.Monsters
{
    public class FlameDemon : MonsterObject
    {
        public int Cast;
        public int Min, Max;
        private DateTime CastTime;

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if ((Cast <= 0 || SEnvir.Now > CastTime) && CanAttack && Functions.InRange(Target.CurrentLocation, CurrentLocation, 8))
            {
                Cast = 3;
                CastTime = SEnvir.Now.AddSeconds(5);
                LineAoE(12, Min, Max, MagicType.MonsterScortchedEarth, Element.Fire);
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
