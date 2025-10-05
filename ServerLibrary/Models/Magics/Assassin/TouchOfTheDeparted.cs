﻿using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.TouchOfTheDeparted)]
    public class TouchOfTheDeparted : MagicObject
    {
        protected override Element Element => Element.None;

        public TouchOfTheDeparted(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
