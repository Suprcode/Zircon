using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AdventOfDevil)]
    public class AdventOfDevil : MagicObject
    {
        protected override Element Element => Element.None;

        public AdventOfDevil(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            
        }

        public override Stats GetPassiveStats()
        {
            var stats = new Stats
            {
                [Stat.MaxMR] = Magic.GetPower()
            };

            return stats;
        }
    }
}
