using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.PledgeOfBlood)]
    public class PledgeOfBlood : MagicObject
    {
        protected override Element Element => Element.None;

        public PledgeOfBlood(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
