using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magic
{
    [MagicType(MagicType.MaelstromBlade)]
    public class MaelstromBlade : MagicObject
    {
        protected override Element Element => Element.None;

        public MaelstromBlade(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
        }
    }
}
