using Library;
using Server.Models;

namespace Server.Envir.Commands.Command.Admin
{
    /// <summary>
    /// Toggles the player being targetted by players or monsters
    /// </summary>
    class ToggleGameMaster : AbstractCommand<IAdminCommand>
    {
        public override string VALUE => "GAMEMASTER";

        public override void Action(PlayerObject player)
        {
            player.GameMaster = !player.GameMaster;
            player.Connection.ReceiveChat($"{VALUE} {(player.GameMaster ? "activated" : "deactivated")}", MessageType.System);
        }
    }
}
