using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using D3DBlend = Silk.NET.Direct3D11.Blend;
using D3DMap = Silk.NET.Direct3D11.Map;
using GdiColor = System.Drawing.Color;
using WinFormsMessage = System.Windows.Forms.Message;

namespace Shared.Rendering.SilkD3D11
{
    public sealed unsafe class SilkD3D11RenderingPipeline : IRenderingPipeline
    {
        private const int LightWidth = 1024;
        private const int LightHeight = 768;
        private const int PoisonSize = 6;
        private const int MaxSprites = 4096;
        private const int MaxVertices = MaxSprites * 6;
        private const uint DxgiSwapChainFlagAllowModeSwitch = 2;
        private const uint DxgiMwaNoAltEnter = 2;
        private const string SpriteShaderFileName = "SpriteD3D11.hlsl";
        private const string OutlineShaderFileName = "OutlineD3D11.hlsl";
        private const string GrayscaleShaderFileName = "GrayscaleD3D11.hlsl";
        private const string DropShadowShaderFileName = "DropShadowD3D11.hlsl";
        private static readonly Size MinimumResolution = new(1024, 768);

        [ComImport]
        [Guid("8BA5FB08-5195-40e2-AC58-0D989C3A0102")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ID3DBlob
        {
            [PreserveSig]
            IntPtr GetBufferPointer();

            [PreserveSig]
            UIntPtr GetBufferSize();
        }

        [DllImport("d3dcompiler_47.dll", EntryPoint = "D3DCompile", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        private static extern int D3DCompile(
            IntPtr pSrcData,
            UIntPtr srcDataSize,
            string pSourceName,
            IntPtr pDefines,
            IntPtr pInclude,
            string pEntrypoint,
            string pTarget,
            uint flags1,
            uint flags2,
            out ID3DBlob code,
            out ID3DBlob errorMsgs);

        private readonly List<WeakReference<ITextureCacheItem>> _controlCache = new();
        private readonly List<ITextureCacheItem> _textureCache = new();
        private readonly List<ISoundCacheItem> _soundCache = new();
        private readonly List<SilkD3D11TextureResource> _textures = new();
        private readonly List<SilkD3D11RenderTarget> _renderTargets = new();
        private readonly List<SpriteBatchItem> _spriteBatch = new(MaxSprites);
        private readonly List<LineBatchItem> _lineBatch = new(1024);
        private readonly Dictionary<BlendMode, ComPtr<ID3D11BlendState>> _blendStates = new();

        private RenderingPipelineContext _context;
        private Graphics _graphics;
        private D3D11 _d3d;
        private DXGI _dxgi;
        private ComPtr<ID3D11Device> _device;
        private ComPtr<ID3D11DeviceContext> _deviceContext;
        private ComPtr<IDXGIFactory1> _factory;
        private ComPtr<IDXGISwapChain> _swapChain;
        private SilkD3D11RenderTarget _backBufferTarget;
        private SilkD3D11RenderTarget _scratchTarget;
        private SilkD3D11RenderTarget _currentTarget;
        private SilkD3D11TextureResource _colourPalette;
        private SilkD3D11TextureResource _lightTexture;
        private SilkD3D11TextureResource _poisonTexture;
        private SilkD3D11TextureResource _solidWhiteTexture;
        private byte[] _paletteData;
        private byte[] _lightData;
        private Size _backBufferSize;
        private float _opacity = 1F;
        private bool _blending;
        private float _blendRate = 1F;
        private BlendMode _blendMode = BlendMode.NORMAL;
        private float _lineWidth = 1F;
        private TextureFilterMode _textureFilter = TextureFilterMode.Point;
        private bool _resetRequested;

        private ComPtr<ID3D11VertexShader> _vertexShader;
        private ComPtr<ID3D11PixelShader> _pixelShader;
        private ComPtr<ID3D11PixelShader> _outlinePixelShader;
        private ComPtr<ID3D11PixelShader> _grayscalePixelShader;
        private ComPtr<ID3D11PixelShader> _dropShadowPixelShader;
        private ComPtr<ID3D11InputLayout> _inputLayout;
        private ComPtr<ID3D11Buffer> _vertexBuffer;
        private ComPtr<ID3D11Buffer> _matrixBuffer;
        private ComPtr<ID3D11Buffer> _effectBuffer;
        private ComPtr<ID3D11SamplerState> _pointSampler;
        private ComPtr<ID3D11SamplerState> _linearSampler;

        public string Id => RenderingPipelineIds.SilkDXD3D11;
        public bool SupportsAtlasTextures => true;
        public bool SupportsBc7Textures => true;

        public void Initialize(RenderingPipelineContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _graphics = Graphics.FromHwnd(IntPtr.Zero);
            ConfigureGraphics(_graphics);

            ApplyWindowStyle();
            ApplyWindowBounds(true);

#pragma warning disable CS0618
            _d3d = D3D11.GetApi();
            _dxgi = DXGI.GetApi();
#pragma warning restore CS0618
            CreateDevice();
            Check(_factory.MakeWindowAssociation(_context.RenderTarget.Handle, DxgiMwaNoAltEnter), "configure D3D11 window association");
            CreateSwapChain();
            CreateTargets();
            CreateShadersAndState();

            LoadTextures();
        }

        public void RunMessageLoop(Form form, Action loop)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));
            if (loop == null)
                throw new ArgumentNullException(nameof(loop));

            void Tick(object sender, EventArgs args)
            {
                while (AppStillIdle)
                    loop();
            }

            void ApplyStartupPlacement(object sender, EventArgs args)
            {
                form.Shown -= ApplyStartupPlacement;
                ApplyWindowStyle();
                ApplyWindowBounds(true);
                RequestReset();
            }

            Application.Idle += Tick;
            form.Shown += ApplyStartupPlacement;
            try
            {
                Application.Run(form);
            }
            finally
            {
                Application.Idle -= Tick;
                form.Shown -= ApplyStartupPlacement;
            }
        }

        public bool RenderFrame(Action drawScene)
        {
            if (drawScene == null)
                throw new ArgumentNullException(nameof(drawScene));

            try
            {
                AttemptReset();
                ResizeBackBufferIfNeeded();
                _currentTarget = _backBufferTarget;
                SetD3DRenderTarget(_currentTarget);
                Clear(RenderClearFlags.Target, GdiColor.Black, 0, 0);

                drawScene();

                EndSpriteBatch();
                FlushLines();
                Check(_swapChain.Present(RenderingPipelineManager.HostSettings.VSync ? 1U : 0U, 0), "present D3D11 swapchain");
                return true;
            }
            catch (Exception ex)
            {
                RenderingPipelineManager.HostSettings.ReportException(ex);
                return false;
            }
        }

        public void ToggleFullScreen()
        {
            RenderingPipelineManager.HostSettings.FullScreen = !RenderingPipelineManager.HostSettings.FullScreen;
            RenderingPipelineManager.HostSettings.NotifyFullScreenChanged();
            ApplyWindowStyle();
            ApplyWindowBounds(true);
            RequestReset();
        }

        public void SetResolution(Size size)
        {
            bool targetAlreadySized = _context.RenderTarget == null || _context.RenderTarget.ClientSize == size || RenderingPipelineManager.HostSettings.FullScreen;
            if (targetAlreadySized && _backBufferSize == size && RenderingPipelineManager.HostSettings.GameSize == size)
                return;

            RenderingPipelineManager.HostSettings.GameSize = size;
            if (!RenderingPipelineManager.HostSettings.FullScreen && _context.RenderTarget != null && _context.RenderTarget.ClientSize != size)
                _context.RenderTarget.ClientSize = size;

            ApplyWindowStyle();
            ApplyWindowBounds(false);
            RequestReset();
        }

        public void SetTargetMonitor(int monitorIndex)
        {
            ApplyWindowStyle();
            ApplyWindowBounds(true);
            RequestReset();
        }

        public void CenterOnSelectedMonitor() => CenterOnSelectedMonitor(false);

        public void ResetDevice()
        {
            ApplyWindowStyle();
            ApplyWindowBounds(true);
            RequestReset();
        }

        public void OnSceneChanged(bool isGameScene)
        {
        }

        public IReadOnlyList<Size> GetSupportedResolutions() =>
            DisplayModeManager.GetSupportedSizes(RenderingPipelineManager.GetSelectedScreen(), MinimumResolution, RenderingPipelineManager.HostSettings.GameSize);

