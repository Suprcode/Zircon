using Server.Models;

namespace Server.Envir.Commands.Handler
{
    public interface CommandHandler
    { 
        void Handle(PlayerObject player, string[] input);
    }
}
