using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Freezing)]
    public class Freezing : MagicObject
    {
        protected override Element Element => Element.Ice;

        public Freezing(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - adds freezing to all ice spells
        }
    }
}
