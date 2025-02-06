using Library;
using Library.SystemModels;
using Server.Models;
using S = Library.Network.ServerPackets;

namespace Server.Envir.Events.Actions
{
    [EventActionType(EventActionType.GlobalMessage)]
    public class GlobalMessage : IWorldEventAction, IPlayerEventAction, IMonsterEventAction, IEventAction
    {
        public void Act(PlayerObject player, EventLog log, MonsterEventAction action)
        {
            SendGlobalMessage(action.StringParameter1);
        }

        public void Act(PlayerObject player, EventLog log, PlayerEventAction action)
        {
            SendGlobalMessage(action.StringParameter1);
        }

        public void Act(EventLog log, WorldEventAction action)
        {
            SendGlobalMessage(action.StringParameter1);
        }

        private static void SendGlobalMessage(string message)
        {
            SEnvir.Broadcast(new S.Chat { Text = message, Type = MessageType.System });
        }
    }
}
