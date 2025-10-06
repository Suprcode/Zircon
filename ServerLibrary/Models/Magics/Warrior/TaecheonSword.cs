using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics.Warrior
{
    [MagicType(MagicType.TaecheonSword)]
    public class TaecheonSword : MagicObject
    {
        protected override Element Element => Element.Fire;

        public TaecheonSword(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            if (!Functions.InRange(CurrentLocation, location, Globals.MagicRange))
            {
                response.Cast = false;
                return response;
            }

            response.Locations.Add(location);
            var cells = CurrentMap.GetCells(location, 0, 2);

            var delay = SEnvir.Now.AddMilliseconds(1500);

            foreach (Cell cell in cells) 
            {
                var distanceFromCentre = Functions.Distance(location, cell.Location);
                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, cell, distanceFromCentre));
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            Cell cell = (Cell)data[1];
            int distanceFromCentre = (int)data[2];

            if (cell?.Objects == null) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                if (i >= cell.Objects.Count) continue;
                MapObject ob = cell.Objects[i];
                if (!Player.CanAttackTarget(ob)) continue;

                var damage = Player.MagicAttack(new List<MagicType> { Type }, ob, extra: distanceFromCentre);
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            var multiplier = Math.Max(0, 4 - extra);

            power += Magic.GetPower() + (Player.GetDC() * multiplier);

            return power;
        }
    }
}
