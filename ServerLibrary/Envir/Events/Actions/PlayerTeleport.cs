using Library.SystemModels;
using Server.Models;
using System.Collections.Generic;
using System.Linq;

namespace Server.Envir.Events.Actions
{
    /// <summary>
    /// Teleports players to target region
    /// </summary>
    [EventActionType(EventActionType.PlayerTeleport)]
    public class PlayerTeleport : IWorldEventAction, IPlayerEventAction, IMonsterEventAction, IEventAction
    {
        public void Act(PlayerObject triggerPlayer, EventLog log, MonsterEventAction action)
        {
            TeleportPlayer(action, log.MonsterEvent.TrackingType, triggerPlayer);
        }

        public void Act(PlayerObject triggerPlayer, EventLog log, PlayerEventAction action)
        {
            TeleportPlayer(action, log.PlayerEvent.TrackingType, triggerPlayer);
        }

        public void Act(EventLog log, WorldEventAction action)
        {
            TeleportPlayer(action, EventTrackingType.Global, null);
        }

        private static void TeleportPlayer(BaseEventAction action, EventTrackingType trackingType, PlayerObject triggerPlayer)
        {
            if (action.RegionParameter1 == null) return;

            if (action.InstanceParameter1 != triggerPlayer?.CurrentMap.Instance) return;

            if (action.Restrict && triggerPlayer != null)
            {
                triggerPlayer.Teleport(action.RegionParameter1, triggerPlayer.CurrentMap.Instance, triggerPlayer.CurrentMap.InstanceSequence);
                return;
            }

            var map = GetTargetMap(action, triggerPlayer?.CurrentMap.Instance, triggerPlayer?.CurrentMap.InstanceSequence ?? 0);

            var targetPlayers = GetTargetPlayers(trackingType, triggerPlayer, map?.Players ?? SEnvir.Players);
            for (int i = 0; i < targetPlayers.Count; i++)
            {
                targetPlayers[i].Teleport(action.RegionParameter1, triggerPlayer?.CurrentMap.Instance, triggerPlayer?.CurrentMap.InstanceSequence ?? 0);

                if (action.Restrict) break;
            }
        }

        private static Map GetTargetMap(BaseEventAction action, InstanceInfo playerInstance, byte playerInstanceSequence)
        {
            return action.MapParameter1 != null
                ? SEnvir.GetMap(action.MapParameter1, playerInstance, playerInstanceSequence)
                : null;
        }

        private static List<PlayerObject> GetTargetPlayers(EventTrackingType trackingType, PlayerObject triggerPlayer, List<PlayerObject> players)
        {
            if (triggerPlayer == null) return players;

            return trackingType switch
            {
                EventTrackingType.Global => players,
                EventTrackingType.Player => [triggerPlayer],
                EventTrackingType.Group => players.Where(pl => triggerPlayer.GroupMembers.Contains(pl)).ToList(),
                EventTrackingType.Guild => players.Where(pl => pl.Character.Account.GuildMember?.Guild == triggerPlayer.Character.Account.GuildMember?.Guild).ToList(),
                EventTrackingType.Instance => players.Where(pl => pl.CurrentMap.Instance == triggerPlayer.CurrentMap.Instance && pl.CurrentMap.InstanceSequence == triggerPlayer.CurrentMap.InstanceSequence).ToList(),
                _ => []
            };
        }
    }
}

