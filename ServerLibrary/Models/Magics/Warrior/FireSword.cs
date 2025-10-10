using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Server.Models.Magics.Warrior
{
    [MagicType(MagicType.FireSword)]
    public class FireSword : MagicObject
    {
        protected override Element Element => Element.Fire;

        public FireSword(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            response.Locations.Add(Player.CurrentLocation);

            var cells = CurrentMap.GetCells(Player.CurrentLocation, 0, 2);

            var orderedCells = OrderCellsAntiClockwise(cells, Player.CurrentLocation);

            int count = 0;

            foreach (Cell cell in orderedCells)
            {
                var delay = SEnvir.Now.AddMilliseconds(1300 + (100 * count));

                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, cell));

                count++;
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            Cell cell = (Cell)data[1];

            if (cell?.Objects == null) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                if (i >= cell.Objects.Count) continue;
                MapObject ob = cell.Objects[i];
                if (!Player.CanAttackTarget(ob)) continue;

                var damage = Player.MagicAttack(new List<MagicType> { Type }, ob);
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower() + Player.GetDC();

            return power;
        }

        /// <summary>
        /// Orders a list of cells from the center point outward in concentric rings,
        /// anti-clockwise within each ring.
        /// </summary>
        private static List<Cell> OrderCellsAntiClockwise(List<Cell> cells, Point center)
        {
            // Compute distance and angle for each cell
            var ordered = cells
                .Select(cell =>
                {
                    int dx = cell.Location.X - center.X;
                    int dy = cell.Location.Y - center.Y;

                    // Because screen coordinates have Y increasing downwards,
                    // invert dy so angles rotate anti-clockwise visually.
                    double angle = Math.Atan2(-dy, dx);

                    // Normalize angle so 0 is at the top
                    angle -= Math.PI / 2;
                    if (angle < -Math.PI)
                        angle += 2 * Math.PI;

                    // Compute distance
                    double distance = Math.Max(Math.Abs(dx), Math.Abs(dy));

                    return new { cell, distance, angle };
                })
                // Sort by distance (inner → outer), then by angle (anti-clockwise)
                .OrderBy(x => x.distance)
                .ThenBy(x => x.angle)
                .Select(x => x.cell)
                .ToList();

            return ordered;
        }
    }
}
