using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magic
{
    [MagicType(MagicType.Infection)]
    public class Infection : MagicObject
    {
        protected override Element Element => Element.None;

        public Infection(PlayerObject player, UserMagic magic) : base(player, magic)
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

            if (ob?.Node == null || !Player.CanAttackTarget(ob) || (ob.Poison & PoisonType.Infection) == PoisonType.Infection) return;

            ob.ApplyPoison(new Poison
            {
                Value = Player.GetSC() + Player.Stats[Stat.CriticalChance] + Player.Stats[Stat.CriticalDamage],
                Type = PoisonType.Infection,
                Owner = Player,
                TickCount = 10 + Magic.Level * 10,
                TickFrequency = TimeSpan.FromSeconds(1),
            });

            Player.LevelMagic(Magic);
        }
    }
}
