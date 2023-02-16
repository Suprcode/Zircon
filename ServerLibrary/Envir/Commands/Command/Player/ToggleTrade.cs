using Library;
using Server.Models;

namespace Server.Envir.Commands.Player {
    class ToggleTrade : AbstractUserCommand {
        public override string VALUE => "ALLOWTRADE";

        public override void Action(PlayerObject player) {
            player.Character.Account.AllowTrade = !player.Character.Account.AllowTrade;
            player.Connection.ReceiveChat(player.Character.Account.AllowTrade ?
                player.Connection.Language.TradingEnabled : player.Connection.Language.TradingDisabled, MessageType.System);
        }
    }
}
