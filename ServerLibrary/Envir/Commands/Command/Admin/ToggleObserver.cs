using Server.Models;

namespace Server.Envir.Commands.Command.Admin
{
    class ToggleObserver : AbstractCommand<IAdminCommand>
    {
        public override string VALUE => "OBSERVER";

        public override void Action(PlayerObject player)
        {
            player.Observer = !player.Observer;
            player.AddAllObjects();
            player.RemoveAllObjects();
        }
    }
}
