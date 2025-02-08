using Library;
using Library.SystemModels;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Server.Envir.Events.Actions
{
    /// <summary>
    /// Spawns monsters near players
    /// </summary>
    [EventActionType(EventActionType.MonsterPlayerSpawn)]
    public class MonsterPlayerSpawn : IWorldEventAction, IPlayerEventAction, IMonsterEventAction, IEventAction
    {
        public void Act(PlayerObject triggerPlayer, EventLog log, MonsterEventAction action)
        {
            SpawnMonster(action, log.MonsterEvent.TrackingType, triggerPlayer);
        }

        public void Act(PlayerObject triggerPlayer, EventLog log, PlayerEventAction action)
        {
            SpawnMonster(action, log.PlayerEvent.TrackingType, triggerPlayer);
        }

        public void Act(EventLog log, WorldEventAction action)
        {
            SpawnMonster(action, EventTrackingType.Global, null);
        }

        private static void SpawnMonster(BaseEventAction action, EventTrackingType trackingType, PlayerObject triggerPlayer)
        {
            if (action.MonsterParameter1 == null) return;

            if (action.InstanceParameter1 != triggerPlayer?.CurrentMap.Instance) return;

            if (action.Restrict && triggerPlayer != null)
            {
                var nearby = triggerPlayer.CurrentMap.GetRandomLocation(triggerPlayer.CurrentLocation, 10);

                MonsterObject mob = MonsterObject.GetMonster(action.MonsterParameter1);
                mob.Spawn(triggerPlayer.CurrentMap, nearby);
                return;
            }

            var map = GetTargetMap(action, triggerPlayer?.CurrentMap.Instance, triggerPlayer?.CurrentMap.InstanceSequence ?? 0);

            var players = GetTargetPlayers(trackingType, triggerPlayer, map?.Players ?? SEnvir.Players);
            foreach (var player in players)
            {
                if (action.RegionParameter1 == null || player.CurrentCell.Regions.Contains(action.RegionParameter1))
                {
                    var nearby = player.CurrentMap.GetRandomLocation(player.CurrentLocation, 10);

                    MonsterObject mob = MonsterObject.GetMonster(action.MonsterParameter1);
                    mob.Spawn(player.CurrentMap, nearby);

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

