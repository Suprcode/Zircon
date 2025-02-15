using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.PoisonDust)]
    public class PoisonDust : MagicObject
    {
        protected override Element Element => Element.None;

        public PoisonDust(PlayerObject player, UserMagic magic) : base(player, magic)
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
                realTargets.Add(target);
            }

            List<uint> targets = new List<uint>();

            var greaterPoisonDust = GetAugmentedSkill(MagicType.AugmentPoisonDust);

            if (greaterPoisonDust != null && SEnvir.Now > greaterPoisonDust.Cooldown)
            {
                var power = greaterPoisonDust.GetPower() + 1;
                var possibleTargets = Player.GetTargets(CurrentMap, location, 3);

                while (power >= realTargets.Count)
                {
                    if (possibleTargets.Count == 0) break;

                    MapObject possibleTarget = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                    possibleTargets.Remove(possibleTarget);

                    if (!Functions.InRange(CurrentLocation, possibleTarget.CurrentLocation, Globals.MagicRange)) continue;

                    realTargets.Add(possibleTarget);
                }
            }

            var delay = SEnvir.Now.AddMilliseconds(500);

            var hasGreaterPoisonDust = false;

            var count = -1;
            foreach (MapObject realTarget in realTargets)
            {
                int shape;

                if (!Player.UsePoison(1, out shape))
                    break;

                if (greaterPoisonDust != null)
                {
                    count++;
                    hasGreaterPoisonDust = true;
                }

                response.Targets.Add(realTarget.ObjectID);

                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, realTarget, shape == 0 ? PoisonType.Green : PoisonType.Red, hasGreaterPoisonDust));
            }

            if (count > 0)
            {
                greaterPoisonDust.Cooldown = SEnvir.Now.AddMilliseconds(greaterPoisonDust.Info.Delay);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = greaterPoisonDust.Info.Index, Delay = greaterPoisonDust.Info.Delay });
            }

            if (target == null)
            {
                response.Locations.Add(location);
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            MapObject ob = (MapObject)data[1];
            PoisonType type = (PoisonType)data[2];
            bool hasGreaterPoisonDust = (bool)data[3];

            if (ob?.Node == null || !Player.CanAttackTarget(ob)) return;

            for (int i = Player.Pets.Count - 1; i >= 0; i--)
                if (Player.Pets[i].Target == null)
                    Player.Pets[i].Target = ob;

            int duration = Magic.GetPower() + Player.GetSC() + Player.Stats[Stat.DarkAttack] * 2;

            ob.ApplyPoison(new Poison
            {
                Value = Magic.Level + 1 + Player.Level / 14,
                Type = type,
                Owner = Player,
                TickCount = duration / 2,
                TickFrequency = TimeSpan.FromSeconds(2),
            });

            Player.LevelMagic(Magic);

            var greaterPoisonDust = GetAugmentedSkill(MagicType.AugmentPoisonDust);

            if (hasGreaterPoisonDust && greaterPoisonDust != null)
            {
                Player.LevelMagic(greaterPoisonDust);
            }
        }
    }
}
