using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.ThunderStrike)]
    public class ThunderStrike : MagicObject
    {
        protected override Element Element => Element.Lightning;

        public ThunderStrike(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override int GetShock(int shock, Stats stats = null)
        {
            var shocked = GetAugmentedSkill(MagicType.Shocked);

            if (shocked != null && SEnvir.Random.Next(4) <= shocked.Level)
            {
                return shocked.GetPower();
            }

            return base.GetShock(shock, stats);
        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            int range = 3;

            if (Magic.Level >= 4)
            {
                range = 6;
            }

            var cells = CurrentMap.GetCells(CurrentLocation, 0, range);

            var delay = SEnvir.Now.AddMilliseconds(500);

            foreach (Cell cell in cells)
            {
                if (cell.Objects == null)
                {
                    if (SEnvir.Random.Next(40) == 0)
                        response.Locations.Add(cell.Location);

                    continue;
                }

                foreach (MapObject ob in cell.Objects)
                {
                    if (SEnvir.Random.Next(2) > 0) continue;
                    if (!Player.CanAttackTarget(ob)) continue;

                    response.Targets.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, ob));
                }
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            MapObject target = (MapObject)data[1];

            Player.MagicAttack(new List<MagicType> { Type }, target);
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower() + Player.GetMC();
            power += power / 2;

            return power;
        }
    }
}
