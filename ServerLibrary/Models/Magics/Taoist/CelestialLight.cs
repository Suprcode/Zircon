using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;
using System.Linq;

namespace Server.Models.Magics
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

            response.Targets.Add(Player.ObjectID);

            var delay = SEnvir.Now.AddMilliseconds(1500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            if (Player.Buffs.Any(x => x.Type == BuffType.CelestialLight)) return;

            var augmentCelestialLight = GetAugmentedSkill(MagicType.AugmentCelestialLight);
            var hasAugmentCelestialLight = augmentCelestialLight != null && Magic.Level >= 3;

            Stats buffStats = new Stats
            {
                [Stat.CelestialLight] = (Magic.Level + 1) * (hasAugmentCelestialLight ? 12 : 10),
            };

            if (hasAugmentCelestialLight)
            {
                buffStats[Stat.SCPercent] = 2 + augmentCelestialLight.Level * 2;
                buffStats[Stat.MagicDefencePercent] = 2 + augmentCelestialLight.Level * 2;

                Player.LevelMagic(augmentCelestialLight);
            }

            Player.BuffAdd(BuffType.CelestialLight, TimeSpan.FromSeconds(Magic.GetPower()), buffStats, true, false, TimeSpan.Zero);

            Player.LevelMagic(Magic);
        }
    }
}
