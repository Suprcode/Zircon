using Server.Infrastructure.Network;
using Server.Models;

namespace Server.Envir.Commands.Command.Admin
{
    class ClearIPBlocks : AbstractCommand<IAdminCommand>
    {
        public override string VALUE => "CLEARIPBLOCKS";

       
        //private readonly IpService IpBanService;
        //public ClearIPBlocks(IpService ipBanService) {
        //    IpBanService = ipBanService;
        //}

        public override void Action(PlayerObject player)
        {
            SEnvir.IpService.Reset();  //TODO: IpBanService should be injected but need to rewrite everything to use DI (AutoFac)
                                          //because reflection based creation is not flexible enough
        }
    }
}
