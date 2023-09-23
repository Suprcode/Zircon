using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.ArtOfShadows)]
    public class ArtOfShadows : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AugmentedSkill => true;

        public ArtOfShadows(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
