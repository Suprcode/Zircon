using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.SoulResonance)]
    public class SoulResonance : MagicObject
    {
        protected override Element Element => Element.None;

        public SoulResonance(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Increase target max HP by depending on your own max HP. Only in groups. Cancelled when ungrouped or dead. Infinite buff.
        }
    }
}
