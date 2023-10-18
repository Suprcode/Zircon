using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Fetter)]
    public class Fetter : MagicObject
    {
        protected override Element Element => Element.None;       

        public Fetter(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            var cells = CurrentMap.GetCells(CurrentLocation, 0, 2);

            var delay = SEnvir.Now.AddMilliseconds(500);

            foreach (Cell cell in cells)
            {
                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, cell));
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            Cell cell = (Cell)data[1];

            if (cell == null || cell.Map != CurrentMap) return;

            if (cell.Objects == null) return;

            foreach (MapObject ob in cell.Objects)
            {
                if (!Player.CanAttackTarget(ob)) continue;

                switch (ob.Race)
                {
                    case ObjectType.Monster:
                        if (ob.Level > Player.Level + 15) continue;

                        ob.ApplyPoison(new Poison
                        {
                            Owner = Player,
                            Value = (3 + Magic.Level) * 2,
                            TickCount = 1,
                            TickFrequency = TimeSpan.FromSeconds(5 + Magic.Level * 3),
                            Type = PoisonType.Slow,
                        });
                        break;
                }

                Player.LevelMagic(Magic);
            }
        }
    }
}
