using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AugmentExplosiveTalisman)]
    public class AugmentExplosiveTalisman : MagicObject
    {
        protected override Element Element => Element.None;

        public AugmentExplosiveTalisman(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //Custom Skill
        }
    }
}
