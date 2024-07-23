using Library;
using Server.Models;

namespace Server.Envir.Commands.Command.Admin
{
    class ToggleSuperman : AbstractCommand<IAdminCommand>
    {
        public override string VALUE => "SUPERMAN";

        public override void Action(PlayerObject player)
        {
            player.Superman = !player.Superman;
            player.Connection.ReceiveChat($"{VALUE} {(player.Superman ? "activated" : "deactivated")}", MessageType.System);
        }
    }
}