using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AdvancedRenounce)]
    public class AdvancedRenounce : MagicObject
    {
        protected override Element Element => Element.None;

        public AdvancedRenounce(PlayerObject player, UserMagic magic) : base(player, magic)
        {
        }

        public override Stats GetPassiveStats()
        {
            var stats = new Stats();

            stats[Stat.MCPercent] += (1 + Magic.Level) * 10;

            return stats;
        }
    }
}
