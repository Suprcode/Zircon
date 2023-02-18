using Server.Models;

namespace Server.Envir.Commands.Handler
{
    public interface ICommandHandler
    { 
        void Handle(PlayerObject player, string[] input);
    }
}
