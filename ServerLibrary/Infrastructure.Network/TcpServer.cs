using Library;
using Library.Network;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using G = Library.Network.GeneralPackets;

namespace Server.Infrastructure.Network
{
    public class TcpServer
    {
        public static bool NetworkStarted { get; set; }

        public static Dictionary<string, DateTime> IPBlocks = new Dictionary<string, DateTime>(); //TODO: this should be supplied & encapsulated in a Service (i.e ConnectionService.CanConnect(IpAddress))

        public static Dictionary<string, int> IPCount = new Dictionary<string, int>();

        public static List<SConnection> Connections = new List<SConnection>();
        public static ConcurrentQueue<SConnection> NewConnections;

        private static TcpListener _listener, _userCountListener;

        public static long DBytesSent, DBytesReceived;
        
        public static long TotalBytesSent, TotalBytesReceived;
        public static long PreviousTotalReceived, PreviousTotalSent;

        public static long DownloadSpeed, UploadSpeed;

        public static void StartNetwork(bool log = true)
        {
            try
            {
                NewConnections = new ConcurrentQueue<SConnection>();

                _listener = new TcpListener(IPAddress.Parse(Config.IPAddress), Config.Port);
                _listener.Start();
                _listener.BeginAcceptTcpClient(Connection, null);

                _userCountListener = new TcpListener(IPAddress.Parse(Config.IPAddress), Config.UserCountPort);
                _userCountListener.Start();
                _userCountListener.BeginAcceptTcpClient(CountConnection, null);

                NetworkStarted = true;
                if (log) SEnvir.Log($"Network Started. Listen: {Config.IPAddress}:{Config.Port}");
            }
            catch (Exception ex)
            {
                NetworkStarted = false;
                SEnvir.Log(ex.ToString());
            }
        }

        public static void StopNetwork(bool log = true)
        {
            TcpListener expiredListener = _listener;
            TcpListener expiredUserListener = _userCountListener;

            _listener = null;
            _userCountListener = null;

            NetworkStarted = false;

            expiredListener?.Stop();
            expiredUserListener?.Stop();

            NewConnections = null;

            try
            {
                Packet p = new G.Disconnect { Reason = DisconnectReason.ServerClosing };
                for (int i = Connections.Count - 1; i >= 0; i--)
                    Connections[i].SendDisconnect(p);

                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }

            if (log) SEnvir.Log("Network Stopped.");
        }

