using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magic
{
    [MagicType(MagicType.Asteroid)]
    public class Asteroid : MagicObject
    {
        public override Element Element => Element.Fire;
        public override bool CanStuck => false;

        public Asteroid(PlayerObject player, UserMagic magic) : base(player, magic)
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
            var cells = Player.CurrentMap.GetCells(location, 0, 3);

            var delay = SEnvir.Now.AddMilliseconds(1200);

            foreach (Cell cell in cells)
                Player.ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type, cell));

            if (Player.Magics.TryGetValue(MagicType.FireWall, out UserMagic augMagic) && augMagic.Info.NeedLevel1 > Player.Level)
                augMagic = null;

            if (augMagic != null)
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

                    Player.ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, MagicType.FireWall, cell, power));
                }
            }

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

                Player.MagicAttack(Type, ob, true);
            }
        }
    }
}