        public Size MeasureText(string text, Font font) => TextRenderer.MeasureText(_graphics, text, font);

        public Size MeasureText(string text, Font font, Size proposedSize, TextFormatFlags format) =>
            TextRenderer.MeasureText(_graphics, text, font, proposedSize, format);

        public float GetHorizontalDpi() => _graphics.DpiX;

        public void ConfigureGraphics(Graphics graphics)
        {
            if (graphics == null)
                throw new ArgumentNullException(nameof(graphics));

            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.TextContrast = 0;
        }

        public GdiColor ConvertHslToRgb(float h, float s, float l)
        {
            float r, g, b;
            if (s == 0)
            {
                r = g = b = l;
            }
            else
            {
                float q = l < 0.5F ? l * (1 + s) : l + s - l * s;
                float p = 2 * l - q;
                r = HueToRgb(p, q, h + 1F / 3F);
                g = HueToRgb(p, q, h);
                b = HueToRgb(p, q, h - 1F / 3F);
            }

            return GdiColor.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }

        public void SetOpacity(float opacity) => _opacity = opacity;
        public float GetOpacity() => _opacity;
        public void SetBlend(bool enabled, float rate, BlendMode mode)
        {
            _blending = enabled;
            _blendRate = rate;
            _blendMode = mode;
        }

        public bool IsBlending() => _blending;
        public float GetBlendRate() => _blendRate;
        public BlendMode GetBlendMode() => _blendMode;
        public float GetLineWidth() => _lineWidth;
        public void SetLineWidth(float width)
        {
            if (width > 0)
                _lineWidth = width;
        }

        public void DrawLine(IReadOnlyList<LinePoint> points, GdiColor colour)
        {
            if (points == null || points.Count < 2)
                return;

            LinePoint[] copy = new LinePoint[points.Count];
            for (int i = 0; i < points.Count; i++)
                copy[i] = points[i];

            _lineBatch.Add(new LineBatchItem(copy, colour, _lineWidth, _opacity));
        }

        public void FlushLines()
        {
            if (_lineBatch.Count == 0)
                return;

            EndSpriteBatch();
            SetD3DRenderTarget(_currentTarget);

            foreach (LineBatchItem item in _lineBatch)
            {
                for (int i = 0; i < item.Points.Count - 1; i++)
                {
                    LinePoint a = item.Points[i];
                    LinePoint b = item.Points[i + 1];
                    DrawLineSegment(a, b, item.Colour, item.Width, item.Opacity);
                }
            }

            _lineBatch.Clear();
        }

        private void DrawLineSegment(LinePoint a, LinePoint b, GdiColor colour, float width, float opacity)
        {
            width = Math.Max(1F, width);
            float half = width * 0.5F;
            float x1 = Snap(a.X);
            float y1 = Snap(a.Y);
            float x2 = Snap(b.X);
            float y2 = Snap(b.Y);

            RectangleF rect;
            if (Math.Abs(x2 - x1) >= Math.Abs(y2 - y1))
            {
                rect = new RectangleF(Math.Min(x1, x2), y1 - half, Math.Max(width, Math.Abs(x2 - x1)), width);
            }
            else
            {
                rect = new RectangleF(x1 - half, Math.Min(y1, y2), width, Math.Max(width, Math.Abs(y2 - y1)));
            }

            DrawSolidRectangle(rect, colour, opacity);
        }

        public void DrawTexture(RenderTexture texture, Rectangle sourceRectangle, RectangleF destinationRectangle, GdiColor colour)
        {
            DrawTextureCore(texture, sourceRectangle, destinationRectangle, Matrix3x2.Identity, colour);
        }

        public void DrawTexture(RenderTexture texture, Rectangle? sourceRectangle, Matrix3x2 transform, Vector3 center, Vector3 translation, GdiColor colour)
        {
            if (!TryGetTexture(texture, out SilkD3D11TextureResource resource))
                return;

            Rectangle source = sourceRectangle ?? new Rectangle(0, 0, resource.Size.Width, resource.Size.Height);
            RectangleF destination = new(0, 0, source.Width, source.Height);
            Matrix3x2 finalTransform = transform;
            if (center != Vector3.Zero)
                finalTransform = Matrix3x2.CreateTranslation(-center.X, -center.Y) * finalTransform;

            finalTransform.M31 += translation.X;
            finalTransform.M32 += translation.Y;
            DrawTextureCore(texture, source, destination, finalTransform, colour);
        }

        public void BeginSpriteBatch()
        {
        }

        public void QueueSprite(RenderTexture texture, Rectangle sourceRectangle, RectangleF destinationRectangle, GdiColor colour)
        {
            FlushLinesIfNeeded();

            if (_spriteBatch.Count >= MaxSprites)
                EndSpriteBatch();

            if (TryGetTexture(texture, out SilkD3D11TextureResource resource))
                _spriteBatch.Add(CreateSpriteBatchItem(resource, sourceRectangle, destinationRectangle, Matrix3x2.Identity, colour, _opacity, null));
        }

        public void EndSpriteBatch()
        {
            if (_spriteBatch.Count == 0)
                return;

            SpriteBatchItem[] sprites = _spriteBatch.ToArray();
            _spriteBatch.Clear();

            SetD3DRenderTarget(_currentTarget);
            foreach (SpriteBatchItem item in sprites)
                DrawSprite(item);
        }

        public RenderSurface GetCurrentSurface() => RenderSurface.From(_currentTarget);

        public void SetSurface(RenderSurface surface)
        {
            if (surface.NativeHandle is not SilkD3D11RenderTarget target)
                throw new ArgumentException("Surface handle must wrap a Silk.NET D3D11 render target.", nameof(surface));

            if (ReferenceEquals(_currentTarget, target))
                return;

            EndSpriteBatch();
            FlushLines();
            _currentTarget = target;
            SetD3DRenderTarget(_currentTarget);
        }

        public RenderSurface GetScratchSurface()
        {
            EnsureScratchTargetSize();
            return RenderSurface.From(_scratchTarget);
        }

        public RenderTexture GetScratchTexture()
        {
            EnsureScratchTargetSize();
            return RenderTexture.From(_scratchTarget.Texture);
        }

        public void ColorFill(RenderSurface surface, Rectangle rectangle, GdiColor color)
        {
            if (surface.NativeHandle is not SilkD3D11RenderTarget target)
                throw new ArgumentException("Surface handle must wrap a Silk.NET D3D11 render target.", nameof(surface));

            if (rectangle.Width <= 0 || rectangle.Height <= 0)
                return;

            SilkD3D11RenderTarget oldTarget = _currentTarget;
            SetSurface(RenderSurface.From(target));
            DrawSolidRectangle(rectangle, color, _opacity);
            SetSurface(RenderSurface.From(oldTarget));
        }

        public RenderTargetResource CreateRenderTarget(Size size)
        {
            SilkD3D11RenderTarget target = CreateRenderTargetCore(size);
            _renderTargets.Add(target);
            return RenderTargetResource.From(RenderTexture.From(target.Texture), RenderSurface.From(target));
        }

        public void ReleaseRenderTarget(RenderTargetResource renderTarget)
        {
            if (renderTarget.Surface.NativeHandle is not SilkD3D11RenderTarget target || target.IsBackBuffer)
                return;

            EndSpriteBatch();
            FlushLines();
            _renderTargets.Remove(target);
            if (_currentTarget == target)
                _currentTarget = _backBufferTarget;
            target.Dispose();
        }

        public Size GetBackBufferSize() => _backBufferSize;

        public void Clear(RenderClearFlags flags, GdiColor colour, float z, int stencil, params Rectangle[] regions)
        {
            if ((flags & RenderClearFlags.Target) == 0 || _currentTarget == null)
                return;

            EndSpriteBatch();
            FlushLines();
            Vector4 color = ToPremultipliedVector(colour, _opacity);
            if (regions != null && regions.Length > 0)
            {
                foreach (Rectangle region in regions)
                    DrawSolidRectangle(region, colour, _opacity);
                return;
            }

            _deviceContext.ClearRenderTargetView(_currentTarget.RenderTargetView, (float*)&color);
        }

        public void FlushSprite() => EndSpriteBatch();

