using System;
using System.Collections.Generic;
using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class Affliction : MonsterObject
    {
        protected override void Attack()
        {
            SetHP(0);
        }

        public override void Die()
        {
            base.Die();

            List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, 1);
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
