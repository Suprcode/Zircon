using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magic
{
    [MagicType(MagicType.TouchOfTheDeparted)]
    public class TouchOfTheDeparted : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AugmentedSkill => true;

        public TouchOfTheDeparted(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
