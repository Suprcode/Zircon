using Library.SystemModels;
using Server.Models;
using System.Linq;

namespace Server.Envir.Events.Triggers
{
    /// <summary>
    /// Triggers when a player moves to another map/instance
    /// </summary>
    [EventTriggerType("PLAYERMOVEMAP")]
    public class PlayerMoveMap : IPlayerEventTrigger, IEventTrigger
    {
        public PlayerEventTriggerType[] PlayerTypes => [PlayerEventTriggerType.PlayerEnter, PlayerEventTriggerType.PlayerLeave];

        public static bool QuickCheck(PlayerObject player)
        {
            if (player.PreviousMap == null)
            {
                return false;
            }

            if (player.CurrentMap != player.PreviousMap)
            {
                return true;
            }

            return false;
        }

        public bool Check(PlayerObject player, PlayerEventTrigger eventTrigger)
        {
            if (eventTrigger.MapParameter1 != null)
            {
                return IsValidMapCheck(player, eventTrigger);
            }

            return eventTrigger.InstanceParameter1 == null || IsValidInstanceTransition(player, eventTrigger);
        }

        private static bool IsValidMapCheck(PlayerObject player, PlayerEventTrigger eventTrigger)
        {
            return eventTrigger.InstanceParameter1 == null || player.CurrentMap.Instance == eventTrigger.InstanceParameter1
                ? IsValidMapTransition(player, eventTrigger)
                : false;
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