        public void RegisterControlCache(ITextureCacheItem control)
        {
            if (control == null)
                return;

            foreach (WeakReference<ITextureCacheItem> reference in _controlCache)
                if (reference.TryGetTarget(out ITextureCacheItem target) && target == control)
                    return;

            _controlCache.Add(new WeakReference<ITextureCacheItem>(control));
        }

        public void UnregisterControlCache(ITextureCacheItem control)
        {
            for (int i = _controlCache.Count - 1; i >= 0; i--)
                if (!_controlCache[i].TryGetTarget(out ITextureCacheItem target) || target == control)
                    _controlCache.RemoveAt(i);
        }

        public RenderTexture CreateTexture(Size size, RenderTextureFormat format, RenderTextureUsage usage, RenderTexturePool pool)
        {
            SilkD3D11TextureResource texture = CreateTextureCore(size, format, usage == RenderTextureUsage.RenderTarget);
            _textures.Add(texture);
            return RenderTexture.From(texture);
        }

        public void ReleaseTexture(RenderTexture texture)
        {
            if (texture.NativeHandle is not SilkD3D11TextureResource resource)
                return;

            EndSpriteBatch();
            FlushLines();
            _textures.Remove(resource);
            resource.Dispose();
        }

        public TextureLock LockTexture(RenderTexture texture, TextureLockMode mode)
        {
            if (texture.NativeHandle is not SilkD3D11TextureResource resource)
                throw new InvalidOperationException("Silk.NET D3D11 texture handle expected.");

            EndSpriteBatch();

            if (mode == TextureLockMode.ReadOnly)
                ReadTexture(resource);

            byte[] data = resource.Data;
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            return TextureLock.From(handle.AddrOfPinnedObject(), GetRowPitch(resource), () =>
            {
                try
                {
                    if (mode != TextureLockMode.ReadOnly)
                        UploadTexture(resource);
                }
                finally
                {
                    handle.Free();
                }
            });
        }

        public void RegisterTextureCache(ITextureCacheItem texture)
        {
            if (texture != null && !_textureCache.Contains(texture))
                _textureCache.Add(texture);
        }

        public void UnregisterTextureCache(ITextureCacheItem texture)
        {
            if (texture != null)
                _textureCache.Remove(texture);
        }

        public void RegisterSoundCache(ISoundCacheItem sound)
        {
            if (sound != null && !_soundCache.Contains(sound))
                _soundCache.Add(sound);
        }

        public void UnregisterSoundCache(ISoundCacheItem sound)
        {
            if (sound != null)
                _soundCache.Remove(sound);
        }

        public void MemoryClear()
        {
            DateTime now = RenderingPipelineManager.HostSettings.CurrentTime;
            for (int i = _controlCache.Count - 1; i >= 0; i--)
            {
                if (!_controlCache[i].TryGetTarget(out ITextureCacheItem control))
                {
                    _controlCache.RemoveAt(i);
                    continue;
                }

                if (now >= control.ExpireTime)
                    control.DisposeTexture();
            }

            for (int i = _textureCache.Count - 1; i >= 0; i--)
                if (now >= _textureCache[i].ExpireTime)
                    _textureCache[i].DisposeTexture();

            for (int i = _soundCache.Count - 1; i >= 0; i--)
                if (now >= _soundCache[i].ExpireTime)
                    _soundCache[i].DisposeSoundBuffer();
        }

        public IReadOnlyList<ISoundCacheItem> GetRegisteredSoundCaches() => _soundCache;
        public RenderTexture GetColourPaletteTexture() => RenderTexture.From(_colourPalette);
        public byte[] GetColourPaletteData() => _paletteData;
        public RenderTexture GetLightTexture() => RenderTexture.From(_lightTexture);
        public Size GetLightTextureSize() => new(LightWidth, LightHeight);
        public RenderTexture GetPoisonTexture() => RenderTexture.From(_poisonTexture);
        public Size GetPoisonTextureSize() => new(PoisonSize, PoisonSize);
        public TextureFilterMode GetTextureFilter() => _textureFilter;
        public void SetTextureFilter(TextureFilterMode mode) => _textureFilter = mode;

        public void Shutdown()
        {
            EndSpriteBatch();
            FlushLines();
            UnbindBackBufferReferences();

            foreach (SilkD3D11RenderTarget target in _renderTargets)
                target.Dispose();
            _renderTargets.Clear();

            foreach (SilkD3D11TextureResource texture in _textures)
                texture.Dispose();
            _textures.Clear();

            _solidWhiteTexture?.Dispose();
            _poisonTexture?.Dispose();
            _lightTexture?.Dispose();
            _colourPalette?.Dispose();
            _scratchTarget?.Dispose();
            _backBufferTarget?.Dispose();

            foreach (ComPtr<ID3D11BlendState> blendState in _blendStates.Values)
                blendState.Dispose();
            _blendStates.Clear();

            _pointSampler.Dispose();
            _linearSampler.Dispose();
            _effectBuffer.Dispose();
            _matrixBuffer.Dispose();
            _vertexBuffer.Dispose();
            _inputLayout.Dispose();
            _dropShadowPixelShader.Dispose();
            _grayscalePixelShader.Dispose();
            _outlinePixelShader.Dispose();
            _pixelShader.Dispose();
            _vertexShader.Dispose();
            _swapChain.Dispose();
            _factory.Dispose();
            _deviceContext.Dispose();
            _device.Dispose();
            _graphics?.Dispose();
        }

        private void DrawTextureCore(RenderTexture texture, Rectangle source, RectangleF destination, Matrix3x2 transform, GdiColor colour)
        {
            if (!TryGetTexture(texture, out SilkD3D11TextureResource resource) || source.Width <= 0 || source.Height <= 0 || destination.Width <= 0 || destination.Height <= 0 || colour.A == 0)
                return;

            FlushLinesIfNeeded();

            SpriteEffect? effect = null;
            RenderingPipelineManager.SpriteShaderEffectRequest? request = RenderingPipelineManager.GetSpriteShaderEffect();
            if (request.HasValue)
            {
                switch (request.Value.Kind)
                {
                    case RenderingPipelineManager.SpriteShaderEffectKind.Outline:
                        EndSpriteBatch();
                        DrawSprite(CreateSpriteBatchItem(resource, source, destination, transform, request.Value.Outline.Colour, 1F, new SpriteEffect(SpriteEffectMode.Outline, request.Value.Outline.Thickness, resource.Size, ToSourceUv(source, resource.Size), null, request.Value.Outline.Thickness, true), BlendMode.NONE, 1F));
                        break;
                    case RenderingPipelineManager.SpriteShaderEffectKind.Grayscale:
                        effect = new SpriteEffect(SpriteEffectMode.Grayscale, 0, resource.Size, Vector4.Zero, null);
                        break;
                    case RenderingPipelineManager.SpriteShaderEffectKind.DropShadow:
                        RenderingPipelineManager.DropShadowEffectSettings shadow = request.Value.DropShadow;
                        RectangleF shadowBounds = shadow.VisibleBounds ?? destination;
                        EndSpriteBatch();
                        DrawSprite(CreateSpriteBatchItem(resource, source, destination, transform, shadow.Colour, _opacity, new SpriteEffect(SpriteEffectMode.DropShadow, shadow.Width, resource.Size, new Vector4(shadowBounds.Left, shadowBounds.Top, shadowBounds.Right, shadowBounds.Bottom), shadow.StartOpacity, shadow.Width, false)));
                        break;
                }
            }

            if (_spriteBatch.Count >= MaxSprites)
                EndSpriteBatch();

            _spriteBatch.Add(CreateSpriteBatchItem(resource, source, destination, transform, colour, _opacity, effect));
        }

