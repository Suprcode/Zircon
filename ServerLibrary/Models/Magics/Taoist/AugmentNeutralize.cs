using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AugmentNeutralize)]
    public class AugmentNeutralize : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AugmentedSkill => true;

        public AugmentNeutralize(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
