using Library;
using Server.DBModels;

namespace Server.Models.Magics.Taoist
{
    [MagicType(MagicType.CorpseExploder)]
    public class CorpseExploder : MagicObject
    {
        protected override Element Element => Element.None;

        public CorpseExploder(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - Explodes dead bodies causes area damage
            //Icon - 490
        }
    }
}