        private void DrawSprite(SpriteBatchItem item)
        {
            if (item.Texture.ShaderResourceView.Handle == null)
                return;

            SpriteVertex* vertices = stackalloc SpriteVertex[6];
            WriteSpriteVertices(vertices, item);
            UpdateBuffer(_vertexBuffer, vertices, (uint)(sizeof(SpriteVertex) * 6));

            Matrix4x4 projection = Matrix4x4.Identity;
            projection.M11 = 2.0f / _currentTarget.Size.Width;
            projection.M22 = -2.0f / _currentTarget.Size.Height;
            projection.M41 = -1.0f;
            projection.M42 = 1.0f;
            projection = Matrix4x4.Transpose(projection);
            UpdateBuffer(_matrixBuffer, &projection, (uint)sizeof(Matrix4x4));

            EffectConstants effect = CreateEffectConstants(item);
            UpdateBuffer(_effectBuffer, &effect, (uint)sizeof(EffectConstants));

            uint stride = (uint)sizeof(SpriteVertex);
            uint offset = 0;
            ID3D11Buffer* vertexBuffer = _vertexBuffer.Handle;
            _deviceContext.IASetInputLayout(_inputLayout);
            _deviceContext.IASetPrimitiveTopology(D3DPrimitiveTopology.D3D11PrimitiveTopologyTrianglelist);
            _deviceContext.IASetVertexBuffers(0, 1, &vertexBuffer, &stride, &offset);
            ID3D11PixelShader* pixelShader = GetPixelShader(item.Effect).Handle;
            _deviceContext.VSSetShader(_vertexShader.Handle, (ID3D11ClassInstance**)null, 0);
            _deviceContext.PSSetShader(pixelShader, (ID3D11ClassInstance**)null, 0);
            ID3D11Buffer* matrixBuffer = _matrixBuffer.Handle;
            ID3D11Buffer* effectBuffer = _effectBuffer.Handle;
            _deviceContext.VSSetConstantBuffers(0, 1, &matrixBuffer);
            _deviceContext.PSSetConstantBuffers(1, 1, &effectBuffer);
            ID3D11ShaderResourceView* srv = item.Texture.ShaderResourceView.Handle;
            _deviceContext.PSSetShaderResources(0, 1, &srv);
            ID3D11SamplerState* sampler = (_textureFilter == TextureFilterMode.Linear ? _linearSampler : _pointSampler).Handle;
            _deviceContext.PSSetSamplers(0, 1, &sampler);

            ApplyBlendState(item.BlendMode, item.BlendRate);
            _deviceContext.Draw(6, 0);

            ID3D11ShaderResourceView* nullSrv = null;
            _deviceContext.PSSetShaderResources(0, 1, &nullSrv);
            ID3D11Buffer* nullBuffer = null;
            _deviceContext.PSSetConstantBuffers(1, 1, &nullBuffer);
        }

        private void DrawSolidRectangle(RectangleF rectangle, GdiColor colour, float opacity)
        {
            EnsureSolidTexture();
            DrawSprite(CreateSpriteBatchItem(_solidWhiteTexture, new Rectangle(0, 0, 1, 1), rectangle, Matrix3x2.Identity, colour, opacity, null, BlendMode.NONE, 1F));
        }

        private SpriteBatchItem CreateSpriteBatchItem(
            SilkD3D11TextureResource resource,
            Rectangle source,
            RectangleF destination,
            Matrix3x2 transform,
            GdiColor colour,
            float opacity,
            SpriteEffect? effect,
            BlendMode? blendMode = null,
            float? blendRate = null)
        {
            return new SpriteBatchItem(resource, source, destination, transform, colour, opacity, blendMode ?? GetActiveBlendMode(), blendRate ?? _blendRate, effect);
        }

        private BlendMode GetActiveBlendMode() => _blending ? _blendMode : BlendMode.NONE;

        private void FlushLinesIfNeeded()
        {
            if (_lineBatch.Count > 0)
                FlushLines();
        }

        private void CreateDevice()
        {
            D3DFeatureLevel[] levels =
            {
                D3DFeatureLevel.Level111,
                D3DFeatureLevel.Level110,
                D3DFeatureLevel.Level101,
                D3DFeatureLevel.Level100
            };

            ID3D11Device* device = null;
            ID3D11DeviceContext* context = null;
            D3DFeatureLevel selected = D3DFeatureLevel.Level110;
            fixed (D3DFeatureLevel* levelPtr = levels)
            {
                Check(_d3d.CreateDevice(null, D3DDriverType.Hardware, IntPtr.Zero, (uint)CreateDeviceFlag.BgraSupport, levelPtr, (uint)levels.Length, D3D11.SdkVersion, &device, &selected, &context), "create D3D11 device");
            }

            _device = new ComPtr<ID3D11Device>(device);
            _deviceContext = new ComPtr<ID3D11DeviceContext>(context);
            _factory = _dxgi.CreateDXGIFactory1<IDXGIFactory1>();
        }

        private void CreateSwapChain()
        {
            Size size = GetTargetSize();
            SwapChainDesc desc = new()
            {
                BufferDesc = new ModeDesc((uint)size.Width, (uint)size.Height, new Rational(60, 1), Format.FormatB8G8R8A8Unorm, ModeScanlineOrder.Unspecified, ModeScaling.Unspecified),
                SampleDesc = new SampleDesc(1, 0),
                BufferUsage = 0x20,
                BufferCount = 1,
                OutputWindow = _context.RenderTarget.Handle,
                Windowed = new Bool32(true),
                SwapEffect = SwapEffect.Discard,
                Flags = DxgiSwapChainFlagAllowModeSwitch
            };

            Check(_factory.CreateSwapChain(_device, ref desc, ref _swapChain), "create D3D11 swapchain");
        }

        private void CreateTargets()
        {
            DisposeTargets();
            CreateBackBuffer();
            _scratchTarget = CreateRenderTargetCore(_backBufferSize);
            _currentTarget = _backBufferTarget;
            SetD3DRenderTarget(_currentTarget);
        }

        private void CreateBackBuffer()
        {
            ComPtr<ID3D11Texture2D> texture = default;
            texture = _swapChain.GetBuffer<ID3D11Texture2D>(0);
            Texture2DDesc desc;
            texture.GetDesc(&desc);
            _backBufferSize = new Size((int)desc.Width, (int)desc.Height);
            _backBufferTarget = CreateRenderTargetFromTexture(texture, _backBufferSize, true);
            _currentTarget = _backBufferTarget;
        }

        private void ResizeBackBufferIfNeeded(bool force = false)
        {
            Size size = GetTargetSize();
            if (!force && size == _backBufferSize)
                return;

            RecreateSwapChain(size);
        }

        private void RequestReset()
        {
            _resetRequested = true;
        }

        private void AttemptReset()
        {
            if (!_resetRequested)
                return;

            _resetRequested = false;
            RecreateSwapChain(RenderingPipelineManager.HostSettings.GameSize);
        }

        private void RecreateSwapChain(Size size)
        {
            EndSpriteBatch();
            FlushLines();
            DisposeTargets();

            if (IsSwapChainFullscreen())
                Check(_swapChain.SetFullscreenState(new Bool32(false), (IDXGIOutput*)null), "exit D3D11 fullscreen before resize");

            ApplyWindowStyle();
            ApplyWindowBounds(RenderingPipelineManager.HostSettings.FullScreen);

            if (!RenderingPipelineManager.HostSettings.FullScreen && _context.RenderTarget != null && _context.RenderTarget.ClientSize != size)
                _context.RenderTarget.ClientSize = size;

            _swapChain.Dispose();
            _swapChain = default;
            CreateSwapChain();
            CreateTargets();
        }

        private bool IsSwapChainFullscreen()
        {
            if (_swapChain.Handle == null)
                return false;

            int fullScreen = 0;
            IDXGIOutput* output = null;
            Check(_swapChain.GetFullscreenState(&fullScreen, &output), "query D3D11 fullscreen state");
            if (output != null)
                output->Release();
            return fullScreen != 0;
        }

        private void DisposeTargets()
        {
            UnbindBackBufferReferences();
            _scratchTarget?.Dispose();
            _scratchTarget = null;
            _backBufferTarget?.Dispose();
            _backBufferTarget = null;
            _currentTarget = null;
        }

        private void UnbindBackBufferReferences()
        {
            if (_deviceContext.Handle == null)
                return;

            ID3D11ShaderResourceView* nullSrv = null;
            _deviceContext.PSSetShaderResources(0, 1, &nullSrv);
            _deviceContext.OMSetRenderTargets(0, null, (ID3D11DepthStencilView*)null);
            _deviceContext.Flush();
        }

        private SilkD3D11RenderTarget CreateRenderTargetCore(Size size)
        {
            SilkD3D11TextureResource texture = CreateTextureCore(size, RenderTextureFormat.A8R8G8B8, true);
            return CreateRenderTargetFromTexture(texture, size, false);
        }

