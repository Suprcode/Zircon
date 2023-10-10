using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AugmentDefiance)]
    public class AugmentDefiance : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AugmentedSkill => true;

        public AugmentDefiance(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Increases HP,AC,MA of Defiance
        }
    }
}
