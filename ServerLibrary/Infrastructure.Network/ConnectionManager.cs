using Library;
using Library.Network;
using Server.Envir;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Server.Infrastructure.Network
{
    public class ConnectionManager<ConnectionType>(IConnectionFactory<ConnectionType> ConnectionFactory, IpAddressManager IpManager) where ConnectionType : BaseConnection
    {
        //public static Dictionary<string, int> IPCount = new Dictionary<string, int>(); //TODO: this isn't used but i imagine its intended to limit connections from single IP

        private bool IsResetting = false;

        public ConcurrentQueue<ConnectionType> PendingConnections = [];
        public List<ConnectionType> Connections = [];

        public bool AcceptingConnections => PendingConnections?.Count >= 15 || IsResetting;

        #region Metadata
        public long DBytesSent, DBytesReceived; //TODO: not sure why these DBytesX exist? They get assigned to TotalBytesX but only get incremented when someone disconnects (incremented by total of TotalBytesX)

        public long TotalBytesSent, TotalBytesReceived;
        public long PreviousTotalReceived, PreviousTotalSent;
        #endregion

        internal void Add(TcpClient tcpClient)
        {
            string ipAddress = tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0]; //TODO: this is duplicated inside BaseConnection - create a utility method
            if (IpManager.IsAllowing(ipAddress))
            {
                var connection = ConnectionFactory.Create(tcpClient);
                if (connection.Connected) PendingConnections?.Enqueue(connection);
            }
        }

        internal void Process()
        {
            while (!PendingConnections.IsEmpty)
            {
                if (!PendingConnections.TryDequeue(out var connection)) break;

                //IPCount.TryGetValue(connection.IPAddress, out var ipCount);
                //IPCount[connection.IPAddress] = ipCount + 1;
                Connections.Add(connection);
            }

            long bytesSent = 0;
            long bytesReceived = 0;
            for (int i = Connections.Count - 1; i >= 0; i--)
            {
                if (i >= Connections.Count) break;

                var connection = Connections[i];
                connection.Process();

                if (connection.TotalPacketsProcessed == 0 && connection.TotalBytesReceived > 1024)
                {
                    connection.TryDisconnect();
                    IpManager.Timeout(connection, Config.PacketBanTime);
                    SEnvir.Log($"{connection.IPAddress} Disconnected, Large Packet");
                    return;
                }

                if (connection.ReceiveList.Count > Config.MaxPacket)
                {
                    connection.TryDisconnect();
                    IpManager.Timeout(connection, Config.PacketBanTime);
                    SEnvir.Log($"{connection.IPAddress} Disconnected, Large amount of Packets");
                    return;
                }

                bytesSent += connection.TotalBytesSent;
                bytesReceived += connection.TotalBytesReceived;
            }

            TotalBytesSent = DBytesSent + bytesSent;
            TotalBytesReceived = DBytesReceived + bytesReceived;
            PreviousTotalReceived = TotalBytesReceived;
            PreviousTotalSent = TotalBytesSent;
        }

        internal void Broadcast(Packet packet)
        {
            for (int i = Connections.Count - 1; i >= 0; i--)
                Connections[i].SendDisconnect(packet);
        }

        internal void Broadcast(Func<ConnectionType, Packet> packet)
        {
            for (int i = Connections.Count - 1; i >= 0; i--)
                Connections[i].SendDisconnect(packet.Invoke(Connections[i]));
        }

        internal void RemoveConnection(ConnectionType connection)
        {
            if (!Connections.Contains(connection))
                throw new InvalidOperationException("Connection was not found in list"); //TODO: why do we want to cause an exception here? Why not just log a warning and return?

            Connections.Remove(connection);
            //IPCount[connection.IPAddress]--;
            DBytesSent += TotalBytesSent;
            DBytesReceived += TotalBytesReceived;
        }

        internal void Reset()
        {
            IsResetting = true;
            PendingConnections?.Clear();
            
            try
            {
                Packet p = new Library.Network.GeneralPackets.Disconnect { Reason = DisconnectReason.ServerClosing };
                for (int i = Connections.Count - 1; i >= 0; i--)
                    Connections[i].SendDisconnect(p);

                Thread.Sleep(2000); // wait for disconnects
                
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }

            IsResetting = false;
        }
    }
}
