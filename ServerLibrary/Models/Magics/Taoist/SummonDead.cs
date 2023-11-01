using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Necromancy)]
    public class SummonDead : MagicObject
    {
        protected override Element Element => Element.None;

        public SummonDead(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - Summons a corpse from the dead ??? or empowers existing summoned skele ??
            //Icon - 514
            //Mon-49 - 3000
        }
    }
}
