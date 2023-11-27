using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Burning)]
    public class Burning : MagicObject
    {
        protected override Element Element => Element.Fire;
        
        public Burning(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
