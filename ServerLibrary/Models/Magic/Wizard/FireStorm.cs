using Library;
using Server.DBModels;
using Server.Envir;
using System.Drawing;

namespace Server.Models.Magic
{
    [MagicType(MagicType.FireStorm)]
    public class FireStorm : MagicObject
    {
        public override Element Element => Element.Fire;

        public FireStorm(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override int GetPower()
        {
            return Magic.GetPower() + Player.GetMC();
        }

        public override MagicCast Cast(MapObject target, Point location)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            if (!Functions.InRange(Player.CurrentLocation, location, Globals.MagicRange))
            {
                response.Cast = false;
                return response;
            }

            response.Locations.Add(location);
            var cells = Player.CurrentMap.GetCells(location, 0, 1);

            var delay = SEnvir.Now.AddMilliseconds(500);

            foreach (Cell cell in cells)
                Player.ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type, cell));

            return response;
        }

        public override void Complete(params object[] data)
        {
            Cell cell = (Cell)data[1];

            if (cell?.Objects == null) return;
            if (cell.Objects.Count == 0) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                if (i >= cell.Objects.Count) continue;
                MapObject ob = cell.Objects[i];
                if (!Player.CanAttackTarget(ob)) continue;

                Player.MagicAttack(Type, ob);
            }
        }
    }
}
