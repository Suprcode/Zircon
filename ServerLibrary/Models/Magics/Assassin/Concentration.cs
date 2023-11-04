using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Concentration)]
    public class Concentration : MagicObject
    {
        protected override Element Element => Element.None;

        public Concentration(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Icon - 384, increases crit chance
            //MagicEx4 - 2300
            //BuffIcon - 200
        }
    }
}

