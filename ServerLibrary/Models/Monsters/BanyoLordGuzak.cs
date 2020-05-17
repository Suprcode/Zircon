using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class BanyoLordGuzak : PachontheChaosbringer
    {
        public DateTime SlaveTime, PuriTime;

        public override void Process()
        {
            base.Process();

            if (Dead) return;

            if (SEnvir.Now > PuriTime)
            {
                PuriTime = SEnvir.Now.AddSeconds(20);

                foreach (MapObject ob in GetTargets(CurrentMap, CurrentLocation, Config.MaxViewRange))
                    Purify(ob);
            }

            if (SEnvir.Now >= SlaveTime)
            {
                SlaveTime = SEnvir.Now.AddSeconds(45);

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
        }
        public override bool SpawnMinion(MonsterObject mob)
        {
            return mob.Spawn(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, 10));
        }
    }
}
