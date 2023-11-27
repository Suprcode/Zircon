using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Vitality)]
    public class Vitality : MagicObject
    {
        protected override Element Element => Element.None;
        public bool LowHP
        {
            get { return (Player.CurrentHP * 100 / Player.Stats[Stat.Health]) < 30; }
        }

        public Vitality(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
