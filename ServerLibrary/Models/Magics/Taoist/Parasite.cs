using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Parasite)]
    public class Parasite : MagicObject
    {
        protected override Element Element => Element.None;

        public Parasite(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //poison target, takes damage, then explode surrounding area
        }
    }
}