        private SilkD3D11RenderTarget CreateRenderTargetFromTexture(SilkD3D11TextureResource texture, Size size, bool isBackBuffer)
        {
            ID3D11Resource* resource = (ID3D11Resource*)texture.Texture.Handle;
            ID3D11RenderTargetView* viewPtr = null;
            Check(_device.CreateRenderTargetView(resource, (RenderTargetViewDesc*)null, &viewPtr), "create D3D11 render target view");
            ComPtr<ID3D11RenderTargetView> view = new(viewPtr);
            texture.RenderTargetView = view;
            return new SilkD3D11RenderTarget(texture, view, size, isBackBuffer);
        }

        private SilkD3D11RenderTarget CreateRenderTargetFromTexture(ComPtr<ID3D11Texture2D> texture, Size size, bool isBackBuffer)
        {
            SilkD3D11TextureResource resource = new(size, RenderTextureFormat.Bgra32, Format.FormatB8G8R8A8Unorm, texture, default, default, null);
            return CreateRenderTargetFromTexture(resource, size, isBackBuffer);
        }

        private SilkD3D11TextureResource CreateTextureCore(Size size, RenderTextureFormat format, bool renderTarget)
        {
            Size safeSize = new(Math.Max(1, size.Width), Math.Max(1, size.Height));
            Format dxFormat = ToDxgiFormat(format);
            Texture2DDesc desc = new()
            {
                Width = (uint)safeSize.Width,
                Height = (uint)safeSize.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = dxFormat,
                SampleDesc = new SampleDesc(1, 0),
                Usage = Usage.Default,
                BindFlags = (uint)(BindFlag.ShaderResource | (renderTarget ? BindFlag.RenderTarget : 0)),
                CPUAccessFlags = 0,
                MiscFlags = 0
            };

            ID3D11Texture2D* texturePtr = null;
            Check(_device.CreateTexture2D(ref desc, null, &texturePtr), "create D3D11 texture");
            ComPtr<ID3D11Texture2D> texture = new(texturePtr);
            ComPtr<ID3D11ShaderResourceView> srv = default;
            ID3D11Resource* resource = (ID3D11Resource*)texture.Handle;
            ID3D11ShaderResourceView* srvPtr = null;
            Check(_device.CreateShaderResourceView(resource, (ShaderResourceViewDesc*)null, &srvPtr), "create D3D11 shader resource view");
            srv = new ComPtr<ID3D11ShaderResourceView>(srvPtr);
            byte[] data = new byte[GetDataSize(safeSize, format)];
            return new SilkD3D11TextureResource(safeSize, format, dxFormat, texture, srv, default, data);
        }

        private void UploadTexture(SilkD3D11TextureResource resource)
        {
            fixed (byte* data = resource.Data)
            {
                _deviceContext.UpdateSubresource((ID3D11Resource*)resource.Texture.Handle, 0, (Box*)null, data, (uint)GetRowPitch(resource), 0);
            }
        }

        private void ReadTexture(SilkD3D11TextureResource resource)
        {
            EnsureStagingTexture(resource);
            _deviceContext.CopyResource((ID3D11Resource*)resource.Texture.Handle, (ID3D11Resource*)resource.StagingTexture.Handle);
            MappedSubresource mapped = default;
            Check(_deviceContext.Map((ID3D11Resource*)resource.StagingTexture.Handle, 0, D3DMap.Read, 0, &mapped), "map D3D11 staging texture");
            try
            {
                int rowPitch = GetRowPitch(resource);
                for (int y = 0; y < resource.Size.Height; y++)
                    Marshal.Copy((IntPtr)((byte*)mapped.PData + mapped.RowPitch * y), resource.Data, rowPitch * y, rowPitch);
            }
            finally
            {
                _deviceContext.Unmap((ID3D11Resource*)resource.StagingTexture.Handle, 0);
            }
        }

        private void EnsureStagingTexture(SilkD3D11TextureResource resource)
        {
            if (resource.StagingTexture.Handle != null)
                return;

            Texture2DDesc desc = new()
            {
                Width = (uint)resource.Size.Width,
                Height = (uint)resource.Size.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = resource.DxgiFormat,
                SampleDesc = new SampleDesc(1, 0),
                Usage = Usage.Staging,
                BindFlags = 0,
                CPUAccessFlags = (uint)CpuAccessFlag.Read,
                MiscFlags = 0
            };
            ComPtr<ID3D11Texture2D> staging = default;
            Check(_device.CreateTexture2D(ref desc, null, ref staging), "create D3D11 staging texture");
            resource.StagingTexture = staging;
        }

        private void CreateShadersAndState()
        {
            byte[] vsBlob = CompileShaderFromFile(SpriteShaderFileName, "VS", "vs_5_0");
            byte[] psBlob = CompileShaderFromFile(SpriteShaderFileName, "PS", "ps_5_0");
            byte[] outlineBlob = CompileShaderFromFile(OutlineShaderFileName, "PS_OUTLINE", "ps_5_0");
            byte[] grayscaleBlob = CompileShaderFromFile(GrayscaleShaderFileName, "PS_GRAY", "ps_5_0");
            byte[] shadowBlob = CompileShaderFromFile(DropShadowShaderFileName, "PS_SHADOW", "ps_5_0");

            ID3D11VertexShader* vertexShader = null;
            ID3D11PixelShader* pixelShader = null;
            ID3D11PixelShader* outlinePixelShader = null;
            ID3D11PixelShader* grayscalePixelShader = null;
            ID3D11PixelShader* dropShadowPixelShader = null;
            fixed (byte* vsPointer = vsBlob)
            fixed (byte* psPointer = psBlob)
            fixed (byte* outlinePointer = outlineBlob)
            fixed (byte* grayscalePointer = grayscaleBlob)
            fixed (byte* shadowPointer = shadowBlob)
            {
                Check(_device.CreateVertexShader(vsPointer, (nuint)vsBlob.Length, (ID3D11ClassLinkage*)null, &vertexShader), "create D3D11 vertex shader");
                Check(_device.CreatePixelShader(psPointer, (nuint)psBlob.Length, (ID3D11ClassLinkage*)null, &pixelShader), "create D3D11 pixel shader");
                Check(_device.CreatePixelShader(outlinePointer, (nuint)outlineBlob.Length, (ID3D11ClassLinkage*)null, &outlinePixelShader), "create D3D11 outline shader");
                Check(_device.CreatePixelShader(grayscalePointer, (nuint)grayscaleBlob.Length, (ID3D11ClassLinkage*)null, &grayscalePixelShader), "create D3D11 grayscale shader");
                Check(_device.CreatePixelShader(shadowPointer, (nuint)shadowBlob.Length, (ID3D11ClassLinkage*)null, &dropShadowPixelShader), "create D3D11 shadow shader");
            }
            _vertexShader = new ComPtr<ID3D11VertexShader>(vertexShader);
            _pixelShader = new ComPtr<ID3D11PixelShader>(pixelShader);
            _outlinePixelShader = new ComPtr<ID3D11PixelShader>(outlinePixelShader);
            _grayscalePixelShader = new ComPtr<ID3D11PixelShader>(grayscalePixelShader);
            _dropShadowPixelShader = new ComPtr<ID3D11PixelShader>(dropShadowPixelShader);

            CreateInputLayout(vsBlob);
            CreateBuffer((uint)(sizeof(SpriteVertex) * MaxVertices), BindFlag.VertexBuffer, ref _vertexBuffer);
            CreateBuffer((uint)sizeof(Matrix4x4), BindFlag.ConstantBuffer, ref _matrixBuffer);
            CreateBuffer((uint)sizeof(EffectConstants), BindFlag.ConstantBuffer, ref _effectBuffer);
            CreateSamplers();
            CreateBlendStates();
        }

        private static byte[] CompileShaderFromFile(string fileName, string entryPoint, string target)
        {
            string path = FindShaderPath(fileName);
            if (path == null)
                throw new FileNotFoundException($"Unable to locate Silk D3D11 shader file '{fileName}'.");

            return CompileShader(File.ReadAllText(path), entryPoint, target, path);
        }

        private static string FindShaderPath(string fileName)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory ?? string.Empty;
            string[] candidates =
            {
                Path.Combine(baseDirectory, "Rendering", "SilkD3D11", "Shaders", fileName),
                Path.Combine(AppContext.BaseDirectory, "Rendering", "SilkD3D11", "Shaders", fileName),
                Path.Combine("Rendering", "SilkD3D11", "Shaders", fileName),
                Path.Combine("RenderingCore", "Rendering", "SilkD3D11", "Shaders", fileName)
            };

