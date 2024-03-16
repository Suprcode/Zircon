using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.SeismicSlam)]
    public class SeismicSlam : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool IgnoreAccuracy => true;

        public SeismicSlam(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            var cells = CurrentMap.GetCells(Functions.Move(CurrentLocation, direction, 3), 0, 3);

            if (Player.GetMagic(MagicType.SwiftBlade, out SwiftBlade swiftBlade) && swiftBlade != null)
            {
                swiftBlade.SwiftBladeLifeSteal = 0;
            }

            var delay = SEnvir.Now.AddMilliseconds(600);

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
                if (!Player.CanAttackTarget(cell.Objects[i])) continue;

                Player.Attack(cell.Objects[i], new List<MagicType> { Type }, true, 0);
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power = power * Magic.GetPower() / 100;

            if (ob.Race == ObjectType.Player)
                power /= 2;

            return power;
        }

        public override void AttackComplete(MapObject target)
        {
            target.ApplyPoison(new Poison
            {
                Owner = Player,
                Type = PoisonType.Paralysis,
                TickFrequency = TimeSpan.FromSeconds(3),
                TickCount = 1,
            });

            target.ApplyPoison(new Poison
            {
                Type = PoisonType.WraithGrip,
                Owner = Player,
                TickCount = 1,
                TickFrequency = TimeSpan.FromMilliseconds(1500),
            });

            target.ApplyPoison(new Poison
            {
                Owner = Player,
                Type = PoisonType.Silenced,
                TickFrequency = TimeSpan.FromSeconds(5),
                TickCount = 1,
            });

            Player.LevelMagic(Magic);
        }
    }
}
