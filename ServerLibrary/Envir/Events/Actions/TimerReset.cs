using Library.SystemModels;
using Server.Envir.Events.Triggers;
using Server.Models;
using System.Linq;

namespace Server.Envir.Events.Actions
{
    [EventActionType(EventActionType.TimerReset)]
    public class TimerReset : IPlayerEventAction, IMonsterEventAction, IEventAction
    {
        public void Act(PlayerObject triggerPlayer, EventLog log, PlayerEventAction action)
        {
            ResetTimer(action.StringParameter1, action.Event.TrackingType, triggerPlayer);
        }

        public void Act(PlayerObject triggerPlayer, EventLog log, MonsterEventAction action)
        {
            ResetTimer(action.StringParameter1, action.Event.TrackingType, triggerPlayer);
        }

        private static void ResetTimer(string name, EventTrackingType type, PlayerObject player)
        {
            if (string.IsNullOrEmpty(name)) return;

            var key = EventTimer.GetKey(type, player);
            var existingTimer = EventTimer.Timers.Find(x => x.Name == name && x.Key == key);

            if (existingTimer != null)
            {
                existingTimer.Time = 0;
            }
        }
    }
}
