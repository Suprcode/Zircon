using Library.SystemModels;
using Server.Envir.Events.Triggers;
using Server.Models;

namespace Server.Envir.Events.Actions
{
    [EventActionType(EventActionType.TimerStart)]
    public class TimerStart : IPlayerEventAction, IMonsterEventAction, IEventAction
    {
        public void Act(PlayerObject triggerPlayer, EventLog log, PlayerEventAction action)
        {
            StartTimer(action.StringParameter1, action.Event.TrackingType, triggerPlayer);
        }

        public void Act(PlayerObject triggerPlayer, EventLog log, MonsterEventAction action)
        {
            StartTimer(action.StringParameter1, action.Event.TrackingType, triggerPlayer);
        }

        private static void StartTimer(string name, EventTrackingType type, PlayerObject player)
        {
            if (string.IsNullOrEmpty(name)) return;

            var key = EventTimer.GetKey(type, player);
            var existingTimer = EventTimer.Timers.Find(x => x.Name == name && x.Key == key);

            if (existingTimer == null)
            {
                EventTimer.Timers.Add(existingTimer = new EventTimer
                {
                    Name = name,
                    Key = key,
                    Player = player,
                    Time = 0
                });
            }

            existingTimer.Started = true;
        }
    }
}
