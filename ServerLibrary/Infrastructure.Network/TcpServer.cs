using Library.Network;
using Server.Envir;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server.Infrastructure.Network
{
    public class TcpServer<ConnectionType>(ConnectionManager<ConnectionType> ConnectionManager) where ConnectionType : BaseConnection
    {
        public bool Started { get; private set; }
        public long DownloadSpeed => ConnectionManager.TotalBytesReceived - ConnectionManager.PreviousTotalReceived;
        public long UploadSpeed => ConnectionManager.TotalBytesSent - ConnectionManager.PreviousTotalSent;

        private TcpListener _listener, 
            _userCountListener; //TODO: move this out - inject ConnectionHandler, inject port and create separate tcpServer for handling UC

        public void StartNetwork(bool log = true)
        {
            try
            {
                _listener = new TcpListener(IPAddress.Parse(Config.IPAddress), Config.Port);
                _listener.Start();
                _listener.BeginAcceptTcpClient(TcpConnection, null);

                _userCountListener = new TcpListener(IPAddress.Parse(Config.IPAddress), Config.UserCountPort);
                _userCountListener.Start();
                _userCountListener.BeginAcceptTcpClient(CountConnection, null);

                Started = true;
                if (log) SEnvir.Log($"Network Started. Listen: {Config.IPAddress}:{Config.Port}");
            }
            catch (Exception ex)
            {
                Started = false;
                SEnvir.Log(ex.ToString());
            }
        }

        public void Process()
        {
            ConnectionManager.Process();
        }

        public void StopNetwork(bool log = true)
        {
            TcpListener expiredListener = _listener;
            TcpListener expiredUserListener = _userCountListener;

            _listener = null;
            _userCountListener = null;

            Started = false;

            expiredListener?.Stop();
            expiredUserListener?.Stop();
            ConnectionManager?.Reset();

            if (log) SEnvir.Log("Network Stopped.");
        }

        public void Broadcast(Packet packet)
        {
            ConnectionManager.Broadcast(packet);
        }

        private void TcpConnection(IAsyncResult result)
        {
            try
            {
                if (_listener == null || !_listener.Server.IsBound) return;
                ConnectionManager.Add(_listener.EndAcceptTcpClient(result));                
            }
            catch (SocketException) { }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
            finally
            {
                while (ConnectionManager.AcceptingConnections)
                    Thread.Sleep(1);

                if (_listener != null && _listener.Server.IsBound)
                    _listener.BeginAcceptTcpClient(TcpConnection, null);
            }
        }

        #region Count Connection Stuff - Separate into a different TcpServer
        private void CountConnection(IAsyncResult result)
        {
            try
            {
                if (_userCountListener == null || !_userCountListener.Server.IsBound) return;

                TcpClient client = _userCountListener.EndAcceptTcpClient(result);
                byte[] data = Encoding.ASCII.GetBytes(string.Format("c;/Zircon/{0}/;", ConnectionManager.Connections.Count));
                client.Client.BeginSend(data, 0, data.Length, SocketFlags.None, CountConnectionEnd, client);
            }
            catch { }
            finally
            {
                if (_userCountListener != null && _userCountListener.Server.IsBound)
                    _userCountListener.BeginAcceptTcpClient(CountConnection, null);
            }
        }
        private void CountConnectionEnd(IAsyncResult result)
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
        #endregion
    }
}
