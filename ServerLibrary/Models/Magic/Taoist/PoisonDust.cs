using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

using S = Library.Network.ServerPackets;

namespace Server.Models.Magic.Taoist
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

            Player.Magics.TryGetValue(MagicType.GreaterPoisonDust, out UserMagic augMagic);

            var realTargets = new HashSet<MapObject>();

            if (Player.CanAttackTarget(target))
            {
                realTargets.Add(target);
            }

            List<uint> targets = new List<uint>();

            if (augMagic != null && SEnvir.Now > augMagic.Cooldown && Player.Level >= augMagic.Info.NeedLevel1)
            {
                var power = augMagic.GetPower() + 1;
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

            var count = -1;
            foreach (MapObject realTarget in realTargets)
            {
                int shape;
                bool aug = false;

                if (!Player.UsePoison(1, out shape))
                    break;

                if (augMagic != null)
                {
                    count++;
                    aug = true;
                }

                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type, realTarget, shape == 0 ? PoisonType.Green : PoisonType.Red, aug));
            }

            if (count > 0)
            {
                augMagic.Cooldown = SEnvir.Now.AddMilliseconds(augMagic.Info.Delay);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = augMagic.Info.Index, Delay = augMagic.Info.Delay });
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
            bool aug = (bool)data[3];

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

            if (aug && Player.Magics.TryGetValue(MagicType.GreaterPoisonDust, out UserMagic augMagic))
            {
                Player.LevelMagic(augMagic);
            }
        }
    }
}
