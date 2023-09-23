using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Massacre)]
    public class Massacre : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool HasMassacre => true;

        public Massacre(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
