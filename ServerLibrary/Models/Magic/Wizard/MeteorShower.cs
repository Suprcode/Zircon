﻿using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magic
{
    [MagicType(MagicType.MeteorShower)]
    public class MeteorShower : MagicObject
    {
        public override Element Element => Element.Fire;

        public MeteorShower(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            var realTargets = new HashSet<MapObject>();
            var possibleTargets = Player.GetTargets(Player.CurrentMap, location, 3);

            while (realTargets.Count < 6 + Magic.Level)
            {
                if (possibleTargets.Count == 0) break;

                MapObject ob = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                possibleTargets.Remove(ob);

                if (!Functions.InRange(Player.CurrentLocation, ob.CurrentLocation, Globals.MagicRange)) continue;

                realTargets.Add(ob);
            }

            foreach (MapObject ob in realTargets)
            {
                var delay = GetDelayFromDistance(500, ob);

                response.Targets.Add(ob.ObjectID);
                Player.ActionList.Add(new DelayedAction( delay, ActionType.DelayMagicNew, Type, ob));
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            MapObject target = (MapObject)data[1];

            Player.MagicAttack(new List<MagicType> { Type }, target);
        }

        public override int ModifyPower1(bool primary, int power)
        {
            power += Magic.GetPower() + Player.GetMC();

            return power;
        }
    }
}
