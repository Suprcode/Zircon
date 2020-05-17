using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class EnragedLordNiJae : LordNiJae
    {
        public DateTime SlaveTime;

        public override void Process()
        {
            base.Process();

            if (Dead || SEnvir.Now < SlaveTime) return;
            
            SlaveTime = SEnvir.Now.AddSeconds(30);
            
            for (int i = MinionList.Count - 1; i >= 0; i--)
            {
                MonsterObject mob = MinionList[i];
                if (mob.CurrentMap == CurrentMap && Functions.InRange(CurrentLocation, mob.CurrentLocation, 10)) continue;

                mob.EXPOwner = null;
                mob.Die();
                mob.Despawn();
            }
            

            SpawnMinions(5 - MinionList.Count, 0, Target);
        }
        public override bool SpawnMinion(MonsterObject mob)
        {
            return mob.Spawn(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, 10));
        }
    }
}
