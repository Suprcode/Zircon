using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Beckon)]
    public class Beckon : MagicObject
    {
        protected override Element Element => Element.None;

        public Beckon(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target
            };

            if (target == null || !Player.CanAttackTarget(target))
            {
                response.Ob = null;
                return response;
            }

            var delay = SEnvir.Now.AddMilliseconds(300);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, target));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            MapObject ob = (MapObject)data[1];

            if (ob == null || ob.CurrentMap != CurrentMap) return;

            switch (ob.Race)
            {
                case ObjectType.Player:
                    if (!Player.CanAttackTarget(ob)) return;
                    if (ob.Level >= Player.Level || ob.Buffs.Any(x => x.Type == BuffType.Endurance)) return;

                    /* if (CurrentMap.Info.SkillDelay > 0)
                     {
                         Connection.ReceiveChat(con => string.Format(con.Language.SkillBadMap, magic.Info.Name), MessageType.System);
                         return;
                     }*/

                    if (SEnvir.Random.Next(10) > 4 + Magic.Level) return;

                    break;
                case ObjectType.Monster:
                    if (!Player.CanAttackTarget(ob)) return;

                    MonsterObject mob = (MonsterObject)ob;
                    if (mob.MonsterInfo.IsBoss || !mob.MonsterInfo.CanPush) return;

                    if (SEnvir.Random.Next(9) > 2 + Magic.Level * 2) return;
                    break;
                case ObjectType.Item:
                    if (SEnvir.Random.Next(9) > 2 + Magic.Level * 2) return;
                    break;
                default:
                    return;
            }

            if (!ob.Teleport(CurrentMap, Functions.Move(CurrentLocation, Direction))) return;

            /*   if (CurrentMap.Info.SkillDelay > 0)
               {
                   TimeSpan delay = TimeSpan.FromMilliseconds(CurrentMap.Info.SkillDelay);

                   Connection.ReceiveChat(con => string.Format(con.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                   UseItemTime = (UseItemTime < SEnvir.Now ? SEnvir.Now : UseItemTime) + delay;
                   Enqueue(new S.ItemUseDelay { Delay = SEnvir.Now - UseItemTime });
               }*/


            if (ob.Race != ObjectType.Item)
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = Player,
                    Type = PoisonType.Paralysis,
                    TickFrequency = TimeSpan.FromSeconds(ob.Race == ObjectType.Monster ? (1 + Magic.Level) : 1),
                    TickCount = 1,
                });
            }

            int delay = Magic.Info.Delay;
            if (SEnvir.Now <= Player.PvPTime.AddSeconds(30))
                delay *= 10;

            Magic.Cooldown = SEnvir.Now.AddMilliseconds(delay);
            Player.Enqueue(new S.MagicCooldown { InfoIndex = Magic.Info.Index, Delay = delay });

            Player.LevelMagic(Magic);
        }

    }
}
