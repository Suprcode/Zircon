using Server.Infrastructure.Network;
using Server.Models;

namespace Server.Envir.Commands.Command.Admin
{
    class ClearIPBlocks : AbstractCommand<IAdminCommand>
    {
        public override string VALUE => "CLEARIPBLOCKS";

        public override void Action(PlayerObject player)
        {
            TcpServer.IPBlocks.Clear();
        }
    }
}
