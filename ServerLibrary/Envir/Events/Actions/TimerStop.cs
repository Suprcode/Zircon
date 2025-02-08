using Library.SystemModels;
using Server.Envir.Events.Triggers;
using Server.Models;

namespace Server.Envir.Events.Actions
{
    [EventActionType(EventActionType.TimerStop)]
    public class TimerStop : IPlayerEventAction, IMonsterEventAction, IEventAction
    {
        public void Act(PlayerObject triggerPlayer, EventLog log, PlayerEventAction action)
        {
            StopTimer(action.StringParameter1, action.Event.TrackingType, triggerPlayer);
        }

        public void Act(PlayerObject triggerPlayer, EventLog log, MonsterEventAction action)
        {
            StopTimer(action.StringParameter1, action.Event.TrackingType, triggerPlayer);
        }

        private static void StopTimer(string name, EventTrackingType type, PlayerObject player)
        {
            if (string.IsNullOrEmpty(name)) return;

            var key = EventTimer.GetKey(type, player);
            var existingTimer = EventTimer.Timers.Find(x => x.Name == name && x.Key == key);

            if (existingTimer != null)
            {
                existingTimer.Started = false;
            }
        }
    }
}
