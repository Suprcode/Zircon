using Library;
using Server.DBModels;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magic
{
    [MagicType(MagicType.Slaying)]
    public class Slaying : MagicObject
    {
        public override Element Element => Element.None;
        public override bool PhysicalSkill => true;
        public override bool HasAttackAnimation => false;

        public Slaying(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override bool CanAttack(MagicType attackType)
        {
            bool success = false;

            if (Player.Level < Magic.Info.NeedLevel1)
                return success;

            if (Player.CanPowerAttack && attackType == Type)
            {
                Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = Player.CanPowerAttack = false });
                success = true;
            }

            if (!Player.CanPowerAttack && SEnvir.Random.Next(5) == 0)
                Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = Player.CanPowerAttack = true });

            return success;
        }

        public override int ModifyPower1(bool primary, int power, MapObject ob)
        {
            return Magic.GetPower();
        }

        public override Stats GetPassiveStats()
        {
            var stats = new Stats();

            stats[Stat.Accuracy] = Magic.Level * 2;
            stats[Stat.MinDC] = Magic.Level * 2;
            stats[Stat.MaxDC] = Magic.Level * 2;

            return stats;
        }
    }
}
