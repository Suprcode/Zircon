using Server.Envir.Commands.Command.Admin;
using Server.Envir.Commands.Handler;
using Server.Models;

namespace Server.Envir.Commands
{
    public class AdminCommandHandler : AbstractCommandHandler<IAdminCommand>
    {
        public override bool IsAllowedByPlayer(PlayerObject player)
        {
            return player.Character.Account.TempAdmin;
        }
    }
}
