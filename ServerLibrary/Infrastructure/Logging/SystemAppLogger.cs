using Server.Infrastructure.Logging.Formatter;
using System.Collections.Concurrent;

namespace Server.Infrastructure.Logging
{
    //TODO: this should ideally be owned by Server and injected
    internal class SystemAppLogger(ConcurrentQueue<string> Logs, ILogFormatter LogFormatter) : ILogger
    {
        public void Log(string message)
        {
            if (Logs.Count < 100)
                Logs.Enqueue(LogFormatter.Format(message));
        }
    }
}
