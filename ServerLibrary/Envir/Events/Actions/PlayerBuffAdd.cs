using Library;
using Library.SystemModels;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Envir.Events.Actions
{
    /// <summary>
    /// Adds Buff to players
    /// </summary>
    [EventActionType(EventActionType.PlayerBuffAdd)]
    public class PlayerBuffAdd : IWorldEventAction, IPlayerEventAction, IMonsterEventAction, IEventAction
    {
        public void Act(PlayerObject triggerPlayer, EventLog log, MonsterEventAction action)
        {
            ApplyBuff(action, log.MonsterEvent.TrackingType, triggerPlayer);
        }

        public void Act(PlayerObject triggerPlayer, EventLog log, PlayerEventAction action)
        {
            ApplyBuff(action, log.PlayerEvent.TrackingType, triggerPlayer);
        }

        public void Act(EventLog log, WorldEventAction action)
        {
            ApplyBuff(action, EventTrackingType.Global, null);
        }

        private static void ApplyBuff(BaseEventAction action, EventTrackingType trackingType, PlayerObject triggerPlayer)
        {
            if (string.IsNullOrEmpty(action.StringParameter1) ||
                !Enum.TryParse(action.StringParameter1, true, out BuffType type)) return;

            if (action.InstanceParameter1 != triggerPlayer?.CurrentMap.Instance) return;

            if (action.Restrict && triggerPlayer != null)
            {
                triggerPlayer.BuffAdd(type, TimeSpan.MaxValue, action.CalculatedStats, false, false, TimeSpan.Zero);
                return;
            }

            var map = GetTargetMap(action, triggerPlayer?.CurrentMap.Instance, triggerPlayer?.CurrentMap.InstanceSequence ?? 0);

            var targetPlayers = GetTargetPlayers(trackingType, triggerPlayer, map?.Players ?? SEnvir.Players);
            for (int i = 0; i < targetPlayers.Count; i++)
            {
                if (action.RegionParameter1 == null || targetPlayers[i].CurrentCell.Regions.Contains(action.RegionParameter1))
                {
                    targetPlayers[i].BuffAdd(type, TimeSpan.MaxValue, action.CalculatedStats, false, false, TimeSpan.Zero);

                    if (action.Restrict) break;
                }
            }
        }

        private static Map GetTargetMap(BaseEventAction action, InstanceInfo playerInstance, byte playerInstanceSequence)
        {
            return action.MapParameter1 != null
                ? SEnvir.GetMap(action.MapParameter1, playerInstance, playerInstanceSequence)
                : action.RegionParameter1 != null
                    ? SEnvir.GetMap(action.RegionParameter1.Map, playerInstance, playerInstanceSequence)
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
