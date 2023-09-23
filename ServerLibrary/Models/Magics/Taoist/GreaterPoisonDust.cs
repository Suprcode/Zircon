using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.GreaterPoisonDust)]
    public class GreaterPoisonDust : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AugmentedSkill => true;

        public GreaterPoisonDust(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
