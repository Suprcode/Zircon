using Library;
using Server.DBModels;
using System.Drawing;

namespace Server.Models.Magic
{
    [MagicType(MagicType.MirrorImage)]
    public class MirrorImage : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public MirrorImage(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast();

            return response;
        }

        public override void MagicComplete(params object[] data)
        {

        }
    }
}
