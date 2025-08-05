using Server.Envir;
using Server.Infrastructure.Network.Tcp.ListenerHandler;
using System;
using System.Net;
using System.Net.Sockets;

namespace Server.Infrastructure.Network.Tcp
{
    public class TcpServer(IListenerHandler ListenerHandler, string IpAddress, ushort Port)
    {
        public bool Started { get; private set; }

        private TcpListener Listener;

        public void Start()
        {
            try
            {
                Listener = new TcpListener(IPAddress.Parse(IpAddress), Port);
                Listener.Start();
                Listener.BeginAcceptTcpClient(HandleConnection, null);
                Started = true;
            }
            catch (Exception ex)
            {
                Started = false;
                SEnvir.ServerLogger.Log(ex.ToString());
            }
        }

        public void Stop()
        {
            TcpListener expiredListener = Listener;

            Listener = null;
            Started = false;
            expiredListener?.Stop();
            ListenerHandler.OnTermination();
        }

        private void HandleConnection(IAsyncResult result)
        {
            try
            {
                if (Listener == null || !Listener.Server.IsBound) return;
                ListenerHandler.OnAcceptBegin(Listener, result);              
            }
            catch (SocketException) { }
            catch (Exception ex)
            {
                ListenerHandler.OnAcceptException(ex);
            }
            finally
            {
                ListenerHandler.OnAcceptEnd(Listener, result);
                if (Listener != null && Listener.Server.IsBound)
                    Listener.BeginAcceptTcpClient(HandleConnection, null);
            }
        }
    }
}
