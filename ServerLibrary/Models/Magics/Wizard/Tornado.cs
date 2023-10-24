using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Tornado)]
    public class Tornado : MagicObject
    {
        protected override Element Element => Element.Wind;

        public Tornado(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - Tornado that randomly moves across the screen damaging its path (level 3 moves faster, level 4 pushes back)
            //Icon - 508
        }
    }
}
