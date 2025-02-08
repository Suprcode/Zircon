using Library.SystemModels;
using Server.Models;
using System.Linq;

namespace Server.Envir.Events.Triggers
{
    /// <summary>
    /// Triggers when all monster spawns on a map/region/instance are killed
    /// </summary>
    [EventTriggerType("MONSTERCLEAR")]
    public class MonsterClear : IMonsterEventTrigger, IEventTrigger
    {
        public MonsterEventTriggerType[] MonsterTypes => [MonsterEventTriggerType.MonsterClear];

        public bool Check(MonsterObject monster, MonsterEventTrigger eventTrigger)
        {
            if (eventTrigger.MapParameter1 != null && eventTrigger.InstanceParameter1 != null)
            {
                if (!IsValidInstance(monster, eventTrigger.InstanceParameter1)) return false;
                if (!IsValidMap(monster, eventTrigger.MapParameter1)) return false;

                var map = SEnvir.GetMap(eventTrigger.MapParameter1, eventTrigger.InstanceParameter1, monster.CurrentMap.InstanceSequence);
                if (map == null) return false;

                return !ContainsAliveMonsters(map, eventTrigger.RegionParameter1);
            }

            if (eventTrigger.MapParameter1 != null)
            {
                if (!IsValidMap(monster, eventTrigger.MapParameter1)) return false;

                var map = SEnvir.GetMap(eventTrigger.MapParameter1);
                if (map == null) return false;

                return !ContainsAliveMonsters(map, eventTrigger.RegionParameter1);
            }

            if (eventTrigger.InstanceParameter1 != null)
            {
                if (!IsValidInstance(monster, eventTrigger.InstanceParameter1)) return false;

                foreach (var instanceMap in eventTrigger.InstanceParameter1.Maps)
                {
                    var map = SEnvir.GetMap(instanceMap.Map, eventTrigger.InstanceParameter1, monster.CurrentMap.InstanceSequence);
                    if (!ContainsAliveMonsters(map, eventTrigger.RegionParameter1)) return false;
                }
            }

            return false;
        }

        private static bool IsValidInstance(MonsterObject monster, InstanceInfo instance)
        {
            return monster.CurrentMap.Instance != null && instance == monster.CurrentMap.Instance;
        }

        private static bool IsValidMap(MonsterObject monster, MapInfo map)
        {
            return monster.CurrentMap.Info == map;
        }

        private static bool ContainsAliveMonsters(Map map, MapRegion region)
        {
            if (region != null)
            {
                return map.Objects.OfType<MonsterObject>().Any(monster => !monster.Dead && monster.CurrentCell.Regions.Contains(region));
            }

            return SEnvir.Spawns.Any(spawn => spawn.CurrentMap == map && spawn.AliveCount > 0);
        }
    }
}
