using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.WraithGrip)]
    public class WraithGrip : MagicObject
    {
        protected override Element Element => Element.None;

        public WraithGrip(PlayerObject player, UserMagic magic) : base(player, magic)
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

            if (target.Race == ObjectType.Player ? target.Level >= Player.Level : target.Level > Player.Level + 15)
            {
                Player.Connection.ReceiveChatWithObservers(con => string.Format(con.Language.WraithLevel, target.Name), MessageType.System);

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

            if (ob?.Node == null || !Player.CanAttackTarget(ob)) return;

            int power = Player.GetSP();

            int duration = Magic.GetPower();

            UserMagic touchOfTheDeparted = GetAugmentedSkill(MagicType.TouchOfTheDeparted);

            ob.ApplyPoison(new Poison
            {
                Value = power,
                Type = PoisonType.WraithGrip,
                Owner = Player,
                TickCount = ob.Race == ObjectType.Player ? duration * 7 / 10 : duration,
                TickFrequency = TimeSpan.FromSeconds(1),
                Extra = touchOfTheDeparted,
            });

            if (touchOfTheDeparted != null)
            {
                ob.ApplyPoison(new Poison
                {
                    Value = power,
                    Type = PoisonType.Paralysis,

                    Owner = Player,
                    TickCount = ob.Race == ObjectType.Player ? duration * 3 / 10 : duration,
                    TickFrequency = TimeSpan.FromSeconds(1),
                });

                Player.LevelMagic(touchOfTheDeparted);
            }

            Player.LevelMagic(Magic);
        }
    }
}
