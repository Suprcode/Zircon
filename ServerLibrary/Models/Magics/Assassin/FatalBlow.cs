using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.FatalBlow)]
    public class FatalBlow : MagicObject
    {
        protected override Element Element => Element.None;

        public FatalBlow(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Passive
            //Icon - 474, 	When the opponent's health is below 30%, damage to the target increases.
            //Attack power increases depending on the training level. (The activation itself is 100% activated)
            //MagicEx10 - 600
        }
    }
}
