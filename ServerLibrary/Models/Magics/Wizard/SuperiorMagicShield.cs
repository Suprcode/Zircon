using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;
using System.Linq;

namespace Server.Models.Magics
{
    [MagicType(MagicType.SuperiorMagicShield)]
    public class SuperiorMagicShield : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public SuperiorMagicShield(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            var delay = SEnvir.Now.AddMilliseconds(1100);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            if (Player.Buffs.Any(x => x.Type == BuffType.SuperiorMagicShield)) return;

            Player.BuffRemove(BuffType.MagicShield);

            Stats buffStats = new Stats
            {
                [Stat.SuperiorMagicShield] = (int)(Player.Stats[Stat.Mana] * (0.25F + Magic.Level * 0.05F))
            };

            Player.BuffAdd(BuffType.SuperiorMagicShield, TimeSpan.MaxValue, buffStats, true, false, TimeSpan.Zero);

            Player.LevelMagic(Magic);
        }
    }
}
