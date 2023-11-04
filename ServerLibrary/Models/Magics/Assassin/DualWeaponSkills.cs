using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.DualWeaponSkills)]
    public class DualWeaponSkills : MagicObject
    {
        protected override Element Element => Element.None;

        public DualWeaponSkills(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Icon - 464, Using dual weapons does extra damage
            //Passive
        }
    }
}