using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AugmentReflectDamage)]
    public class AugmentReflectDamage : MagicObject
    {
        protected override Element Element => Element.None;

        public AugmentReflectDamage(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
