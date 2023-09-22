using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magic
{
    [MagicType(MagicType.SwirlingBlade)]
    public class SwirlingBlade : MagicObject
    {
        protected override Element Element => Element.None;

        public SwirlingBlade(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
        }
    }
}
