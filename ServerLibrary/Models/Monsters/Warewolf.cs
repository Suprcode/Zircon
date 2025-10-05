using Library;
using Server.Envir;
using System;

namespace Server.Models.Monsters
{
    public class Warewolf : MonsterObject
    {
        public DateTime CastTime;

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (SEnvir.Now > CastTime && CanAttack && Functions.InRange(Target.CurrentLocation, CurrentLocation, 8))
            {
                CastTime = SEnvir.Now.AddSeconds(10);
                LineAoE(8, -2, 2, MagicType.GreaterFrozenEarth, Element.Ice);
            }

            base.ProcessTarget();
        }
    }
}
