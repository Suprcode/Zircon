using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AugmentEvilSlayer)]
    public class AugmentEvilSlayer : MagicObject
    {
        protected override Element Element => Element.None;
        
        public AugmentEvilSlayer(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //Custom Skill
        }
    }
}
