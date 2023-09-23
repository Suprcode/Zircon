using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Evasion)]
    public class Evasion : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public Evasion(PlayerObject player, UserMagic magic) : base(player, magic)
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

            Stats buffStats = new Stats
            {
                [Stat.EvasionChance] = 4 + Magic.Level * 2,
            };

            ob.BuffAdd(BuffType.Evasion, TimeSpan.FromSeconds(Magic.GetPower()), buffStats, false, false, TimeSpan.Zero);
        }

        public override void MagicFinalise()
        {

        }
    }
}
