using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics.Assassin
{
    [MagicType(MagicType.CrescentMoon)]
    public class CrescentMoon : MagicObject
    {
        protected override Element Element => Element.None;

        public CrescentMoon(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            response.Locations.Add(Player.CurrentLocation);
            var cells = CurrentMap.GetCells(Player.CurrentLocation, 0, 3);

            var delay = SEnvir.Now.AddMilliseconds(1500);

            foreach (Cell cell in cells)
            {
                //TODO - Create an attack grid for better delays?
                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, cell));
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
            power += Magic.GetPower() * Player.GetSP();

            return power;
        }
    }
}
