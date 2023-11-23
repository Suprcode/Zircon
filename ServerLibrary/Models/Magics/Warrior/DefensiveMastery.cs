using Library;
using Library.Network.ClientPackets;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.DefensiveMastery)]
    public class DefensiveMastery : MagicObject
    {
        protected override Element Element => Element.None;

        public DefensiveMastery(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override Stats GetPassiveStats()
        {
            var stats = new Stats
            {
                [Stat.DefensiveMastery] = Magic.GetPower()
            };

            return stats;
        }
    }
}
