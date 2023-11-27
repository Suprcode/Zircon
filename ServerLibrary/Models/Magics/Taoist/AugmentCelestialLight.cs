using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AugmentCelestialLight)]
    public class AugmentCelestialLight : MagicObject
    {
        protected override Element Element => Element.None;

        public AugmentCelestialLight(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
