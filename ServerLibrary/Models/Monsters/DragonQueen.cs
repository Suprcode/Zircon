using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Library.SystemModels;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class DragonQueen : YumgonWitch
    {
        public MonsterInfo DragonLordInfo;
        public DateTime SlaveTime;

        public override Element AoEElement
        {
            get => AttackElement;
            set { }
        }


        public override void Process()
        {
            base.Process();

            if (Dead || SEnvir.Now < SlaveTime) return;

            SlaveTime = SEnvir.Now.AddSeconds(15);

            for (int i = MinionList.Count - 1; i >= 0; i--)
            {
                MonsterObject mob = MinionList[i];
                if (mob.CurrentMap == CurrentMap && Functions.InRange(CurrentLocation, mob.CurrentLocation, 15)) continue;

                mob.EXPOwner = null;
                mob.Die();
                mob.Despawn();
            }
            
            SpawnMinions(10 - MinionList.Count, 0, Target);
        }
        public override bool SpawnMinion(MonsterObject mob)
        {
            return mob.Spawn(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, 10));
        }

        public override void Die()
        {
            base.Die();

            if (DragonLordInfo == null) return;

            MonsterObject mob = GetMonster(DragonLordInfo);

            mob.Spawn(CurrentMap, CurrentLocation);
        }
    }
}
