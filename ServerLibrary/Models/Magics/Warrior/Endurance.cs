using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Endurance)]
    public class Endurance : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public Endurance(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - Needs anim and sound confirming
        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null,
                Direction = MirDirection.Down
            };

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            Player.BuffAdd(BuffType.Endurance, TimeSpan.FromSeconds(10 + Magic.Level * 5), null, false, false, TimeSpan.Zero);

            Player.LevelMagic(Magic);
        }
    }
}
