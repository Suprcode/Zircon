using Library;
using Server.DBModels;

namespace Server.Models.Magic
{
    [MagicType(MagicType.Swordsmanship)]
    public class Swordsmanship : MagicObject
    {
        public override Element Element => Element.None;
        public override bool PhysicalSkill => true;
        public override bool HasAttackAnimation => false;

        public Swordsmanship(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override bool CanAttack(MagicType attackType)
        {
            if (Player.Level < Magic.Info.NeedLevel1)
                return false;

            return true;
        }

        public override Stats GetPassiveStats()
        {
            var stats = new Stats();

            stats[Stat.Accuracy] = Magic.GetPower();

            return stats;
        }
    }
}
