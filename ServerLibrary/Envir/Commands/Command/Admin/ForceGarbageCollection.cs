using Library;
using Server.Models;
using System;

namespace Server.Envir.Commands.Command.Admin
{
    class ForceGarbageCollection : AbstractCommand<IAdminCommand>
    {
        public override string VALUE => "GCOLLECT";

        public override void Action(PlayerObject player)
        {
            DateTime time = Time.Now;
            GC.Collect(2, GCCollectionMode.Forced);
            player.Connection.ReceiveChat($"[GC COLLECT] {(Time.Now - time).Ticks / TimeSpan.TicksPerMillisecond}ms", MessageType.System);
        }
    }
}
