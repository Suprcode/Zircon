using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;
using System.Linq;

namespace Server.Models.Magic
{
    [MagicType(MagicType.Might)]
    public class Might : MagicObject
    {
        public override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public Might(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            //TODO!!
            //p.Direction = MirDirection.Down;

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            if (Player.Buffs.Any(x => x.Type == BuffType.Defiance))
            {
                Player.BuffRemove(BuffType.Defiance);
                Player.ChangeHP(-(Player.CurrentHP / 2));
            }

            int value = 4 + Magic.Level * 6;

            Stats buffStats = new Stats
            {
                [Stat.MinDC] = value,
                [Stat.MaxDC] = value,
            };

            Player.BuffAdd(BuffType.Might, TimeSpan.FromSeconds(60 + Magic.Level * 30), buffStats, false, false, TimeSpan.Zero);

            Player.LevelMagic(Magic);
        }
    }
}
