using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Might)]
    public class Might : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public Might(PlayerObject player, UserMagic magic) : base(player, magic)
        {

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
            if (Player.Buffs.Any(x => x.Type == BuffType.Defiance))
            {
                Player.BuffRemove(BuffType.Defiance);
            }

            var duration = TimeSpan.FromSeconds(60 + Magic.Level * 30);

            var amount = 5 + Magic.Level * 5;

            Stats buffStats = new Stats
            {
                [Stat.DCPercent] = amount,
                [Stat.MagicDefencePercent] = -amount,
                [Stat.PhysicalDefencePercent] = -amount
            };

            Player.BuffAdd(BuffType.Might, duration, buffStats, false, false, TimeSpan.Zero);

            Player.LevelMagic(Magic);
        }

        public override void AttackCompletePassive(MapObject target, List<MagicType> types)
        {
            if (Player.Buffs.Any(x => x.Type == BuffType.Might))
            {
                Player.LevelMagic(Magic);
            }
        }
    }
}
