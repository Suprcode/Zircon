using System.IO;
using System.Text;

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
        Source,
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

    public static class ZlContainerHeader
    {
        public static readonly byte[] Signature = Encoding.ASCII.GetBytes("ZL2");

        public static readonly int ByteCount =
            Signature.Length +
            sizeof(int) +  // Version
            sizeof(int) +  // Image count
            sizeof(int) +  // Atlas page count
            sizeof(byte) + // Default payload compression
            sizeof(byte) + // Flags
            sizeof(short) + // Reserved
            sizeof(long) + // Metadata offset
            sizeof(int) +  // Metadata size
            sizeof(long) + // Index offset
            sizeof(int);   // Index size

        public const int HasAtlasFlag = 1;

        public static bool ReadSignature(BinaryReader reader)
        {
            byte[] signature = reader.ReadBytes(Signature.Length);
            if (signature.Length != Signature.Length)
                return false;

            for (int i = 0; i < Signature.Length; i++)
            {
                if (signature[i] != Signature[i])
                    return false;
            }

            return true;
        }
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
