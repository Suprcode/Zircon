using Library;
using Server.Models;

namespace Server.Envir.Commands.Player {
    class ToggleExtractorLock : AbstractUserCommand {
        public override string VALUE => "EXTRACTORLOCK";

        public override void Action(PlayerObject player) {
            player.ExtractorLock = !player.ExtractorLock;
            player.Connection.ReceiveChat(player.ExtractorLock ? "Extraction Enabled" : "Extraction Locked", MessageType.System);
        }
    }
}
