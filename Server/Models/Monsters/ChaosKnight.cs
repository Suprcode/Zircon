using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Server.Envir;
using S = Library.Network.ServerPackets;

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
