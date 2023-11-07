using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Spiritualism)]
    public class Spiritualism : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public override Element GetElement(Element element)
        {
            var amulet = Player.Equipment[(int)EquipmentSlot.Amulet]?.Info;

            return amulet != null && amulet.Shape == 0 ? amulet.Stats.GetAffinityElement() : base.GetElement(element);
        }

        public Spiritualism(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null,
                Direction = MirDirection.Down
            };

            if (!Player.UseAmulet(1, 0, out Stats stats))
            {
                response.Cast = false;
                return response;
            }

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, stats));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var stats = (Stats)data[1];

            Stats buffStats = new()
            {
                [Stat.MaxAC] = 5 + Magic.Level,
                [Stat.MaxMR] = 5 + Magic.Level
            };

            if (stats[Stat.FireAffinity] > 0)
            {
                buffStats[Stat.FireResistance] = 5 + Magic.Level;
                buffStats[Stat.MaxAC] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            if (stats[Stat.IceAffinity] > 0)
            {
                buffStats[Stat.IceResistance] = 5 + Magic.Level;
                buffStats[Stat.MaxAC] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            if (stats[Stat.LightningAffinity] > 0)
            {
                buffStats[Stat.LightningResistance] = 5 + Magic.Level;
                buffStats[Stat.MaxAC] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            if (stats[Stat.WindAffinity] > 0)
            {
                buffStats[Stat.WindResistance] = 5 + Magic.Level;
                buffStats[Stat.MaxAC] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            if (stats[Stat.HolyAffinity] > 0)
            {
                buffStats[Stat.HolyResistance] = 5 + Magic.Level;
                buffStats[Stat.MaxAC] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            if (stats[Stat.DarkAffinity] > 0)
            {
                buffStats[Stat.DarkResistance] = 5 + Magic.Level;
                buffStats[Stat.MaxAC] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            if (stats[Stat.PhantomAffinity] > 0)
            {
                buffStats[Stat.PhantomResistance] = 5 + Magic.Level;
                buffStats[Stat.MaxAC] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            TimeSpan duration = TimeSpan.FromSeconds(Magic.GetPower() + Player.GetSC() * 2);

            Player.BuffAdd(BuffType.Spiritualism, duration, buffStats, false, false, TimeSpan.Zero);

            Player.LevelMagic(Magic);
        }
    }
}
