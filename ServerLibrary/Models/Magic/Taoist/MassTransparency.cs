using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magic
{
    [MagicType(MagicType.MassTransparency)]
    public class MassTransparency : MagicObject
    {
        protected override Element Element => Element.None;

        public MassTransparency(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
        }
    }
}
