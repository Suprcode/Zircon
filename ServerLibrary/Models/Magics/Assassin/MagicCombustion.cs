using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.MagicCombustion)]
    public class MagicCombustion : MagicObject
    {
        protected override Element Element => Element.None;

        public MagicCombustion(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Icon - 478, Burns away the target's MP. (Applies only to PvP.)
            //MagicEx3 - 840??
        }
    }
}