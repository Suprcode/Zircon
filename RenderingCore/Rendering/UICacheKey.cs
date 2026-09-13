using System;
using System.Drawing;

namespace Shared.Rendering
{
    /// <summary>Values are quantized to 1/10000; sub-pixel origins are retained.</summary>
    public readonly record struct UICacheKey(
        object Session, long Generation, Size BackBuffer, Size Scene,
        int WindowScale, int UIScale, int TextRasterScale, int OriginX, int OriginY,
        TextureFilterMode Filter, int CoordinateVersion);

    public static partial class RenderingPipelineManager
    {
        private static long _uiCacheGeneration;
        public static UICacheKey GetUICacheKey(float windowScale, float textRasterScale)
        {
            static int Normalize(float value) => (int)MathF.Round(value * 10000F);
            return new UICacheKey(_activeSession, _uiCacheGeneration, GetBackBufferSize(), Settings.ActiveSceneSize,
                Normalize(windowScale), Normalize(CurrentUIScale), Normalize(textRasterScale),
                Normalize(_uiScaleOrigin.X), Normalize(_uiScaleOrigin.Y), GetTextureFilter(), 1);
        }

        internal static void InvalidateUICacheGeneration()
        {
            ReleaseUICacheTargets(_activeSession);
            _uiCacheGeneration++;
        }
    }
}
