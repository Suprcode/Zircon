using Server.Envir;
using Server.Infrastructure.Logging.Formatter;
using Server.Infrastructure.Scheduler;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace Server.Infrastructure.Logging
{
    internal class UserChatFileLogger : ILogger
    {
        private readonly string LOG_PATH = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".\\Chat Logs.txt"));

        private readonly ILogFormatter LogFormatter;
        private readonly ConcurrentQueue<string> Logs = [];
        
        public UserChatFileLogger(SingleThreadScheduler scheduler, ILogFormatter logFormatter)
        {
            LogFormatter = logFormatter;
            scheduler.ScheduleRecurring(WriteLogs, TimeSpan.FromSeconds(10));
        }

        public void Log(string message)
        {
            if (Logs.Count < 1000)
                Logs.Enqueue(LogFormatter.Format(message));
        }

        private void WriteLogs()
        {
            List<string> lines = [];
            while (Logs.TryDequeue(out string line))
                lines.Add(line);
            File.AppendAllLines(LOG_PATH, lines);
        }
    }
}
