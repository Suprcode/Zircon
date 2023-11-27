using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.ChangeOfSeasons)]
    public class ChangeOfSeasons : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public ChangeOfSeasons(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //Skill removed from official
        }
    }
}
