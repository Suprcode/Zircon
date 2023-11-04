using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.ChainOfFire)]
    public class ChainOfFire : MagicObject
    {
        protected override Element Element => Element.None;

        public ChainOfFire(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Icon - 520, Augments Chain and siphons targets damage to the others. At high level does explosive damage
            //MagicEx7 - 41
        }
    }
}