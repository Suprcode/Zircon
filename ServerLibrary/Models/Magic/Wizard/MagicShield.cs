using Library;
using Library.Network.ClientPackets;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;
using System.Linq;

namespace Server.Models.Magic
{
    [MagicType(MagicType.MagicShield)]
    public class MagicShield : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public MagicShield(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            var delay = SEnvir.Now.AddMilliseconds(1100);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            if (Player.Buffs.Any(x => x.Type == BuffType.MagicShield)) return;

            Stats buffStats = new Stats
            {
                [Stat.MagicShield] = 50
            };

            Player.BuffAdd(BuffType.MagicShield, TimeSpan.FromSeconds(30 + Magic.Level * 20 + Player.GetMC() / 2 + Player.Stats[Stat.PhantomAttack] * 2), buffStats, true, false, TimeSpan.Zero);

            Player.LevelMagic(Magic);
        }
    }
}
