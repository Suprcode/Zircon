using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class MirrorImage : MonsterObject
    {
        public Element Element { get; set; }


        public override void ProcessSearch()
        {
        }

        public override bool CanAttackTarget(MapObject ob)
        {
            return false;
        }
        

    }
}
