using System;
using System.Linq;

namespace Server.Infrastructure.Service.Connection
{
    public class UserConnectionService(IConnectionFactory<UserConnection> connectionFactory, IpAddressService IpManager) 
        : AbstractConnectionService<UserConnection>(connectionFactory, IpManager)
    {
        public int Players => ActiveConnections?.Count(c => c.Stage == GameStage.Game) ?? 0;
        public int Observers => ActiveConnections?.Count(c => c.Stage == GameStage.Observer) ?? 0;
    }
}
