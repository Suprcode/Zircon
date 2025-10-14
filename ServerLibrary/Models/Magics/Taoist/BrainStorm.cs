using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;

namespace Server.Models.Magics.Taoist
{
    [MagicType(MagicType.BrainStorm)]
    public class BrainStorm : MagicObject
    {
        protected override Element Element => Element.Holy;


        public BrainStorm(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override int GetShock(int shock, Stats stats = null)
        {
            return Magic.GetPower();
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
                if ((target.Poison & PoisonType.Binding) != PoisonType.Binding)
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

                var delay = SEnvir.Now.AddMilliseconds(1000 + Functions.Distance(CurrentLocation, realTarget.CurrentLocation) * 48);

                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, realTarget, realTarget == target, stats));
            }

            if (count > 0)
            {
                MagicCooldown();
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            MapObject target = (MapObject)data[1];
            bool primary = (bool)data[2];
            var stats = (Stats)data[3];

            if (target?.Node == null || target.Dead) return;

            Player.LevelMagic(Magic);

            var binding = target.PoisonList.Find(x => x.Type == PoisonType.Binding);

            if (binding != null)
            {
                // Need to remove straight away so it doesn't bring the target out of shock
                target.PoisonList.Remove(binding);
            }

            var damage = Player.MagicAttack([Type], target, primary, stats);
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Player.GetSC() * (Magic.Level + 1);

            return power;
        }
    }
}
