using Library;
using Server.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models.Magics.Assassin
{
    [MagicType(MagicType.DragonBlood)]
    public class DragonBlood : MagicObject
    {
        protected override Element Element => Element.None;

        public DragonBlood(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Icon - 382
            // Inflicts poison on the target hit by your attacks, dealing a percentage of damage every 2 seconds for 10 seconds. Up to 4 times
            //Zee effects stack.You must wear a poison bottle for free use
            //Passive?
        }
    }
}
