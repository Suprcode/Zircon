using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.LightningStrike)]
    public class LightningStrike : MagicObject
    {
        protected override Element Element => Element.None;

        public LightningStrike(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - fire bounce for lightning
        }
    }
}
