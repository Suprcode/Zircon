using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.LifeSteal)]
    public class LifeSteal : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public LifeSteal(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            if (!Functions.InRange(CurrentLocation, location, Globals.MagicRange) || !Player.UseAmulet(2, 0))
            {
                response.Cast = false;
                return response;
            }

            response.Locations.Add(location);

            var cells = CurrentMap.GetCells(location, 0, 3);

            var delay = SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, location) * 48);

            foreach (var cell in cells)
            {
                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, cell));
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var cell = (Cell)data[1];

            if (cell?.Objects == null) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = cell.Objects[i];

                if (ob?.Node == null || !Player.CanHelpTarget(ob)) continue;

                Stats buffStats = new Stats
                {
                    [Stat.LifeSteal] = 4 + (Magic.Level) * 2
                };

                ob.BuffAdd(BuffType.LifeSteal, TimeSpan.FromSeconds(Magic.GetPower() + Player.GetSC() + Player.Stats[Stat.DarkAttack] * 2), buffStats, true, false, TimeSpan.Zero);

                Player.LevelMagic(Magic);
            }
        }
    }
}
