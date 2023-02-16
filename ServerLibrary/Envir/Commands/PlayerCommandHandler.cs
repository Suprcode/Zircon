using Server.Envir.Commands.Command.Player;
using Server.Envir.Commands.Handler;
using Server.Envir.Commands.Player;
using Server.Models;
using System.Collections.Generic;

namespace Server.Envir.Commands
{
    public class PlayerCommandHandler : AbstractCommandHandler
    {
        public PlayerCommandHandler()
        {
            InitializeCommands(new List<UserCommand>()
            {
                new BlockWhisper(),
                new ClearBelt(),
                new ToggleTrade(),

                new ToggleExtractorLock(),

                new ToggleCompanionLock3(),
                new ToggleCompanionLock5(),
                new ToggleCompanionLock7(),
                new ToggleCompanionLock10(),
                new ToggleCompanionLock11(),
                new ToggleCompanionLock13(),
                new ToggleCompanionLock15(),

                new ToggleGroupRecall(),
                new GroupRecall(),
                new GroupRoll(),

                new ToggleGuildInvite(),
                new LeaveGuild()
            });
        }

        public override bool IsAllowedByPlayer(PlayerObject player)
        {
            return true;
        }
    }
}
