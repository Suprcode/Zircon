using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.ReigningStep)]
    public class ReigningStep : MagicObject
    {
        protected override Element Element => Element.None;

        public ReigningStep(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
        }
    }
}
