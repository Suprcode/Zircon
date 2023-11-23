using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.WillowDance)]
    public class WillowDance : MagicObject
    {
        protected override Element Element => Element.None;

        public WillowDance(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override Stats GetPassiveStats()
        {
            var stats = new Stats
            {
                [Stat.Agility] = Magic.GetPower()
            };

            return stats;
        }
    }
}
