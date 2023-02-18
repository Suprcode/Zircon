using Server.Models;

namespace Server.Envir.Commands.Handler
{
    public interface IValidatingCommandHandler : ICommandHandler 
    {
        bool IsAllowedByPlayer(PlayerObject player);
        bool CommandExists(string command);
    }
}
