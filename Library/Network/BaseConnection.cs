using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using G = Library.Network.GeneralPackets;


//TODO Dispose ?
namespace Library.Network
{
    public abstract class BaseConnection
    {
        public static Dictionary<string, DiagnosticValue> Diagnostics = new Dictionary<string, DiagnosticValue>();
        public static Dictionary<Type, MethodInfo> PacketMethods = new Dictionary<Type, MethodInfo>();
        public static bool Monitor;

        public bool Connected { get; set; }
        protected bool Sending { get; set; }

        public int TotalBytesSent { get; set; }
        public int TotalBytesReceived { get; set; }

        public bool AdditionalLogging;

        protected TcpClient Client;

        public DateTime TimeConnected { get; set; }
        public TimeSpan Duration => Time.Now - TimeConnected;

        protected abstract TimeSpan TimeOutDelay { get; }
        public DateTime TimeOutTime { get; set; }

        private bool _disconnecting;
        public bool Disconnecting
        {
            get { return _disconnecting; }
            set
            {
                if (_disconnecting == value) return;
                _disconnecting = value;
                TimeOutTime = Time.Now.AddSeconds(2);
            }
        }

        public ConcurrentQueue<Packet> ReceiveList = new ConcurrentQueue<Packet>();
        public ConcurrentQueue<Packet> SendList = new ConcurrentQueue<Packet>();
        private byte[] _rawData = new byte[0];

        public EventHandler<Exception> OnException;

        protected BaseConnection(TcpClient client)
        {
            Client = client;
            Client.NoDelay = true;


            Connected = true;
            TimeConnected = Time.Now;
        }

        protected void BeginReceive()
        {
            try
            {
                if (Client == null || !Client.Connected) return;

                byte[] rawBytes = new byte[8 * 1024];

                Client.Client.BeginReceive(rawBytes, 0, rawBytes.Length, SocketFlags.None, ReceiveData, rawBytes);
            }
            catch (Exception ex)
            {
                if (AdditionalLogging)
                    OnException(this, ex);
                Disconnecting = true;
            }
        }
        private void ReceiveData(IAsyncResult result)
        {
            try
            {
                if (!Connected) return;

                int dataRead = Client.Client.EndReceive(result);

                if (dataRead == 0)
                {
                    Disconnecting = true;
                    return;
                }
                TotalBytesReceived += dataRead;

                UpdateTimeOut();

                byte[] rawBytes = result.AsyncState as byte[];

                byte[] temp = _rawData;
                _rawData = new byte[dataRead + temp.Length];
                Buffer.BlockCopy(temp, 0, _rawData, 0, temp.Length);
                Buffer.BlockCopy(rawBytes, 0, _rawData, temp.Length, dataRead);

                Packet p;

                while ((p = Packet.ReceivePacket(_rawData, out _rawData)) != null)
                    ReceiveList.Enqueue(p);

                BeginReceive();
            }
            catch (Exception ex)
            {
                if (AdditionalLogging)
                    OnException(this, ex);
                Disconnecting = true;
            }
        }
        private void BeginSend(List<byte> data)
        {
            if (!Connected || data.Count == 0) return;

            try
            {
                Sending = true;
                TotalBytesSent += data.Count;
                Client.Client.BeginSend(data.ToArray(), 0, data.Count, SocketFlags.None, SendData, null);
                UpdateTimeOut();
            }
            catch (Exception ex)
            {
                if (AdditionalLogging)
                    OnException(this, ex);
                Disconnecting = true;
                Sending = false;
            }
        }
        private void SendData(IAsyncResult result)
        {
            try
            {
                Sending = false;
                Client.Client.EndSend(result);
                UpdateTimeOut();
            }
            catch (Exception ex)
            {
                if (AdditionalLogging)
                    OnException(this, ex);
                Disconnecting = true;
            }
        }
        public virtual void Enqueue(Packet p)
        {
            if (!Connected || p == null) return;

            SendList.Enqueue(p);
        }

        public abstract void TryDisconnect();

        public virtual void Disconnect()
        {
            if (!Connected) return;

            Connected = false;

            SendList = null;
            ReceiveList = null;
            _rawData = null;

            Client.Client.Dispose();
            Client = null;
        }

        public abstract void TrySendDisconnect(Packet p);

