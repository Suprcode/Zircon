using Server.Envir;
using System;
using System.Collections.Generic;
using System.Net;

namespace Server.Infrastructure.Network
{
    public class IpService
    {
        public static Dictionary<string, DateTime> IPBlocks = new Dictionary<string, DateTime>(); //TODO: this should be supplied & encapsulated in a Service (i.e ConnectionService.CanConnect(IpAddress))

        public bool IsBanned(SConnection connection)
        {
            return IsBanned(connection.IPAddress);
        }

        public bool IsBanned(string ipAddress)
        {
            return IPBlocks.TryGetValue(ipAddress, out DateTime bannedUntil) && bannedUntil > SEnvir.Now;
        }

        public void Ban(SConnection connection, TimeSpan duration)
        {
            var bannedUntil = DateTime.Now.Add(duration);
            if (!IPBlocks.TryAdd(connection.IPAddress, bannedUntil))
                IPBlocks[connection.IPAddress] = bannedUntil;
            connection.TryDisconnect();
        }

        public void Ban(SConnection connection)
        {
            Ban(connection, TimeSpan.MaxValue);
        }

        public void Reset()
        {
            IPBlocks.Clear();
        }
    }
}
