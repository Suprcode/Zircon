using Library.SystemModels;
using Server.Models;
using System.Linq;

namespace Server.Envir.Events.Triggers
{
    /// <summary>
    /// Triggers when a player moves to another map/region/instance
    /// </summary>
    [EventTriggerType("PLAYERMOVE")]
    public class PlayerMove : IPlayerEventTrigger, IEventTrigger
    {
        public PlayerEventTriggerType[] PlayerTypes => [PlayerEventTriggerType.PlayerEnter, PlayerEventTriggerType.PlayerLeave];

        public static bool QuickCheck(PlayerObject player)
        {
            if (player.CurrentMap != player.PreviousMap)
            {
                return true;
            }

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
            if (eventTrigger.MapParameter1 != null && !IsValidMapTransition(player, eventTrigger))
            {
                return false;
            }

            if (eventTrigger.RegionParameter1 != null && !IsValidRegionTransition(player, eventTrigger))
            {
                return false;
            }

            if (eventTrigger.InstanceParameter1 != null && !IsValidInstanceTransition(player, eventTrigger))
            {
                return false;
            }

            return true;
        }

        private static bool IsValidMapTransition(PlayerObject player, PlayerEventTrigger eventTrigger)
        {
            if (player.CurrentMap == null || player.PreviousMap == null)
            {
                return false;
            }

            return eventTrigger.Type switch
            {
                PlayerEventTriggerType.PlayerEnter => eventTrigger.MapParameter1 == player.CurrentMap.Info && eventTrigger.MapParameter1 != player.PreviousMap.Info,
                PlayerEventTriggerType.PlayerLeave => eventTrigger.MapParameter1 != player.CurrentMap.Info && eventTrigger.MapParameter1 == player.PreviousMap.Info,
                _ => false
            };
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

        private static bool IsValidInstanceTransition(PlayerObject player, PlayerEventTrigger eventTrigger)
        {
            if (player.CurrentMap == null || player.PreviousMap == null)
            {
                return false;
            }

            return eventTrigger.Type switch
            {
                PlayerEventTriggerType.PlayerEnter => eventTrigger.InstanceParameter1 == player.CurrentMap.Instance && eventTrigger.InstanceParameter1 != player.PreviousMap.Instance,
                PlayerEventTriggerType.PlayerLeave => eventTrigger.InstanceParameter1 != player.CurrentMap.Instance && eventTrigger.InstanceParameter1 == player.PreviousMap.Instance,
                _ => false
            };
        }
    }
}
