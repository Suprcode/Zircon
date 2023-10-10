using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AugmentReflectDamage)]
    public class AugmentReflectDamage : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AugmentedSkill => true;

        public AugmentReflectDamage(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //increases duration, damage, and success chance of reflect damage
        }
    }
}
