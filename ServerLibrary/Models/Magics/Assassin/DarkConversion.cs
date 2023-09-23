using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;
using System.Linq;

namespace Server.Models.Magics
{
    [MagicType(MagicType.DarkConversion)]
    public class DarkConversion : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public DarkConversion(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override bool CheckCost()
        {
            if (Player.Buffs.Any(x => x.Type == BuffType.DarkConversion)) return true;

            if (Magic.Cost > Player.CurrentMP)
            {
                return false;
            }

            return true;
        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            if (Player.Buffs.Any(x => x.Type == BuffType.DarkConversion))
            {
                return response;
            }

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var ob = Player;

            if (ob?.Node == null || !Player.CanHelpTarget(ob) || ob.Buffs.Any(x => x.Type == BuffType.DarkConversion)) return;

            Stats buffStats = new Stats
            {
                [Stat.DarkConversion] = Magic.GetPower(),
            };

            ob.BuffAdd(BuffType.DarkConversion, TimeSpan.MaxValue, buffStats, false, false, TimeSpan.FromSeconds(2));
        }

        public override void MagicConsume()
        {
            if (Player.Buffs.Any(x => x.Type == BuffType.DarkConversion))
            {
                Player.BuffRemove(BuffType.DarkConversion);
                return;
            }

            Player.ChangeMP(-Magic.Cost);
        }

        public override void MagicFinalise()
        {
            
        }
    }
}
