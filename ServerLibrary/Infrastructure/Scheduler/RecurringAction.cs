using System;

namespace Server.Infrastructure.Scheduler
{
    internal record RecurringAction(Action Action, TimeSpan Interval) { }
}
