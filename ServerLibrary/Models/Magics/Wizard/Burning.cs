using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Burning)]
    public class Burning : MagicObject
    {
        protected override Element Element => Element.None;

        public Burning(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - adds burning to all fire spells
        }
    }
}
