using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Infection)]
    public class Infection : MagicObject
    {
        protected override Element Element => Element.None;

        public Infection(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
