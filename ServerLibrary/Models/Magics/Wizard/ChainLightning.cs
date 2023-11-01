using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.ChainLightning)]
    public class ChainLightning : MagicObject
    {
        protected override Element Element => Element.Lightning;

        public ChainLightning(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override int GetShock(int shock, Stats stats = null)
        {
            var shocked = GetAugmentedSkill(MagicType.Shocked);

            if (shocked != null && SEnvir.Random.Next(4) <= shocked.Level)
            {
                return shocked.GetPower();
            }

            return base.GetShock(shock, stats);
        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target
            };

            if (target != null)
            {
                if (ChainLightningStart(new List<Point> { target.CurrentLocation }, new List<Point>(), true))
                    response.Locations.Add(target.CurrentLocation);
            }

            return response;
        }

        private bool ChainLightningStart(List<Point> locations, List<Point> previousLocations, bool primary, int powerDivisor = 0)
        {
            DateTime delay = SEnvir.Now.AddMilliseconds(primary ? 600 : 200);

            var newLocations = new List<Point>();

            if (primary)
            {
                newLocations.AddRange(locations);
            }
            else
            {
                foreach (var location in locations)
                {
                    var cells = CurrentMap.GetCells(location, 0, 1);

                    foreach (var cell in cells)
                    {
                        if (cell.Objects == null) continue;

                        if (previousLocations.Contains(cell.Location)) continue;

                        foreach (var ob in cell.Objects)
                        {
                            if (ob.Race != ObjectType.Monster || !Player.CanAttackTarget(ob) || !Functions.InRange(CurrentLocation, ob.CurrentLocation, Globals.MagicRange))
                                continue;

                            if (SEnvir.Random.Next(powerDivisor) == 0)
                                newLocations.Add(cell.Location);

                            break;
                        }
                    }
                }
            }

            if (newLocations.Count > 0)
            {
                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, newLocations, previousLocations, primary, powerDivisor));
            }

            return true;
        }

        public override void MagicComplete(params object[] data)
        {
            List<Point> locations = (List<Point>)data[1];
            List<Point> previousLocations = (List<Point>)data[2];
            bool primary = (bool)data[3];
            int powerDivisor = (int)data[4];

            var actualTargets = new List<MapObject>();

            foreach (var location in locations)
            {
                var cell = CurrentMap.GetCell(location);

                if (cell.Objects == null) continue;

                foreach (var ob in cell.Objects)
                {
                    if (ob.Race != ObjectType.Monster || !Player.CanAttackTarget(ob) || !Functions.InRange(CurrentLocation, ob.CurrentLocation, Globals.MagicRange))
                        continue;

                    actualTargets.Add(ob);
                }
            }

            foreach (var ob in actualTargets)
            {
                if (Player.MagicAttack(new List<MagicType> { Type }, ob, true, null, powerDivisor) < 1)
                    locations.Remove(ob.CurrentLocation);
            }

            if (!primary)
            {
                Player.Broadcast(new S.ObjectProjectile
                {
                    ObjectID = Player.ObjectID,
                    Direction = Direction,
                    CurrentLocation = CurrentLocation,
                    Type = Type,
                    Targets = new List<uint>(),
                    Locations = locations
                });
            }

            previousLocations.AddRange(locations);

            if (locations.Count > 0)
            {
                ChainLightningStart(locations, previousLocations, false, ++powerDivisor);
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower() + Player.GetMC();

            return power;
        }

        public override int ModifyPowerMultiplier(bool primary, int power, Stats stats = null, int extra = 0)
        {
            power = power * 5 / (extra + 5);

            return power;
        }
    }
}
