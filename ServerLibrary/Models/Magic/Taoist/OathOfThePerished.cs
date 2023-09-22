using Library;
using Server.DBModels;

namespace Server.Models.Magic
{
    [MagicType(MagicType.OathOfThePerished)]
    public class OathOfThePerished : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AugmentedSkill => true;

        public OathOfThePerished(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            
        }
    }
}
