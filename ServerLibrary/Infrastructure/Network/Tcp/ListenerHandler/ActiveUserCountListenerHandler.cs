using System;
using System.Net.Sockets;
using System.Text;

namespace Server.Infrastructure.Network.Tcp.ListenerHandler
{
    public class ActiveUserCountListenerHandler(Func<int> UserCountSupplier) : IListenerHandler
    {
        public void OnAcceptBegin(TcpListener l, IAsyncResult r)
        {
            TcpClient client = l.EndAcceptTcpClient(r);
            byte[] data = Encoding.ASCII.GetBytes(string.Format("c;/Zircon/{0}/;", UserCountSupplier.Invoke()));
            client.Client.BeginSend(data, 0, data.Length, SocketFlags.None, TerminateConnection, client);
        }

        public void OnAcceptEnd(TcpListener l, IAsyncResult r)
        {
            // Do Nothing
        }

        public void OnException(Exception ex)
        {
            // Do Nothing
        }

        private void TerminateConnection(IAsyncResult result)
        {
            try
            {
                var client = result.AsyncState as TcpClient;

                if (client == null) return;
                client.Client.EndSend(result);
                client.Client.Dispose();
            }
            catch { }
        }
    }
}
