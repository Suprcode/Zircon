using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.BurningFire)]
    public class BurningFire : MagicObject
    {
        protected override Element Element => Element.None;

        public BurningFire(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Icon 456, Put a sword in the ground for 5 seconds, damages nearby targets
            //MagicEx6 - 900, 1000, 1100
        }
    }
}