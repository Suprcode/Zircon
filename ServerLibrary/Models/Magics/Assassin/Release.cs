using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Release)]
    public class Release : MagicObject
    {
        protected override Element Element => Element.None;
        
        public Release(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - BuffIcon - 165
        }
    }
}
