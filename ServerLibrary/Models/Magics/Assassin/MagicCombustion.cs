using Library;
using Server.DBModels;
using Server.Envir;
using Server.Envir.Commands.Command.Admin;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.MagicCombustion)]
    public class MagicCombustion : MagicObject
    {
        protected override Element Element => Element.None;

        public MagicCombustion(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - Needs sound
        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target
            };

            if (!Player.CanAttackTarget(target) || target.Race != ObjectType.Player)
            {
                response.Ob = null;
                return response;
            }

            var delay = GetDelayFromDistance(500, target);

            Player.ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, target));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            PlayerObject target = (PlayerObject)data[1];

            if (target?.Node == null || target.Dead) return;

            var mp = Player.GetDC() * Magic.GetPower() / 100;

            target.ChangeMP(-mp);

            Player.LevelMagic(Magic);
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower();

            return power;
        }
    }
}