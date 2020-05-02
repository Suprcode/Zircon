using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class FrostLordHwa : MonsterObject
    {
        public Stat Affinity;

        public DateTime TeleportTime, TingTime;

        public FrostLordHwa()
        {
            switch (SEnvir.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    Affinity = Stat.FireAffinity;
                    break;
                case DayOfWeek.Tuesday:
                    Affinity = Stat.IceAffinity;
                    break;
                case DayOfWeek.Wednesday:
                    Affinity = Stat.LightningAffinity;
                    break;
                case DayOfWeek.Thursday:
                    Affinity = Stat.WindAffinity;
                    break;
                case DayOfWeek.Friday:
                    Affinity = Stat.HolyAffinity;
                    break;
                case DayOfWeek.Saturday:
                    Affinity = Stat.DarkAffinity;
                    break;
                case DayOfWeek.Sunday:
                    Affinity = Stat.PhantomAffinity;
                    break;
            }
        }

        public override void ApplyBonusStats()
        {
            base.ApplyBonusStats();

            Stats[Affinity] = 1;
        }

        public override void ProcessAI()
        {
            base.ProcessAI();


            if (Dead) return;

            if (SEnvir.Now > TingTime)
            {
                TingTime = SEnvir.Now.AddMinutes(1);

                foreach (MapObject ob in GetTargets(CurrentMap, CurrentLocation, Config.MaxViewRange))
                {
                    ob.Teleport(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, Config.MaxViewRange));

                    ob.ApplyPoison(new Poison
                    {
                        Owner = this,
                        Type = PoisonType.Abyss,
                        TickFrequency = TimeSpan.FromSeconds(10),
                        TickCount = 1,
                    });

                    ob.ApplyPoison(new Poison
                    {
                        Owner = this,
                        Type = PoisonType.Silenced,
                        TickFrequency = TimeSpan.FromSeconds(10),
                        TickCount = 1,
                    });

                    ob.ApplyPoison(new Poison
                    {
                        Owner = this,
                        Type = PoisonType.Red,
                        TickFrequency = TimeSpan.FromSeconds(10),
                        TickCount = 1,
                    });

                    ob.ApplyPoison(new Poison
                    {
                        Owner = this,
                        Type = PoisonType.WraithGrip,
                        TickFrequency = TimeSpan.FromSeconds(5),
                        TickCount = 1,
                    });
                }



            }
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (!Functions.InRange(Target.CurrentLocation, CurrentLocation, 3) && SEnvir.Now > TeleportTime)
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

                    TeleportTime = SEnvir.Now.AddSeconds(5);
                }
            }
            

            if (Target.Race == ObjectType.Monster)
            {
                Target.SetHP(0);
                return;
            }

            base.ProcessTarget();
        }

    }
}
