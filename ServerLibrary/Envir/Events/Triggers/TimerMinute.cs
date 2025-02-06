using Library.SystemModels;
using Server.Models;
using System.Collections.Generic;

namespace Server.Envir.Events.Triggers
{
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

            var existingTimer = EventTimer.Timers.Find(x => x.Name == name && x.Key == key);

            if (existingTimer == null)
            {
                return false;
            }

            return true;
        }
    }

    public class EventTimer
    {
        public string Name { get; set; }
        public string Key { get; set; }

        public PlayerObject Player { get; set; }

        public int Time { get; set; }

        public bool Started { get; set; }

        public static List<EventTimer> Timers { get; set; } = [];
        public static string GetKey(EventTrackingType type, PlayerObject context)
        {
            if (context == null)
                return "";

            switch (type)
            {
                case EventTrackingType.Global:
                    return "Global";

                case EventTrackingType.Player:
                    return $"Player:{context.Character.Index}";

                case EventTrackingType.Group:
                    bool hasGroupMembers = context.GroupMembers != null && context.GroupMembers.Count > 0;
                    if (!hasGroupMembers)
                        return null;
                    int groupLeaderIndex = context.GroupMembers[0].Character.Index;
                    return $"Group:{groupLeaderIndex}";

                case EventTrackingType.Guild:
                    var guildMember = context.Character.Account.GuildMember;
                    if (guildMember == null)
                        return null;
                    return $"Guild:{guildMember.Guild.GuildName}";

                case EventTrackingType.Instance:
                    var instance = context.CurrentMap.Instance;
                    if (instance == null)
                        return null;
                    return $"Instance:{instance.Index}:{context.CurrentMap.InstanceSequence}";

                default:
                    return null;
            }
        }
    }
}
