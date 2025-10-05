using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.PotionMastery)]
    public class PotionMastery : MagicObject
    {
        protected override Element Element => Element.None;

        public PotionMastery(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
