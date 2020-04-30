using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class JinhwanSpirit : MonsterObject
    {
        public int SpawnChance = 100;

        public override void Die()
        {
            base.Die();
            
            if (SEnvir.Random.Next(SpawnChance) > 0) return;

            SpawnMinions(20, 10, null);
        }
    }
}
