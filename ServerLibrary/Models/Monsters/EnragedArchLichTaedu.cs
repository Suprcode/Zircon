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
    public class EnragedArchLichTaedu : ArchLichTaedu
    {
        private bool CanTeleport = true;
        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            int result = base.Attacked(attacker, power, element, canReflect, ignoreShield, canCrit);

            if (result < 0 || Dead || !CanTeleport || CurrentHP > Stats[Stat.Health] / 2) return result;

            CanTeleport = false;

            TeleportNearby(7, 12);

            return result;
        }


        public DateTime TeleportTime { get; set; }
        public bool Bonus;
        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (!Functions.InRange(Target.CurrentLocation, CurrentLocation, 1) && SEnvir.Now > TeleportTime && CanAttack)
            {
                TeleportTime = SEnvir.Now.AddSeconds(1);

                if (SEnvir.Random.Next(7) == 0)
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
            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            UpdateAttackTime();

            int damage = GetDC();

            if (Bonus)
                damage *= 2;

            ActionList.Add(new DelayedAction(
                SEnvir.Now.AddMilliseconds(400),
                ActionType.DelayAttack,
                Target,
                damage,
                AttackElement));
            Bonus = false;
        }
    }
}
