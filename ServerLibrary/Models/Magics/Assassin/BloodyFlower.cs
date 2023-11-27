using Library;
using Library.Network.ClientPackets;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.BloodyFlower)]
    public class BloodyFlower : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AttackSkill => true;

        public BloodyFlower(PlayerObject player, UserMagic magic) : base(player, magic)
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
                [Stat.LifeSteal] = Magic.GetPower()
            };

            return stats;
        }
    }
}
