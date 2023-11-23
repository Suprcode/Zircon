using Library;
using Library.Network.ClientPackets;
using Server.DBModels;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.WaningMoon)]
    public class WaningMoon : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AttackSkill => true;

        public override int Order => 999;

        public WaningMoon(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override AttackCast AttackCast(MagicType attackType)
        {
            var response = new AttackCast();

            if (attackType != MagicType.None)
                return response;

            if (SEnvir.Random.Next(Globals.MagicMaxLevel + 1) > Magic.Level)
                return response;

            response.Magics.Add(Type);

            if (SEnvir.Random.Next(2) == 0)
                Player.Broadcast(new S.ObjectSound { ObjectID = Player.ObjectID, Magic = Type });

            return response;
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower();

            return power;
        }
    }
}
