using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Concentration)]
    public class Concentration : MagicObject
    {
        protected override Element Element => Element.None;

        public Concentration(PlayerObject player, UserMagic magic) : base(player, magic)
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
            Stats buffStats = new Stats
            {
                [Stat.CriticalChance] = 5 + Magic.Level
            };

            Player.BuffAdd(BuffType.Concentration, TimeSpan.FromSeconds(Magic.GetPower()), buffStats, false, false, TimeSpan.Zero);

            Player.LevelMagic(Magic);
        }
    }
}

