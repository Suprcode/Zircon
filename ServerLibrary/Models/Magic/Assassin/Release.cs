using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magic
{
    [MagicType(MagicType.Release)]
    public class Release : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AugmentedSkill => true;

        public Release(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            
        }
    }
}
