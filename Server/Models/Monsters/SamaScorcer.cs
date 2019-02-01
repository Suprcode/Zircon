using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    class SamaScorcer : MonsterObject
    {
        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;

            return Target.CurrentLocation != CurrentLocation && Functions.InRange(CurrentLocation, Target.CurrentLocation, 3);
        }

        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            foreach (MapObject ob in CurrentMap.Objects) //Expensive.
            {
                if (ob.Race != ObjectType.Monster) continue;

                MonsterObject mob = (MonsterObject)ob;

                if (mob.MonsterInfo.Flag == MonsterFlag.BloodStone) return 0;
            }

            return base.Attacked(attacker, power, element, canReflect, ignoreShield, canCrit);
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (!CanAttack) return;

            if (SEnvir.Random.Next(5) == 0 && !Functions.InRange(Target.CurrentLocation, CurrentLocation, 5))
                Target.Teleport(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, 2));

            base.ProcessTarget();
        }
    }
}
