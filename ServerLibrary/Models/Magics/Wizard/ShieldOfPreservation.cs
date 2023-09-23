using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.ShieldOfPreservation)]
    public class ShieldOfPreservation : MagicObject
    {
        protected override Element Element => Element.None;

        public ShieldOfPreservation(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
        }
    }
}
