using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magic
{
    [MagicType(MagicType.DragonBreath)]
    public class DragonBreath : MagicObject
    {
        protected override Element Element => Element.None;

        public DragonBreath(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
        }
    }
}
