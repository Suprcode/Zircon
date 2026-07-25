using System.IO;

namespace Shared.Envir
{
    public sealed class ZlAtlasPageMetadata
    {
        public int Id;
        public int Position;
        public short Width;
        public short Height;
        public ZlAtlasLayer Layer;
        public ZlImageCodec Codec;
        public int DataSize;
        public ZlRuntimeTexturePreference RuntimePreference;
        public int Bc7DataSize;
        public int FallbackDataSize;

        public static ZlAtlasPageMetadata Read(BinaryReader reader)
        {
            return new ZlAtlasPageMetadata
            {
                Id = reader.ReadInt32(),
                Position = reader.ReadInt32(),
                Width = reader.ReadInt16(),
                Height = reader.ReadInt16(),
                Layer = (ZlAtlasLayer)reader.ReadByte(),
                Codec = (ZlImageCodec)reader.ReadByte(),
                DataSize = reader.ReadInt32(),
                RuntimePreference = (ZlRuntimeTexturePreference)reader.ReadByte(),
                Bc7DataSize = reader.ReadInt32(),
                FallbackDataSize = reader.ReadInt32()
            };
        }
    }
}
