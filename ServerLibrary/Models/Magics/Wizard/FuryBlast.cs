using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.FuryBlast)]
    public class FuryBlast : MagicObject
    {
        protected override Element Element => Element.None;

        public FuryBlast(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
        }
    }
}
