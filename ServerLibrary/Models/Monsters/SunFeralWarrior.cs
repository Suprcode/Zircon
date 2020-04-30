using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class SunFeralWarrior : BanyaLeftGuard
    {
        public bool CanSpawn = true;


        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            int result = base.Attacked(attacker, power, element, canReflect, ignoreShield, canCrit);

            if (!CanSpawn || result <= 0) return result;

            CanSpawn = false;

            SpawnMinions(2, 6, null);

            return result;
        }

        public override void RangeAttack()
        {
            if (!Functions.InRange(Target.CurrentLocation, CurrentLocation, Globals.MagicRange))
                return;

            if (SEnvir.Random.Next(2) == 1)
                AttackMagic(MagicType.FireBall, Element.Fire, true);
            else
                AttackAoE(1, MagicType.FireStorm, Element.Fire);
        }
    }
}
