using System;
using System.Net.Sockets;

namespace Server.Infrastructure.Service.Connection
{
    internal class UserConnectionFactory(Func<Action<UserConnection>> disconnectCallback) : IConnectionFactory<UserConnection>
    {
        private readonly Func<Action<UserConnection>> DisconnectCallback = disconnectCallback;

        public UserConnection Create(TcpClient tcpClient)
        {
            return new UserConnection(tcpClient, DisconnectCallback.Invoke());
        }
    }
}
