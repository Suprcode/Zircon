using Library;
using Server.DBModels;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Slaying)]
    public class Slaying : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AttackSkill => true;
        private bool CanPowerAttack;

        public Slaying(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override AttackCast AttackCast(MagicType attackType)
        {
            var response = new AttackCast();

            if (CanPowerAttack && attackType == Type)
            {
                CanPowerAttack = false;
                Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = false });
                response.Cast = true;
                response.Magics.Add(Type);
            }

            if (!CanPowerAttack && SEnvir.Random.Next(5) == 0)
            {
                CanPowerAttack = true;
                Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = true });
            }

            return response;
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower();

            return power;
        }

        public override Stats GetPassiveStats()
        {
            var stats = new Stats
            {
                [Stat.Accuracy] = Magic.Level * 2,
                [Stat.MinDC] = Magic.Level * 2,
                [Stat.MaxDC] = Magic.Level * 2
            };

            return stats;
        }
    }
}
