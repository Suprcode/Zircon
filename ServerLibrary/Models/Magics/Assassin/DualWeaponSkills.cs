using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.DualWeaponSkills)]
    public class DualWeaponSkills : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AttackSkill => true;   

        public DualWeaponSkills(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override AttackCast AttackCast(MagicType attackType)
        {
            var response = new AttackCast();

            if (Player.Equipment[(int)EquipmentSlot.Weapon]?.Info.ItemEffect == ItemEffect.DualWield)
            {
                response.Magics.Add(Type);
            }

            return response;
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += power * Magic.GetPower() / 100;

            return power;
        }
    }
}