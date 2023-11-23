using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.BurningFire)]
    public class BurningFire : MagicObject
    {
        protected override Element Element => Element.Fire;

        public BurningFire(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - Needs sound
        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
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

            int maxCount = Math.Clamp(Magic.Level, 1, 3);
            int currentCount = 0;

            for (int i = Player.SpellList.Count - 1; i >= 0; i--)
            {
                if (Player.SpellList[i].Effect != SpellEffect.BurningFire) continue;

                currentCount++;
            }

            if (currentCount >= maxCount)
            {
                response.Cast = false;
                return response;
            }

            response.Locations.Add(location);

            var delay = SEnvir.Now.AddMilliseconds(1000 + 600);

            var cell = Player.CurrentMap.GetCell(location);

            Player.ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, cell));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            Cell cell = (Cell)data[1];

            if (cell == null) return;

            if (cell.Objects != null)
            {
                for (int i = cell.Objects.Count - 1; i >= 0; i--)
                {
                    if (cell.Objects[i].Race != ObjectType.Spell) continue;

                    SpellObject spell = (SpellObject)cell.Objects[i];

                    if (spell.Effect != SpellEffect.BurningFire) continue;

                    spell.Despawn();
                }
            }

            SpellObject ob = new()
            {
                DisplayLocation = cell.Location,
                TickCount = 15,
                TickFrequency = TimeSpan.FromSeconds(1),
                Owner = Player,
                Effect = SpellEffect.BurningFire,
                Magic = Magic,
            };

            ob.Spawn(cell.Map, cell.Location);

            Player.LevelMagic(Magic);
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower() + Player.GetSP();

            return power;
        }

        public void Explode(Map map, Point location)
        {
            if (Player?.Node == null || Player.CurrentMap != map) return;

            var cells = map.GetCells(location, 0, 1);

            foreach (var cell in cells)
            {
                if (cell?.Objects == null) continue;

                for (int i = cell.Objects.Count - 1; i >= 0; i--)
                {
                    if (i >= cell.Objects.Count) continue;
                    MapObject ob = cell.Objects[i];
                    if (!Player.CanAttackTarget(ob)) continue;

                    Player.MagicAttack(new List<MagicType> { Type }, ob, true);
                }
            }

            CurrentMap.Broadcast(location, new S.MapEffect { Location = location, Effect = Effect.BurningFireExplode });
        }
    }
}