using Library;
using Server.DBModels;
using Server.Envir;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Interchange)]
    public class Interchange : MagicObject
    {
        protected override Element Element => Element.None;

        public Interchange(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target
            };

            if (target == null)
                return response;

            if (!Player.CanAttackTarget(target))
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
            MapObject target = (MapObject)data[1];

            if (target == null || target.CurrentMap != CurrentMap) return;

            switch (target.Race)
            {
                case ObjectType.Player:
                    if (!Player.CanAttackTarget(target)) return;
                    if (target.Level >= Player.Level || target.Buffs.Any(x => x.Type == BuffType.Endurance)) return;
                    break;
                case ObjectType.Monster:
                    if (!Player.CanAttackTarget(target)) return;
                    if (target.Level >= Player.Level || !((MonsterObject)target).MonsterInfo.CanPush) return;
                    break;
                case ObjectType.Item:
                    break;
                default:
                    return;
            }

            if (SEnvir.Random.Next(9) > 2 + Magic.Level * 2) return;

            Point current = CurrentLocation;

            /*  if (CurrentMap.Info.SkillDelay > 0) return;
              {
                  TimeSpan delay = TimeSpan.FromMilliseconds(CurrentMap.Info.SkillDelay);

                  Connection.ReceiveChat(string.Format(Connection.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                  foreach (SConnection con in Connection.Observers)
                      con.ReceiveChat(string.Format(con.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                  UseItemTime = (UseItemTime < SEnvir.Now ? SEnvir.Now : UseItemTime) + delay;
                  Enqueue(new S.ItemUseDelay { Delay = SEnvir.Now - UseItemTime });
              }*/

            Player.Teleport(CurrentMap, target.CurrentLocation);
            target.Teleport(CurrentMap, current);

            if (target.Race == ObjectType.Player)
            {
                Player.PvPTime = SEnvir.Now;
                ((PlayerObject)target).PvPTime = SEnvir.Now;
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
