using Library.Network;
using System.Net.Sockets;

namespace Server.Infrastructure.Service.Connection
{
    public interface IConnectionFactory<ConnectionType> where ConnectionType : BaseConnection
    {
        public ConnectionType Create(TcpClient tcpClient);
    }
}
