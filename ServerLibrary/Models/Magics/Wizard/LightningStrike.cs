using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.LightningStrike)]
    public class LightningStrike : MagicObject
    {
        protected override Element Element => Element.Lightning;
        private int MaxStrike => Magic.Level + 2;

        public LightningStrike(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - Effect should be a particle effect connecting the targets together, not a projectile
            //https://youtu.be/DDmB8CTco8o?t=52 
        }

        public override int GetShock(int shock, Stats stats = null)
        {
            var shocked = GetAugmentedSkill(MagicType.Shocked);

            if (shocked != null && SEnvir.Random.Next(4) <= shocked.Level)
            {
                return shocked.GetPower();
            }

            return base.GetShock(shock, stats);
        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            if (!Player.CanAttackTarget(target))
                return new MagicCast();

            var response = Strike(Player, target, -1);

            response.Targets.Add(target.ObjectID);

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            MapObject target = (MapObject)data[1];
            Point targetLocation = (Point)data[2];
            int strikesRemaining = (int)data[3];

            if (!Player.CanAttackTarget(target))
                return;

            if (!Functions.InRange(target.CurrentLocation, targetLocation, Globals.MagicRange))
                return;

            if (Player.MagicAttack(new List<MagicType> { Type }, target, true, null, strikesRemaining) < 1)
                return;

            var targets = new List<MapObject>();

            foreach (var ob in Player.GetTargets(Player.CurrentMap, target.CurrentLocation, 3))
            {
                if (!Player.CanAttackTarget(ob)) continue;

                if (ob.Race != ObjectType.Monster) continue;

                targets.Add(ob);
            }

            if (targets.Count > 0)
            {
                var nextTarget = targets[SEnvir.Random.Next(targets.Count)];

                Strike(target, nextTarget, --strikesRemaining);
            }
        }

        private MagicCast Strike(MapObject source, MapObject target, int strikesRemaining = -1)
        {
            var response = new MagicCast
            {
                Ob = source
            };

            if (!Player.CanAttackTarget(target) || strikesRemaining == 0)
                return response;

            var delay = SEnvir.Now.AddMilliseconds(Functions.Distance(source.CurrentLocation, target.CurrentLocation) * 48);

            if (strikesRemaining == -1)
            {
                strikesRemaining = MaxStrike;
                delay = delay.AddMilliseconds(500);
            }
            else
            {
                Player.Broadcast(new S.ObjectProjectile
                {
                    ObjectID = source.ObjectID,
                    Direction = source.Direction,
                    CurrentLocation = source.CurrentLocation,
                    Type = Type,
                    Targets = new List<uint> { target.ObjectID },
                    Locations = new List<Point>()
                });
            }

            Player.ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, target, target.CurrentLocation, strikesRemaining));

            return response;
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int strikesRemaining = 0)
        {
            power += Player.GetMC() + Magic.GetPower();

            return power;
        }

        public override int ModifyPowerMultiplier(bool primary, int power, Stats stats = null, int strikesRemaining = 0)
        {
            var multiplier = (int)((MaxStrike - strikesRemaining) * (1 / MaxStrike));

            power += (power * multiplier);

            return power;
        }
    }
}
