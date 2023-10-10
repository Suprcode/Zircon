using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.IceRain)]
    public class IceRain : MagicObject
    {
        protected override Element Element => Element.None;

        public IceRain(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - blizzard type skill
        }
    }
}
