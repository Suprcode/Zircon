using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Mindfulness)]
    public class Mindfulness : MagicObject
    {
        protected override Element Element => Element.None;

        public Mindfulness(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Deals a certain amount of soul magic damage. And depending on the target, an additional effect of no attack, no normal attack, or running away is given for a certain period of time.
            //MagicEx3 - 1190, 1210, 1300
        }
    }
}
