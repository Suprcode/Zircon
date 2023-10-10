using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.DefensiveMastery)]
    public class DefensiveMastery : MagicObject
    {
        protected override Element Element => Element.None;

        public DefensiveMastery(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //passive, Chance of getting MaxAC increases with training
        }
    }
}
