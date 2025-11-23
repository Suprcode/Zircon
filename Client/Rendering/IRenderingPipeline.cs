using Client.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace Client.Rendering
{
    public interface IRenderingPipeline
    {
        string Id { get; }

        void Initialize(RenderingPipelineContext context);

        void RunMessageLoop(Form form, Action loop);

        bool RenderFrame(Action drawScene);

        void ToggleFullScreen();

        void SetResolution(Size size);

        void SetTargetMonitor(int monitorIndex);

        void CenterOnSelectedMonitor();

        void ResetDevice();

        void OnSceneChanged(bool isGameScene);

        IReadOnlyList<Size> GetSupportedResolutions();

        Size MeasureText(string text, Font font);

        Size MeasureText(string text, Font font, Size proposedSize, TextFormatFlags format);

        float GetHorizontalDpi();

        void ConfigureGraphics(Graphics graphics);

        Color ConvertHslToRgb(float h, float s, float l);

        void SetOpacity(float opacity);

        float GetOpacity();

        void SetBlend(bool enabled, float rate, BlendMode mode);

        bool IsBlending();

        float GetBlendRate();

        BlendMode GetBlendMode();

        float GetLineWidth();

        void SetLineWidth(float width);

        void DrawLine(IReadOnlyList<LinePoint> points, Color colour);

        void DrawTexture(RenderTexture texture, Rectangle sourceRectangle, RectangleF destinationRectangle, Color colour);

        void DrawTexture(RenderTexture texture, Rectangle? sourceRectangle, Matrix3x2 transform, Vector3 center, Vector3 translation, Color colour);

        RenderSurface GetCurrentSurface();

        void SetSurface(RenderSurface surface);

        RenderSurface GetScratchSurface();

        RenderTexture GetScratchTexture();

        void ColorFill(RenderSurface surface, Rectangle rectangle, Color color);

        RenderTargetResource CreateRenderTarget(Size size);

        void ReleaseRenderTarget(RenderTargetResource renderTarget);

        Size GetBackBufferSize();

        void Clear(RenderClearFlags flags, Color colour, float z, int stencil, params Rectangle[] regions);

        void FlushSprite();

        void RegisterControlCache(ITextureCacheItem control);

        void UnregisterControlCache(ITextureCacheItem control);

        RenderTexture CreateTexture(Size size, RenderTextureFormat format, RenderTextureUsage usage, RenderTexturePool pool);

        void ReleaseTexture(RenderTexture texture);

        TextureLock LockTexture(RenderTexture texture, TextureLockMode mode);

        void RegisterTextureCache(ITextureCacheItem texture);

        void UnregisterTextureCache(ITextureCacheItem texture);

        void RegisterSoundCache(ISoundCacheItem sound);

        void UnregisterSoundCache(ISoundCacheItem sound);

        void MemoryClear();

        IReadOnlyList<ISoundCacheItem> GetRegisteredSoundCaches();

        RenderTexture GetColourPaletteTexture();

        byte[] GetColourPaletteData();

        RenderTexture GetLightTexture();

        Size GetLightTextureSize();

        RenderTexture GetPoisonTexture();

        Size GetPoisonTextureSize();

        TextureFilterMode GetTextureFilter();

        void SetTextureFilter(TextureFilterMode mode);

        void Shutdown();
    }
}
