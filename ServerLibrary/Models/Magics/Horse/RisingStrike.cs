using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.RisingStrike)]
    public class RisingStrike : MagicObject
    {
        private const int ImpactDelay = 500;
        private const int ConfusionChance = 3;

        protected override Element Element => Element.None;

        public RisingStrike(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null,
            };

            response.Locations.Add(CurrentLocation);

            ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(ImpactDelay), ActionType.DelayMagic, Type, CurrentMap));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            Map map = (Map)data[1];

            if (Player.CurrentMap != map || Player.Horse == HorseType.None || Player.Dead) return;

            List<Cell> cells = map.GetCells(Player.CurrentLocation, 0, 2);
            bool confusedTarget = false;

            foreach (Cell cell in cells)
            {
                if (cell?.Objects == null) continue;

                for (int i = cell.Objects.Count - 1; i >= 0; i--)
                {
                    if (i >= cell.Objects.Count) continue;

                    MapObject ob = cell.Objects[i];
                    if (!Player.CanAttackTarget(ob)) continue;

                    if (TryConfuse(ob))
                        confusedTarget = true;
                }
            }

            if (confusedTarget)
                Player.LevelMagic(Magic);
        }

        private bool TryConfuse(MapObject ob)
        {
            if (!CanConfuse(ob) || SEnvir.Random.Next(ConfusionChance) != 0) return false;

            ob.ApplyPoison(new Poison
            {
                Type = PoisonType.Fear,
                Owner = Player,
                TickCount = 1,
                TickFrequency = TimeSpan.FromSeconds(Magic.Level + 2),
            });

            return true;
        }

        private bool CanConfuse(MapObject ob)
        {
            if ((ob.Poison & PoisonType.Fear) == PoisonType.Fear) return false;

            switch (ob.Race)
            {
                case ObjectType.Player:
                    return ob.Level < Player.Level;
                case ObjectType.Monster:
                    return !((MonsterObject)ob).MonsterInfo.IsBoss;
                default:
                    return false;
            }
        }
    }
}
