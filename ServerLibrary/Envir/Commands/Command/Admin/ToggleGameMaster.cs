using Server.Models;

namespace Server.Envir.Commands.Command.Admin
{
    class ToggleGameMaster : AbstractUserCommand
    {
        public override string VALUE => "GAMEMASTER";

        public override void Action(PlayerObject player)
        {
            player.GameMaster = !player.GameMaster;
        }
    }
}
