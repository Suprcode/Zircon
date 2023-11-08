using Library;
using Library.Network.ClientPackets;
using Server.DBModels;
using Server.Envir;

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
