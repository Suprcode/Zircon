using Library;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Shuriken)]
    public class Shuriken : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;
        public override bool IgnoreAccuracy => true;
        public decimal SwiftBladeLifeSteal { get; set; }

        public Shuriken(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

    }
}
