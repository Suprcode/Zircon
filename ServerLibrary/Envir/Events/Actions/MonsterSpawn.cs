using Library.SystemModels;
using Server.Models;
using System.Linq;

namespace Server.Envir.Events.Actions
{
    [EventActionType(EventActionType.MonsterSpawn)]
    public class MonsterSpawn : IWorldEventAction, IPlayerEventAction, IMonsterEventAction, IEventAction
    {
        public void Act(PlayerObject player, EventLog log, MonsterEventAction action)
        {
            SpawnMonster(action, player.CurrentMap.Instance, player.CurrentMap.InstanceSequence);
        }

        public void Act(PlayerObject player, EventLog log, PlayerEventAction action)
        {
            SpawnMonster(action, player.CurrentMap.Instance, player.CurrentMap.InstanceSequence);
        }

        public void Act(EventLog log, WorldEventAction action)
        {
            SpawnMonster(action, null, 0);
        }

        private static void SpawnMonster(BaseEventAction action, InstanceInfo playerInstance, byte playerInstanceSequence)
        {
            if (action.InstanceParameter1 != playerInstance)
                return;

            if (action.RespawnParameter1 != null && action.InstanceParameter1 == null)
            {
                var spawn = SEnvir.Spawns.FirstOrDefault(x => x.Info == action.RespawnParameter1 && x.CurrentMap.Instance == null);
                spawn?.DoSpawn(true);
                return;
            }

            if (action.RespawnParameter1 == null && action.InstanceParameter1 != null)
            {
                var spawn = SEnvir.Spawns.FirstOrDefault(x => x.Info == action.RespawnParameter1 && x.CurrentMap.Instance == playerInstance && x.CurrentMap.InstanceSequence == playerInstanceSequence);
                spawn?.DoSpawn(true);
                return;
            }

            if (action.MonsterParameter1 == null)
                return;

            if (action.MapParameter1 != null)
            {
                var map = action.InstanceParameter1 == null
                    ? SEnvir.GetMap(action.MapParameter1)
                    : SEnvir.GetMap(action.MapParameter1, playerInstance, playerInstanceSequence);

                var monster = MonsterObject.GetMonster(action.MonsterParameter1);

                monster?.Spawn(map, map.GetRandomLocation());
            }
            else if (action.RegionParameter1 != null)
            {
                var map = action.InstanceParameter1 == null
                    ? SEnvir.GetMap(action.RegionParameter1.Map)
                    : SEnvir.GetMap(action.RegionParameter1.Map, playerInstance, playerInstanceSequence);

                var monster = MonsterObject.GetMonster(action.MonsterParameter1);

                monster?.Spawn(action.RegionParameter1, map.Instance, map.InstanceSequence);
            }
        }
    }
}
