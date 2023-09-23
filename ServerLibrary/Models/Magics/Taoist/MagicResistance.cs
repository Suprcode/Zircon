using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.MagicResistance)]
    public class MagicResistance : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public MagicResistance(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            if (!Functions.InRange(CurrentLocation, location, Globals.MagicRange) || !Player.UseAmulet(1, 0, out Stats stats))
            {
                response.Cast = false;
                return response;
            }

            response.Locations.Add(location);

            var cells = CurrentMap.GetCells(location, 0, 3);

            var delay = SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, location) * 48);

            foreach (var cell in cells)
            {
                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, cell, stats));
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var cell = (Cell)data[1];
            var stats = (Stats)data[2];

            if (cell?.Objects == null) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = cell.Objects[i];

                if (ob?.Node == null || !Player.CanHelpTarget(ob)) continue;

                Stats buffStats = new Stats
                {
                    [Stat.MaxMR] = 5 + Magic.Level
                };

                if (stats[Stat.FireAffinity] > 0)
                {
                    buffStats[Stat.FireResistance] = 1;
                    buffStats[Stat.MinMR] = 0;
                    buffStats[Stat.MaxMR] = 0;
                }

                if (stats[Stat.IceAffinity] > 0)
                {
                    buffStats[Stat.IceResistance] = 1;
                    buffStats[Stat.MinMR] = 0;
                    buffStats[Stat.MaxMR] = 0;
                }

                if (stats[Stat.LightningAffinity] > 0)
                {
                    buffStats[Stat.LightningResistance] = 1;
                    buffStats[Stat.MinMR] = 0;
                    buffStats[Stat.MaxMR] = 0;
                }

                if (stats[Stat.WindAffinity] > 0)
                {
                    buffStats[Stat.WindResistance] = 1;
                    buffStats[Stat.MinMR] = 0;
                    buffStats[Stat.MaxMR] = 0;
                }

                if (stats[Stat.HolyAffinity] > 0)
                {
                    buffStats[Stat.HolyResistance] = 1;
                    buffStats[Stat.MinMR] = 0;
                    buffStats[Stat.MaxMR] = 0;
                }

                if (stats[Stat.DarkAffinity] > 0)
                {
                    buffStats[Stat.DarkResistance] = 1;
                    buffStats[Stat.MinMR] = 0;
                    buffStats[Stat.MaxMR] = 0;
                }

                if (stats[Stat.PhantomAffinity] > 0)
                {
                    buffStats[Stat.PhantomResistance] = 1;
                    buffStats[Stat.MinMR] = 0;
                    buffStats[Stat.MaxMR] = 0;
                }


                ob.BuffAdd(BuffType.MagicResistance, TimeSpan.FromSeconds(Magic.GetPower() + Player.GetSC() * 2), buffStats, true, false, TimeSpan.Zero);

                Player.LevelMagic(Magic);
            }
        }
    }
}
