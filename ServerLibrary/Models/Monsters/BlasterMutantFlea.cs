using Library;
using Server.Envir;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public sealed class BlasterMutantFlea : MonsterObject
    {
        public int AttackRange = 5;

        protected override bool InAttackRange()
        {
            return Target.CurrentMap == CurrentMap &&
                   Target.CurrentLocation != CurrentLocation &&
                   Functions.InRange(CurrentLocation, Target.CurrentLocation, AttackRange);
        }

        protected override void Attack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            Broadcast(new S.ObjectRangeAttack
            {
                ObjectID = ObjectID,
                Direction = Direction,
                Location = CurrentLocation,
                Targets = new List<uint> { Target.ObjectID },
            });

            UpdateAttackTime();

            ActionList.Add(new DelayedAction(
                SEnvir.Now.AddMilliseconds(400 + Functions.Distance(CurrentLocation, Target.CurrentLocation) * Globals.ProjectileSpeed),
                ActionType.DelayAttack,
                Target,
                GetDC(),
                AttackElement));
        }
    }
}
