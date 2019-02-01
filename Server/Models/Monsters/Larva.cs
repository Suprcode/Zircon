using System;
using System.Collections.Generic;
using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class Larva : MonsterObject
    {

        public int Range = 1;
        public override void Process()
        {
            base.Process();

            if (Dead) return;

            if (Target == null)
            {
                SetHP(0);
                return;
            }
        }

        protected override void Attack()
        {
            SetHP(0);
        }

        public override void Die()
        {
            base.Die();

            List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, Range);
            foreach (MapObject target in targets)
            {
                ActionList.Add(new DelayedAction(
                                   SEnvir.Now.AddMilliseconds(800),
                                   ActionType.DelayAttack,
                                   target,
                                   GetDC(),
                                   AttackElement));
            }
        }
    }
}
