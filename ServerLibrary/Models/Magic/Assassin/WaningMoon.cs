using Library;
using Library.Network.ClientPackets;
using Server.DBModels;
using Server.Envir;

namespace Server.Models.Magic
{
    [MagicType(MagicType.WaningMoon)]
    public class WaningMoon : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AttackSkill => true;

        public WaningMoon(PlayerObject player, UserMagic magic) : base(player, magic)
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
    }
}
