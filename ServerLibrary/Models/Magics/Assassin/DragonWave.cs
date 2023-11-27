using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.DragonWave)]
    public class DragonWave : MagicObject
    {
        protected override Element Element => Element.None;
        

        public DragonWave(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += power * Magic.GetPower() / 100;

            return power;
        }
    }
}