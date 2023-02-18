using Server.Models;

namespace Server.Envir.Commands.Command.Admin
{
    class ToggleGameMaster : AbstractCommand<IAdminCommand>
    {
        public override string VALUE => "GAMEMASTER";

        public override void Action(PlayerObject player)
        {
            player.GameMaster = !player.GameMaster;
        }
    }
}
