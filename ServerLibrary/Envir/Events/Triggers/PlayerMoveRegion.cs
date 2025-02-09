using Library.SystemModels;
using Server.Models;
using System.Linq;

namespace Server.Envir.Events.Triggers
{
    /// <summary>
    /// Triggers when a player moves to another region
    /// </summary>
    [EventTriggerType("PLAYERMOVEREGION")]
    public class PlayerMoverRegion : IPlayerEventTrigger, IEventTrigger
    {
        public PlayerEventTriggerType[] PlayerTypes => [PlayerEventTriggerType.PlayerEnter, PlayerEventTriggerType.PlayerLeave];

        public static bool QuickCheck(PlayerObject player)
        {
            if (player.PreviousCell == null)
            {
                return false;
            }

            if (player.PreviousCell.Regions.Count != player.CurrentCell.Regions.Count)
            {
                return true;
            }

            var previousRegionIndexes = player.PreviousCell.Regions.Select(r => r.Index).ToHashSet();
            var currentRegionIndexes = player.CurrentCell.Regions.Select(r => r.Index).ToHashSet();

            if (player.CurrentCell.Regions.Any(r => !previousRegionIndexes.Contains(r.Index)))
            {
                return true;
            }

            if (player.PreviousCell.Regions.Any(r => !currentRegionIndexes.Contains(r.Index)))
            {
                return true;
            }

            return false;
        }

        public bool Check(PlayerObject player, PlayerEventTrigger eventTrigger)
        {
            if (eventTrigger.RegionParameter1 == null)
            {
                return false;
            }

            return IsValidRegionCheck(player, eventTrigger);
        }

        private static bool IsValidRegionCheck(PlayerObject player, PlayerEventTrigger eventTrigger)
        {
            return eventTrigger.InstanceParameter1 == null || player.CurrentMap.Instance == eventTrigger.InstanceParameter1
                ? IsValidRegionTransition(player, eventTrigger)
                : false;
        }

        private static bool IsValidRegionTransition(PlayerObject player, PlayerEventTrigger eventTrigger)
        {
            if (player.CurrentCell == null || player.PreviousCell == null)
            {
                return false;
            }

            return eventTrigger.Type switch
            {
                PlayerEventTriggerType.PlayerEnter => player.CurrentCell.Regions.Contains(eventTrigger.RegionParameter1) && !player.PreviousCell.Regions.Contains(eventTrigger.RegionParameter1),
                PlayerEventTriggerType.PlayerLeave => !player.CurrentCell.Regions.Contains(eventTrigger.RegionParameter1) && player.PreviousCell.Regions.Contains(eventTrigger.RegionParameter1),
                _ => false
            };
        }
    }
}
