using Server.Infrastructure.Network;
using Server.Models;

namespace Server.Envir.Commands.Command.Admin
{
    class ClearIPBlocks : AbstractCommand<IAdminCommand>
    {
        public override string VALUE => "CLEARIPBLOCKS";

        public override void Action(PlayerObject player)
        {
            SEnvir.IpManager.Reset();  //TODO: this should be injected but need to rewrite everything to use DI (AutoFac) because reflection based creation is not flexible enough
        }
    }
}
