using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AugmentPotionMastery)]
    public class AugmentPotionMastery : MagicObject
    {
        protected override Element Element => Element.None;

        public AugmentPotionMastery(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //Custom Skill
        }
    }
}
