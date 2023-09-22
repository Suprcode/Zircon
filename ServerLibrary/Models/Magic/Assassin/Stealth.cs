using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magic
{
    [MagicType(MagicType.Stealth)]
    public class Stealth : MagicObject
    {
        protected override Element Element => Element.None;

        public Stealth(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
        }
    }
}
