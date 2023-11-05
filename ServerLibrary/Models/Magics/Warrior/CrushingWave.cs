using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.CrushingWave)]
    public class CrushingWave : MagicObject
    {
        protected override Element Element => Element.None;

        public CrushingWave(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - Make slower projectile, has charge up. MagicEx6 - 0
        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            for (int i = 1; i <= 12; i++)
            {
                var loc = Functions.Move(CurrentLocation, direction, i);
                Cell cell = CurrentMap.GetCell(loc);

                if (cell == null) continue;
                response.Locations.Add(cell.Location);

                var delay = SEnvir.Now.AddMilliseconds(400 + i * 60);

                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, cell, true));

                delay = SEnvir.Now.AddMilliseconds(200 + i * 60);

                switch (direction)
                {
                    case MirDirection.Up:
                    case MirDirection.Right:
                    case MirDirection.Down:
                    case MirDirection.Left:
                        ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, CurrentMap.GetCell(Functions.Move(loc, Functions.ShiftDirection(direction, -2))), false));
                        ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, CurrentMap.GetCell(Functions.Move(loc, Functions.ShiftDirection(direction, 2))), false));
                        break;
                    case MirDirection.UpRight:
                    case MirDirection.DownRight:
                    case MirDirection.DownLeft:
                    case MirDirection.UpLeft:
                        ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, CurrentMap.GetCell(Functions.Move(loc, Functions.ShiftDirection(direction, -1))), false));
                        ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, CurrentMap.GetCell(Functions.Move(loc, Functions.ShiftDirection(direction, 1))), false));
                        break;
                }
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var cell = (Cell)data[1];
            if (cell?.Objects == null) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                if (!Player.CanAttackTarget(cell.Objects[i])) continue;
                Player.Attack(cell.Objects[i], new List<MagicType> { Type }, (bool)data[2], 0);
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            if (!primary)
                power = power * Magic.GetPower() / 100;

            if (ob.Race == ObjectType.Player)
                power /= 2;

            return power;
        }
    }
}
