using Library;

namespace Server.Infrastructure.Logging.Formatter
{
    public class StdLogFormatter : ILogFormatter
    {
        public string Format(string message)
        {
            return string.Format("[{0:F}]: {1}", Time.Now, message);
        }
    }
}
