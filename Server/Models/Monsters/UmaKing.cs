using Library;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class UmaKing : MonsterObject
    {
        public int RangeChance = 5;

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (!InAttackRange())
            {
                if (CanAttack)
                {
                    if (SEnvir.Random.Next(RangeChance) == 0)
                        RangeAttack();
                }


                if (CurrentLocation == Target.CurrentLocation)
                {
                    MirDirection direction = (MirDirection) SEnvir.Random.Next(8);
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

            if (SEnvir.Random.Next(RangeChance) > 0)
            {
                if (InAttackRange())
                    Attack();
            }
            else RangeAttack();
        }

        public virtual void RangeAttack()
        {
            switch (SEnvir.Random.Next(3))
            {
                case 0: 
                    MassLightningBall();
                    break;
                default:
                    MassThunderBolt();
                    break;
            }
        }
    }
}
