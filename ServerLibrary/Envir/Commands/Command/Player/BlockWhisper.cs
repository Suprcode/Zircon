using Library;
using Server.Models;
using Server.Envir.Commands.Command;
using Server.Envir.Commands.Command.Player;

namespace Server.Envir.Commands.Player {
    class BlockWhisper : AbstractCommand<IPlayerCommand> {
        public override string VALUE => "BLOCKWHISPER";

        public override void Action(PlayerObject player) {
            player.BlockWhisper = !player.BlockWhisper;
            player.Connection.ReceiveChat(player.BlockWhisper ? player.Connection.Language.WhisperDisabled : player.Connection.Language.WhisperEnabled, MessageType.System);
        }
    }
}