            foreach (string candidate in candidates)
                if (File.Exists(candidate))
                    return candidate;

            return null;
        }

        private static byte[] CompileShader(string source, string entryPoint, string target, string sourceName)
        {
            byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
            ID3DBlob code = null;
            ID3DBlob errors = null;

            fixed (byte* sourcePtr = sourceBytes)
            {
                int result = D3DCompile((IntPtr)sourcePtr, (UIntPtr)sourceBytes.Length, sourceName, IntPtr.Zero, IntPtr.Zero, entryPoint, target, 0, 0, out code, out errors);
                if (result < 0)
                {
                    string message = GetCompilerErrorMessage(errors);
                    ReleaseComObject(errors);
                    throw new InvalidOperationException(message);
                }
            }

            try
            {
                if (code == null)
                    throw new InvalidOperationException("Shader compiler returned a null bytecode blob.");

                IntPtr pointer = code.GetBufferPointer();
                if (pointer == IntPtr.Zero)
                    throw new InvalidOperationException("Shader compiler returned a bytecode blob with a null buffer.");

                int size = checked((int)code.GetBufferSize());
                if (size == 0)
                    throw new InvalidOperationException("Shader compiler returned an empty bytecode blob.");

                byte[] bytecode = new byte[size];
                Marshal.Copy(pointer, bytecode, 0, size);
                return bytecode;
            }
            finally
            {
                ReleaseComObject(errors);
                ReleaseComObject(code);
            }
        }

        private void CreateInputLayout(byte[] vsBlob)
        {
            byte* position = (byte*)Marshal.StringToHGlobalAnsi("POSITION");
            byte* texcoord = (byte*)Marshal.StringToHGlobalAnsi("TEXCOORD");
            byte* color = (byte*)Marshal.StringToHGlobalAnsi("COLOR");
            try
            {
                InputElementDesc* elements = stackalloc InputElementDesc[3];
                elements[0] = new InputElementDesc(position, 0, Format.FormatR32G32Float, 0, 0, InputClassification.PerVertexData, 0);
                elements[1] = new InputElementDesc(texcoord, 0, Format.FormatR32G32Float, 0, 8, InputClassification.PerVertexData, 0);
                elements[2] = new InputElementDesc(color, 0, Format.FormatR32G32B32A32Float, 0, 16, InputClassification.PerVertexData, 0);
                ID3D11InputLayout* inputLayout = null;
                fixed (byte* vsPointer = vsBlob)
                    Check(_device.CreateInputLayout(elements, 3, vsPointer, (nuint)vsBlob.Length, &inputLayout), "create D3D11 input layout");
                _inputLayout = new ComPtr<ID3D11InputLayout>(inputLayout);
            }
            finally
            {
                Marshal.FreeHGlobal((IntPtr)position);
                Marshal.FreeHGlobal((IntPtr)texcoord);
                Marshal.FreeHGlobal((IntPtr)color);
            }
        }

        private void CreateBuffer(uint size, BindFlag bindFlags, ref ComPtr<ID3D11Buffer> buffer)
        {
            BufferDesc desc = new()
            {
                ByteWidth = size,
                Usage = Usage.Dynamic,
                BindFlags = (uint)bindFlags,
                CPUAccessFlags = (uint)CpuAccessFlag.Write,
                MiscFlags = 0,
                StructureByteStride = 0
            };
            Check(_device.CreateBuffer(ref desc, null, ref buffer), "create D3D11 buffer");
        }

        private static string GetCompilerErrorMessage(ID3DBlob errors)
        {
            if (errors == null)
                return "Unknown shader compiler error.";

            IntPtr pointer = errors.GetBufferPointer();
            int size = checked((int)errors.GetBufferSize());
            if (pointer == IntPtr.Zero || size == 0)
                return "Unknown shader compiler error.";

            return Marshal.PtrToStringAnsi(pointer, size) ?? "Unknown shader compiler error.";
        }

        private static void ReleaseComObject(object comObject)
        {
            if (comObject != null)
                Marshal.ReleaseComObject(comObject);
        }

        private void CreateSamplers()
        {
            SamplerDesc desc = new()
            {
                Filter = Filter.MinMagMipPoint,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                ComparisonFunc = ComparisonFunc.Never,
                MinLOD = 0,
                MaxLOD = float.MaxValue
            };
            ID3D11SamplerState* pointSampler = null;
            Check(_device.CreateSamplerState(ref desc, &pointSampler), "create D3D11 point sampler");
            _pointSampler = new ComPtr<ID3D11SamplerState>(pointSampler);
            desc.Filter = Filter.MinMagMipLinear;
            ID3D11SamplerState* linearSampler = null;
            Check(_device.CreateSamplerState(ref desc, &linearSampler), "create D3D11 linear sampler");
            _linearSampler = new ComPtr<ID3D11SamplerState>(linearSampler);
        }

        private void CreateBlendStates()
        {
            CreateBlendState(BlendMode.NONE, D3DBlend.One, D3DBlend.InvSrcAlpha, D3DBlend.One, D3DBlend.InvSrcAlpha);
            CreateBlendState(BlendMode.NORMAL, D3DBlend.InvDestColor, D3DBlend.One, D3DBlend.InvDestAlpha, D3DBlend.One);
            CreateBlendState(BlendMode.LIGHT, D3DBlend.InvDestColor, D3DBlend.One, D3DBlend.InvDestAlpha, D3DBlend.One);
            CreateBlendState(BlendMode.LIGHTINV, D3DBlend.InvDestColor, D3DBlend.One, D3DBlend.InvDestAlpha, D3DBlend.One);
            CreateBlendState(BlendMode.INVNORMAL, D3DBlend.InvDestColor, D3DBlend.One, D3DBlend.InvDestAlpha, D3DBlend.One);
            CreateBlendState(BlendMode.INVLIGHTINV, D3DBlend.InvDestColor, D3DBlend.One, D3DBlend.InvDestAlpha, D3DBlend.One);
            CreateBlendState(BlendMode.INVCOLOR, D3DBlend.InvDestColor, D3DBlend.One, D3DBlend.InvDestAlpha, D3DBlend.One);
            CreateBlendState(BlendMode.INVBACKGROUND, D3DBlend.InvDestColor, D3DBlend.One, D3DBlend.InvDestAlpha, D3DBlend.One);
            CreateBlendState(BlendMode.INVLIGHT, D3DBlend.BlendFactor, D3DBlend.InvSrcColor, D3DBlend.BlendFactor, D3DBlend.InvSrcAlpha);
            CreateBlendState(BlendMode.COLORFY, D3DBlend.SrcAlpha, D3DBlend.One, D3DBlend.SrcAlpha, D3DBlend.One);
            CreateBlendState(BlendMode.MASK, D3DBlend.Zero, D3DBlend.InvSrcAlpha, D3DBlend.Zero, D3DBlend.InvSrcAlpha);
            CreateBlendState(BlendMode.EFFECTMASK, D3DBlend.DestAlpha, D3DBlend.One, D3DBlend.DestAlpha, D3DBlend.One);
            CreateBlendState(BlendMode.HIGHLIGHT, D3DBlend.BlendFactor, D3DBlend.One, D3DBlend.BlendFactor, D3DBlend.One);
            CreateBlendState(BlendMode.LIGHTMAP, D3DBlend.Zero, D3DBlend.SrcColor, D3DBlend.Zero, D3DBlend.SrcAlpha);
        }

        private void CreateBlendState(BlendMode mode, D3DBlend source, D3DBlend destination, D3DBlend sourceAlpha, D3DBlend destinationAlpha)
        {
            BlendDesc desc = default;
            desc.RenderTarget[0].BlendEnable = new Bool32(true);
            desc.RenderTarget[0].SrcBlend = source;
            desc.RenderTarget[0].DestBlend = destination;
            desc.RenderTarget[0].BlendOp = BlendOp.Add;
            desc.RenderTarget[0].SrcBlendAlpha = sourceAlpha;
            desc.RenderTarget[0].DestBlendAlpha = destinationAlpha;
            desc.RenderTarget[0].BlendOpAlpha = BlendOp.Add;
            desc.RenderTarget[0].RenderTargetWriteMask = (byte)ColorWriteEnable.All;

            ID3D11BlendState* blendStatePtr = null;
            Check(_device.CreateBlendState(ref desc, &blendStatePtr), "create D3D11 blend state");
            ComPtr<ID3D11BlendState> blendState = new(blendStatePtr);
            _blendStates[mode] = blendState;
        }

