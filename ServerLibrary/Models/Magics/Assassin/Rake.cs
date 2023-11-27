using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Rake)]
    public class Rake : MagicObject
    {
        protected override Element Element => Element.None;
        protected override int Slow => 1;
        protected override int SlowLevel => 10;

        public Rake(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            if (!Player.Buffs.Any(x => x.Type == BuffType.Cloak))
            {
                return response;
            }

            Player.BuffRemove(BuffType.Cloak);

            var cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction));

            var delay = SEnvir.Now.AddMilliseconds(600);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, cell));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var cell = (Cell)data[1];

            if (cell?.Objects == null) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                if (i >= cell.Objects.Count) continue;
                MapObject ob = cell.Objects[i];
                if (!Player.CanAttackTarget(ob)) continue;

                if (Player.MagicAttack(new List<MagicType> { Type }, ob, true) > 0) break;
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Player.GetDC() * Magic.GetPower() / 100;

            return power;
        }
    }
}
