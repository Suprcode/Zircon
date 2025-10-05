using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AdvancedPotionMastery)]
    public class AdvancedPotionMastery : MagicObject
    {
        protected override Element Element => Element.None;

        public AdvancedPotionMastery(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //Custom Skill
        }
    }
}
