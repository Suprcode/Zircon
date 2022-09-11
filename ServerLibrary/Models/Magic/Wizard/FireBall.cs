using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magic
{
    [MagicType(MagicType.FireBall)]
    public class FireBall : MagicObject
    {
        public override Element Element => Element.Fire;

        public FireBall(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast Cast(MapObject target, Point location)
        {
            var response = new MagicCast();

            if (!Player.CanAttackTarget(target))
            {
                response.Locations.Add(location);
                return response;
            }

            response.Targets.Add(target.ObjectID);

            var delay = GetDelayFromDistance(500, target);

            Player.ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type, target));

            return response;
        }

        public override void Complete(params object[] data)
        {
            MapObject target = (MapObject)data[1];

            Player.MagicAttack(Type, target);
        }

        public override int GetPower()
        {
            return Magic.GetPower() + Player.GetMC();
        }
    }
}
