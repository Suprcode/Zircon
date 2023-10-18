using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.MagicImmunity)]
    public class MagicImmunity : MagicObject
    {
        protected override Element Element => Element.None;

        public MagicImmunity(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
