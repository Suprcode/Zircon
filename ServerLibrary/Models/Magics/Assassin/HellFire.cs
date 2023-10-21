using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.HellFire)]
    public class HellFire : MagicObject
    {
        protected override Element Element => Element.Fire;

        public HellFire(PlayerObject player, UserMagic magic) : base(player, magic)
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
                response.Cast = false;
                return response;
            }

            response.Targets.Add(target.ObjectID);

            var delay = SEnvir.Now.AddMilliseconds(1200);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, target));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var ob = (MapObject)data[1];

            if (ob?.Node == null || !Player.CanAttackTarget(ob)) return;

            if (Player.MagicAttack(new List<MagicType> { Type }, ob, true) <= 0) return;

            int power = Math.Min(Player.GetSC(), Player.GetMC()) / 2;

            int duration = Magic.GetPower();

            ob.ApplyPoison(new Poison
            {
                Value = power,
                Type = PoisonType.HellFire,
                Owner = Player,
                TickCount = duration / 2,
                TickFrequency = TimeSpan.FromSeconds(2),
            });

            Player.LevelMagic(Magic);
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower() + Player.GetDC();

            return power;
        }
    }
}
