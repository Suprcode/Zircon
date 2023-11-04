using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Vitality)]
    public class Vitality : MagicObject
    {
        protected override Element Element => Element.None;

        public Vitality(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Icon - 472
            //Passive
            //When your physical strength falls below a certain level, the amount of recovery increases rapidly.
            //(100% activation) The physical strength standard activated (applied) increases depending on the training level.
            //(Applies to all situations where physical strength is recovered (+, increased), such as recovery techniques,
            //recovery medicine, and natural recovery.

            //No anim?
        }
    }
}
