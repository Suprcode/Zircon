using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magic
{
    [MagicType(MagicType.GreaterHolyStrike)]
    public class GreaterHolyStrike : MagicObject
    {
        protected override Element Element => Element.None;

        public GreaterHolyStrike(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
        }
    }
}
