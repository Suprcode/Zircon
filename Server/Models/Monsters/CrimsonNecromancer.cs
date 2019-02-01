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
    public class CrimsonNecromancer : MonsterObject
    {
        public DateTime DebuffTime;
        
        public override void ProcessTarget()
        {
            if (CanAttack && SEnvir.Now > DebuffTime)
            {
                DebuffTime = SEnvir.Now.AddSeconds(10);

                List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, 3);

                if (targets.Count > 0)
                {
                    Direction = Functions.DirectionFromPoint(CurrentLocation, targets[0].CurrentLocation);

                    Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                    UpdateAttackTime();

                    foreach (MapObject target in targets)
                        target.BuffAdd(BuffType.MagicWeakness, TimeSpan.FromSeconds(10), null, false, false, TimeSpan.Zero);
                }
            }

            base.ProcessTarget();
        }
    }
}
