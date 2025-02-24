using Library;
using Library.SystemModels;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Envir.Events.Actions
{
    /// <summary>
    /// Gives players an item
    /// </summary>
    [EventActionType(EventActionType.ItemGive)]
    public class ItemGive : IWorldEventAction, IPlayerEventAction, IMonsterEventAction, IEventAction
    {
        public void Act(PlayerObject triggerPlayer, EventLog log, PlayerEventAction action)
        {
            GiveItem(action, log.MonsterEvent.TrackingType, triggerPlayer);
        }

        public void Act(PlayerObject triggerPlayer, EventLog log, MonsterEventAction action)
        {
            GiveItem(action, log.PlayerEvent.TrackingType, triggerPlayer);
        }

        public void Act(EventLog log, WorldEventAction action)
        {
            GiveItem(action, EventTrackingType.Global, null);
        }

        private static void GiveItem(BaseEventAction action, EventTrackingType trackingType, PlayerObject triggerPlayer)
        {
            if (action.ItemParameter1 == null) return;

            if (action.InstanceParameter1 != triggerPlayer?.CurrentMap.Instance) return;

            if (action.Restrict && triggerPlayer != null)
            {
                GiveItem(triggerPlayer, action.ItemParameter1, action.CalculatedStats);
                return;
            }

            var map = GetTargetMap(action, triggerPlayer?.CurrentMap.Instance, triggerPlayer?.CurrentMap.InstanceSequence ?? 0);

            var targetPlayers = GetTargetPlayers(trackingType, triggerPlayer, map?.Players ?? SEnvir.Players);
            for (int i = 0; i < targetPlayers.Count; i++)
            {
                GiveItem(targetPlayers[i], action.ItemParameter1, action.CalculatedStats);

                if (action.Restrict) break;
            }
        }

        private static void GiveItem(PlayerObject player, ItemInfo item, Stats stats)
        {
            ItemCheck check = new(item, 1, UserItemFlags.None, TimeSpan.Zero);
            if (!player.CanGainItems(false, check)) return;

            var userItem = SEnvir.CreateFreshItem(check);
      
            foreach (var stat in stats.Values)
            {
                userItem.AddStat(stat.Key, stat.Value, StatSource.Enhancement);
                userItem.StatsChanged();
            }

            player.GainItem(userItem);
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
