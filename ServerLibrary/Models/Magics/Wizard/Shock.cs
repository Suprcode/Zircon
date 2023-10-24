using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Shock)]
    public class Shock : MagicObject
    {
        protected override Element Element => Element.Lightning;

        public Shock(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - adds shock to all electric spells
        }
    }
}
