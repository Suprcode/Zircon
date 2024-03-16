using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.SwiftBlade)]
    public class SwiftBlade : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;
        public override bool IgnoreAccuracy => true;
        public decimal SwiftBladeLifeSteal { get; set; }

        public SwiftBlade(PlayerObject player, UserMagic magic) : base(player, magic)
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

            var cells = CurrentMap.GetCells(location, 0, 3);
            SwiftBladeLifeSteal = 0;

            var delay = SEnvir.Now.AddMilliseconds(900);

            foreach (Cell cell in cells)
            {
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
                if (!Player.CanAttackTarget(cell.Objects[i])) continue;

                Player.Attack(cell.Objects[i], new List<MagicType> { Type }, true, 0);
            }
        }

        public override decimal LifeSteal(bool primary, decimal lifestealAmount)
        {
            lifestealAmount = Math.Min(lifestealAmount, 2000 - SwiftBladeLifeSteal);
            SwiftBladeLifeSteal += lifestealAmount;

            return lifestealAmount;
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power = power * Magic.GetPower() / 100;

            if (ob.Race == ObjectType.Player)
                power /= 2;

            return power;
        }
    }
}
