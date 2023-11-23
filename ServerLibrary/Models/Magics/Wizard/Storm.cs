using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Storm)]
    public class Storm : MagicObject
    {
        protected override Element Element => Element.Wind;

        public Storm(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //MagicEx7 - 900?
            //TODO - Green storm covers screen, particle effect
            //Icon - 492
            //https://www.youtube.com/watch?v=XRRkVxSLj1E&t=62s
        }
    }
}
