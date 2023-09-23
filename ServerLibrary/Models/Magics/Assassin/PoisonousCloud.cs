using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Server.Models.Magics
{
    [MagicType(MagicType.PoisonousCloud)]
    public class PoisonousCloud : MagicObject
    {
        protected override Element Element => Element.None;

        public PoisonousCloud(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            if (Player.CurrentCell.Objects.FirstOrDefault(x => x.Race == ObjectType.Spell && ((SpellObject)x).Effect == SpellEffect.PoisonousCloud) != null) return;

            List<Cell> cells = CurrentMap.GetCells(CurrentLocation, 0, 2);

            int duration = Magic.GetPower();
            foreach (Cell cell in cells)
            {
                SpellObject ob = new SpellObject
                {
                    Visible = cell == Player.CurrentCell,
                    DisplayLocation = CurrentLocation,
                    TickCount = 1,
                    TickFrequency = TimeSpan.FromSeconds(duration),
                    Owner = Player,
                    Effect = SpellEffect.PoisonousCloud,
                    Power = 5,
                };

                ob.Spawn(CurrentMap, cell.Location);
            }

            Player.LevelMagic(Magic);
        }

        public override void MagicFinalise()
        {

        }
    }
}
