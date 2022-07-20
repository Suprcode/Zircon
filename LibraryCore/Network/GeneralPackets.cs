namespace Library.Network.GeneralPackets
{
    public sealed class Connected : Packet { }
    public sealed class Ping : Packet { }
    public sealed class CheckVersion : Packet
    {
    }
    public sealed class Version : Packet
    {
        public byte[] ClientHash { get; set; }
    }
    public sealed class GoodVersion : Packet
    {
        public byte[] DatabaseKey { get; set; }
    }
    public sealed class PingResponse : Packet
    {
        public int Ping { get; set; }
    }

    public sealed class Disconnect : Packet
    {
        public DisconnectReason Reason { get; set; }
    }
}
