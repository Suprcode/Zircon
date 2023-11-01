using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Neutralize)]
    public class Neutralize : MagicObject
    {
        protected override Element Element => Element.None;

        public Neutralize(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target
            };

            var types = new List<MagicType> { Type };

            var realTargets = new HashSet<MapObject>();

            if (Player.CanAttackTarget(target))
            {
                realTargets.Add(target);
            }

            var augmentNeutralize = GetAugmentedSkill(MagicType.AugmentNeutralize);
            bool hasAugmentNeutralize = false;

            if (augmentNeutralize != null && SEnvir.Now > augmentNeutralize.Cooldown)
            {
                types.Add(MagicType.AugmentNeutralize);
                var power = augmentNeutralize.GetPower() + 1;
                var possibleTargets = Player.GetTargets(CurrentMap, location, 2);

                while (power >= realTargets.Count)
                {
                    if (possibleTargets.Count == 0) break;

                    MapObject possibleTarget = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                    possibleTargets.Remove(possibleTarget);

                    if (!Functions.InRange(CurrentLocation, possibleTarget.CurrentLocation, Globals.MagicRange)) continue;

                    realTargets.Add(possibleTarget);
                }
            }

            var count = -1;
            foreach (MapObject realTarget in realTargets)
            {
                if (!Player.UseAmulet(1, 0, out Stats stats))
                    break;

                if (augmentNeutralize != null)
                {
                    count++;
                    hasAugmentNeutralize = true;
                }

                var delay = SEnvir.Now.AddMilliseconds(1400 + Functions.Distance(CurrentLocation, realTarget.CurrentLocation) * 48);

                response.Targets.Add(realTarget.ObjectID);

                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, realTarget, target == realTarget, stats, hasAugmentNeutralize));
            }

            if (count > 0)
            {
                augmentNeutralize.Cooldown = SEnvir.Now.AddMilliseconds(augmentNeutralize.Info.Delay);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = augmentNeutralize.Info.Index, Delay = augmentNeutralize.Info.Delay });
            }

            if (target == null)
                response.Locations.Add(location);

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var ob = (MapObject)data[1];
            bool primary = (bool)data[2];
            var stats = (Stats)data[3];
            var hasAugmentNeutralize = (bool)data[4];

            if (ob?.Node == null || !Player.CanAttackTarget(ob) || ob.Level >= Player.Level || (ob.Poison & PoisonType.Neutralize) == PoisonType.Neutralize) return;

            int time = 5 + Magic.Level * 2;

            ob.ApplyPoison(new Poison
            {
                Type = PoisonType.Neutralize,
                Owner = Player,
                TickCount = time,
                TickFrequency = TimeSpan.FromSeconds(1),
            });

            Player.LevelMagic(Magic);

            var augmentNeutralize = GetAugmentedSkill(MagicType.AugmentNeutralize);

            if (!primary && hasAugmentNeutralize && augmentNeutralize != null)
            {
                Player.LevelMagic(augmentNeutralize);
            }
        }
    }
}
