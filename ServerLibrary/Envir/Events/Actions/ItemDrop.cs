using Library.SystemModels;
using Server.DBModels;
using Server.Models;

namespace Server.Envir.Events.Actions
{
    [EventActionType(EventActionType.ItemDrop)]
    public class ItemDrop : IWorldEventAction, IPlayerEventAction, IMonsterEventAction, IEventAction
    {
        public void Act(PlayerObject player, EventLog log, PlayerEventAction action)
        {
            AccountInfo owner = null;

            if (log.MonsterEvent.TrackingType != EventTrackingType.Global)
            {
                owner = player.Character.Account;
            }

            DropItem(action, player.CurrentMap.Instance, player.CurrentMap.InstanceSequence, false, owner);
        }

        public void Act(PlayerObject player, EventLog log, MonsterEventAction action)
        {
            AccountInfo owner = null;

            if (log.MonsterEvent.TrackingType != EventTrackingType.Global)
            {
                owner = player.Character.Account;
            }

            DropItem(action, player.CurrentMap.Instance, player.CurrentMap.InstanceSequence, true, owner);
        }

        public void Act(EventLog log, WorldEventAction action)
        {
            DropItem(action, null, 0, false, null);
        }

        private static void DropItem(BaseEventAction action, InstanceInfo playerInstance, byte playerInstanceSequence, bool monsterDrop, AccountInfo owner)
        {
            if (action.ItemParameter1 == null) return;

            if (action.InstanceParameter1 != playerInstance)
                return;

            if (action.MapParameter1 != null)
            {
                var map = action.InstanceParameter1 == null
                    ? SEnvir.GetMap(action.MapParameter1)
                    : SEnvir.GetMap(action.MapParameter1, playerInstance, playerInstanceSequence);

                UserItem item = SEnvir.CreateDropItem(action.ItemParameter1);
                item.Count = 1;

                ItemObject ob = new ItemObject
                {
                    Item = item,
                    Account = owner,
                    MonsterDrop = monsterDrop,
                };

                ob.Spawn(map, map.GetRandomLocation());
            }
            else if (action.RegionParameter1 != null)
            {
                var map = action.InstanceParameter1 == null
                    ? SEnvir.GetMap(action.RegionParameter1.Map)
                    : SEnvir.GetMap(action.RegionParameter1.Map, playerInstance, playerInstanceSequence);

                UserItem item = SEnvir.CreateDropItem(action.ItemParameter1);
                item.Count = 1;

                ItemObject ob = new ItemObject
                {
                    Item = item,
                    Account = owner,
                    MonsterDrop = monsterDrop,
                };

                ob.Spawn(action.RegionParameter1, map.Instance, map.InstanceSequence);
            }
        }
    }
}
