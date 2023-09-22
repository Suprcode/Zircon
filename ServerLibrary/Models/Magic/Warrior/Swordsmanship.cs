using Library;
using Server.DBModels;

namespace Server.Models.Magic
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

            if (Player.Level < Magic.Info.NeedLevel1)
                return response;

            response.Cast = true;

            return response;
        }

        public override Stats GetPassiveStats()
        {
            var stats = new Stats();

            stats[Stat.Accuracy] = Magic.GetPower();

            return stats;
        }
    }
}
