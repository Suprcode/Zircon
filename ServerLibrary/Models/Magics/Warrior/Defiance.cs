using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;
using System.Linq;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Defiance)]
    public class Defiance : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public Defiance(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - Augment BuffIcon 201
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
            if (Player.Buffs.Any(x => x.Type == BuffType.Might))
            {
                Player.BuffRemove(BuffType.Might);
            }

            TimeSpan duration = TimeSpan.FromSeconds(60 + Magic.Level * 30);

            Stats buffStats = new Stats
            {
                [Stat.PhysicalDefencePercent] = 5 + Magic.Level * 5,
                [Stat.MagicDefencePercent] = 5 + Magic.Level * 5
            };

            int offence = 20;

            var augmentDefiance = GetAugmentedSkill(MagicType.AugmentDefiance);

            if (augmentDefiance != null)
            {
                offence = Math.Max(0, 20 - Magic.Level * 5);

                duration += TimeSpan.FromSeconds(10 + Magic.Level * 10);

                Player.LevelMagic(augmentDefiance);
            }

            buffStats[Stat.DCPercent] = -offence;

            Player.BuffAdd(BuffType.Defiance, duration, buffStats, false, false, TimeSpan.Zero);

            Player.LevelMagic(Magic);
        }
    }
}
