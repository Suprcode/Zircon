using Library.SystemModels;
using Server.Models;

namespace Server.Envir.Events.Triggers
{
    /// <summary>
    /// Triggers when a monster dies
    /// </summary>
    [EventTriggerType("MONSTERDIE")]
    public class MonsterDie : IMonsterEventTrigger, IEventTrigger
    {
        public MonsterEventTriggerType[] MonsterTypes => [MonsterEventTriggerType.MonsterDie];

        public bool Check(MonsterObject monster, MonsterEventTrigger eventTrigger)
        {
            if ((monster.DropSet & eventTrigger.DropSet) != eventTrigger.DropSet)
            {
                return false;
            }

            if (eventTrigger.MapParameter1 != null && monster.CurrentMap?.Info != eventTrigger.MapParameter1)
            {
                return false;
            }

            if (eventTrigger.RegionParameter1 != null && !monster.CurrentCell.Regions.Contains(eventTrigger.RegionParameter1))
            {
                return false;
            }

            if (eventTrigger.InstanceParameter1 != null && monster.CurrentMap.Instance != eventTrigger.InstanceParameter1)
            {
                return false;
            }

            return true;
        }
    }
}
