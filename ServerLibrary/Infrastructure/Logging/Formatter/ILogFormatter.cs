
namespace Server.Infrastructure.Logging.Formatter
{
    public interface ILogFormatter
    {
        string Format(string message);
    }
}
