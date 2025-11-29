using Client.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace Client.Rendering
{
    public static class RenderingPipelineManager
    {
        private const string DefaultPipelineId = RenderingPipelineIds.SlimDXD3D9;
        private static readonly Dictionary<string, Func<IRenderingPipeline>> PipelineFactories = new(StringComparer.OrdinalIgnoreCase)
        {
            { RenderingPipelineIds.SharpDXD3D9, () => new SharpDXD3D9.SharpDXD3D9RenderingPipeline() },
            { RenderingPipelineIds.SharpDXD3D11, () => new SharpDXD3D11.SharpDXD3D11RenderingPipeline() }
        };

        private static IRenderingPipeline _activePipeline;
        private static RenderingPipelineContext _context;
        private static readonly Graphics FallbackGraphics;
        private static readonly List<ITextureCacheItem> FallbackControlCache = new();
        private static readonly List<ITextureCacheItem> FallbackTextureCache = new();
        private static readonly List<ISoundCacheItem> FallbackSoundCache = new();
        private static float _fallbackOpacity = 1F;
        private static bool _fallbackBlending;
        private static float _fallbackBlendRate = 1F;
        private static BlendMode _fallbackBlendMode = BlendMode.NORMAL;
        private static SpriteShaderEffectRequest? _spriteShaderEffect;
        private static float _fallbackLineWidth = 1F;
        private static TextureFilterMode _fallbackTextureFilter = TextureFilterMode.Point;
        private static readonly object GraphicsLock = new();

        static RenderingPipelineManager()
        {
            FallbackGraphics = Graphics.FromHwnd(IntPtr.Zero);
            ConfigureFallbackGraphics(FallbackGraphics);
        }

        public static string DefaultPipelineIdentifier => DefaultPipelineId;
        public static string? ActivePipelineId => _activePipeline?.Id;
        public static IReadOnlyCollection<string> AvailablePipelineIds => PipelineFactories.Keys;
        public static bool SupportsMultiplePipelines => PipelineFactories.Count > 1;
        public static bool IsDefaultPipelineOnly => PipelineFactories.Count == 1 && PipelineFactories.ContainsKey(DefaultPipelineId);

        public static string NormalizePipelineId(string pipelineId)
        {
            string requestedId = string.IsNullOrWhiteSpace(pipelineId) ? DefaultPipelineId : pipelineId;

            if (IsDefaultPipelineOnly)
                return DefaultPipelineId;

            if (!PipelineFactories.ContainsKey(requestedId) && PipelineFactories.ContainsKey(DefaultPipelineId))
                return DefaultPipelineId;

            return requestedId;
        }

        public static void RegisterFactory(string pipelineId, Func<IRenderingPipeline> factory)
        {
            if (string.IsNullOrWhiteSpace(pipelineId))
                throw new ArgumentException("Pipeline identifier must be provided.", nameof(pipelineId));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            PipelineFactories[pipelineId] = factory;
        }

        public static void Initialize(string pipelineId, RenderingPipelineContext context)
        {
            if (_activePipeline != null)
                throw new InvalidOperationException("A rendering pipeline has already been initialized.");

            if (!PipelineFactories.TryGetValue(pipelineId, out Func<IRenderingPipeline>? factory))
                throw new ArgumentException($"Unknown rendering pipeline '{pipelineId}'.", nameof(pipelineId));

            _context = context;

            _activePipeline = factory();
            try
            {
                _activePipeline.Initialize(context);
            }
            catch
            {
                _activePipeline = null;
                throw;
            }
        }

        public static string InitializeWithFallback(string requestedPipelineId, RenderingPipelineContext context)
        {
            string pipelineToUse = string.IsNullOrWhiteSpace(requestedPipelineId) ? DefaultPipelineId : requestedPipelineId;

            try
            {
                Initialize(pipelineToUse, context);
                return _activePipeline!.Id;
            }
            catch (Exception ex)
            {
                if (_activePipeline != null || pipelineToUse.Equals(DefaultPipelineId, StringComparison.OrdinalIgnoreCase) || !PipelineFactories.ContainsKey(DefaultPipelineId))
                    throw;

                Initialize(DefaultPipelineId, context);
                Console.WriteLine($"Falling back to rendering pipeline '{DefaultPipelineId}' after '{pipelineToUse}' failed: {ex.Message}");
                return _activePipeline!.Id;
            }
        }

        public static void SwitchPipeline(string pipelineId)
        {
            pipelineId = NormalizePipelineId(pipelineId);

            if (string.Equals(ActivePipelineId, pipelineId, StringComparison.OrdinalIgnoreCase)) return;

            Shutdown();

            InitializeWithFallback(pipelineId, _context);
        }

        public static void Shutdown()
        {
            if (_activePipeline == null)
                return;

            _activePipeline.Shutdown();
            _activePipeline = null;
        }

        public static void RunMessageLoop(Form form, Action loop)
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            if (form == null)
                throw new ArgumentNullException(nameof(form));

            if (loop == null)
                throw new ArgumentNullException(nameof(loop));

            _activePipeline.RunMessageLoop(form, loop);
        }

        public static bool RenderFrame(Action drawScene)
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            return _activePipeline.RenderFrame(drawScene);
        }

        public static void ToggleFullScreen()
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            _activePipeline.ToggleFullScreen();
        }

        public static void SetResolution(Size size)
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            _activePipeline.SetResolution(size);
        }

        public static void SetTargetMonitor(int monitorIndex)
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            _activePipeline.SetTargetMonitor(monitorIndex);
        }

        public static void CenterOnSelectedMonitor()
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            _activePipeline.CenterOnSelectedMonitor();
        }

        public static void ResetDevice()
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            _activePipeline.ResetDevice();
        }

        public static void OnSceneChanged(bool isGameScene)
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            _activePipeline.OnSceneChanged(isGameScene);
        }

        public static IReadOnlyList<Size> GetSupportedResolutions()
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            return _activePipeline.GetSupportedResolutions();
        }

        public static Size MeasureText(string text, Font font)
        {
            if (_activePipeline != null)
                return _activePipeline.MeasureText(text, font);

            lock (GraphicsLock)
            {
                return TextRenderer.MeasureText(FallbackGraphics, text, font);
            }
        }

        public static Size MeasureText(string text, Font font, Size proposedSize, TextFormatFlags format)
        {
            if (_activePipeline != null)
                return _activePipeline.MeasureText(text, font, proposedSize, format);

            lock (GraphicsLock)
            {
                return TextRenderer.MeasureText(FallbackGraphics, text, font, proposedSize, format);
            }
        }

        public static float GetHorizontalDpi()
        {
            if (_activePipeline != null)
                return _activePipeline.GetHorizontalDpi();

            lock (GraphicsLock)
            {
                return FallbackGraphics.DpiX;
            }
        }

        public static void ConfigureGraphics(Graphics graphics)
        {
            if (graphics == null)
                throw new ArgumentNullException(nameof(graphics));

            if (_activePipeline != null)
            {
                _activePipeline.ConfigureGraphics(graphics);
                return;
            }

            ConfigureFallbackGraphics(graphics);
        }

        public static Color ConvertHslToRgb(float h, float s, float l)
        {
            if (_activePipeline != null)
                return _activePipeline.ConvertHslToRgb(h, s, l);

            return ConvertHslToRgbFallback(h, s, l);
        }

        public static void SetOpacity(float opacity)
        {
            if (_activePipeline != null)
            {
                _activePipeline.SetOpacity(opacity);
                return;
            }

            _fallbackOpacity = opacity;
        }

        public static float GetOpacity()
        {
            if (_activePipeline != null)
                return _activePipeline.GetOpacity();

            return _fallbackOpacity;
        }

        public static void SetBlend(bool enabled, float rate = 1F, BlendMode mode = BlendMode.NORMAL)
        {
            if (_activePipeline != null)
            {
                _activePipeline.SetBlend(enabled, rate, mode);
                return;
            }

            _fallbackBlending = enabled;
            _fallbackBlendRate = rate;
            _fallbackBlendMode = mode;
        }

        public static bool IsBlending()
        {
            if (_activePipeline != null)
                return _activePipeline.IsBlending();

            return _fallbackBlending;
        }

        public static float GetBlendRate()
        {
            if (_activePipeline != null)
                return _activePipeline.GetBlendRate();

            return _fallbackBlendRate;
        }

        public static BlendMode GetBlendMode()
        {
            if (_activePipeline != null)
                return _activePipeline.GetBlendMode();

            return _fallbackBlendMode;
        }

        public static float GetLineWidth()
        {
            if (_activePipeline != null)
                return _activePipeline.GetLineWidth();

            return _fallbackLineWidth;
        }

        public static void SetLineWidth(float width)
        {
            if (_activePipeline != null)
            {
                _activePipeline.SetLineWidth(width);
                return;
            }

            _fallbackLineWidth = width;
        }

        public static void EnableOutlineEffect(Color colour, float thickness)
        {
            _spriteShaderEffect = new SpriteShaderEffectRequest(new OutlineEffectSettings(colour, thickness));
        }

        public static void EnableGrayscaleEffect()
        {
            _spriteShaderEffect = new SpriteShaderEffectRequest(SpriteShaderEffectKind.Grayscale);
        }

        public static void DisableSpriteShaderEffect()
        {
            _spriteShaderEffect = null;
        }

        public static void DisableOutlineEffect()
        {
            DisableSpriteShaderEffect();
        }

        internal static SpriteShaderEffectRequest? GetSpriteShaderEffect() => _spriteShaderEffect;

        public static void DrawLine(IReadOnlyList<LinePoint> points, Color colour)
        {
            if (points == null || points.Count == 0)
                return;

            if (_activePipeline != null)
            {
                _activePipeline.DrawLine(points, colour);
            }
        }

        public static void DrawTexture(RenderTexture texture, Rectangle sourceRectangle, RectangleF destinationRectangle, Color colour)
        {
            if (!texture.IsValid)
                throw new ArgumentException("A valid texture handle is required.", nameof(texture));

            if (sourceRectangle.Width <= 0 || sourceRectangle.Height <= 0)
                return;

            if (destinationRectangle.Width <= 0 || destinationRectangle.Height <= 0)
                return;

            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            _activePipeline.DrawTexture(texture, sourceRectangle, destinationRectangle, colour);
        }

        public static void DrawTexture(RenderTexture texture, Rectangle? sourceRectangle, Matrix3x2 transform, Vector3 center, Vector3 translation, Color colour)
        {
            if (!texture.IsValid)
                throw new ArgumentException("A valid texture handle is required.", nameof(texture));

            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            _activePipeline.DrawTexture(texture, sourceRectangle, transform, center, translation, colour);
        }

        public static RenderSurface GetCurrentSurface()
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            return _activePipeline.GetCurrentSurface();
        }

        public static void SetSurface(RenderSurface surface)
        {
            if (!surface.IsValid)
                throw new ArgumentException("A valid surface handle is required.", nameof(surface));

            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            _activePipeline.SetSurface(surface);
        }

        public static RenderSurface GetScratchSurface()
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            return _activePipeline.GetScratchSurface();
        }

        public static RenderTexture GetScratchTexture()
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            return _activePipeline.GetScratchTexture();
        }

        public static void ColorFill(RenderSurface surface, Rectangle rectangle, Color colorFill)
        {
            if (!surface.IsValid)
                throw new ArgumentException("A valid surface handle is required.", nameof(surface));

            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            _activePipeline.ColorFill(surface, rectangle, colorFill);
        }

        public static RenderTargetResource CreateRenderTarget(Size size)
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            return _activePipeline.CreateRenderTarget(size);
        }

        public static void ReleaseRenderTarget(RenderTargetResource renderTarget)
        {
            if (!renderTarget.IsValid)
                return;

            if (_activePipeline == null)
            {
                if (renderTarget.Surface.NativeHandle is IDisposable disposableSurface)
                    disposableSurface.Dispose();

                if (renderTarget.Texture.NativeHandle is IDisposable disposableTexture)
                    disposableTexture.Dispose();

                return;
            }

            _activePipeline.ReleaseRenderTarget(renderTarget);
        }

        public static Size GetBackBufferSize()
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            return _activePipeline.GetBackBufferSize();
        }

        public static RenderTexture GetColourPaletteTexture()
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            return _activePipeline.GetColourPaletteTexture();
        }

        public static byte[] GetColourPaletteData()
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            return _activePipeline.GetColourPaletteData();
        }

        public static RenderTexture GetLightTexture()
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            return _activePipeline.GetLightTexture();
        }

        public static Size GetLightTextureSize()
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            return _activePipeline.GetLightTextureSize();
        }

        public static RenderTexture GetPoisonTexture()
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            return _activePipeline.GetPoisonTexture();
        }

        public static Size GetPoisonTextureSize()
        {
            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            return _activePipeline.GetPoisonTextureSize();
        }

        public static TextureFilterMode GetTextureFilter()
        {
            if (_activePipeline == null)
                return _fallbackTextureFilter;

            return _activePipeline.GetTextureFilter();
        }

        public static void SetTextureFilter(TextureFilterMode mode)
        {
            if (_activePipeline != null)
            {
                _activePipeline.SetTextureFilter(mode);
            }
            else
            {
                _fallbackTextureFilter = mode;
            }
        }

        public static void Clear(RenderClearFlags flags, Color colour, float z, int stencil, params Rectangle[] regions)
        {
            if (_activePipeline != null)
            {
                _activePipeline.Clear(flags, colour, z, stencil, regions);
                return;
            }
        }

        public static void FlushSprite()
        {
            if (_activePipeline != null)
            {
                _activePipeline.FlushSprite();
            }
        }

        public static void RegisterControlCache(ITextureCacheItem control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            if (_activePipeline != null)
            {
                _activePipeline.RegisterControlCache(control);
                return;
            }

            if (!FallbackControlCache.Contains(control))
                FallbackControlCache.Add(control);
        }

        public static void UnregisterControlCache(ITextureCacheItem control)
        {
            if (control == null)
                return;

            if (_activePipeline != null)
            {
                _activePipeline.UnregisterControlCache(control);
                return;
            }

            FallbackControlCache.Remove(control);
        }

        public static RenderTexture CreateTexture(Size size, RenderTextureFormat format, RenderTextureUsage usage, RenderTexturePool pool)
        {
            if (_activePipeline != null)
                return _activePipeline.CreateTexture(size, format, usage, pool);

            throw new InvalidOperationException("Rendering pipeline is not initialized.");
        }

        public static void ReleaseTexture(RenderTexture texture)
        {
            if (!texture.IsValid)
                return;

            if (_activePipeline != null)
            {
                _activePipeline.ReleaseTexture(texture);
                return;
            }

            throw new InvalidOperationException("Rendering pipeline is not initialized.");
        }

        public static TextureLock LockTexture(RenderTexture texture, TextureLockMode mode)
        {
            if (!texture.IsValid)
                throw new ArgumentException("A valid texture handle is required.", nameof(texture));

            if (_activePipeline != null)
                return _activePipeline.LockTexture(texture, mode);

            throw new InvalidOperationException("Rendering pipeline is not initialized.");
        }

        public static void RegisterTextureCache(ITextureCacheItem texture)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            if (_activePipeline != null)
            {
                _activePipeline.RegisterTextureCache(texture);
                return;
            }

            if (!FallbackTextureCache.Contains(texture))
                FallbackTextureCache.Add(texture);
        }

        public static void UnregisterTextureCache(ITextureCacheItem texture)
        {
            if (texture == null)
                return;

            if (_activePipeline != null)
            {
                _activePipeline.UnregisterTextureCache(texture);
                return;
            }

            FallbackTextureCache.Remove(texture);
        }

        public static void RegisterSoundCache(ISoundCacheItem sound)
        {
            if (sound == null)
                throw new ArgumentNullException(nameof(sound));

            if (_activePipeline != null)
            {
                _activePipeline.RegisterSoundCache(sound);
                return;
            }

            if (!FallbackSoundCache.Contains(sound))
                FallbackSoundCache.Add(sound);
        }

        public static void UnregisterSoundCache(ISoundCacheItem sound)
        {
            if (sound == null)
                return;

            if (_activePipeline != null)
            {
                _activePipeline.UnregisterSoundCache(sound);
                return;
            }

            FallbackSoundCache.Remove(sound);
        }

        public static IReadOnlyList<ISoundCacheItem> GetRegisteredSoundCaches()
        {
            if (_activePipeline != null)
                return _activePipeline.GetRegisteredSoundCaches();

            return FallbackSoundCache;
        }

        public static void MemoryClear()
        {
            if (_activePipeline != null)
            {
                _activePipeline.MemoryClear();
                return;
            }

            DateTime now = CEnvir.Now;

            for (int i = FallbackControlCache.Count - 1; i >= 0; i--)
            {
                ITextureCacheItem control = FallbackControlCache[i];
                if (now < control.ExpireTime)
                    continue;

                control.DisposeTexture();
            }

            for (int i = FallbackTextureCache.Count - 1; i >= 0; i--)
            {
                ITextureCacheItem texture = FallbackTextureCache[i];
                if (now < texture.ExpireTime)
                    continue;

                texture.DisposeTexture();
            }

            for (int i = FallbackSoundCache.Count - 1; i >= 0; i--)
            {
                ISoundCacheItem sound = FallbackSoundCache[i];
                if (now < sound.ExpireTime)
                    continue;

                sound.DisposeSoundBuffer();
            }
        }

        private static void ConfigureFallbackGraphics(Graphics graphics)
        {
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            graphics.TextContrast = 0;
        }

        private static Color ConvertHslToRgbFallback(float h, float s, float l)
        {
            float r, g, b;

            if (s == 0)
            {
                r = g = b = l;
            }
            else
            {
                float q = l < 0.5f ? l * (1 + s) : l + s - l * s;
                float p = 2 * l - q;
                r = HueToRgb(p, q, h + 1f / 3f);
                g = HueToRgb(p, q, h);
                b = HueToRgb(p, q, h - 1f / 3f);
            }

            return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }

        private static float HueToRgb(float p, float q, float t)
        {
            if (t < 0f) t += 1f;
            if (t > 1f) t -= 1f;
            if (t < 1f / 6f) return p + (q - p) * 6f * t;
            if (t < 1f / 2f) return q;
            if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
            return p;
        }

        internal readonly struct OutlineEffectSettings
        {
            public Color Colour { get; }
            public float Thickness { get; }

            public OutlineEffectSettings(Color colour, float thickness)
            {
                Colour = colour;
                Thickness = thickness;
            }
        }

        internal readonly struct SpriteShaderEffectRequest
        {
            public SpriteShaderEffectKind Kind { get; }
            public OutlineEffectSettings Outline { get; }

            public SpriteShaderEffectRequest(OutlineEffectSettings outline)
            {
                Kind = SpriteShaderEffectKind.Outline;
                Outline = outline;
            }

            public SpriteShaderEffectRequest(SpriteShaderEffectKind kind)
            {
                Kind = kind;
                Outline = default;
            }
        }

        internal enum SpriteShaderEffectKind
        {
            Outline,
            Grayscale
        }
    }
}
