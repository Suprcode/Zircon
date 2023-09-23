using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.TrapOctagon)]
    public class TrapOctagon : MagicObject
    {
        protected override Element Element => Element.None;

        public TrapOctagon(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            if (!Functions.InRange(CurrentLocation, location, Globals.MagicRange) || !Player.UseAmulet(2, 0))
            {
                response.Cast = false;
                return response;
            }

            var delay = SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, location) * 48);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, CurrentMap, location));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var map = (Map)data[1];
            var location = (Point)data[2];

            if (map != CurrentMap) return;

            List<MapObject> targets = Player.GetTargets(CurrentMap, location, 1);

            List<MapObject> trappedMonsters = new List<MapObject>();

            foreach (MapObject target in targets)
            {
                if (target.Race != ObjectType.Monster || target.Level >= Player.Level + SEnvir.Random.Next(3)) continue;

                trappedMonsters.Add((MonsterObject)target);
            }

            if (trappedMonsters.Count == 0) return;

            int duration = Player.GetSC() + Magic.GetPower();

            List<Point> locationList = new List<Point>
            {
                new Point(location.X - 1, location.Y - 2),
                new Point(location.X - 1, location.Y + 2),
                new Point(location.X + 1, location.Y - 2),
                new Point(location.X + 1, location.Y + 2),

                new Point(location.X - 2, location.Y - 1),
                new Point(location.X - 2, location.Y + 1),
                new Point(location.X + 2, location.Y - 1),
                new Point(location.X + 2, location.Y + 1)
            };

            foreach (Point point in locationList)
            {
                SpellObject ob = new SpellObject
                {
                    DisplayLocation = point,
                    TickCount = duration * 4, //Checking every 1/4 of a second to see if all monsters were disturbed.
                    TickFrequency = TimeSpan.FromMilliseconds(250),
                    Owner = Player,
                    Effect = SpellEffect.TrapOctagon,
                    Magic = Magic,
                    Targets = trappedMonsters,
                };

                ob.Spawn(map, point);
            }

            DateTime shockTime = SEnvir.Now.AddSeconds(duration);
            foreach (MonsterObject monster in trappedMonsters)
            {
                if (shockTime <= monster.ShockTime) continue;

                monster.ShockTime = SEnvir.Now.AddSeconds(duration);
                Player.LevelMagic(Magic);
            }
        }
    }
}
