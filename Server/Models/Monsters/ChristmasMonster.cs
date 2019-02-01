using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class ChristmasMonster : MonsterObject
    {
        public override bool CanMove => false;
        public override bool CanAttack => false;

        public ChristmasMonster()
        {
            ExtraExperienceRate = 10;
        }

        public override void RefreshStats()
        {
            base.RefreshStats();
        }

        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            return base.Attacked(attacker, 1, element, canReflect, ignoreShield, false, canStruck);
        }

        public override void ProcessNameColour()
        {
            NameColour = Color.AliceBlue;

            if (SEnvir.Now < ShockTime)
                NameColour = Color.Peru;
            else if (SEnvir.Now < RageTime)
                NameColour = Color.Red;
        }

        public override void Die()
        {
            if (SEnvir.Random.Next(15) == 0)
            {

                for (int i = CurrentMap.Objects.Count - 1; i >= 0; i--)
                {
                    MonsterObject mob = CurrentMap.Objects[i] as MonsterObject;

                    if (mob == null) continue;

                    if (mob.PetOwner != null) continue;

                    if (mob is Guard || mob is ChristmasMonster) continue;

                    if (mob.Dead || mob.MoveDelay == 0 || !mob.CanMove) continue;

                    if (mob.Target != null) continue;

                    if (mob.Level >= 300) continue;

                    mob.Teleport(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, 15));
                }
            }

            if (EXPOwner != null)
            {
                List<MapObject> targets = EXPOwner.GetTargets(CurrentMap, CurrentLocation, 18);

                foreach (MapObject mapObject in targets)
                {
                    if (mapObject.Race != ObjectType.Monster) continue;

                    MonsterObject mob = (MonsterObject) mapObject;

                    if (mob.MonsterInfo.IsBoss || mob.Dead) continue;

                    if (mob.EXPOwner != null && mob.EXPOwner != EXPOwner) continue;
                    
                    if (mob is ChristmasMonster) continue;

                    mob.ExtraExperienceRate = Math.Max(mob.ExtraExperienceRate, ExtraExperienceRate);
                    mob.EXPOwner = EXPOwner;
                    mob.SetHP(0);
                }
            }
            

            base.Die();


        }
    }
}
