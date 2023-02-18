using Library;
using Server.Models;

namespace Server.Envir.Commands.Command.Player
{
    class ToggleGroupRecall : AbstractCommand<IPlayerCommand>
    {
        public override string VALUE => "ALLOWRECALL";

        public override void Action(PlayerObject player)
        {
            player.Character.Account.AllowGroupRecall = !player.Character.Account.AllowGroupRecall;
            player.Connection.ReceiveChat(player.Character.Account.AllowGroupRecall ? player.Connection.Language.GroupRecallEnabled : player.Connection.Language.GroupRecallDisabled, MessageType.System);
            foreach (SConnection con in player.Connection.Observers)
                con.ReceiveChat(player.Character.Account.AllowGroupRecall ? con.Language.GroupRecallEnabled : con.Language.GroupRecallDisabled, MessageType.System);
        }
    }
}
