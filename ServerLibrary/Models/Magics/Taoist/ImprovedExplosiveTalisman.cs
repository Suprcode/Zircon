using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;

using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.ImprovedExplosiveTalisman)]
    public class ImprovedExplosiveTalisman : MagicObject
    {
        protected override Element Element => Element.Dark;

        public ImprovedExplosiveTalisman(PlayerObject player, UserMagic magic) : base(player, magic)
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
                realTargets.Add(target);

            var augmentExplosiveTalisman = GetAugmentedSkill(MagicType.AugmentExplosiveTalisman);

            if (augmentExplosiveTalisman != null && SEnvir.Now > augmentExplosiveTalisman.Cooldown)
            {
                var power = augmentExplosiveTalisman.GetPower() + 1;
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

            var hasAugmentExplosiveTalisman = false;

            var count = -1;
            foreach (MapObject realTarget in realTargets)
            {
                if (!Player.UseAmulet(1, 0, out Stats stats))
                    break;

                if (augmentExplosiveTalisman != null)
                {
                    count++;
                    hasAugmentExplosiveTalisman = true;
                }

                response.Targets.Add(realTarget.ObjectID);

                var delay = SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, realTarget.CurrentLocation) * 48);

                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, realTarget, realTarget == target, stats, hasAugmentExplosiveTalisman));
            }

            if (count > 0)
            {
                augmentExplosiveTalisman.Cooldown = SEnvir.Now.AddMilliseconds(augmentExplosiveTalisman.Info.Delay);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = augmentExplosiveTalisman.Info.Index, Delay = augmentExplosiveTalisman.Info.Delay });
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
            var hasAugmentExplosiveTalisman = (bool)data[4];

            var magics = new List<MagicType> { Type };

            if (hasAugmentExplosiveTalisman)
            {
                magics.Add(MagicType.AugmentExplosiveTalisman);
            }

            var damage = Player.MagicAttack(magics, target, primary, stats);

            var augmentExplosiveTalisman = GetAugmentedSkill(MagicType.AugmentExplosiveTalisman);

            if (!primary && damage > 0 && hasAugmentExplosiveTalisman && augmentExplosiveTalisman != null)
            {
                Player.LevelMagic(augmentExplosiveTalisman);
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower() + Player.GetSC();

            return power;
        }

        public override int ModifyPowerMultiplier(bool primary, int power, Stats stats = null, int extra = 0)
        {
            if (stats != null && stats[Stat.DarkAffinity] >= 1)
                power += (int)(power * 0.6F);

            if (!primary)
            {
                power = (int)(power * 0.65F);
                //  if (ob.Race == ObjectType.Player)
                //      power = (int)(power * 0.5F);
            }

            return power;
        }
    }
}
