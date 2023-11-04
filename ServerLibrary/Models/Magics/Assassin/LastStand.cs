using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.LastStand)]
    public class LastStand : MagicObject
    {
        protected override Element Element => Element.None;

        public LastStand(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Icon - 448, increases strength when reaching low hp, automatic
            //Passive
            //BuffIcon - 204
            //No anim?
        }
    }
}
