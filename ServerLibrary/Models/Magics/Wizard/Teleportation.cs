using Library;
using Server.DBModels;
using Server.Envir;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Teleportation)]
    public class Teleportation : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public Teleportation(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            { 
                Ob = null 
            };

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            if (CurrentMap.Info.SkillDelay > 0)
            {
                Player.Connection.ReceiveChatWithObservers(con => string.Format(con.Language.SkillBadMap, Magic.Info.Name), MessageType.System);
                return;
            }

            if (SEnvir.Random.Next(9) > 2 + Magic.Level * 2) return;
            /*
            if (CurrentMap.Info.SkillDelay > 0)
            {
                TimeSpan delay = TimeSpan.FromMilliseconds(CurrentMap.Info.SkillDelay * 3);

                Connection.ReceiveChat(con => string.Format(con.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                UseItemTime = (UseItemTime < SEnvir.Now ? SEnvir.Now : UseItemTime) + delay;
                Enqueue(new S.ItemUseDelay { Delay = SEnvir.Now - UseItemTime });
            }*/

            Player.Teleport(CurrentMap, CurrentMap.GetRandomLocation());
            Player.LevelMagic(Magic);
        }
    }
}
