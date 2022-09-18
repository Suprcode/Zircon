using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;
using System.Linq;

namespace Server.Models.Magic
{
    [MagicType(MagicType.Defiance)]
    public class Defiance : MagicObject
    {
        public override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public Defiance(PlayerObject player, UserMagic magic) : base(player, magic)
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
            if (Player.Buffs.Any(x => x.Type == BuffType.Might))
            {
                Player.BuffRemove(BuffType.Might);
                Player.ChangeHP(-(Player.CurrentHP / 2));
            }

            Stats buffStats = new Stats
            {
                [Stat.Defiance] = 1,
            };

            Player.BuffAdd(BuffType.Defiance, TimeSpan.FromSeconds(60 + Magic.Level * 30), buffStats, false, false, TimeSpan.Zero);

            Player.LevelMagic(Magic);
        }
    }
}
