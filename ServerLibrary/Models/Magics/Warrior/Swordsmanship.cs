using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Swordsmanship)]
    public class Swordsmanship : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AttackSkill => true;

        public Swordsmanship(PlayerObject player, UserMagic magic) : base(player, magic)
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
