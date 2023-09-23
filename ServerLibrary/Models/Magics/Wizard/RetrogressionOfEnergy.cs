using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.RetrogressionOfEnergy)]
    public class RetrogressionOfEnergy : MagicObject
    {
        protected override Element Element => Element.None;

        public RetrogressionOfEnergy(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
        }
    }
}
