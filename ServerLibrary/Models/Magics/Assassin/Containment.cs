using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Containment)]
    public class Containment : MagicObject
    {
        protected override Element Element => Element.None;

        public Containment(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - Needs sound
        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            var realTargets = new HashSet<MapObject>();
            var possibleTargets = Player.GetTargets(Player.CurrentMap, Player.CurrentLocation, 3);

            while (possibleTargets.Count > 0 && realTargets.Count < 1 + Magic.Level)
            {
                MapObject ob = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                possibleTargets.Remove(ob);

                if (!Functions.InRange(Player.CurrentLocation, ob.CurrentLocation, Globals.MagicRange)) continue;

                realTargets.Add(ob);
            }

            var delay = SEnvir.Now.AddMilliseconds(500);

            foreach (MapObject ob in realTargets)
            {
                response.Targets.Add(ob.ObjectID);
                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, ob));
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            MapObject target = (MapObject)data[1];

            if (target?.Node == null || target.Dead) return;

            if (target.Level > Player.Level + 2) return;

            if (SEnvir.Random.Next(Globals.MagicMaxLevel + 1) > Magic.Level)
            {
                if (SEnvir.Random.Next(2) == 0) Player.LevelMagic(Magic);
                return;
            }

            Player.LevelMagic(Magic);

            target.ApplyPoison(new Poison
            {
                Owner = Player,
                Type = PoisonType.Containment,
                TickCount = 3,
                TickFrequency = TimeSpan.FromSeconds(1),
                Value = Player.GetSP()
            });
        }
    }
}
