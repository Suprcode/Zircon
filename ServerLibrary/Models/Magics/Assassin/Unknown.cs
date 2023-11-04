using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Unknown)]
    public class Unknown : MagicObject
    {
        protected override Element Element => Element.None;

        public Unknown(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Icon - 494, poisons enemy, preventing recovery and continually damaging
            //MagicEx7 - 1100
        }
    }
}