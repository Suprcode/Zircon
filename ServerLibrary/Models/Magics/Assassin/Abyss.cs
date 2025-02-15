using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Abyss)]
    public class Abyss : MagicObject
    {
        protected override Element Element => Element.None;

        public Abyss(PlayerObject player, UserMagic magic) : base(player, magic)
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

            if ((target.Race == ObjectType.Player && target.Level >= Player.Level) || (target.Race == ObjectType.Monster && ((MonsterObject)target).MonsterInfo.IsBoss))
            {
                Player.Connection.ReceiveChatWithObservers(con => string.Format(con.Language.AbyssLevel, target.Name), MessageType.System);

                response.Ob = null;
                response.Cast = false;
                return response;
            }

            response.Targets.Add(target.ObjectID);

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, target));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var ob = (MapObject)data[1];

            if (ob?.Node == null || !Player.CanAttackTarget(ob) || (ob.Poison & PoisonType.Abyss) == PoisonType.Abyss) return;

            int power = Player.GetSP();

            int duration = (Magic.Level + 3) * 2;

            if (ob.Race == ObjectType.Monster)
                duration *= 2;

            ob.ApplyPoison(new Poison
            {
                Value = power,
                Type = PoisonType.Abyss,
                Owner = Player,
                TickCount = duration,
                TickFrequency = TimeSpan.FromSeconds(1),
            });

            if (ob.Race == ObjectType.Monster)
                ((MonsterObject)ob).Target = null;

            Player.LevelMagic(Magic);
        }
    }
}
