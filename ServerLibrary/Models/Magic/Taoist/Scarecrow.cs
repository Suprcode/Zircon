using Library;
using Server.DBModels;

namespace Server.Models.Magic
{
    [MagicType(MagicType.Scarecrow)]
    public class Scarecrow : MagicObject
    {
        protected override Element Element => Element.None;

        public Scarecrow(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
        }
    }
}
