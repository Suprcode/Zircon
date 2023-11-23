using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AdventOfDemon)]
    public class AdventOfDemon : MagicObject
    {
        protected override Element Element => Element.None;

        public AdventOfDemon(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override Stats GetPassiveStats()
        {
            var stats = new Stats
            {
                [Stat.MaxAC] = Magic.GetPower()
            };

            return stats;
        }
    }
}
