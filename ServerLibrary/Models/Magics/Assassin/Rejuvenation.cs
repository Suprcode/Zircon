using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Rejuvenation)]
    public class Rejuvenation : MagicObject
    {
        protected override Element Element => Element.None;

        public Rejuvenation(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
