using Server.Envir;
using System;

namespace Server.Models.Monsters
{
    public class ChaosKnight : MonsterObject
    {
        public DateTime CastTime;

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (InAttackRange() && CanAttack && SEnvir.Now > CastTime)
            {
                CastTime = SEnvir.Now.AddSeconds(20);

                PoisonousCloud();
            }

            base.ProcessTarget();
        }
    }
}
