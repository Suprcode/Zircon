using Library.SystemModels;
using Server.Envir.Commands.Command.Player;
using Server.Models;

namespace Server.Envir.Events.Triggers
{
    /// <summary>
    /// Triggers when a player dies
    /// </summary>
    [EventTriggerType("PLAYERCOMMAND")]
    public class PlayerCommand : IPlayerEventTrigger, IEventTrigger
    {
        public PlayerEventTriggerType[] PlayerTypes => [PlayerEventTriggerType.PlayerCommand];

        public bool Check(PlayerObject player, PlayerEventTrigger eventTrigger)
        {
            if (string.IsNullOrEmpty(eventTrigger.StringParameter1))
            {
                return false;
            }

            if (!Event.LastCommand.TryGetValue(player.Name, out string value))
            {
                return false;
            }

            if (string.Compare(eventTrigger.StringParameter1, value, true) != 0)
            {
                return false;
            }

            return true;
        }
    }
}
