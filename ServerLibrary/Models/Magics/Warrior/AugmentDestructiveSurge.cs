using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AugmentDestructiveSurge)]
    public class AugmentDestructiveSurge : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AugmentedSkill => true;

        public AugmentDestructiveSurge(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Line Cham-Ohui
            //1.Line stalemate - Can inflict damage over a wider area than aggravated damage.
            //2.You must be at level 4 in the circuit-advanced martial arts skill to learn it.
            //3.When learning martial arts, Line Cham -Advanced martial arts are deleted.
        }
    }
}
