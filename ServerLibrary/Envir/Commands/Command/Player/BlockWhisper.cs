using Library;
using Server.Models;

namespace Server.Envir.Commands.Player {
    class BlockWhisper : AbstractUserCommand {
        public override string VALUE => "BLOCKWHISPER";

        public override void Action(PlayerObject player) {
            player.BlockWhisper = !player.BlockWhisper;
            player.Connection.ReceiveChat(player.BlockWhisper ? player.Connection.Language.WhisperDisabled : player.Connection.Language.WhisperEnabled, MessageType.System);
        }
    }
}
