using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.CombatKick)]
    public class CombatKick : MagicObject
    {
        protected override Element Element => Element.None;

        public CombatKick(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            var delay = SEnvir.Now.AddMilliseconds(500);

            var cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, direction));

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, cell, direction));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var cell = (Cell)data[1];
            var direction = (MirDirection)data[2];

            if (cell?.Objects == null) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = cell.Objects[i];
                if (!Player.CanAttackTarget(ob) || ob.Level >= Player.Level || SEnvir.Random.Next(16) >= 6 + Magic.Level * 3 + Player.Level - ob.Level) continue;

                //CanPush check ?

                if (ob.Pushed(direction, Magic.GetPower()) <= 0) continue;

                Player.Attack(ob, new List<MagicType> { Type }, true, 0);
                Player.LevelMagic(Magic);
                break;
            }
        }
    }
}
