﻿using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.EmpoweredHealing)]
    public class EmpoweredHealing : MagicObject
    {
        protected override Element Element => Element.None;

        public EmpoweredHealing(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
