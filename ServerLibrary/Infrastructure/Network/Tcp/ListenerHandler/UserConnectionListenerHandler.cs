using Server.Envir;
using Server.Infrastructure.Service.Connection;
using System;
using System.Net.Sockets;
using System.Threading;

namespace Server.Infrastructure.Network.Tcp.ListenerHandler
{
    public class UserConnectionListenerHandler(AbstractConnectionService<UserConnection> ConnectionService) : IListenerHandler
    {
        public void OnAcceptBegin(TcpListener listener, IAsyncResult result)
        {
            ConnectionService.Add(listener.EndAcceptTcpClient(result));
        }

        public void OnAcceptEnd(TcpListener listener, IAsyncResult result)
        {

            while (ConnectionService.AcceptingConnections)
                Thread.Sleep(1);
        }

        public void OnException(Exception ex)
        {
            SEnvir.Log(ex.ToString());
        }
    }
}
