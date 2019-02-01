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
    public class QueenOfDawn : MonsterObject
    {
        public DateTime TeleportTime, TingTime;

        public bool Bonus;

        public override void ProcessAI()
        {
            base.ProcessAI();

            if (Dead) return;

            if (SEnvir.Now > TingTime)
            {
                TingTime = SEnvir.Now.AddMinutes(1);

                List<MapObject> obs = new List<MapObject>(CurrentMap.Objects);

                foreach (MapObject ob in obs)
                {
                    if (!CanAttackTarget(ob)) continue;

                    ob.Teleport(CurrentMap, CurrentMap.GetRandomLocation());
                }
            }
        }
        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (!Functions.InRange(Target.CurrentLocation, CurrentLocation, 1) && SEnvir.Now > TeleportTime && CanAttack && SpawnInfo != null)
            {
                TeleportTime = SEnvir.Now.AddSeconds(1);

                if (SEnvir.Random.Next(10) == 0)
                {
                    MirDirection dir = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
                    Cell cell = null;
                    for (int i = 0; i < 8; i++)
                    {
                        cell = CurrentMap.GetCell(Functions.Move(Target.CurrentLocation, Functions.ShiftDirection(dir, i), 1));

                        if (cell == null || cell.Movements != null)
                        {
                            cell = null;
                            continue;
                        }
                        break;
                    }

                    if (cell != null)
                    {
                        Direction = Functions.DirectionFromPoint(cell.Location, Target.CurrentLocation);
                        Teleport(CurrentMap, cell.Location);
                        Bonus = true;
                    }
                }
            }

            base.ProcessTarget();
        }

        protected override void Attack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            UpdateAttackTime();

            if (SEnvir.Random.Next(5) > 0)
            {
                Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                ActionList.Add(new DelayedAction(
                    SEnvir.Now.AddMilliseconds(400),
                    ActionType.DelayAttack,
                    Target,
                    GetDC(),
                    AttackElement));
            }
            else
            {
                Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Targets = new List<uint> { Target.ObjectID } });

                foreach (MapObject target in GetTargets(CurrentMap, CurrentLocation, 2))
                {
                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(400),
                        ActionType.DelayAttack,
                        target,
                        GetDC(),
                        AttackElement));
                }
            }

        }

    }
}