        private static void Connection(IAsyncResult result)
        {
            try
            {
                if (_listener == null || !_listener.Server.IsBound) return;

                TcpClient client = _listener.EndAcceptTcpClient(result);

                string ipAddress = client.Client.RemoteEndPoint.ToString().Split(':')[0];

                if (!IPBlocks.TryGetValue(ipAddress, out DateTime banDate) || banDate < SEnvir.Now)
                {
                    SConnection Connection = new SConnection(client);

                    if (Connection.Connected)
                        NewConnections?.Enqueue(Connection);
                }
            }
            catch (SocketException)
            {

            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
            finally
            {
                while (NewConnections?.Count >= 15)
                    Thread.Sleep(1);

                if (_listener != null && _listener.Server.IsBound)
                    _listener.BeginAcceptTcpClient(Connection, null);
            }
        }

        private static void CountConnection(IAsyncResult result)
        {
            try
            {
                if (_userCountListener == null || !_userCountListener.Server.IsBound) return;

                TcpClient client = _userCountListener.EndAcceptTcpClient(result);

                byte[] data = Encoding.ASCII.GetBytes(string.Format("c;/Zircon/{0}/;", Connections.Count));

                client.Client.BeginSend(data, 0, data.Length, SocketFlags.None, CountConnectionEnd, client);
            }
            catch { }
            finally
            {
                if (_userCountListener != null && _userCountListener.Server.IsBound)
                    _userCountListener.BeginAcceptTcpClient(CountConnection, null);
            }
        }
        private static void CountConnectionEnd(IAsyncResult result)
        {
            try
            {
                TcpClient client = result.AsyncState as TcpClient;

                if (client == null) return;

                client.Client.EndSend(result);

                client.Client.Dispose();
            }
            catch { }
        }

        public static void Process()
        {
            SConnection connection;
            while (!NewConnections.IsEmpty)
            {
                if (!NewConnections.TryDequeue(out connection)) break;

                IPCount.TryGetValue(connection.IPAddress, out var ipCount);

                IPCount[connection.IPAddress] = ipCount + 1;

                Connections.Add(connection);
            }

            long bytesSent = 0;
            long bytesReceived = 0;

            for (int i = Connections.Count - 1; i >= 0; i--)
            {
                if (i >= Connections.Count) break;

                connection = Connections[i];

                connection.Process();
                bytesSent += connection.TotalBytesSent;
                bytesReceived += connection.TotalBytesReceived;
            }

            TotalBytesSent = DBytesSent + bytesSent;
            TotalBytesReceived = DBytesReceived + bytesReceived;

            DownloadSpeed = TotalBytesReceived - PreviousTotalReceived;
            UploadSpeed = TotalBytesSent - PreviousTotalSent;

            PreviousTotalReceived = TotalBytesReceived;
            PreviousTotalSent = TotalBytesSent;
        }

        //TODO: work out what SendDisconnect is doing differently to a Enqueue and see if we can consolodate.
        internal static void Broadcast(Packet packet)
        {
            for (int i = Connections.Count - 1; i >= 0; i--)
                Connections[i].SendDisconnect(packet);
        }

        #region Not Connection Stuff
        //TODO: pull out the chat logic from here - its not a network component
        internal static void BroadcastOnlineMessage()
        {
            foreach (SConnection conn in Connections)
            {
                conn.ReceiveChat(string.Format(conn.Language.OnlineCount, Connections.Count(x => x.Stage == GameStage.Game), Connections.Count(x => x.Stage == GameStage.Observer)), MessageType.Hint);

                switch (conn.Stage)
                {
                    case GameStage.Game:
                        if (conn.Player.Character.Observable)
                            conn.ReceiveChat(string.Format(conn.Language.ObserverCount, conn.Observers.Count), MessageType.Hint);
                        break;
                    case GameStage.Observer:
                        conn.ReceiveChat(string.Format(conn.Language.ObserverCount, conn.Observed.Observers.Count), MessageType.Hint);
                        break;
                }
            }
        }

        internal static void BroadcastSystemMessage(Func<SConnection, string> messageSupplier)
        {
            foreach (SConnection con in Connections)
            {
                switch (con.Stage)
                {
                    case GameStage.Game:
                    case GameStage.Observer:
                        con.ReceiveChat(messageSupplier.Invoke(con), MessageType.System);
                        break;
                    default:
                        continue;
                }
            }
        }

        internal static void BroadcastMessage(string text, List<ClientUserItem> linkedItems, MessageType messageType, Predicate<SConnection> shouldReceive)
        {
            foreach (SConnection con in Connections)
            {
                switch (con.Stage)
                {
                    case GameStage.Game:
                    case GameStage.Observer:
                        if (!shouldReceive.Invoke(con)) continue;

                        con.ReceiveChat(text, messageType, linkedItems);
                        break;
                    default: continue;
                }
            }
        }
        #endregion


        internal static void IpBan(string ipAddress, TimeSpan duration)
        {
            IPBlocks[ipAddress] = SEnvir.Now.Add(duration);

            for (int i = Connections.Count - 1; i >= 0; i--)
                if (Connections[i].IPAddress == ipAddress)
                    Connections[i].TryDisconnect();
        }

        internal static void Disconnect(SConnection connection)
        {
            if (!Connections.Contains(connection))
                throw new InvalidOperationException("Connection was not found in list"); //TODO: do we want to cause an exception here? Why not just log a warning and return?

            Connections.Remove(connection);
            IPCount[connection.IPAddress]--;
            DBytesSent += TotalBytesSent;
            DBytesReceived += TotalBytesReceived;
        }


    }
}
