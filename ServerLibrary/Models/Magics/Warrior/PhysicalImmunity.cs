using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.PhysicalImmunity)]
    public class PhysicalImmunity : MagicObject
    {
        protected override Element Element => Element.None;

        public PhysicalImmunity(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
