using Library;
using Library.SystemModels;
using Server.Models;
using S = Library.Network.ServerPackets;

namespace Server.Envir.Events.Actions
{
    [EventActionType(EventActionType.PlayerMessage)]
    public class PlayerMessage : IPlayerEventAction, IMonsterEventAction, IEventAction
    {
        public void Act(PlayerObject player, EventLog log, MonsterEventAction action)
        {
            SendPlayerMessage(player, action.StringParameter1);
        }

        public void Act(PlayerObject player, EventLog log, PlayerEventAction action)
        {
            SendPlayerMessage(player, action.StringParameter1);
        }

        private static void SendPlayerMessage(PlayerObject player, string message)
        {
            player.Broadcast(new S.Chat { Text = message, Type = MessageType.System });
        }
    }
}
