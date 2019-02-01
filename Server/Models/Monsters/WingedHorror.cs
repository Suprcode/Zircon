using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class WingedHorror : UmaKing
    {

        public override void RangeAttack()
        {
            switch (SEnvir.Random.Next(3))
            {
                case 0:
                    MassLightningBall();
                    break;
                case 1:
                    LineAoE(10, 0, 8, MagicType.LightningBeam, Element.Lightning);
                    break;
                case 2:
                    MassThunderBolt();
                    break;
            }
        }
    }
}
