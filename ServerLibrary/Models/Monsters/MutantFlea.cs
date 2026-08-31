using Library;
using Server.Envir;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public sealed class MutantFlea : MonsterObject
    {
        public int AttackRange = 7;

        protected override bool InAttackRange()
        {
            return Target.CurrentMap == CurrentMap &&
                   Target.CurrentLocation != CurrentLocation &&
                   Functions.InRange(CurrentLocation, Target.CurrentLocation, AttackRange);
        }

        protected override void Attack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            bool melee = Functions.InRange(CurrentLocation, Target.CurrentLocation, 1);

            if (melee)
            {
                Broadcast(new S.ObjectAttack
                {
                    ObjectID = ObjectID,
                    Direction = Direction,
                    Location = CurrentLocation,
                });
            }
            else
            {
                Broadcast(new S.ObjectRangeAttack
                {
                    ObjectID = ObjectID,
                    Direction = Direction,
                    Location = CurrentLocation,
                    Targets = new List<uint> { Target.ObjectID },
                });
            }

            UpdateAttackTime();

            int delay = melee
                ? 400
                : 400 + Functions.Distance(CurrentLocation, Target.CurrentLocation) * Globals.ProjectileSpeed;

            ActionList.Add(new DelayedAction(
                SEnvir.Now.AddMilliseconds(delay),
                ActionType.DelayAttack,
                Target,
                GetDC(),
                AttackElement));
        }
    }
}
