using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Seance)]
    public class Seance : MagicObject
    {
        protected override Element Element => Element.None;

        public Seance(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Depending on the talisman worn, one's abilities are strengthened and the effectiveness of martial arts of that attribute is increased.
            //BuffIcon - 394
            //MagicEx2 1580
        }
    }
}
