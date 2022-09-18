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
        public override bool AttackSkill => true;

        public Slaying(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override AttackCast AttackCast(MagicType attackType)
        {
            var response = new AttackCast();

            if (Player.Level < Magic.Info.NeedLevel1)
                return response;

            if (Player.CanPowerAttack && attackType == Type)
            {
                Player.CanPowerAttack = false;
                Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = false });
                response.Cast = true;
                response.Magics.Add(Type);
            }

            if (!Player.CanPowerAttack && SEnvir.Random.Next(5) == 0)
            {
                Player.CanPowerAttack = true;
                Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = true });
            }

            return response;
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
