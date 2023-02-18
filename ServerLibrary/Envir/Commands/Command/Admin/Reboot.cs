using Library;
using Server.Models;
using System;

namespace Server.Envir.Commands.Command.Admin
{
    class Reboot : AbstractCommand<IAdminCommand>
    {
        public override string VALUE => "REBOOT";

        public override void Action(PlayerObject player)
        {
            DateTime time = Time.Now;
            player.MarketPlaceCancelSuperior();
            player.Connection.ReceiveChat($"[Reboot Command] {(Time.Now - time).Ticks / TimeSpan.TicksPerMillisecond}ms", MessageType.System);
        }
    }
}
