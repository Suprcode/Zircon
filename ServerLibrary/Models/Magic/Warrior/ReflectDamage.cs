using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magic
{
    [MagicType(MagicType.ReflectDamage)]
    public class ReflectDamage : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public ReflectDamage(PlayerObject player, UserMagic magic) : base(player, magic)
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
            Stats buffStats = new Stats
            {
                [Stat.ReflectDamage] = 5 + Magic.Level * 3,
            };

            Player.BuffAdd(BuffType.ReflectDamage, TimeSpan.FromSeconds(15 + Magic.Level * 10), buffStats, false, false, TimeSpan.Zero);

            Player.LevelMagic(Magic);
        }
    }
}
