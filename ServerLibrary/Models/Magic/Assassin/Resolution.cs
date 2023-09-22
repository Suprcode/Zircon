using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magic
{
    [MagicType(MagicType.Resolution)]
    public class Resolution : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AugmentedSkill => true;

        public Resolution(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
