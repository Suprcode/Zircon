using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magics.Wizard
{
    [MagicType(MagicType.IceAura)]
    public class IceAura : MagicObject
    {
        protected override Element Element => Element.Ice;

        public IceAura(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var dir = Functions.DirectionFromPoint(Player.CurrentLocation, location);

            var response = new MagicCast
            {
                Ob = null
            };

            var newLocation = Functions.Move(Player.CurrentLocation, dir, 5);

            response.Locations.Add(newLocation);

            for (int i = 1; i <= 8; i++)
            {
                var loc = Functions.Move(CurrentLocation, direction, i);
                Cell cell = CurrentMap.GetCell(loc);

                if (cell == null) continue;

                var delay = GetDelayFromDistance(500, cell);

                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, cell, true));
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            Cell cell = (Cell)data[1];
            bool primary = (bool)data[2];

            if (cell?.Objects == null) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                if (i >= cell.Objects.Count) continue;
                MapObject ob = cell.Objects[i];
                if (!Player.CanAttackTarget(ob)) continue;

                SpellObject spellOb = new SpellObject
                {
                    DisplayLocation = cell.Location,
                    TickCount = 2,
                    TickFrequency = TimeSpan.FromSeconds(5 + Magic.Level * 3),
                    Owner = Player,
                    Effect = SpellEffect.IceAura,
                    Magic = Magic
                };

                spellOb.Spawn(cell.Map, cell.Location);
                break;
            }
        }

        //public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        //{
        //    //power += Magic.GetPower() + Player.GetMC();

        //    return power;
        //}

        //public override int ModifyPowerMultiplier(bool primary, int power, Stats stats = null, int extra = 0)
        //{
        //    if (!primary)
        //        power = (int)(power * 0.3F);

        //    return power;
        //}
    }
}
