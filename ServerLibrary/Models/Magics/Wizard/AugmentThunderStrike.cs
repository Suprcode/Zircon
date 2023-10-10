using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AugmentThunderStrike)]
    public class AugmentThunderStrike : MagicObject
    {
        protected override Element Element => Element.Lightning;
        public override bool AugmentedSkill => true;

        public AugmentThunderStrike(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
