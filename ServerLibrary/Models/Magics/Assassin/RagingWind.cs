using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.RagingWind)]
    public class RagingWind : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public RagingWind(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var ob = Player;

            if (ob?.Node == null || !Player.CanHelpTarget(ob)) return;

            ob.BuffAdd(BuffType.RagingWind, TimeSpan.FromSeconds(Magic.GetPower()), null, false, false, TimeSpan.Zero);
        }

        public override void MagicFinalise()
        {

        }
    }
}
