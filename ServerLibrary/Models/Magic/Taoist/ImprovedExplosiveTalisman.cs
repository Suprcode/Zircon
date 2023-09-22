using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;

using S = Library.Network.ServerPackets;

namespace Server.Models.Magic
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

            Player.Magics.TryGetValue(MagicType.AugmentExplosiveTalisman, out UserMagic augMagic);

            var realTargets = new HashSet<MapObject>();

            if (Player.CanAttackTarget(target))
                realTargets.Add(target);

            if (augMagic != null && SEnvir.Now > augMagic.Cooldown && Player.Level >= augMagic.Info.NeedLevel1)
            {
                var power = augMagic.GetPower() + 1;
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

            var aug = false;

            var count = -1;
            foreach (MapObject realTarget in realTargets)
            {
                if (!Player.UseAmulet(1, 0, out Stats stats))
                    break;

                if (augMagic != null)
                {
                    count++;
                    aug = true;
                }

                response.Targets.Add(realTarget.ObjectID);

                var delay = SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, realTarget.CurrentLocation) * 48);

                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type, realTarget, realTarget == target, stats, aug));
            }

            if (count > 0)
            {
                augMagic.Cooldown = SEnvir.Now.AddMilliseconds(augMagic.Info.Delay);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = augMagic.Info.Index, Delay = augMagic.Info.Delay });
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
            var aug = (bool)data[4];

            var magics = new List<MagicType> { Type };

            if (aug)
            {
                magics.Add(MagicType.AugmentExplosiveTalisman);
            }

            var damage = Player.MagicAttack(magics, target, primary, stats);

            if (damage > 0 && aug && Player.Magics.TryGetValue(MagicType.AugmentExplosiveTalisman, out UserMagic augMagic))
            {
                Player.LevelMagic(augMagic);
            }
        }

        public override int ModifyPower1(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower() + Player.GetSC();

            return power;
        }

        public override int ModifyPower2(bool primary, int power, Stats stats = null)
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
