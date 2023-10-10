using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Resolution)]
    public class Resolution : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AugmentedSkill => true;

        public Resolution(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Increases the accuracy of Karma and allows it to ignore your opponent's defense by a set amount. Accuracy and defense ignored increases with training.
        }
    }
}
