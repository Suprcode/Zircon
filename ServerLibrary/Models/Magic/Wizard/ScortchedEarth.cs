using Library;
using Library.Network.ClientPackets;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Server.Models.Magic
{
    [MagicType(MagicType.ScortchedEarth)]
    public class ScortchedEarth : MagicObject
    {
        protected override Element Element => Element.Fire;

        public ScortchedEarth(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            for (int i = 1; i <= 8; i++)
            {
                location = Functions.Move(CurrentLocation, direction, i);
                Cell cell = CurrentMap.GetCell(location);

                if (cell == null) continue;
                response.Locations.Add(cell.Location);

                var delay = SEnvir.Now.AddMilliseconds(800);

                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type, cell, true));

                switch (direction)
                {
                    case MirDirection.Up:
                    case MirDirection.Right:
                    case MirDirection.Down:
                    case MirDirection.Left:
                        ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type, CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(direction, -2))), false));
                        ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type, CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(direction, 2))), false));
                        break;
                    case MirDirection.UpRight:
                    case MirDirection.DownRight:
                    case MirDirection.DownLeft:
                    case MirDirection.UpLeft:
                        ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type, CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(direction, 1))), false));
                        ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type, CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(direction, -1))), false));
                        break;
                }
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            Cell cell = (Cell)data[1];
            bool primary = (bool)data[2];

            if (cell?.Objects == null) return;
            if (cell.Objects.Count == 0) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                if (i >= cell.Objects.Count) continue;
                MapObject ob = cell.Objects[i];
                if (!Player.CanAttackTarget(ob)) continue;

                Player.MagicAttack(new List<MagicType> { Type }, ob, primary);
            }
        }

        public override int ModifyPower1(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower() + Player.GetMC();

            return power;
        }

        public override int ModifyPower2(bool primary, int power, Stats stats = null)
        {
            if (!primary)
                power = (int)(power * 0.3F);

            return power;
        }
    }
}
