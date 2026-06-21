using System.IO;

namespace Shared.Envir
{
    public enum ZlImageCodec : byte
    {
        Dxt1,
        Dxt5,
        Bgra32,
        Bc7,
        Png,
    }

    public enum ZlRuntimeTexturePreference : byte
    {
        None,
        Bgra32,
        Bc7Dxt5,
        Bc7,
        Dxt1,
        Dxt5,
        SourceType,
    }

    public enum ZlContainerCompression : byte
    {
        None,
        DeflateFast,
        DeflateBest,
    }

    public enum ZlEntryType : byte
    {
        ImagePayload = 1,
        AtlasPagePayload = 4,
    }

    public enum ZlAtlasLayer : byte
    {
        Image,
        Shadow,
        Overlay,
    }

    public sealed class Zl2Entry
    {
        public ZlEntryType Type;
        public int Id;
        public int UncompressedSize;
        public int CompressedSize;
        public long Offset;
        public ZlContainerCompression Compression;
        public ZlImageCodec Codec;

        public static Zl2Entry Read(BinaryReader reader)
        {
            return new Zl2Entry
            {
                Type = (ZlEntryType)reader.ReadByte(),
                Id = reader.ReadInt32(),
                UncompressedSize = reader.ReadInt32(),
                CompressedSize = reader.ReadInt32(),
                Offset = reader.ReadInt64(),
                Compression = (ZlContainerCompression)reader.ReadByte(),
                Codec = (ZlImageCodec)reader.ReadByte()
            };
        }
    }
}
