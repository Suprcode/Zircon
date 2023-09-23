using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

using S = Library.Network.ServerPackets;

namespace Server.Models.Magic
{
    [MagicType(MagicType.Renounce)]
    public class Renounce : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public Renounce(PlayerObject player, UserMagic magic) : base(player, magic)
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
                [Stat.HealthPercent] = -(1 + Magic.Level) * 10,
                [Stat.MCPercent] = (1 + Magic.Level) * 10,
            };
            int health = Player.CurrentHP;

            BuffInfo buff = Player.BuffAdd(BuffType.Renounce, TimeSpan.FromSeconds(30 + Magic.Level * 30), buffStats, false, false, TimeSpan.Zero);

            buff.Stats[Stat.RenounceHPLost] = health - Player.CurrentHP;
            Player.Enqueue(new S.BuffChanged() { Index = buff.Index, Stats = new Stats(buff.Stats) });

            Player.LevelMagic(Magic);
        }
    }
}