        private void ApplyBlendState(BlendMode mode, float blendRate)
        {
            if (!_blendStates.TryGetValue(mode, out ComPtr<ID3D11BlendState> blendState))
                blendState = _blendStates[BlendMode.NONE];

            Vector4 factor = new(blendRate, blendRate, blendRate, blendRate);
            _deviceContext.OMSetBlendState(blendState, (float*)&factor, uint.MaxValue);
        }

        private void SetD3DRenderTarget(SilkD3D11RenderTarget target)
        {
            if (target == null)
                return;

            ID3D11RenderTargetView* view = target.RenderTargetView.Handle;
            _deviceContext.OMSetRenderTargets(1, &view, (ID3D11DepthStencilView*)null);
            Viewport viewport = new(0, 0, target.Size.Width, target.Size.Height, 0, 1);
            _deviceContext.RSSetViewports(1, &viewport);
        }

        private void UpdateBuffer(ComPtr<ID3D11Buffer> buffer, void* data, uint size)
        {
            MappedSubresource mapped = default;
            Check(_deviceContext.Map((ID3D11Resource*)buffer.Handle, 0, D3DMap.WriteDiscard, 0, &mapped), "map D3D11 buffer");
            System.Buffer.MemoryCopy(data, mapped.PData, size, size);
            _deviceContext.Unmap((ID3D11Resource*)buffer.Handle, 0);
        }

        private void LoadTextures()
        {
            _colourPalette = CreateTextureCore(new Size(256, 1), RenderTextureFormat.A8R8G8B8, false);
            _paletteData = new byte[256 * 4];
            for (int i = 0; i < 256; i++)
            {
                _paletteData[i * 4] = (byte)i;
                _paletteData[i * 4 + 1] = (byte)i;
                _paletteData[i * 4 + 2] = (byte)i;
                _paletteData[i * 4 + 3] = 255;
            }
            Buffer.BlockCopy(_paletteData, 0, _colourPalette.Data, 0, _paletteData.Length);
            UploadTexture(_colourPalette);

            _lightData ??= LightGenerator.CreateLightData(LightWidth, LightHeight);
            _lightTexture = CreateTextureCore(new Size(LightWidth, LightHeight), RenderTextureFormat.A8R8G8B8, false);
            Buffer.BlockCopy(_lightData, 0, _lightTexture.Data, 0, _lightData.Length);
            UploadTexture(_lightTexture);

            _poisonTexture = CreateTextureCore(new Size(PoisonSize, PoisonSize), RenderTextureFormat.A8R8G8B8, false);
            Array.Fill<byte>(_poisonTexture.Data, 255);
            UploadTexture(_poisonTexture);
        }

        private void EnsureSolidTexture()
        {
            if (_solidWhiteTexture != null)
                return;

            _solidWhiteTexture = CreateTextureCore(new Size(1, 1), RenderTextureFormat.A8R8G8B8, false);
            _solidWhiteTexture.Data[0] = 255;
            _solidWhiteTexture.Data[1] = 255;
            _solidWhiteTexture.Data[2] = 255;
            _solidWhiteTexture.Data[3] = 255;
            UploadTexture(_solidWhiteTexture);
        }

        private void EnsureScratchTargetSize()
        {
            Size size = GetTargetSize();
            if (_scratchTarget != null && _scratchTarget.Size == size)
                return;

            _scratchTarget?.Dispose();
            _scratchTarget = CreateRenderTargetCore(size);
        }

        private bool TryGetTexture(RenderTexture texture, out SilkD3D11TextureResource resource)
        {
            resource = texture.NativeHandle switch
            {
                SilkD3D11TextureResource d3dTexture => d3dTexture,
                SilkD3D11RenderTarget target => target.Texture,
                _ => null
            };

            return resource != null;
        }

        private static void WriteSpriteVertices(SpriteVertex* vertices, SpriteBatchItem item)
        {
            float left = item.Destination.Left;
            float right = item.Destination.Right;
            float top = item.Destination.Top;
            float bottom = item.Destination.Bottom;
            float u1 = item.Source.Left / (float)item.Texture.Size.Width;
            float v1 = item.Source.Top / (float)item.Texture.Size.Height;
            float u2 = item.Source.Right / (float)item.Texture.Size.Width;
            float v2 = item.Source.Bottom / (float)item.Texture.Size.Height;

            if (item.Effect.HasValue && item.Effect.Value.GeometryExpand > 0)
            {
                float expand = item.Effect.Value.GeometryExpand;
                left -= expand;
                right += expand;
                top -= expand;
                bottom += expand;

                if (item.Effect.Value.ExpandUvs)
                {
                    u1 -= expand / item.Texture.Size.Width;
                    v1 -= expand / item.Texture.Size.Height;
                    u2 += expand / item.Texture.Size.Width;
                    v2 += expand / item.Texture.Size.Height;
                }
            }

            Vector2 p0 = Vector2.Transform(new Vector2(left, top), item.Transform);
            Vector2 p1 = Vector2.Transform(new Vector2(right, top), item.Transform);
            Vector2 p2 = Vector2.Transform(new Vector2(right, bottom), item.Transform);
            Vector2 p3 = Vector2.Transform(new Vector2(left, bottom), item.Transform);
            Vector4 colour = ToPremultipliedVector(item.Colour, item.Opacity);

            vertices[0] = new SpriteVertex(p0, new Vector2(u1, v1), colour);
            vertices[1] = new SpriteVertex(p1, new Vector2(u2, v1), colour);
            vertices[2] = new SpriteVertex(p2, new Vector2(u2, v2), colour);
            vertices[3] = vertices[0];
            vertices[4] = vertices[2];
            vertices[5] = new SpriteVertex(p3, new Vector2(u1, v2), colour);
        }

        private EffectConstants CreateEffectConstants(SpriteBatchItem item)
        {
            SpriteEffect effect = item.Effect ?? default;
            Vector4 outlineColour = effect.Mode == SpriteEffectMode.None ? Vector4.Zero : ToColorVector(item.Colour);
            if (effect.Mode == SpriteEffectMode.DropShadow)
            {
                return new EffectConstants
                {
                    Source = effect.Source,
                    OutlineColour = outlineColour,
                    Effect = new Vector4((float)effect.Mode, effect.Amount, effect.Extra ?? 1F, 0F)
                };
            }

            return new EffectConstants
            {
                Source = effect.Source == Vector4.Zero ? ToSourceUv(item.Source, item.Texture.Size) : effect.Source,
                OutlineColour = outlineColour,
                Effect = new Vector4((float)effect.Mode, effect.Amount, effect.TextureSize.Width, effect.TextureSize.Height)
            };
        }

        private ComPtr<ID3D11PixelShader> GetPixelShader(SpriteEffect? effect)
        {
            if (!effect.HasValue)
                return _pixelShader;

            return effect.Value.Mode switch
            {
                SpriteEffectMode.Grayscale => _grayscalePixelShader,
                SpriteEffectMode.Outline => _outlinePixelShader,
                SpriteEffectMode.DropShadow => _dropShadowPixelShader,
                _ => _pixelShader
            };
        }

        private Size GetTargetSize()
        {
            Size size = _context.RenderTarget.ClientSize;
            if (RenderingPipelineManager.HostSettings.FullScreen || RenderingPipelineManager.HostSettings.Borderless)
                size = RenderingPipelineManager.GetSelectedScreen().Bounds.Size;

            if (size.Width <= 0 || size.Height <= 0)
                size = RenderingPipelineManager.HostSettings.GameSize;

            return new Size(Math.Max(1, size.Width), Math.Max(1, size.Height));
        }

        private void ApplyWindowStyle()
        {
            if (_context?.RenderTarget is not Form form)
                return;

            bool fullScreen = RenderingPipelineManager.HostSettings.FullScreen || RenderingPipelineManager.HostSettings.Borderless;
            form.FormBorderStyle = fullScreen ? FormBorderStyle.None : FormBorderStyle.FixedSingle;
            form.ControlBox = !fullScreen;
            form.TopMost = RenderingPipelineManager.HostSettings.FullScreen;
        }

