using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.EvilSlayer)]
    public class EvilSlayer : MagicObject
    {
        protected override Element Element => Element.Holy;


        public EvilSlayer(PlayerObject player, UserMagic magic) : base(player, magic)
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

            var augmentEvilSlayer = GetAugmentedSkill(MagicType.AugmentEvilSlayer);

            if (augmentEvilSlayer != null && SEnvir.Now > augmentEvilSlayer.Cooldown)
            {
                var power = augmentEvilSlayer.GetPower() + 1;

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
            var hasAugmentEvilSlayer = false;

            foreach (MapObject realTarget in realTargets)
            {
                Stats stats = null;

                if (Player.Equipment[(int)EquipmentSlot.Amulet]?.Info.Stats[Stat.HolyAffinity] > 0)
                    Player.UseAmulet(1, 0, out stats);
                else
                    stats = null;

                if (augmentEvilSlayer != null)
                {
                    count++;
                    hasAugmentEvilSlayer = true;
                }

                response.Targets.Add(realTarget.ObjectID);

                var delay = SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, realTarget.CurrentLocation) * 48);

                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, realTarget, realTarget == target, stats, hasAugmentEvilSlayer));
            }

            if (count > 0)
            {
                augmentEvilSlayer.Cooldown = SEnvir.Now.AddMilliseconds(augmentEvilSlayer.Info.Delay);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = augmentEvilSlayer.Info.Index, Delay = augmentEvilSlayer.Info.Delay });
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
            bool hasAugmentEvilSlayer = (bool)data[4];

            var magics = new List<MagicType> { Type };

            if (hasAugmentEvilSlayer)
            {
                magics.Add(MagicType.AugmentEvilSlayer);
            }

            var damage = Player.MagicAttack(magics, target, primary, stats);

            var augmentEvilSlayer = GetAugmentedSkill(MagicType.AugmentEvilSlayer);

            if (!primary && damage > 0 && hasAugmentEvilSlayer && augmentEvilSlayer != null)
            {
                Player.LevelMagic(augmentEvilSlayer);
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower() + Player.GetSC();

            return power;
        }

        public override int ModifyPowerMultiplier(bool primary, int power, Stats stats = null, int extra = 0)
        {
            if (stats != null && stats[Stat.HolyAffinity] >= 1)
                power += (int)(power * 0.3F);

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
