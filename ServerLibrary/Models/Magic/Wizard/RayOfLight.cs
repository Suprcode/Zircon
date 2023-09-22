using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magic
{
    [MagicType(MagicType.RayOfLight)]
    public class RayOfLight : MagicObject
    {
        protected override Element Element => Element.None;

        public RayOfLight(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
        }
    }
}
