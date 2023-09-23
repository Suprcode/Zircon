using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.PledgeOfBlood)]
    public class PledgeOfBlood : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AugmentedSkill => true;

        public PledgeOfBlood(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
