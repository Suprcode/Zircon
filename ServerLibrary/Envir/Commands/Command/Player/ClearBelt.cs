using Server.Envir.Commands.Command;
using Server.Envir.Commands.Command.Player;
using Server.Models;

namespace Server.Envir.Commands.Player {
    class ClearBelt : AbstractCommand<IPlayerCommand>
    {
        public override string VALUE => "CLEARBELT";

        public override void Action(PlayerObject player) {
            for (int i = player.Character.BeltLinks.Count - 1; i >= 0; i--)
                player.Character.BeltLinks[i].Delete();
        }
    }
}
