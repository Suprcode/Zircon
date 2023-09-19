using Library;
using Server.DBModels;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magic
{
    [MagicType(MagicType.GustBlast)]
    public class GustBlast : MagicObject
    {
        public override Element Element => Element.Wind;

        protected override int Repel => 10;

        public GustBlast(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast();

            if (!Player.CanAttackTarget(target))
            {
                response.Locations.Add(location);
                return response;
            }

            response.Targets.Add(target.ObjectID);

            var delay = GetDelayFromDistance(500, target);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type, target));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            MapObject target = (MapObject)data[1];

            Player.MagicAttack(new List<MagicType> { Type }, target);
        }

        public override int ModifyPower1(bool primary, int power, MapObject ob)
        {
            power += Magic.GetPower() + Player.GetMC();

            return power;
        }
    }
}
