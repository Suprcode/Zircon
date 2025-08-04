using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Infrastructure.Network.Tcp.ListenerHandler
{
    public interface IListenerHandler
    {
        public void OnAcceptBegin(TcpListener listener, IAsyncResult result);
        public void OnAcceptEnd(TcpListener listener, IAsyncResult result);
        public void OnAcceptException(Exception ex);

        public void OnTermination();
    }
}
