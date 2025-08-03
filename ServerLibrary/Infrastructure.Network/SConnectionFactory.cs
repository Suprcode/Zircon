using System;
using System.Net.Sockets;

namespace Server.Infrastructure.Network
{
    internal class SConnectionFactory(Func<Action<SConnection>> disconnectCallback) : IConnectionFactory<SConnection>
    {
        private readonly Func<Action<SConnection>> DisconnectCallback = disconnectCallback;

        public SConnection Create(TcpClient tcpClient)
        {
            return new SConnection(tcpClient, DisconnectCallback.Invoke());
        }
    }
}
