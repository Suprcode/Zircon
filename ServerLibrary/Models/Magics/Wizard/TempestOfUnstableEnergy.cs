﻿using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.TempestOfUnstableEnergy)]
    public class TempestOfUnstableEnergy : MagicObject
    {
        protected override Element Element => Element.None;

        public TempestOfUnstableEnergy(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
        }
    }
}
