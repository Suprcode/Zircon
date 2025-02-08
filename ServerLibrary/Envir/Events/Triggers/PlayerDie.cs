using Library.SystemModels;
using Server.Models;

namespace Server.Envir.Events.Triggers
{
    /// <summary>
    /// Triggers when a player dies
    /// </summary>
    [EventTriggerType("PLAYERDIE")]
    public class PlayerDie : IPlayerEventTrigger, IEventTrigger
    {
        public PlayerEventTriggerType[] PlayerTypes => [PlayerEventTriggerType.PlayerDie];

        public bool Check(PlayerObject player, PlayerEventTrigger eventTrigger)
        {
            if (eventTrigger.MapParameter1 != null && player.CurrentMap.Info != eventTrigger.MapParameter1)
            {
                return false;
            }

            if (eventTrigger.RegionParameter1 != null && !player.CurrentCell.Regions.Contains(eventTrigger.RegionParameter1))
            {
                return false;
            }

            if (eventTrigger.InstanceParameter1 != null && player.CurrentMap.Instance != eventTrigger.InstanceParameter1)
            {
                return false;
            }

            return true;
        }
    }
}
