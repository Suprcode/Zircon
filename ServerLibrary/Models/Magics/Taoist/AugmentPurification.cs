using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AugmentPurification)]
    public class AugmentPurification : MagicObject
    {
        protected override Element Element => Element.None;

        public AugmentPurification(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //Custom Skill
        }
    }
}
