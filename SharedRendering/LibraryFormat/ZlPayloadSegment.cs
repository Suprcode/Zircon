using Shared.Envir;
using Shared.Rendering;

namespace SharedRendering.LibraryFormat
{
    internal readonly struct ZlPayloadSegment
    {
        public int Offset { get; }
        public int Size { get; }

        private ZlPayloadSegment(int offset, int size)
        {
            Offset = offset;
            Size = size;
        }

        public static ZlPayloadSegment Select(ZlImageCodec sourceCodec, RenderTextureFormat textureFormat, int primarySize, int runtimeSize, int fallbackSize)
        {
            RenderTextureFormat primaryFormat = ConvertFormat(sourceCodec);

            if (textureFormat == primaryFormat || runtimeSize <= 0 && fallbackSize <= 0)
                return new ZlPayloadSegment(0, primarySize);

            if (textureFormat == RenderTextureFormat.Bc7 || textureFormat == RenderTextureFormat.Dxt1)
                return new ZlPayloadSegment(primarySize, runtimeSize);

            if (textureFormat == RenderTextureFormat.Dxt5 && primaryFormat != RenderTextureFormat.Dxt5 && fallbackSize > 0)
                return new ZlPayloadSegment(primarySize + runtimeSize, fallbackSize);

            return new ZlPayloadSegment(0, primarySize);
        }

        private static RenderTextureFormat ConvertFormat(ZlImageCodec codec)
        {
            return codec switch
            {
                ZlImageCodec.Dxt1 => RenderTextureFormat.Dxt1,
                ZlImageCodec.Dxt5 => RenderTextureFormat.Dxt5,
                ZlImageCodec.Bgra32 => RenderTextureFormat.Bgra32,
                ZlImageCodec.Bc7 => RenderTextureFormat.Bc7,
                ZlImageCodec.Png => RenderTextureFormat.Bgra32,
                _ => RenderTextureFormat.Dxt5,
            };
        }
    }

}
