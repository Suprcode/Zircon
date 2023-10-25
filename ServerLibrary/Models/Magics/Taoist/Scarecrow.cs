using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Scarecrow)]
    public class Scarecrow : MagicObject
    {
        protected override Element Element => Element.None;

        public Scarecrow(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - choose a target, summon a scarecrow in front of the caster - any damage done to the scarecrow is dealt to the target
            //HP of scarecrow increases with level??
        }
    }
}