        public virtual void SendDisconnect(Packet p)
        {
            if (!Connected || Disconnecting)
            {
                Disconnecting = true;
                return;
            }

            List<byte> data = new List<byte>();

            data.AddRange(p.GetPacketBytes());

            BeginSendDisconnect(data);
        }
        private void BeginSendDisconnect(List<byte> data)
        {
            if (!Connected || data.Count == 0) return;

            if (Disconnecting) return;

            try
            {
                Disconnecting = true;

                TotalBytesSent += data.Count;
                Client.Client.BeginSend(data.ToArray(), 0, data.Count, SocketFlags.None, SendDataDisconnect, null);
            }
            catch (Exception ex)
            {
                if (AdditionalLogging)
                    OnException(this, ex);
            }
        }
        private void SendDataDisconnect(IAsyncResult result)
        {

            try
            {
                Client.Client.EndSend(result);
            }
            catch (Exception ex)
            {
                if (AdditionalLogging)
                    OnException(this, ex);
            }
        }

        public virtual void Process()
        {
            if (Client == null || !Client.Connected)
            {
                TryDisconnect();
                return;
            }

            while (!ReceiveList.IsEmpty && !Disconnecting)
            {
                try
                {
                    Packet p;
                    if (!ReceiveList.TryDequeue(out p)) continue;

                    ProcessPacket(p);
                }
                catch (NotImplementedException ex)
                {
                    OnException(this, ex);
                }
                catch (Exception ex)
                {
                    OnException(this, ex);
                    throw ex;
                }
            }

            if (Time.Now >= TimeOutTime)
            {
                if (!Disconnecting)
                    TrySendDisconnect(new G.Disconnect { Reason = DisconnectReason.TimedOut });
                else
                    TryDisconnect();

                return;
            }

            if (!Disconnecting && Sending)
                UpdateTimeOut();

            if (SendList.IsEmpty || Sending) return;

            List<byte> data = new List<byte>();
            while (!SendList.IsEmpty)
            {
                Packet p;

                if (!SendList.TryDequeue(out p)) continue;

                if (p == null) continue;

                try
                {
                    byte[] bytes = p.GetPacketBytes();

                    data.AddRange(bytes);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(this, ex);
                    Disconnecting = true;
                    return;
                }


                if (!Monitor) continue;
                
                DiagnosticValue value;
                Type type = p.GetType();

                if (!Diagnostics.TryGetValue(type.FullName, out value))
                    Diagnostics[type.FullName] = value = new DiagnosticValue { Name = type.FullName };

                value.Count++;
                value.TotalSize += p.Length;
                
                if (p.Length > value.LargestSize)
                    value.LargestSize = p.Length;
            }

            BeginSend(data);
        }

        private void ProcessPacket(Packet p)
        {
            if (p == null) return;

            DateTime start = Time.Now;
            
            MethodInfo info;
            if (!PacketMethods.TryGetValue(p.PacketType, out info))
                PacketMethods[p.PacketType] = info = GetType().GetMethod("Process", new[] { p.PacketType });

            if (info == null)
                throw new NotImplementedException($"Not Implemented Exception: Method Process({p.PacketType}).");

            info.Invoke(this, new object[] { p });

            if (!Monitor) return;

            TimeSpan execution = Time.Now - start;
            DiagnosticValue value;

            if (!Diagnostics.TryGetValue(p.PacketType.FullName, out value))
                Diagnostics[p.PacketType.FullName] = value = new DiagnosticValue { Name = p.PacketType.FullName };

            value.Count++;
            value.TotalTime += execution;
            value.TotalSize += p.Length;

            if (execution > value.LargestTime)
                value.LargestTime = execution;

            if (p.Length > value.LargestSize)
                value.LargestSize = p.Length;


        }
        public void UpdateTimeOut()
        {
            if (Disconnecting) return;

            TimeOutTime = Time.Now + TimeOutDelay;
        }
    }


    public class DiagnosticValue
    {
        public string Name { get; set; }
        public TimeSpan TotalTime { get; set; }
        public TimeSpan LargestTime { get; set; }
        public int Count { get; set; }
        public long TotalSize { get; set; }
        public long LargestSize { get; set; }

        public long TotalTicks => TotalTime.Ticks;
        public long TotalMilliseconds => TotalTicks / TimeSpan.TicksPerMillisecond;

        public long LargestTicks => LargestTime.Ticks;
        public long LargestMilliseconds => LargestTicks / TimeSpan.TicksPerMillisecond;
    }
}
