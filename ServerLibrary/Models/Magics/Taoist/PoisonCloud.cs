using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magics.Taoist
{
    [MagicType(MagicType.PoisonCloud)]
    public class PoisonCloud : MagicObject
    {
        protected override Element Element => Element.None;

        public PoisonCloud(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            response.Locations.Add(Player.CurrentLocation);
            var cells = CurrentMap.GetCells(Player.CurrentLocation, 0, 2);

            var delay = SEnvir.Now.AddMilliseconds(2500);

            foreach (Cell cell in cells)
            {
                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, cell));
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

                if (ob?.Node == null || !Player.CanAttackTarget(ob)) return;

                for (int j = Player.Pets.Count - 1; j >= 0; j--)
                    if (Player.Pets[j].Target == null)
                        Player.Pets[j].Target = ob;

                int duration = Magic.GetPower() + Player.GetSC() + Player.Stats[Stat.DarkAttack] * 2;

                ob.ApplyPoison(new Poison
                {
                    Value = Magic.Level + 1 + Player.Level / 14,
                    Type = PoisonType.Green,
                    Owner = Player,
                    TickCount = duration / 2,
                    TickFrequency = TimeSpan.FromSeconds(2),
                });

                Player.LevelMagic(Magic);
            }
        }
    }
}
