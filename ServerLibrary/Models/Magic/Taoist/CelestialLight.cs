using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;
using System.Linq;

namespace Server.Models.Magic
{
    [MagicType(MagicType.CelestialLight)]
    public class CelestialLight : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public CelestialLight(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target
            };

            if (Player.Buffs.Any(x => x.Type == BuffType.CelestialLight))
            {
                return response;
            }

            response.Ob = null;

            if (!Player.UseAmulet(20, 0))
            {
                response.Cast = false;
                return response;
            }

            response.Targets.Add(target.ObjectID);

            var delay = SEnvir.Now.AddMilliseconds(1500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            if (Player.Buffs.Any(x => x.Type == BuffType.CelestialLight)) return;

            Stats buffStats = new Stats
            {
                [Stat.CelestialLight] = (Magic.Level + 1) * 10,
            };

            Player.BuffAdd(BuffType.CelestialLight, TimeSpan.FromSeconds(Magic.GetPower()), buffStats, true, false, TimeSpan.Zero);

            Player.LevelMagic(Magic);
        }
    }
}
