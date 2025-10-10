using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics.Wizard
{
    [MagicType(MagicType.IceBreaker)]
    public class IceBreaker : MagicObject
    {
        protected override Element Element => Element.Ice;
        protected override int Slow => 5;
        protected override int SlowLevel => 5;

        public IceBreaker(PlayerObject player, UserMagic magic) : base(player, magic)
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

            foreach (Cell cell in cells)
            {
                var distance = Functions.Distance(Player.CurrentLocation, cell.Location);

                var delay = SEnvir.Now.AddMilliseconds(500 + (500 * distance));

                Player.ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, cell));
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

                Player.MagicAttack(new List<MagicType> { Type }, ob);
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower() + Player.GetMC();

            return power;
        }
    }
}
