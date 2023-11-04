using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Chain)]
    public class Chain : MagicObject
    {
        protected override Element Element => Element.None;

        public Chain(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Icon - 520, chains monsters together
            //MagicEx7 - 0
            //Particle chain effect, MagicEx7 - 80
        }
    }
}
