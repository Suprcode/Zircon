using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Containment)]
    public class HolySlaughter : MagicObject
    {
        protected override Element Element => Element.None;

        public HolySlaughter(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Icon - 440, attacks in a wide radius for 3 seconds, stops them from moving
            //MagicEx2 - 2030, 2040
        }
    }
}
