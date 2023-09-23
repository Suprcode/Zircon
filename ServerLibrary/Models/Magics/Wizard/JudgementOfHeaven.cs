using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.JudgementOfHeaven)]
    public class JudgementOfHeaven : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public JudgementOfHeaven(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            var delay = SEnvir.Now.AddMilliseconds(600);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            Stats buffStats = new Stats
            {
                [Stat.JudgementOfHeaven] = (2 + Magic.Level) * 20,
            };

            Player.BuffAdd(BuffType.JudgementOfHeaven, TimeSpan.FromSeconds(30 + Magic.Level * 30), buffStats, false, false, TimeSpan.Zero);

            Player.LevelMagic(Magic);
        }
    }
}
