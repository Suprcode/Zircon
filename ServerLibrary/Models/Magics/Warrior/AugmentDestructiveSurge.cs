using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AugmentDestructiveSurge)]
    public class AugmentDestructiveSurge : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AttackSkill => true;
        

        public AugmentDestructiveSurge(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower();

            return power;
        }
    }
}
