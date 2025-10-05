using Server.Envir;
using System;

namespace Server.Models.Monsters
{
    public class QuartzTurtleSub : MonsterObject
    {
        public DateTime SlaveTime;

        public override void Process()
        {
            base.Process();

            if (Dead) return;

            if (SEnvir.Now >= SlaveTime)
            {
                SlaveTime = SEnvir.Now.AddSeconds(20);

                SpawnMinions(10 - MinionList.Count, 0, Target);
            }
        }
        public override bool SpawnMinion(MonsterObject mob)
        {
            return mob.Spawn(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, 10));
        }
    }
}
