using Library.Network;
using System.Net.Sockets;

namespace Server.Infrastructure.Network
{
    public interface IConnectionFactory<ConnectionType> where ConnectionType : BaseConnection
    {
        public ConnectionType Create(TcpClient tcpClient);
    }
}
