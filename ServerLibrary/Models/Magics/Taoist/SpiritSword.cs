using Library;
using Library.Network.ClientPackets;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.SpiritSword)]
    public class SpiritSword : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AttackSkill => true;

        public SpiritSword(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override AttackCast AttackCast(MagicType attackType)
        {
            var response = new AttackCast();

            response.Magics.Add(Type);

            return response;
        }

        public override Stats GetPassiveStats()
        {
            var stats = new Stats
            {
                [Stat.Accuracy] = Magic.GetPower()
            };

            return stats;
        }
    }
}
