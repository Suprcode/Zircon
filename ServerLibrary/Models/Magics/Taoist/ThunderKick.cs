using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.ThunderKick)]
    public class ThunderKick : MagicObject
    {
        protected override Element Element => Element.None;

        public ThunderKick(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            var front = Functions.Move(CurrentLocation, direction);

            var frontCell = CurrentMap.GetCell(front);

            if (frontCell?.Objects != null && frontCell.Objects.Count > 0)
            {
                response.Locations.Add(front);
            }

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, front, direction));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var front = (Point)data[1];
            var direction = (MirDirection)data[2];

            var frontCell = CurrentMap.GetCell(front);

            if (frontCell?.Objects == null || frontCell.Objects.Count == 0) return;

            var cells = CurrentMap.GetCells(front, 0, 1);

            foreach (var cell in cells)
            {
                if (cell?.Objects == null) continue;

                for (int i = cell.Objects.Count - 1; i >= 0; i--)
                {
                    MapObject ob = cell.Objects[i];
                    if (!Player.CanAttackTarget(ob) || ob.Level >= Player.Level || SEnvir.Random.Next(16) >= 6 + Magic.Level * 3 + Player.Level - ob.Level) continue;

                    //CanPush check ?

                    var dir = Functions.DirectionFromPoint(front, ob.CurrentLocation);
                    if (cell.Location != front && ob.Pushed(dir, Magic.GetPower()) <= 0) continue;

                    Player.Attack(ob, new List<MagicType> { Type }, true, 0);
                    Player.LevelMagic(Magic);
                    break;
                }
            } 
        }
    }
}
