using Server.Models;

namespace Server.Envir.Commands.Command.Player
{
    class LeaveGuild : AbstractUserCommand
    {
        public override string VALUE => "LEAVEGUILD";

        public override void Action(PlayerObject player)
        {
            player.GuildLeave();
        }
    }
}
