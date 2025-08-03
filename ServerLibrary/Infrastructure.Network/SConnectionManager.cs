using System;
using System.Linq;

namespace Server.Infrastructure.Network
{
    public class SConnectionManager(IConnectionFactory<SConnection> connectionFactory, IpAddressManager IpManager) 
        : ConnectionManager<SConnection>(connectionFactory, IpManager)
    {
        public int Players => Connections?.Count(c => c.Stage == GameStage.Game) ?? 0;
        public int Observers => Connections?.Count(c => c.Stage == GameStage.Observer) ?? 0;
    }
}
