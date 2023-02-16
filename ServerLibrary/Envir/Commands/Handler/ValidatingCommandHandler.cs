using Server.Models;

namespace Server.Envir.Commands.Handler
{
    public interface ValidatingCommandHandler : CommandHandler 
    {
        bool IsAllowedByPlayer(PlayerObject player);
        bool CommandExists(string command);
    }
}
