using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Stealth)]
    public class Stealth : MagicObject
    {
        protected override Element Element => Element.None;

        public Stealth(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Eunryun
            //It is a martial arts skill that consumes 1 stack of the number of stacks before planting to prevent the concealment from
            //being released with a certain probability when performing an act that causes the concealment to be released while in the
            //stealth state.The more you practice, the more likely it is that the concealment status will not be released . .

        }
    }
}
