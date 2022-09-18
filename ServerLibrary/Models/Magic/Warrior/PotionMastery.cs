using Library;
using Library.Network.ClientPackets;
using Server.DBModels;
using Server.Envir;

namespace Server.Models.Magic
{
    [MagicType(MagicType.PotionMastery)]
    public class PotionMastery : MagicObject
    {
        public override Element Element => Element.None;

        public PotionMastery(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }
    }
}
