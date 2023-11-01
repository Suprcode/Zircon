using Library;
using Library.Network.ClientPackets;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;

using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Purification)]
    public class Purification : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public Purification(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target
            };

            var realTargets = new HashSet<MapObject>();

            if (target != null && (Player.CanAttackTarget(target) || Player.CanHelpTarget(target)))
            {
                realTargets.Add(target);
            }
            else
            {
                realTargets.Add(Player);
                response.Ob = null;
            }

            var augmentPurification = GetAugmentedSkill(MagicType.AugmentPurification);

            if (augmentPurification != null && SEnvir.Now > augmentPurification.Cooldown)
            {
                var power = augmentPurification.GetPower() + 1;

                var possibleTargets = Player.GetAllObjects(location, 3);

                while (power >= realTargets.Count)
                {
                    if (possibleTargets.Count == 0) break;

                    MapObject possibleTarget = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                    possibleTargets.Remove(possibleTarget);

                    if (!Functions.InRange(CurrentLocation, possibleTarget.CurrentLocation, Globals.MagicRange)) continue;

                    if (!Player.CanAttackTarget(target) && Player.CanHelpTarget(target))
                    {
                        realTargets.Add(possibleTarget);
                    }
                }
            }

            var count = -1;

            var hasAugmentPurification = false;

            foreach (MapObject realTarget in realTargets)
            {
                if (!Player.UseAmulet(2, 0))
                    break;

                if (augmentPurification != null)
                {
                    hasAugmentPurification = true;
                    count++;
                }

                response.Targets.Add(realTarget.ObjectID);

                var delay = SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, realTarget.CurrentLocation) * 48);

                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, realTarget, hasAugmentPurification));
            }

            if (count > 0)
            {
                augmentPurification.Cooldown = SEnvir.Now.AddMilliseconds(augmentPurification.Info.Delay);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = augmentPurification.Info.Index, Delay = augmentPurification.Info.Delay });
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var ob = (MapObject)data[1];
            var hasAugmentPurification = (bool)data[2];

            if (ob?.Node == null) return;

            if (SEnvir.Random.Next(100) > 40 + Magic.Level * 20) return;

            var augmentPurification = GetAugmentedSkill(MagicType.AugmentPurification);

            int result = Player.Purify(ob);

            for (int i = 0; i < result; i++)
            {
                Player.LevelMagic(Magic);

                if (hasAugmentPurification && augmentPurification != null)
                {
                    Player.LevelMagic(augmentPurification);
                }
            }
        }
    }
}
