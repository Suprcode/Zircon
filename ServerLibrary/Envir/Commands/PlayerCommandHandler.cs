using Server.Envir.Commands.Command.Player;
using Server.Envir.Commands.Handler;
using Server.Models;

namespace Server.Envir.Commands
{
    public class PlayerCommandHandler : AbstractCommandHandler<IPlayerCommand>
    {
        public override bool IsAllowedByPlayer(PlayerObject player)
        {
            return true;
        }
    }
}
