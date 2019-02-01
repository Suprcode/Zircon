using System;
using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class ZumaKing : ZumaGuardian
    {
        public int MaxStage = 7;
        public int Stage;
        
        public ZumaKing()
        {
            AvoidFireWall = false;
        }
        

        protected override void OnSpawned()
        {
            base.OnSpawned();

            Stage = MaxStage;
        }

        public override void Wake()
        {
            base.Wake();

            ActionTime = SEnvir.Now.AddSeconds(2);
        }

        public override void Process()
        {
            base.Process();

            if (Dead) return;

            if (CurrentHP*MaxStage/Stats[Stat.Health] >= Stage || Stage <= 0) return;

            Stage--;
            SpawnMinions(4, 8, Target);
        }
        
        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (!InAttackRange())
            {
                if (CanAttack)
                {
                    if (SEnvir.Random.Next(5) == 0)
                        RangeAttack();
                }


                if (CurrentLocation == Target.CurrentLocation)
                {
                    MirDirection direction = (MirDirection)SEnvir.Random.Next(8);
                    int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                    for (int d = 0; d < 8; d++)
                    {
                        if (Walk(direction)) break;

                        direction = Functions.ShiftDirection(direction, rotation);
                    }
                }
                else
                    MoveTo(Target.CurrentLocation);
            }

            if (!CanAttack) return;

            if (SEnvir.Random.Next(5) > 0)
            {
                if (InAttackRange())
                    Attack();
            }
            else RangeAttack();
        }

        public void RangeAttack()
        {
            switch (SEnvir.Random.Next(3))
            {
                case 0:
                    FireWall();
                    break;
                default:
                    LineAoE(12, -2, 2, MagicType.MonsterScortchedEarth, Element.Fire);
                    break;
            }
        }
    }
}
