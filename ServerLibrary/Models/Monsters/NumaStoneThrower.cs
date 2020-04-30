using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
   public class NumaStoneThrower : SkeletonAxeThrower
    {

        protected override void Attack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Targets = new List<uint> { Target.ObjectID } });

            UpdateAttackTime();

            if (SEnvir.Random.Next(FearRate) == 0)
                FearTime = SEnvir.Now.AddSeconds(FearDuration + SEnvir.Random.Next(4));

            foreach (MapObject target in GetTargets(CurrentMap, Target.CurrentLocation, 2))
            {
                ActionList.Add(new DelayedAction(
                                   SEnvir.Now.AddMilliseconds(400 + Functions.Distance(CurrentLocation, Target.CurrentLocation) * Globals.ProjectileSpeed),
                                   ActionType.DelayAttack,
                                   target,
                                   GetDC(),
                                   AttackElement));
            }

        }
    }
}
