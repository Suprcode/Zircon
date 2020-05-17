using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Library.SystemModels;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class QuartzTree : TreeMonster
    {
        public DateTime SlaveTime;

        private bool SubSpawned = false;
        public MonsterInfo SubBossInfo;

        public override void Process()
        {
            base.Process();

            if (Dead) return;

            if (!SubSpawned && CurrentHP < Stats[Stat.Health] / 4)
            {
                SubSpawned = true;
                SpawnSub();
            }

            if (SEnvir.Now >= SlaveTime)
            {
                SlaveTime = SEnvir.Now.AddSeconds(60);

                for (int i = MinionList.Count - 1; i >= 0; i--)
                {
                    MonsterObject mob = MinionList[i];
                    if (mob.CurrentMap == CurrentMap && Functions.InRange(CurrentLocation, mob.CurrentLocation, 30)) continue;

                    mob.EXPOwner = null;
                    mob.Die();
                    mob.Despawn();
                }


                Broadcast(new S.ObjectShow { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                SpawnMinions(10 - MinionList.Count, 0, Target);
            }
        }
        public void SpawnSub()
        {
            if (SubBossInfo == null) return;

            MonsterObject mob = GetMonster(SubBossInfo);

            if (!SpawnMinion(mob)) return;

            mob.Target = Target;
        }

        public override bool SpawnMinion(MonsterObject mob)
        {
            return mob.Spawn(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, 5));
        }

        public override void ProcessRegen()
        {
        }
    }
}
