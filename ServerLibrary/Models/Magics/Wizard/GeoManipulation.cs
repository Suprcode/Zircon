using Library;
using Server.DBModels;
using Server.Envir;
using System.Drawing;

using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.GeoManipulation)]
    public class GeoManipulation : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public GeoManipulation(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            if (!Functions.InRange(CurrentLocation, location, Globals.MagicRange))
            {
                response.Cast = false;
                return response;
            }

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, location));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            Point location = (Point)data[1];

            /* if (CurrentMap.Info.SkillDelay > 0)
             {
                 Connection.ReceiveChat(string.Format(Connection.Language.SkillBadMap, magic.Info.Name), MessageType.System);

                 foreach (SConnection con in Connection.Observers)
                     con.ReceiveChat(string.Format(con.Language.SkillBadMap, magic.Info.Name), MessageType.System);
                 return;
             }*/

            if (location == CurrentLocation) return;

            if (SEnvir.Random.Next(100) > 25 + Magic.Level * 25) return;

            if (!Player.Teleport(CurrentMap, location, false)) return;

            /*
            if (CurrentMap.Info.SkillDelay > 0)
            {
                TimeSpan delay = TimeSpan.FromMilliseconds(CurrentMap.Info.SkillDelay);

                Connection.ReceiveChat(string.Format(Connection.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                UseItemTime = (UseItemTime < SEnvir.Now ? SEnvir.Now : UseItemTime) + delay;
                Enqueue(new S.ItemUseDelay { Delay = SEnvir.Now - UseItemTime });
            }*/

            Player.LevelMagic(Magic);

            int delay = Magic.Info.Delay;
            if (SEnvir.Now <= Player.PvPTime.AddSeconds(30))
                delay *= 10;

            Magic.Cooldown = SEnvir.Now.AddMilliseconds(delay);
            Player.Enqueue(new S.MagicCooldown { InfoIndex = Magic.Info.Index, Delay = delay });
        }
    }
}
