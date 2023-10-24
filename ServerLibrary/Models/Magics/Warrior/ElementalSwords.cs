using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.ElementalSwords)]
    public class ElementalSwords : MagicObject
    {
        protected override Element Element => Element.None;

        public ElementalSwords(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - Throws swords at the enemy. Number of swords increases with training.
            //Icon - 502
        }
    }
}
