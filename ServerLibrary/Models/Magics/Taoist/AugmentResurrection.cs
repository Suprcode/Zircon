using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AugmentResurrection)]
    public class AugmentResurrection : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AugmentedSkill => true;

        public AugmentResurrection(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //Custom Skill
        }
    }
}
