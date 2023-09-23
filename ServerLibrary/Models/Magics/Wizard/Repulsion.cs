using Library;
using Server.DBModels;
using Server.Envir;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Repulsion)]
    public class Repulsion : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public Repulsion(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            var delay = SEnvir.Now.AddMilliseconds(500);

            for (MirDirection d = MirDirection.Up; d <= MirDirection.UpLeft; d++)
            {
                var cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, d));
                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, cell, d));
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            Cell cell = (Cell)data[1];
            var direction = (MirDirection)data[2]; 

            if (cell?.Objects == null) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = cell.Objects[i];
                if (!Player.CanAttackTarget(ob) || ob.Level >= Player.Level || SEnvir.Random.Next(16) >= 6 + Magic.Level * 3 + Player.Level - ob.Level) continue;

                //CanPush check ?

                if (ob.Pushed(direction, Magic.GetPower()) <= 0) continue;

                Player.LevelMagic(Magic);
                break;
            }
        }
    }
}
