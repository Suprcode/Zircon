using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Assault)]
    public class Assault : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AugmentedSkill => true;

        public Assault(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
