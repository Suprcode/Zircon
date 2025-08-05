
namespace Server.Infrastructure.Logging
{
    internal class CompositeLogger(params ILogger[] DelegateLoggers) : ILogger
    {
        public void Log(string message)
        {
            foreach (var logger in DelegateLoggers)
                logger.Log(message);
        }
    }
}