        private void ApplyWindowBounds(bool forceCenter = false)
        {
            if (_context?.RenderTarget is not Form form)
                return;

            if (RenderingPipelineManager.HostSettings.FullScreen || RenderingPipelineManager.HostSettings.Borderless)
            {
                form.Bounds = RenderingPipelineManager.GetSelectedScreen().Bounds;
                return;
            }

            form.ClientSize = RenderingPipelineManager.HostSettings.GameSize;
            CenterOnSelectedMonitor(forceCenter);
        }

        private void CenterOnSelectedMonitor(bool force)
        {
            if (_context?.RenderTarget is not Form form)
                return;

            Screen screen = RenderingPipelineManager.GetSelectedScreen();
            if (!force && !RenderingPipelineManager.HostSettings.FullScreen && !RenderingPipelineManager.HostSettings.Borderless && screen.Bounds.Contains(form.Bounds))
                return;

            Rectangle bounds = screen.Bounds;
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(bounds.X + (bounds.Width - form.Width) / 2, bounds.Y + (bounds.Height - form.Height) / 2);
        }

        private static bool AppStillIdle => !PeekMessage(out PeekMsg msg, IntPtr.Zero, 0, 0, 0);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern bool PeekMessage(out PeekMsg msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

        [StructLayout(LayoutKind.Sequential)]
        private struct PeekMsg
        {
            private readonly IntPtr HWnd;
            private readonly WinFormsMessage Msg;
            private readonly IntPtr WParam;
            private readonly IntPtr LParam;
            private readonly uint Time;
            private readonly Point P;
        }

        private static float HueToRgb(float p, float q, float t)
        {
            if (t < 0F) t += 1F;
            if (t > 1F) t -= 1F;
            if (t < 1F / 6F) return p + (q - p) * 6F * t;
            if (t < 1F / 2F) return q;
            if (t < 2F / 3F) return p + (q - p) * (2F / 3F - t) * 6F;
            return p;
        }

        private static Vector4 ToSourceUv(Rectangle source, Size size) =>
            new(source.Left / (float)size.Width, source.Top / (float)size.Height, source.Right / (float)size.Width, source.Bottom / (float)size.Height);

        private static Vector4 ToPremultipliedVector(GdiColor colour, float opacity)
        {
            float alpha = colour.A / 255F * opacity;
            return new Vector4(colour.R / 255F, colour.G / 255F, colour.B / 255F, alpha);
        }

        private static Vector4 ToColorVector(GdiColor colour) => new(colour.R / 255F, colour.G / 255F, colour.B / 255F, colour.A / 255F);

        private static float Snap(float value) => (float)Math.Floor(value) + 0.5F;

        private static Format ToDxgiFormat(RenderTextureFormat format) => format switch
        {
            RenderTextureFormat.Dxt1 => Format.FormatBC1Unorm,
            RenderTextureFormat.Dxt5 => Format.FormatBC3Unorm,
            RenderTextureFormat.Bc7 => Format.FormatBC7Unorm,
            _ => Format.FormatB8G8R8A8Unorm
        };

        private static int GetDataSize(Size size, RenderTextureFormat format)
        {
            if (format == RenderTextureFormat.Dxt1)
                return ((size.Width + 3) / 4) * ((size.Height + 3) / 4) * 8;
            if (format == RenderTextureFormat.Dxt5 || format == RenderTextureFormat.Bc7)
                return ((size.Width + 3) / 4) * ((size.Height + 3) / 4) * 16;
            return size.Width * size.Height * 4;
        }

        private static int GetRowPitch(SilkD3D11TextureResource resource)
        {
            if (resource.Format == RenderTextureFormat.Dxt1)
                return ((resource.Size.Width + 3) / 4) * 8;
            if (resource.Format == RenderTextureFormat.Dxt5 || resource.Format == RenderTextureFormat.Bc7)
                return ((resource.Size.Width + 3) / 4) * 16;
            return resource.Size.Width * 4;
        }

        private static void Check(int result, string operation)
        {
            if (result < 0)
                throw new InvalidOperationException($"{operation} failed with HRESULT 0x{result:X8}.");
        }

        [StructLayout(LayoutKind.Sequential)]
        private readonly struct SpriteVertex
        {
            public readonly Vector2 Position;
            public readonly Vector2 TexCoord;
            public readonly Vector4 Colour;

            public SpriteVertex(Vector2 position, Vector2 texCoord, Vector4 colour)
            {
                Position = position;
                TexCoord = texCoord;
                Colour = colour;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct EffectConstants
        {
            public Vector4 Source;
            public Vector4 OutlineColour;
            public Vector4 Effect;
            public Vector4 Padding;
        }

        private readonly struct SpriteBatchItem
        {
            public readonly SilkD3D11TextureResource Texture;
            public readonly Rectangle Source;
            public readonly RectangleF Destination;
            public readonly Matrix3x2 Transform;
            public readonly GdiColor Colour;
            public readonly float Opacity;
            public readonly BlendMode BlendMode;
            public readonly float BlendRate;
            public readonly SpriteEffect? Effect;

            public SpriteBatchItem(SilkD3D11TextureResource texture, Rectangle source, RectangleF destination, Matrix3x2 transform, GdiColor colour, float opacity, BlendMode blendMode, float blendRate, SpriteEffect? effect)
            {
                Texture = texture;
                Source = source;
                Destination = destination;
                Transform = transform;
                Colour = colour;
                Opacity = opacity;
                BlendMode = blendMode;
                BlendRate = blendRate;
                Effect = effect;
            }
        }

        private readonly struct SpriteEffect
        {
            public readonly SpriteEffectMode Mode;
            public readonly float Amount;
            public readonly Size TextureSize;
            public readonly Vector4 Source;
            public readonly float? Extra;
            public readonly float GeometryExpand;
            public readonly bool ExpandUvs;

            public SpriteEffect(SpriteEffectMode mode, float amount, Size textureSize, Vector4 source, float? extra, float geometryExpand = 0F, bool expandUvs = false)
            {
                Mode = mode;
                Amount = amount;
                TextureSize = textureSize;
                Source = source;
                Extra = extra;
                GeometryExpand = geometryExpand;
                ExpandUvs = expandUvs;
            }
        }

        private readonly struct LineBatchItem
        {
            public readonly IReadOnlyList<LinePoint> Points;
            public readonly GdiColor Colour;
            public readonly float Width;
            public readonly float Opacity;

            public LineBatchItem(IReadOnlyList<LinePoint> points, GdiColor colour, float width, float opacity)
            {
                Points = points;
                Colour = colour;
                Width = width;
                Opacity = opacity;
            }
        }

        private sealed class SilkD3D11TextureResource : IDisposable
        {
            public SilkD3D11TextureResource(Size size, RenderTextureFormat format, Format dxgiFormat, ComPtr<ID3D11Texture2D> texture, ComPtr<ID3D11ShaderResourceView> shaderResourceView, ComPtr<ID3D11RenderTargetView> renderTargetView, byte[] data)
            {
                Size = size;
                Format = format;
                DxgiFormat = dxgiFormat;
                Texture = texture;
                ShaderResourceView = shaderResourceView;
                RenderTargetView = renderTargetView;
                Data = data ?? new byte[GetDataSize(size, format)];
            }

            public Size Size { get; }
            public RenderTextureFormat Format { get; }
            public Format DxgiFormat { get; }
            public ComPtr<ID3D11Texture2D> Texture;
            public ComPtr<ID3D11Texture2D> StagingTexture;
            public ComPtr<ID3D11ShaderResourceView> ShaderResourceView;
            public ComPtr<ID3D11RenderTargetView> RenderTargetView;
            public byte[] Data { get; }

            public void Dispose()
            {
                RenderTargetView.Dispose();
                ShaderResourceView.Dispose();
                StagingTexture.Dispose();
                Texture.Dispose();
            }
        }

        private sealed class SilkD3D11RenderTarget : IDisposable
        {
            public SilkD3D11RenderTarget(SilkD3D11TextureResource texture, ComPtr<ID3D11RenderTargetView> renderTargetView, Size size, bool isBackBuffer)
            {
                Texture = texture;
                RenderTargetView = renderTargetView;
                Size = size;
                IsBackBuffer = isBackBuffer;
            }

            public SilkD3D11TextureResource Texture { get; }
            public ComPtr<ID3D11RenderTargetView> RenderTargetView;
            public Size Size { get; }
            public bool IsBackBuffer { get; }

            public void Dispose()
            {
                Texture.Dispose();
            }
        }

    }
}
