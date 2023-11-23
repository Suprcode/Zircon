using Library;
using Library.Network.ClientPackets;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Discipline)]
    public class Discipline : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AttackSkill => true;

        public Discipline(PlayerObject player, UserMagic magic) : base(player, magic)
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
                [Stat.Accuracy] = Magic.GetPower() / 3,
                [Stat.MinDC] = Magic.GetPower()
            };

            return stats;
        }
    }
}
