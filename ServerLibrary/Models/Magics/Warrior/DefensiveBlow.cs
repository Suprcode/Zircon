using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.DefensiveBlow)]
    public class DefensiveBlow : MagicObject
    {
        protected override Element Element => Element.None;

        public DefensiveBlow(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - Reduces enemies defense by fixed percent. increases with training.
            //Icon - 488
        }
    }
}
