using Library.SystemModels;
using Server.Models;
using System.Linq;

namespace Server.Envir.Events.Actions
{
    /// <summary>
    /// Spawns a monster on a map
    /// </summary>
    [EventActionType(EventActionType.MonsterSpawn)]
    public class MonsterSpawn : IWorldEventAction, IPlayerEventAction, IMonsterEventAction, IEventAction
    {
        public void Act(PlayerObject triggerPlayer, EventLog log, MonsterEventAction action)
        {
            SpawnMonster(action, triggerPlayer.CurrentMap.Instance, triggerPlayer.CurrentMap.InstanceSequence);
        }

        public void Act(PlayerObject triggerPlayer, EventLog log, PlayerEventAction action)
        {
            SpawnMonster(action, triggerPlayer.CurrentMap.Instance, triggerPlayer.CurrentMap.InstanceSequence);
        }

        public void Act(EventLog log, WorldEventAction action)
        {
            SpawnMonster(action, null, 0);
        }

        private static void SpawnMonster(BaseEventAction action, InstanceInfo instance, byte instanceSequence)
        {
            if (action.InstanceParameter1 != instance) return;

            if (TrySpawnFromRespawn(action, instance, instanceSequence)) return;
            if (action.MonsterParameter1 == null) return;

            var map = GetTargetMap(action, instance, instanceSequence);
            if (map == null) return;

            var monster = MonsterObject.GetMonster(action.MonsterParameter1);

            if (action.RegionParameter1 != null)
                monster?.Spawn(action.RegionParameter1, map.Instance, map.InstanceSequence);
            else
                monster?.Spawn(map, map.GetRandomLocation());
        }

        private static bool TrySpawnFromRespawn(BaseEventAction action, InstanceInfo instance, byte instanceSequence)
        {
            if (action.RespawnParameter1 == null) return false;

            var spawn = SEnvir.Spawns.FirstOrDefault(x =>
                x.Info == action.RespawnParameter1 &&
                (action.InstanceParameter1 == null ? x.CurrentMap.Instance == null :
                 x.CurrentMap.Instance == instance && x.CurrentMap.InstanceSequence == instanceSequence));

            spawn?.DoSpawn(true);
            return spawn != null;
        }

        private static Map GetTargetMap(BaseEventAction action, InstanceInfo instance, byte instanceSequence)
        {
            return action.MapParameter1 != null
                ? SEnvir.GetMap(action.MapParameter1, instance, instanceSequence)
                : action.RegionParameter1 != null
                    ? SEnvir.GetMap(action.RegionParameter1.Map, instance, instanceSequence)
                    : null;
        }
    }
}
