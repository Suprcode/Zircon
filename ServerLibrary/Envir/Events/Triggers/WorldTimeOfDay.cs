using Library.SystemModels;

namespace Server.Envir.Events.Triggers
{
    /// <summary>
    /// Triggers when time of day changes
    /// </summary>
    [EventTriggerType("TIMEOFDAY")]
    public class WorldTimeOfDay : IWorldEventTrigger, IEventTrigger
    {
        public WorldEventTriggerType[] WorldTypes => [WorldEventTriggerType.Dawn, WorldEventTriggerType.Day, WorldEventTriggerType.Dusk, WorldEventTriggerType.Night];

        public bool Check(WorldEventTrigger eventTrigger)
        {
            return eventTrigger.Type switch
            {
                WorldEventTriggerType.Dawn => SEnvir.TimeOfDay == Library.TimeOfDay.Dawn,
                WorldEventTriggerType.Day => SEnvir.TimeOfDay == Library.TimeOfDay.Day,
                WorldEventTriggerType.Dusk => SEnvir.TimeOfDay == Library.TimeOfDay.Dusk,
                WorldEventTriggerType.Night => SEnvir.TimeOfDay == Library.TimeOfDay.Night,
                _ => false,
            };
        }
    }
}
