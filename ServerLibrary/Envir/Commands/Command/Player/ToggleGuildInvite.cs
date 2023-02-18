using Library;
using Server.Models;

namespace Server.Envir.Commands.Command.Player
{
    class ToggleGuildInvite : AbstractCommand<IPlayerCommand>
    {
        public override string VALUE => "ALLOWGUILD";

        public override void Action(PlayerObject player)
        {
            player.Character.Account.AllowGuild = !player.Character.Account.AllowGuild;
            player.Connection.ReceiveChat(player.Character.Account.AllowGuild ? player.Connection.Language.GuildInviteEnabled : player.Connection.Language.GuildInviteDisabled, MessageType.System);
        }
    }
}
