using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics.Taoist
{
    [MagicType(MagicType.BindingTalisman)]
    public class BindingTalisman : MagicObject
    {
        protected override Element Element => Element.Holy;


        public BindingTalisman(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target
            };

            var realTargets = new HashSet<MapObject>();

            if (Player.CanAttackTarget(target))
            {
                if ((target.Poison & PoisonType.Binding) == PoisonType.Binding)
                {
                    response.Ob = null;
                    return response;
                }

                realTargets.Add(target);
            }

            var count = -1;
            foreach (MapObject realTarget in realTargets)
            {
                if (!Player.UseAmulet(1, 0, out Stats stats))
                    break;

                response.Targets.Add(realTarget.ObjectID);

                var delay = SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, realTarget.CurrentLocation) * 48);

                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, realTarget, realTarget == target, stats));
            }

            if (count > 0)
            {
                MagicCooldown();
            }

            if (target == null)
                response.Locations.Add(location);

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            MapObject target = (MapObject)data[1];
            bool primary = (bool)data[2];
            var stats = (Stats)data[3];

            if (target?.Node == null || target.Dead) return;

            Player.LevelMagic(Magic);

            var damage = Player.MagicAttack(new List<MagicType> { Type }, target, primary, stats);

            target.ApplyPoison(new Poison
            {
                Owner = Player,
                Type = PoisonType.Binding,
                TickCount = Magic.GetPower(),
                TickFrequency = TimeSpan.FromSeconds(1),
                Value = damage
            });
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Player.GetSC();

            return power;
        }
    }
}
