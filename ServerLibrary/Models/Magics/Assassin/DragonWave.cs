using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.DragonWave)]
    public class DragonWave : MagicObject
    {
        protected override Element Element => Element.None;

        public DragonWave(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Icon - 542, Augments FlameSplash, uses less MP, does more damage
            //No Anim
        }
    }
}