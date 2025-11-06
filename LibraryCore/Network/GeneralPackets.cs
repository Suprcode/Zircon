using MemoryPack;

namespace Library.Network.GeneralPackets
{
    [MemoryPackable]
    public sealed partial class Connected : Packet { }
    [MemoryPackable]
    public sealed partial class Ping : Packet { }
    [MemoryPackable]
    public sealed partial class CheckVersion : Packet
    {
    }
    [MemoryPackable]
    public sealed partial class Version : Packet
    {
        public byte[] ClientHash { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GoodVersion : Packet
    {
        public byte[] DatabaseKey { get; set; }
    }
    [MemoryPackable]
    public sealed partial class PingResponse : Packet
    {
        public int Ping { get; set; }
    }

    [MemoryPackable]
    public sealed partial class Disconnect : Packet
    {
        public DisconnectReason Reason { get; set; }
    }
}
