﻿using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.GhostWalk)]
    public class GhostWalk : MagicObject
    {
        protected override Element Element => Element.None;

        public GhostWalk(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
