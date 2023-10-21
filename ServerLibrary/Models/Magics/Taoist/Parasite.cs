using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Parasite)]
    public class Parasite : MagicObject
    {
        protected override Element Element => Element.None;

        public Parasite(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target
            };

            if (!Player.CanAttackTarget(target))
            {
                response.Locations.Add(location);
                response.Ob = null;
                return response;
            }

            response.Targets.Add(target.ObjectID);

            var delay = SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, location) * 48);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, target));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var ob = (MapObject)data[1];

            if (ob?.Node == null || !Player.CanAttackTarget(ob) || (ob.Poison & PoisonType.Parasite) == PoisonType.Parasite) return;

            ob.ApplyPoison(new Poison
            {
                Value = Magic.GetPower(),
                Type = PoisonType.Parasite,
                Owner = Player,
                TickCount = 10 + Magic.Level * 5,
                TickFrequency = TimeSpan.FromSeconds(2),
            });

            Player.LevelMagic(Magic);
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower() + Player.GetSC() / 2;

            return power;
        }
    }
}
