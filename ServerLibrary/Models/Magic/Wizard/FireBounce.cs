using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magic
{
    [MagicType(MagicType.FireBounce)]
    public class FireBounce : MagicObject
    {
        public override Element Element => Element.Fire;

        public FireBounce(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            if (!Player.CanAttackTarget(target))
                return new MagicCast();

            var bounce = Bounce(Player, target, -1);

            bounce.Targets.Add(target.ObjectID);

            return bounce;
        }

        public override void MagicComplete(params object[] data)
        {
            MapObject target = (MapObject)data[1];
            Point targetLocation = (Point)data[2];
            int bounce = (int)data[3];

            if (!Player.CanAttackTarget(target))
                return;

            if (!Functions.InRange(target.CurrentLocation, targetLocation, Globals.MagicRange))
                return;

            if (Player.MagicAttack(new List<MagicType> { Type }, target, true) < 1)
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

                Bounce(target, nextTarget, --bounce);
            }
        }

        private MagicCast Bounce(MapObject source, MapObject target, int bounce = -1)
        {
            var response = new MagicCast();

            if (!Player.CanAttackTarget(target) || bounce == 0)
                return response;

            var delay = SEnvir.Now.AddMilliseconds(Functions.Distance(source.CurrentLocation, target.CurrentLocation) * 48);

            if (bounce == -1)
            {
                bounce = Magic.Level + 2;
                delay = delay.AddMilliseconds(500);
            }
            else
            {
                Player.Broadcast(new S.ObjectProjectile
                {
                    ObjectID = source.ObjectID,
                    Direction = source.Direction,
                    CurrentLocation = source.CurrentLocation,
                    Type = Magic.Info.Magic,
                    Targets = new List<uint> { target.ObjectID },
                    Locations = new List<Point>()
                });
            }

            Player.ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type, target, target.CurrentLocation, bounce));

            return response;
        }

        public override int ModifyPower1(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower() + Player.GetMC();

            return power;
        }
    }
}
