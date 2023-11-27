using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AugmentPoisonDust)]
    public class GreaterPoisonDust : MagicObject
    {
        protected override Element Element => Element.None;

        public GreaterPoisonDust(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
