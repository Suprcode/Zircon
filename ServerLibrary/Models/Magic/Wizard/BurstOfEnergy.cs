using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magic
{
    [MagicType(MagicType.BurstOfEnergy)]
    public class BurstOfEnergy : MagicObject
    {
        protected override Element Element => Element.None;

        public BurstOfEnergy(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
        }
    }
}
