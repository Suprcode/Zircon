using System.Drawing;
using System.IO;

namespace Shared.Envir
{
    public sealed class ZlImageMetadata
    {
        public int Version;
        public int Position;
        public short Width;
        public short Height;
        public short OffSetX;
        public short OffSetY;
        public byte ShadowType;
        public short ShadowWidth;
        public short ShadowHeight;
        public short ShadowOffSetX;
        public short ShadowOffSetY;
        public short OverlayWidth;
        public short OverlayHeight;
        public int AtlasPage;
        public int ShadowAtlasPage = -1;
        public int OverlayAtlasPage = -1;
        public Rectangle SourceRectangle;
        public Rectangle VisibleBounds;
        public ZlImageCodec ImageCodec;
        public ZlImageCodec ShadowCodec;
        public ZlImageCodec OverlayCodec;
        public ZlRuntimeTexturePreference ImageRuntimePreference;
        public ZlRuntimeTexturePreference ShadowRuntimePreference;
        public ZlRuntimeTexturePreference OverlayRuntimePreference;
        public int StoredImageDataSize;
        public int ImageBc7DataSize;
        public int ImageFallbackDataSize;
        public int StoredShadowDataSize;
        public int ShadowBc7DataSize;
        public int ShadowFallbackDataSize;
        public int StoredOverlayDataSize;
        public int OverlayBc7DataSize;
        public int OverlayFallbackDataSize;

        public static ZlImageMetadata Read(BinaryReader reader, int version)
        {
            ZlImageMetadata metadata = new ZlImageMetadata
            {
                Version = version,
                Position = reader.ReadInt32(),
                Width = reader.ReadInt16(),
                Height = reader.ReadInt16(),
                OffSetX = reader.ReadInt16(),
                OffSetY = reader.ReadInt16(),
                ShadowType = reader.ReadByte(),
                ShadowWidth = reader.ReadInt16(),
                ShadowHeight = reader.ReadInt16(),
                ShadowOffSetX = reader.ReadInt16(),
                ShadowOffSetY = reader.ReadInt16(),
                OverlayWidth = reader.ReadInt16(),
                OverlayHeight = reader.ReadInt16(),
            };

            metadata.ImageCodec = version == 0 ? ZlImageCodec.Dxt1 : ZlImageCodec.Dxt5;
            metadata.ShadowCodec = metadata.ImageCodec;
            metadata.OverlayCodec = metadata.ImageCodec;
            metadata.ImageRuntimePreference = ZlRuntimeTexturePreference.Bgra32;
            metadata.ShadowRuntimePreference = ZlRuntimeTexturePreference.Bgra32;
            metadata.OverlayRuntimePreference = ZlRuntimeTexturePreference.Bgra32;
            metadata.SourceRectangle = new Rectangle(0, 0, metadata.Width, metadata.Height);

            if (version < 2)
                return metadata;

            metadata.AtlasPage = reader.ReadInt32();
            metadata.SourceRectangle = ReadRectangle(reader);
            metadata.VisibleBounds = ReadRectangle(reader);
            metadata.ImageCodec = (ZlImageCodec)reader.ReadByte();
            metadata.ShadowCodec = (ZlImageCodec)reader.ReadByte();
            metadata.OverlayCodec = (ZlImageCodec)reader.ReadByte();
            metadata.ImageRuntimePreference = (ZlRuntimeTexturePreference)reader.ReadByte();
            metadata.ShadowRuntimePreference = (ZlRuntimeTexturePreference)reader.ReadByte();
            metadata.OverlayRuntimePreference = (ZlRuntimeTexturePreference)reader.ReadByte();
            metadata.StoredImageDataSize = reader.ReadInt32();
            metadata.ImageBc7DataSize = reader.ReadInt32();
            metadata.ImageFallbackDataSize = reader.ReadInt32();
            metadata.StoredShadowDataSize = reader.ReadInt32();
            metadata.ShadowBc7DataSize = reader.ReadInt32();
            metadata.ShadowFallbackDataSize = reader.ReadInt32();
            metadata.StoredOverlayDataSize = reader.ReadInt32();
            metadata.OverlayBc7DataSize = reader.ReadInt32();
            metadata.OverlayFallbackDataSize = reader.ReadInt32();

            return metadata;
        }

        private static Rectangle ReadRectangle(BinaryReader reader)
        {
            return new Rectangle(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16());
        }
    }
}
