using Library;
using Server.DBModels;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.FatalBlow)]
    public class FatalBlow : MagicObject
    {
        protected override Element Element => Element.None;

        public override bool AttackSkill => true;

        public FatalBlow(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override AttackCast AttackCast(MagicType attackType)
        {
            var response = new AttackCast();

            response.Magics.Add(Type);

            return response;
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            if (ob.CurrentHP < ob.Stats[Stat.Health] * 100 / 30)
            {
                power += power * Magic.GetPower() / 100;

                Player.LevelMagic(Magic);
            }

            return power;
        }

        public override void AttackComplete(MapObject target)
        {
            
        }
    }
}
