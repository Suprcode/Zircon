using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Shared.Rendering
{
    public static class RenderingPipelineManager
    {
        private const string DefaultPipelineId = RenderingPipelineIds.SilkDXD3D11;
        private static readonly Dictionary<string, Func<IRenderingPipeline>> PipelineFactories = new(StringComparer.OrdinalIgnoreCase)
        {
            { RenderingPipelineIds.SilkDXD3D11, () => new SilkD3D11.SilkD3D11RenderingPipeline() },
            { RenderingPipelineIds.SilkVulkan, () => new SilkVulkan.SilkVulkanRenderingPipeline() }
        };

        private static IRenderingPipeline _activePipeline;
        private static RenderingPipelineContext _context;
        private static PipelineSession _activeSession;
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
        private static readonly Dictionary<PipelineSession, RenderTexture> SolidFillTextures = new();
        private static readonly object GraphicsLock = new();
        private static string _pendingPipelineId;
        internal static bool DrawingDpiText { get; private set; }
        private static bool DrawingBorderBackground { get; set; }
        private static PointF DpiTextOrigin { get; set; }
        private static bool DpiTextRightAligned { get; set; }
        private static int _uiScaleDepth;
        private static float _uiScale = 1F;
        private static object _uiScaleSurface;
        private static PointF _uiScaleOrigin;

        public static float CurrentUIScale => IsUIScaleActive ? _uiScale : 1F;
        public static bool UsesFractionalUIScale => IsUIScaleActive && Math.Abs(_uiScale - MathF.Round(_uiScale)) > 0.001F;
        public static bool IsUIScaleActive => _uiScaleDepth > 0 && _uiScale > 1F &&
                                              ReferenceEquals(_activePipeline?.GetCurrentSurface().NativeHandle, _uiScaleSurface);

        public static void PushUIScale(float scale, PointF origin = default)
        {
            if (_uiScaleDepth++ == 0)
            {
                _uiScale = float.IsFinite(scale) ? Math.Max(1F, scale) : 1F;
                _uiScaleSurface = _activePipeline?.GetCurrentSurface().NativeHandle;
                _uiScaleOrigin = origin;
            }
        }

        public static void SetUIScaleOrigin(PointF origin)
        {
            if (_uiScaleDepth <= 0)
                throw new InvalidOperationException("No UI scale scope is active.");

            _uiScaleOrigin = origin;
        }

        public static void PopUIScale()
        {
            if (_uiScaleDepth <= 0)
                throw new InvalidOperationException("No UI scale scope is active.");

            if (--_uiScaleDepth == 0)
            {
                _uiScale = 1F;
                _uiScaleSurface = null;
                _uiScaleOrigin = PointF.Empty;
            }
        }

        private static PointF ScaleUIPoint(PointF point)
        {
            float scale = CurrentUIScale;
            return scale == 1F
                ? point
                : new PointF(
                    _uiScaleOrigin.X + (point.X - _uiScaleOrigin.X) * scale,
                    _uiScaleOrigin.Y + (point.Y - _uiScaleOrigin.Y) * scale);
        }

        private static RectangleF ScaleUIRectangle(RectangleF rectangle)
        {
            float scale = CurrentUIScale;
            return scale == 1F
                ? rectangle
                : new RectangleF(
                    _uiScaleOrigin.X + (rectangle.X - _uiScaleOrigin.X) * scale,
                    _uiScaleOrigin.Y + (rectangle.Y - _uiScaleOrigin.Y) * scale,
                    rectangle.Width * scale, rectangle.Height * scale);
        }

        private static Matrix3x2 ScaleUITransform(Matrix3x2 transform, Vector3 center, Vector3 translation)
        {
            if (center != Vector3.Zero)
                transform = Matrix3x2.CreateTranslation(-center.X, -center.Y) * transform;

            transform.M31 += translation.X;
            transform.M32 += translation.Y;

            float scale = CurrentUIScale;
            if (scale == 1F)
                return transform;

            Matrix3x2 uiTransform =
                Matrix3x2.CreateTranslation(-_uiScaleOrigin.X, -_uiScaleOrigin.Y) *
                Matrix3x2.CreateScale(scale) *
                Matrix3x2.CreateTranslation(_uiScaleOrigin.X, _uiScaleOrigin.Y);

            return transform * uiTransform;
        }

        public static void DrawDpiText(RenderTexture texture, Rectangle source, RectangleF destination, PointF origin,
            bool rightAligned, Color colour)
        {
            bool previous = DrawingDpiText;
            PointF previousOrigin = DpiTextOrigin;
            bool previousRightAligned = DpiTextRightAligned;
            DrawingDpiText = true;
            DpiTextOrigin = ScaleUIPoint(origin);
            DpiTextRightAligned = rightAligned;
            try
            {
                DrawTexture(texture, source, destination, colour);
            }
            finally
            {
                DrawingDpiText = previous;
                DpiTextOrigin = previousOrigin;
                DpiTextRightAligned = previousRightAligned;
            }
        }

        internal static RectangleF AlignTextDestination(RectangleF destination, Size physicalSize, Size sourceSize)
        {
            if (!DrawingDpiText) return destination;
            Size logical = Settings.ActiveSceneSize;
            if (logical.Width <= 0 || logical.Height <= 0) return destination;
            float coordinateScale = CurrentUIScale;
            float sx = physicalSize.Width / (float)logical.Width;
            float sy = physicalSize.Height / (float)logical.Height;
            float originX = MathF.Round(DpiTextOrigin.X * sx) / sx;
            float originY = MathF.Round(DpiTextOrigin.Y * sy) / sy;
            float alignedY = originY + MathF.Round((destination.Top - DpiTextOrigin.Y) * sy) / sy;

            if (DpiTextRightAligned)
            {
                float alignedRight = MathF.Round(destination.Right * sx) / sx;
                return new RectangleF(alignedRight - sourceSize.Width / sx, alignedY,
                    sourceSize.Width / sx, sourceSize.Height / sy);
            }

            // Snap text runs relative to a shared parent origin. This keeps the raster
            // spacing between separately coloured lines stable while their parent scrolls.
            RectangleF aligned = new RectangleF(
                originX + MathF.Round((destination.Left - DpiTextOrigin.X) * sx) / sx,
                alignedY,
                sourceSize.Width / sx,
                sourceSize.Height / sy);

            if (coordinateScale == 1F && (aligned.Width > destination.Width || aligned.Height > destination.Height))
            {
                aligned.X -= (aligned.Width - destination.Width) / 2F;
                aligned.Y -= (aligned.Height - destination.Height) / 2F;
            }

            return aligned;
        }
        internal static RenderingHostSettings HostSettings => Settings;
        internal static Control RenderTarget => _context?.RenderTarget;
        private static RenderingHostSettings Settings => _context?.Settings ?? DefaultSettings;
        private static readonly RenderingHostSettings DefaultSettings = new();

        static RenderingPipelineManager()
        {
            FallbackGraphics = Graphics.FromHwnd(IntPtr.Zero);
            ConfigureFallbackGraphics(FallbackGraphics);
        }

        public sealed class PipelineSession : IDisposable
        {
            internal PipelineSession(IRenderingPipeline pipeline, RenderingPipelineContext context)
            {
                Pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
                Context = context ?? throw new ArgumentNullException(nameof(context));
            }

            internal IRenderingPipeline Pipeline { get; }
            internal RenderingPipelineContext Context { get; }
            public string PipelineId => Pipeline.Id;
            public bool IsDisposed { get; private set; }

            public IDisposable Activate()
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(PipelineSession));

                return RenderingPipelineManager.Activate(this);
            }

            public void Dispose()
            {
                if (IsDisposed)
                    return;

                IsDisposed = true;
                RenderingPipelineManager.DestroySession(this);
            }
        }

        private sealed class PipelineActivation : IDisposable
        {
            private readonly PipelineSession _previousSession;
            private readonly IRenderingPipeline _previousPipeline;
            private readonly RenderingPipelineContext _previousContext;
            private bool _disposed;

            public PipelineActivation(PipelineSession session)
            {
                _previousSession = _activeSession;
                _previousPipeline = _activePipeline;
                _previousContext = _context;
                SetActiveSession(session);
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;
                _activeSession = _previousSession;
                _activePipeline = _previousPipeline;
                _context = _previousContext;
            }
        }

        public static string DefaultPipelineIdentifier => DefaultPipelineId;
        public static string ActivePipelineId => _activePipeline?.Id;
        public static bool SupportsCachedRenderTargets => _activePipeline?.SupportsCachedRenderTargets ?? false;
        public static bool SupportsAtlasTextures => _activePipeline?.SupportsAtlasTextures ?? false;
        public static bool SupportsBc7Textures => _activePipeline?.SupportsBc7Textures ?? false;
        public static IReadOnlyCollection<string> AvailablePipelineIds => PipelineFactories.Keys;
        public static bool SupportsMultiplePipelines => PipelineFactories.Count > 1;
        public static bool IsDefaultPipelineOnly => PipelineFactories.Count == 1 && PipelineFactories.ContainsKey(DefaultPipelineId);
        public static IReadOnlyList<DisplayMonitorInfo> GetDisplayMonitors()
        {
            Screen[] screens = Screen.AllScreens;
            List<DisplayMonitorInfo> monitors = new(screens.Length);

            for (int i = 0; i < screens.Length; i++)
            {
                Screen screen = screens[i];
                monitors.Add(new DisplayMonitorInfo(i, screen.DeviceName, screen.Primary, screen.Bounds));
            }

            return monitors;
        }

        public static int GetSelectedMonitorIndex()
        {
            IReadOnlyList<DisplayMonitorInfo> monitors = GetDisplayMonitors();

            if (monitors.Count == 0)
                return 0;

            if (!string.IsNullOrWhiteSpace(Settings.DefaultMonitor))
            {
                DisplayMonitorInfo configuredMonitor = monitors.FirstOrDefault(x => string.Equals(x.DeviceName, Settings.DefaultMonitor, StringComparison.OrdinalIgnoreCase));

                if (configuredMonitor != null)
                    return configuredMonitor.Index;
            }

            DisplayMonitorInfo primaryMonitor = monitors.FirstOrDefault(x => x.Primary) ?? monitors[0];
            Settings.DefaultMonitor = primaryMonitor.DeviceName;
            return primaryMonitor.Index;
        }

        public static DisplayMonitorInfo GetSelectedMonitor()
        {
            IReadOnlyList<DisplayMonitorInfo> monitors = GetDisplayMonitors();

            if (monitors.Count == 0)
                return null;

            int index = GetSelectedMonitorIndex();

            if (index < 0 || index >= monitors.Count)
                index = 0;

            return monitors[index];
        }

        public static Screen GetSelectedScreen()
        {
            Screen[] screens = Screen.AllScreens;

            if (screens.Length == 0)
                return Screen.PrimaryScreen;

            int index = GetSelectedMonitorIndex();

            if (index < 0 || index >= screens.Length)
                index = 0;

            return screens[index];
        }

        public static Rectangle GetSelectedMonitorDisplayBounds()
        {
            return GetMonitorDisplayBounds(GetSelectedScreen());
        }

        public static Rectangle GetMonitorDisplayBounds(Screen screen)
        {
            return DisplayModeManager.GetBounds(screen);
        }

        public static Rectangle GetMonitorDisplayBounds(string deviceName, Rectangle fallbackBounds)
        {
            return DisplayModeManager.GetBounds(deviceName, fallbackBounds);
        }

        public static void SelectMonitor(int monitorIndex)
        {
            IReadOnlyList<DisplayMonitorInfo> monitors = GetDisplayMonitors();

            if (monitors.Count == 0)
                return;

            if (monitorIndex < 0 || monitorIndex >= monitors.Count)
                monitorIndex = monitors.FirstOrDefault(x => x.Primary)?.Index ?? 0;

            Settings.DefaultMonitor = monitors[monitorIndex].DeviceName;
            SetTargetMonitor(monitorIndex);
        }

        public static IReadOnlyList<GraphicsAdapterInfo> GetGraphicsAdapters(string pipelineId)
        {
            if (string.Equals(pipelineId, RenderingPipelineIds.SilkVulkan, StringComparison.OrdinalIgnoreCase))
                return SilkVulkan.SilkVulkanRenderingPipeline.GetAvailableGraphicsAdapters();

            return Array.Empty<GraphicsAdapterInfo>();
        }

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
            CreateSession(pipelineId, context);
        }

        public static PipelineSession CreateSession(string pipelineId, RenderingPipelineContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!PipelineFactories.TryGetValue(pipelineId, out Func<IRenderingPipeline> factory))
                throw new ArgumentException($"Unknown rendering pipeline '{pipelineId}'.", nameof(pipelineId));

            IRenderingPipeline pipeline = factory();
            PipelineSession session = new(pipeline, context);
            PipelineSession previousSession = _activeSession;
            IRenderingPipeline previousPipeline = _activePipeline;
            RenderingPipelineContext previousContext = _context;
            try
            {
                SetActiveSession(session);
                pipeline.Initialize(context);
                return session;
            }
            catch
            {
                if (ReferenceEquals(_activeSession, session))
                {
                    _activeSession = previousSession;
                    _activePipeline = previousPipeline;
                    _context = previousContext;
                }

                pipeline.Shutdown();
                throw;
            }
        }

        public static string InitializeWithFallback(string requestedPipelineId, RenderingPipelineContext context)
        {
            return CreateSessionWithFallback(requestedPipelineId, context).PipelineId;
        }

        public static PipelineSession CreateSessionWithFallback(string requestedPipelineId, RenderingPipelineContext context)
        {
            string pipelineToUse = string.IsNullOrWhiteSpace(requestedPipelineId) ? DefaultPipelineId : requestedPipelineId;

            try
            {
                return CreateSession(pipelineToUse, context);
            }
            catch (Exception ex)
            {
                if (pipelineToUse.Equals(DefaultPipelineId, StringComparison.OrdinalIgnoreCase) || !PipelineFactories.ContainsKey(DefaultPipelineId))
                    throw;

                PipelineSession session = CreateSession(DefaultPipelineId, context);
                Console.WriteLine($"Falling back to rendering pipeline '{DefaultPipelineId}' after '{pipelineToUse}' failed: {ex.Message}");
                return session;
            }
        }

        public static IDisposable Activate(PipelineSession session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            return new PipelineActivation(session);
        }

        private static void SetActiveSession(PipelineSession session)
        {
            _activeSession = session;
            _activePipeline = session?.Pipeline;
            _context = session?.Context;
        }

        public static void SwitchPipeline(string pipelineId)
        {
            pipelineId = NormalizePipelineId(pipelineId);

            if (string.Equals(ActivePipelineId, pipelineId, StringComparison.OrdinalIgnoreCase)) return;

            RenderingPipelineContext context = _context ?? throw new InvalidOperationException("No rendering pipeline context is available.");

            InvalidateAllControlTextures();
            Shutdown();

            InitializeWithFallback(pipelineId, context);
        }

        public static void RequestSwitchPipeline(string pipelineId)
        {
            string normalizedPipelineId = NormalizePipelineId(pipelineId);
            _pendingPipelineId = string.Equals(ActivePipelineId, normalizedPipelineId, StringComparison.OrdinalIgnoreCase)
                ? null
                : normalizedPipelineId;
        }

        public static bool ApplyPendingPipelineSwitch()
        {
            if (string.IsNullOrWhiteSpace(_pendingPipelineId))
                return false;

            string pipelineId = _pendingPipelineId;
            _pendingPipelineId = null;

            if (string.Equals(ActivePipelineId, pipelineId, StringComparison.OrdinalIgnoreCase))
                return false;

            string previousPipelineId = ActivePipelineId;
            RenderingPipelineContext context = _context ?? throw new InvalidOperationException("No rendering pipeline context is available.");
            RenderingHostSettings settings = context.Settings;

            try
            {
                InvalidateAllControlTextures();
                Shutdown();

                string activePipelineId = InitializeWithFallback(pipelineId, context);
                settings.RenderingPipeline = activePipelineId;
            }
            catch (Exception ex)
            {
                settings.ReportException(ex);

                if (string.IsNullOrWhiteSpace(previousPipelineId))
                    throw;

                string restoredPipelineId = InitializeWithFallback(previousPipelineId, context);
                settings.RenderingPipeline = restoredPipelineId;
            }

            return true;
        }

        private static void InvalidateAllControlTextures()
        {
            _activePipeline?.InvalidateTextureCaches();
            Settings.InvalidateRenderCaches?.Invoke();
        }

        public static void Shutdown()
        {
            if (_activeSession == null)
                return;

            _activeSession.Dispose();
        }

        private static void DestroySession(PipelineSession session)
        {
            if (session == null)
                return;

            if (SolidFillTextures.TryGetValue(session, out RenderTexture solidFillTexture) && solidFillTexture.IsValid)
            {
                session.Pipeline.ReleaseTexture(solidFillTexture);
                SolidFillTextures.Remove(session);
            }

            session.Pipeline.Shutdown();

            if (ReferenceEquals(_activeSession, session))
                SetActiveSession(null);
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

            IReadOnlyList<DisplayMonitorInfo> monitors = GetDisplayMonitors();
            if (monitorIndex >= 0 && monitorIndex < monitors.Count)
                Settings.DefaultMonitor = monitors[monitorIndex].DeviceName;

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
            using Font pixelFont = CreatePixelFont(font);
            if (_activePipeline != null)
                return _activePipeline.MeasureText(text, pixelFont);

            lock (GraphicsLock)
            {
                return TextRenderer.MeasureText(FallbackGraphics, text, pixelFont);
            }
        }

        public static Size MeasureText(string text, Font font, Size proposedSize, TextFormatFlags format)
        {
            using Font pixelFont = CreatePixelFont(font);
            if (_activePipeline != null)
                return _activePipeline.MeasureText(text, pixelFont, proposedSize, format);

            lock (GraphicsLock)
            {
                return TextRenderer.MeasureText(FallbackGraphics, text, pixelFont, proposedSize, format);
            }
        }

        // GDI TextRenderer uses the screen DC's DPI for point fonts, not Bitmap.SetResolution.
        // Keep layout at 96 DPI and scale the raster font explicitly for the destination texture.
        public static Font CreatePixelFont(Font font, float scale = 1F) =>
            new Font(font.FontFamily, (font.Unit == GraphicsUnit.Pixel ? font.Size : font.SizeInPoints * 96F / 72F) * scale,
                font.Style, GraphicsUnit.Pixel, font.GdiCharSet, font.GdiVerticalFont);

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

        public static void EnableSolidShadowFillEffect(float opacity)
        {
            _spriteShaderEffect = new SpriteShaderEffectRequest(SpriteShaderEffectKind.SolidShadowFill, Math.Clamp(opacity, 0F, 1F));
        }

        public static void EnableDropShadowEffect(Color colour, float width, float startOpacity, RectangleF? visibleBounds = null)
        {
            if (IsUIScaleActive)
            {
                width *= CurrentUIScale;
                if (visibleBounds.HasValue)
                    visibleBounds = ScaleUIRectangle(visibleBounds.Value);
            }
            _spriteShaderEffect = new SpriteShaderEffectRequest(new DropShadowEffectSettings(colour, width, startOpacity, visibleBounds));
        }

        public static void EnableColourGradeEffect(float exposure, float contrast, float saturation, Color tint, float tintStrength)
        {
            _spriteShaderEffect = new SpriteShaderEffectRequest(new ColourGradeEffectSettings(exposure, contrast, saturation, tint, tintStrength));
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
                if (IsUIScaleActive)
                {
                    LinePoint[] scaledPoints = new LinePoint[points.Count];
                    for (int i = 0; i < points.Count; i++)
                    {
                        PointF point = ScaleUIPoint(new PointF(points[i].X, points[i].Y));
                        scaledPoints[i] = new LinePoint(point.X, point.Y);
                    }
                    points = scaledPoints;
                }
                _activePipeline.DrawLine(points, colour);
            }
        }

        public static void FlushLines()
        {
            _activePipeline?.FlushLines();
        }

        public static void DrawTextureBlend(RenderTexture texture, Rectangle? sourceRectangle, Matrix3x2 transform, Vector3 center, Vector3 translation, Color colour, float blendRate, BlendMode mode = BlendMode.NORMAL)
        {
            if (!texture.IsValid)
                throw new ArgumentException("A valid texture handle is required.", nameof(texture));

            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            if (IsUIScaleActive)
            {
                transform = ScaleUITransform(transform, center, translation);
                center = Vector3.Zero;
                translation = Vector3.Zero;
            }

            if (_activePipeline is SilkVulkan.SilkVulkanRenderingPipeline vulkanPipeline)
            {
                vulkanPipeline.DrawTextureBlend(texture, sourceRectangle, transform, center, translation, colour, blendRate, mode);
                return;
            }

            bool oldBlend = _activePipeline.IsBlending();
            float oldRate = _activePipeline.GetBlendRate();
            BlendMode oldMode = _activePipeline.GetBlendMode();

            try
            {
                _activePipeline.SetBlend(true, blendRate, mode);
                _activePipeline.DrawTexture(texture, sourceRectangle, transform, center, translation, colour);
            }
            finally
            {
                _activePipeline.SetBlend(oldBlend, oldRate, oldMode);
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

            _activePipeline.DrawTexture(texture, sourceRectangle, ScaleUIRectangle(destinationRectangle), colour);
        }

        public static void DrawTexture(RenderTexture texture, Rectangle? sourceRectangle, Matrix3x2 transform, Vector3 center, Vector3 translation, Color colour)
        {
            if (!texture.IsValid)
                throw new ArgumentException("A valid texture handle is required.", nameof(texture));

            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            if (IsUIScaleActive)
            {
                transform = ScaleUITransform(transform, center, translation);
                center = Vector3.Zero;
                translation = Vector3.Zero;
            }

            _activePipeline.DrawTexture(texture, sourceRectangle, transform, center, translation, colour);
        }

        public static void BeginSpriteBatch()
        {
            _activePipeline?.BeginSpriteBatch();
        }

        public static void QueueSprite(RenderTexture texture, Rectangle sourceRectangle, RectangleF destinationRectangle, Color colour)
        {
            if (!texture.IsValid)
                throw new ArgumentException("A valid texture handle is required.", nameof(texture));

            if (_activePipeline == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            _activePipeline.QueueSprite(texture, sourceRectangle, ScaleUIRectangle(destinationRectangle), colour);
        }

        public static void EndSpriteBatch()
        {
            _activePipeline?.EndSpriteBatch();
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

        public static void FillRectangle(Rectangle rectangle, Color colour, bool alignToBorder = false)
        {
            if (rectangle.Width <= 0 || rectangle.Height <= 0 || colour.A == 0)
                return;

            RenderTexture texture = GetSolidFillTexture();
            bool previous = DrawingBorderBackground;
            DrawingBorderBackground = alignToBorder;
            try
            {
                DrawTexture(texture, new Rectangle(0, 0, 1, 1), new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height), colour);
            }
            finally
            {
                DrawingBorderBackground = previous;
            }
        }

        internal static RectangleF AlignBorderBackground(RectangleF destination, Size physicalSize)
        {
            if (!DrawingBorderBackground) return destination;
            Size logical = Settings.ActiveSceneSize;
            if (logical.Width <= 0 || logical.Height <= 0) return destination;
            float sx = physicalSize.Width / (float)logical.Width;
            float sy = physicalSize.Height / (float)logical.Height;

            // Borders snap their centres to floor(edge) + 0.5 framebuffer pixels.
            // The bottom/right strokes are one pixel inside the exclusive bounds,
            // so flooring all four fill edges covers exactly the bordered rectangle.
            return RectangleF.FromLTRB(
                MathF.Floor(destination.Left * sx) / sx,
                MathF.Floor(destination.Top * sy) / sy,
                MathF.Floor(destination.Right * sx) / sx,
                MathF.Floor(destination.Bottom * sy) / sy);
        }

        private static RenderTexture GetSolidFillTexture()
        {
            if (_activeSession == null)
                throw new InvalidOperationException("No rendering pipeline has been initialized.");

            if (SolidFillTextures.TryGetValue(_activeSession, out RenderTexture solidFillTexture) && solidFillTexture.IsValid)
                return solidFillTexture;

            solidFillTexture = CreateTexture(new Size(1, 1), RenderTextureFormat.A8R8G8B8, RenderTextureUsage.None, RenderTexturePool.Managed);

            byte[] whitePixel = { 255, 255, 255, 255 };
            using (TextureLock textureLock = LockTexture(solidFillTexture, TextureLockMode.Discard))
            {
                Marshal.Copy(whitePixel, 0, textureLock.DataPointer, whitePixel.Length);
            }

            SolidFillTextures[_activeSession] = solidFillTexture;
            return solidFillTexture;
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

        public static SizeF GetBackBufferPixelSize()
        {
            Size logicalSize = Settings.ActiveSceneSize;
            Size physicalSize = GetBackBufferSize();
            float uiScale = CurrentUIScale;

            return new SizeF(
                Math.Max(1, logicalSize.Width) / (float)Math.Max(1, physicalSize.Width) / uiScale,
                Math.Max(1, logicalSize.Height) / (float)Math.Max(1, physicalSize.Height) / uiScale);
        }

        public static RectangleF GetPixelAlignedBorderBounds(Rectangle rectangle)
        {
            SizeF pixel = GetBackBufferPixelSize();
            RectangleF scaled = ScaleUIRectangle(rectangle);
            Size physical = GetBackBufferSize();
            Size logical = Settings.ActiveSceneSize;
            float sx = physical.Width / (float)Math.Max(1, logical.Width);
            float sy = physical.Height / (float)Math.Max(1, logical.Height);
            // Return stroke centres, not pixel edges. Re-projecting a centre cannot
            // round down into the preceding pixel when the renderer snaps the line.
            return RectangleF.FromLTRB(
                rectangle.Left + (MathF.Floor(scaled.Left * sx) + 0.5F - scaled.Left * sx) * pixel.Width,
                rectangle.Top + (MathF.Floor(scaled.Top * sy) + 0.5F - scaled.Top * sy) * pixel.Height,
                rectangle.Right + (MathF.Floor(scaled.Right * sx) - 0.5F - scaled.Right * sx) * pixel.Width,
                rectangle.Bottom + (MathF.Floor(scaled.Bottom * sy) - 0.5F - scaled.Bottom * sy) * pixel.Height);
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
            {
                return _activePipeline.LockTexture(texture, mode);
            }

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

            DateTime now = Settings.CurrentTime;

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

        internal readonly struct DropShadowEffectSettings
        {
            public Color Colour { get; }
            public float Width { get; }
            public float StartOpacity { get; }
            public RectangleF? VisibleBounds { get; }

            public DropShadowEffectSettings(Color colour, float width, float startOpacity, RectangleF? visibleBounds)
            {
                Colour = colour;
                Width = width;
                StartOpacity = startOpacity;
                VisibleBounds = visibleBounds;
            }
        }

        internal readonly struct ColourGradeEffectSettings
        {
            public float Exposure { get; }
            public float Contrast { get; }
            public float Saturation { get; }
            public Color Tint { get; }
            public float TintStrength { get; }

            public ColourGradeEffectSettings(float exposure, float contrast, float saturation, Color tint, float tintStrength)
            {
                Exposure = Math.Clamp(exposure, -2F, 2F);
                Contrast = Math.Clamp(contrast, 0F, 2F);
                Saturation = Math.Clamp(saturation, 0F, 2F);
                Tint = tint;
                TintStrength = Math.Clamp(tintStrength, 0F, 1F);
            }
        }

        internal readonly struct SpriteShaderEffectRequest
        {
            public SpriteShaderEffectKind Kind { get; }
            public float Amount { get; }
            public OutlineEffectSettings Outline { get; }
            public DropShadowEffectSettings DropShadow { get; }
            public ColourGradeEffectSettings ColourGrade { get; }

            public SpriteShaderEffectRequest(OutlineEffectSettings outline)
            {
                Kind = SpriteShaderEffectKind.Outline;
                Amount = 0F;
                Outline = outline;
                DropShadow = default;
                ColourGrade = default;
            }

            public SpriteShaderEffectRequest(SpriteShaderEffectKind kind, float amount = 0F)
            {
                Kind = kind;
                Amount = amount;
                Outline = default;
                DropShadow = default;
                ColourGrade = default;
            }

            public SpriteShaderEffectRequest(DropShadowEffectSettings dropShadow)
            {
                Kind = SpriteShaderEffectKind.DropShadow;
                Amount = 0F;
                Outline = default;
                DropShadow = dropShadow;
                ColourGrade = default;
            }

            public SpriteShaderEffectRequest(ColourGradeEffectSettings colourGrade)
            {
                Kind = SpriteShaderEffectKind.ColourGrade;
                Amount = 0F;
                Outline = default;
                DropShadow = default;
                ColourGrade = colourGrade;
            }
        }

        internal enum SpriteShaderEffectKind
        {
            Outline,
            Grayscale,
            DropShadow,
            SolidShadowFill,
            ColourGrade
        }
    }
}
