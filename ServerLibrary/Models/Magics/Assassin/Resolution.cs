﻿using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Resolution)]
    public class Resolution : MagicObject
    {
        protected override Element Element => Element.None;

        public Resolution(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
