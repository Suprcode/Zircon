using Library;
using Server.DBModels;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Hemorrhage)]
    public class Hemorrhage : MagicObject
    {
        protected override Element Element => Element.None;

        public Hemorrhage(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target
            };

            if (!Player.CanAttackTarget(target))
            {
                response.Ob = null;
                response.Locations.Add(location);
                return response;
            }

            response.Targets.Add(target.ObjectID);

            var delay = GetDelayFromDistance(500, target);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, target));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            MapObject target = (MapObject)data[1];

            var damage = Player.MagicAttack(new List<MagicType> { Type }, target);

            if (target.Dead) return;

            if (target.Race == ObjectType.Monster && ((MonsterObject)target).MonsterInfo.IsBoss) return;

            if (damage > 0)
            {
                target.ApplyPoison(new Poison
                {
                    Type = PoisonType.Hemorrhage,
                    Owner = Player,
                    TickCount = Magic.GetPower(),
                    TickFrequency = TimeSpan.FromSeconds(1),
                    Value = Player.GetSP()
                });
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Player.GetSP();

            return power;
        }
    }
}