using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.DarkSoulPrison)]
    public class DarkSoulPrison : MagicObject
    {
        protected override Element Element => Element.Dark;
        public override bool UpdateCombatTime => false;

        public DarkSoulPrison(PlayerObject player, UserMagic magic) : base(player, magic)
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

            var power = Magic.Level + 5;

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, location, power));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var location = (Point)data[1];
            var power = (int)data[2];

            List<Cell> cells = CurrentMap.GetCells(location, 0, 3);

            foreach (Cell cell in cells)
            {
                if (cell.Objects != null)
                {
                    for (int i = cell.Objects.Count - 1; i >= 0; i--)
                    {
                        if (cell.Objects[i].Race != ObjectType.Spell) continue;

                        SpellObject spell = (SpellObject)cell.Objects[i];

                        if (spell.Effect != SpellEffect.DarkSoulPrison) continue;

                        spell.Despawn();
                    }
                }

                SpellObject ob = new SpellObject
                {
                    Visible = cell.Location == location,
                    DisplayLocation = cell.Location,
                    TickCount = power,
                    TickFrequency = TimeSpan.FromSeconds(2),
                    Owner = Player,
                    Effect = SpellEffect.DarkSoulPrison,
                    Magic = Magic,
                };

                ob.Spawn(cell.Map.Info, cell.Map.Instance, cell.Map.InstanceSequence, cell.Location);
            }

            Player.LevelMagic(Magic);
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower() + Player.GetSP();

            return power;
        }

        public override int ModifyPowerMultiplier(bool primary, int power, Stats stats = null, int extra = 0)
        {
            power = (int)(power * 0.40M);

            return power;
        }
    }
}
