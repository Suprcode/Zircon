using Library;
using Library.Network.ClientPackets;
using Server.DBModels;
using Server.Envir;

namespace Server.Models.Magic
{
    [MagicType(MagicType.CalamityOfFullMoon)]
    public class CalamityOfFullMoon : MagicObject
    {
        public override Element Element => Element.None;
        public override bool AttackSkill => true;

        public CalamityOfFullMoon(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override AttackCast AttackCast(MagicType attackType)
        {
            var response = new AttackCast();

            if (Player.Level < Magic.Info.NeedLevel1)
                return response;

            if (SEnvir.Random.Next(2) != 0)
                return response;

            response.Magics.Add(Type);

            return response;
        }

        public override int ModifyPower1(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower();

            return power;
        }
    }
}
