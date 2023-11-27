using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AugmentDefiance)]
    public class AugmentDefiance : MagicObject
    {
        protected override Element Element => Element.None;

        public AugmentDefiance(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //Official anim is red skull, but leaving as default defiance anim otherwise a waste
        }
    }
}
