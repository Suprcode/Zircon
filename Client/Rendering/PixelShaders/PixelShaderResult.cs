using System.Drawing;

namespace Client.Rendering.PixelShaders
{
    public readonly struct PixelShaderResult
    {
        public PixelShaderResult(RenderTexture texture, Size size, PointF offset)
        {
            Texture = texture;
            Size = size;
            Offset = offset;
        }

        public RenderTexture Texture { get; }

        public Size Size { get; }

        public PointF Offset { get; }

        public static PixelShaderResult FromTexture(RenderTexture texture, Size size)
        {
            return new PixelShaderResult(texture, size, PointF.Empty);
        }
    }
}
