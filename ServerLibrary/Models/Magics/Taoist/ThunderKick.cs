using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.ThunderKick)]
    public class ThunderKick : MagicObject
    {
        protected override Element Element => Element.None;

        public ThunderKick(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
        }
    }
}
