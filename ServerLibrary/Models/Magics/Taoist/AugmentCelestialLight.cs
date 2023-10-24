using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AugmentCelestialLight)]
    public class AugmentCelestialLight : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AugmentedSkill => true;

        public AugmentCelestialLight(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - Additional recovery
            //Icon - 462
            //Just provide this ability at level 4 Celestial Light??
        }
    }
}
