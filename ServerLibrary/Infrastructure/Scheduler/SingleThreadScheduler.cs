using System;
using System.Collections.Generic;
using System.Threading;

namespace Server.Infrastructure.Scheduler
{
    public class SingleThreadScheduler
    {
        private readonly Thread Thread;

        private readonly object Lock = new();
        private readonly PriorityQueue<RecurringAction, DateTime> ScheduledActions = new();

        public SingleThreadScheduler(string schedulerName = "default")
        {
            Thread = new Thread(Loop)
            {
                IsBackground = true,
                Name = $"SingleThreadScheduler[{schedulerName}-{Guid.NewGuid()}]"
            };
            Thread.Start();
        }

        public void ScheduleRecurring(Action action, TimeSpan interval)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(interval, TimeSpan.Zero);
            lock (Lock)
            {
                ScheduledActions.Enqueue(new RecurringAction(action, interval), DateTime.UtcNow);
                Monitor.Pulse(Lock);
            }
        }

        private void Loop()
        {
            while (true) //TODO: should i bother to add state (Running, Start, Stop)?
            {
                foreach (var action in GetAvailableActions())
                {
                    try { action(); }
                    catch (Exception) { }
                }
                lock (Lock) Monitor.Wait(Lock, NextScheduledTime());
            }
        }

        private List<Action> GetAvailableActions()
        {
            List<Action> actionsDue = [];
            var now = DateTime.UtcNow;

            lock (Lock)
            {
                while (ScheduledActions.TryPeek(out var action, out var nextTime) && nextTime <= now)
                {
                    var awd = ScheduledActions.Dequeue();
                    actionsDue.Add(awd.Action);
                    ScheduledActions.Enqueue(awd, now.Add(awd.Interval));
                }
                return actionsDue;
            }
        }

        private TimeSpan NextScheduledTime()
        {
            if (ScheduledActions.TryPeek(out var _, out var nextTime))
            {
                var nextSchedule = nextTime - DateTime.UtcNow;
                if (nextSchedule < TimeSpan.Zero) return TimeSpan.Zero;
                return nextSchedule;
            }
            return TimeSpan.FromMilliseconds(int.MaxValue);
        }
    }
}
