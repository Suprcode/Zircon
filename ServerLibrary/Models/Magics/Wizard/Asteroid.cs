using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Asteroid)]
    public class Asteroid : MagicObject
    {
        protected override Element Element => Element.Fire;
        public override bool CanStruck => false;

        public Asteroid(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override int GetBurn(int burn, Stats stats = null)
        {
            var burning = GetAugmentedSkill(MagicType.Burning);

            if (burning != null)
            {
                return burning.GetPower();
            }

            return base.GetBurn(burn, stats);
        }

        public override int GetBurnLevel(int burnLevel, Stats stats = null)
        {
            var burning = GetAugmentedSkill(MagicType.Burning);

            if (burning != null)
            {
                return burning.Level + 1;
            }

            return base.GetBurnLevel(burnLevel, stats);
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

            response.Locations.Add(location);
            var cells = Player.CurrentMap.GetCells(location, 0, 3);

            var delay = SEnvir.Now.AddMilliseconds(1200);

            foreach (Cell cell in cells)
            {
                Player.ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, cell));
            }

            if (Player.GetMagic(MagicType.FireWall, out FireWall fireWall))
            {
                foreach (ConquestWar war in SEnvir.ConquestWars)
                {
                    if (war.Map != Player.CurrentMap) continue;

                    for (int i = Player.SpellList.Count - 1; i >= 0; i--)
                    {
                        if (Player.SpellList[i].Effect != SpellEffect.FireWall) continue;

                        Player.SpellList[i].Despawn();
                    }
                    break;
                }

                int power = (Magic.Level + 2) * 5;

                delay = SEnvir.Now.AddMilliseconds(2250);

                foreach (Cell cell in cells)
                {
                    if (Math.Abs(cell.Location.X - location.X) + Math.Abs(cell.Location.Y - location.Y) >= 3) continue;

                    Player.ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, MagicType.FireWall, cell, power));
                }
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

                Player.MagicAttack(new List<MagicType> { Type }, ob, true);
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower() + Player.GetMC();

            return power;
        }
    }
}
