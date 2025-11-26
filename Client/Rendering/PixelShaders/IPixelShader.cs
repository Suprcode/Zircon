using System.Drawing;

namespace Client.Rendering.PixelShaders
{
    public interface IPixelShader
    {
        PixelShaderResult Apply(RenderTexture texture, Size sourceSize);
    }
}
