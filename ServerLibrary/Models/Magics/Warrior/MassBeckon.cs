using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.MassBeckon)]
    public class MassBeckon : MagicObject
    {
        protected override Element Element => Element.None;

        public MassBeckon(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null,
                Direction = MirDirection.Down
            };

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            List<MapObject> targets = Player.GetTargets(CurrentMap, CurrentLocation, 9);

            foreach (MapObject ob in targets)
            {
                if (ob.Race != ObjectType.Monster) continue;

                if (!Player.CanAttackTarget(ob)) continue;
                if (ob.Level - 10 > Player.Level || !((MonsterObject)ob).MonsterInfo.CanPush) continue;

                if (SEnvir.Random.Next(9) > 2 + Magic.Level * 2) continue;

                if (!ob.Teleport(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, 3))) continue;

                ob.ApplyPoison(new Poison
                {
                    Owner = Player,
                    Type = PoisonType.Paralysis,
                    TickFrequency = TimeSpan.FromSeconds(1 + Magic.Level),
                    TickCount = 1,
                });

                Player.LevelMagic(Magic);
            }
        }
    }
}
