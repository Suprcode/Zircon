using Library.Network;
using Server.Envir;
using System;
using System.Collections.Generic;

namespace Server.Infrastructure.Service
{
    public class IpAddressService
    {
        public Dictionary<string, DateTime> IpAddressTimeOuts = [];

        public bool IsAllowing(string ipAddress)
        {
            return !IpAddressTimeOuts.TryGetValue(ipAddress, out DateTime timeout) || timeout <= SEnvir.Now;
        }

        public void Timeout(BaseConnection connection, TimeSpan duration)
        {
            var bannedUntil = DateTime.Now.Add(duration);
            if (!IpAddressTimeOuts.TryAdd(connection.IPAddress, bannedUntil))
                IpAddressTimeOuts[connection.IPAddress] = bannedUntil;
            connection.TryDisconnect();
        }

        public void Reset()
        {
            IpAddressTimeOuts.Clear();
        }
    }
}
