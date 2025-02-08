using Library.SystemModels;
using Server.Models;
using System.Collections.Generic;
using System.Linq;

namespace Server.Envir.Events.Triggers
{
    /// <summary>
    /// Triggers once every minute for each active timer
    /// </summary>
    [EventTriggerType("TIMERMINUTE")]
    public class TimerMinute : IPlayerEventTrigger, IEventTrigger
    {
        public PlayerEventTriggerType[] PlayerTypes => [PlayerEventTriggerType.TimerMinute];

        public bool Check(PlayerObject player, PlayerEventTrigger eventTrigger)
        {
            return TimerCheck(eventTrigger.StringParameter1, eventTrigger.Event.TrackingType, player);
        }

        private static bool TimerCheck(string name, EventTrackingType type, PlayerObject player)
        {
            if (string.IsNullOrEmpty(name)) return false;

            var key = EventTimer.GetKey(type, player);
            return EventTimer.Timers.Any(timer => timer.Name == name && timer.Key == key);
        }
    }

    public class EventTimer
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public PlayerObject Player { get; set; }
        public int Time { get; set; }
        public bool Started { get; set; }

        public static List<EventTimer> Timers { get; } = new();

        public static string GetKey(EventTrackingType type, PlayerObject context)
        {
            if (context == null) return string.Empty;

            return type switch
            {
                EventTrackingType.Global => "Global",
                EventTrackingType.Player => $"Player:{context.Character.Index}",
                EventTrackingType.Group => context.GroupMembers?.Any() == true
                    ? $"Group:{context.GroupMembers[0].Character.Index}"
                    : null,
                EventTrackingType.Guild => context.Character.Account.GuildMember?.Guild.GuildName is string guildName
                    ? $"Guild:{guildName}"
                    : null,
                EventTrackingType.Instance => context.CurrentMap.Instance is { } instance
                    ? $"Instance:{instance.Index}:{context.CurrentMap.InstanceSequence}"
                    : null,
                _ => null
            };
        }
    }
}
