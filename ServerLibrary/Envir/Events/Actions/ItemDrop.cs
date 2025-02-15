using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Models;

namespace Server.Envir.Events.Actions
{
    /// <summary>
    /// Drops one item on a map or region
    /// </summary>
    [EventActionType(EventActionType.ItemDrop)]
    public class ItemDrop : IWorldEventAction, IPlayerEventAction, IMonsterEventAction, IEventAction
    {
        public void Act(PlayerObject triggerPlayer, EventLog log, PlayerEventAction action)
        {
            var owner = log.PlayerEvent.TrackingType == EventTrackingType.Global ? null : triggerPlayer.Character.Account;
            DropItem(action, triggerPlayer.CurrentMap.Instance, triggerPlayer.CurrentMap.InstanceSequence, false, owner);
        }

        public void Act(PlayerObject triggerPlayer, EventLog log, MonsterEventAction action)
        {
            var owner = log.MonsterEvent.TrackingType == EventTrackingType.Global ? null : triggerPlayer.Character.Account;
            DropItem(action, triggerPlayer.CurrentMap.Instance, triggerPlayer.CurrentMap.InstanceSequence, true, owner);
        }

        public void Act(EventLog log, WorldEventAction action)
        {
            DropItem(action, null, 0, false, null);
        }

        private static void DropItem(BaseEventAction action, InstanceInfo instance, byte instanceSequence, bool monsterDrop, AccountInfo owner)
        {
            if (action.ItemParameter1 == null || action.InstanceParameter1 != instance)
                return;

            var map = GetMap(action, instance, instanceSequence);
            if (map == null) return;

            var item = CreateItem(action);
            SpawnItem(item, owner, monsterDrop, action, map, instance, instanceSequence);
        }

        private static Map GetMap(BaseEventAction action, InstanceInfo instance, byte instanceSequence)
        {
            return action.MapParameter1 != null
                ? SEnvir.GetMap(action.MapParameter1, instance, instanceSequence)
                : action.RegionParameter1 != null
                    ? SEnvir.GetMap(action.RegionParameter1.Map, instance, instanceSequence)
                    : null;
        }

        private static UserItem CreateItem(BaseEventAction action)
        {
            var item = SEnvir.CreateDropItem(action.ItemParameter1);
            item.Count = 1;

            foreach (var stat in action.CalculatedStats.Values)
            {
                item.AddStat(stat.Key, stat.Value, StatSource.Enhancement);
                item.StatsChanged();
            }

            return item;
        }

        private static void SpawnItem(UserItem item, AccountInfo owner, bool monsterDrop, BaseEventAction action, Map map, InstanceInfo instance, byte instanceSequence)
        {
            var ob = new ItemObject { Item = item, Account = owner, MonsterDrop = monsterDrop };

            if (action.RegionParameter1 != null)
                ob.Spawn(action.RegionParameter1, instance, instanceSequence);
            else
                ob.Spawn(map, map.GetRandomLocation());
        }
    }
}
