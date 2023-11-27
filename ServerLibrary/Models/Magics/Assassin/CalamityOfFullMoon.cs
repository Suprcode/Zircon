using Library;
using Server.DBModels;
using Server.Envir;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.CalamityOfFullMoon)]
    public class CalamityOfFullMoon : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AttackSkill => true;
        private bool CanAttack;

        public CalamityOfFullMoon(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override AttackCast AttackCast(MagicType attackType)
        {
            var response = new AttackCast();

            if (CanAttack && attackType == Type)
            {
                CanAttack = false;
                Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = false });
                response.Cast = true;
                response.Magics.Add(Type);
            }

            if (!CanAttack && !Player.Buffs.Any(x => x.Type == BuffType.Cloak))
            {
                if (SEnvir.Random.Next(Globals.MagicMaxLevel + 1) > Magic.Level)
                {
                    CanAttack = true;
                    Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = true });
                }
            }

            return response;
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower();

            return power;
        }
    }
}
