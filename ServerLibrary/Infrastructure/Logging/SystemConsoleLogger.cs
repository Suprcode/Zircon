using Server.Infrastructure.Logging.Formatter;
using System;

namespace Server.Infrastructure.Logging
{
    //TODO: this should ideally be owned by ServerCore and injected
    public class SystemConsoleLogger(ILogFormatter LogFormatter) : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(LogFormatter.Format(message));           
        }
    }
}
