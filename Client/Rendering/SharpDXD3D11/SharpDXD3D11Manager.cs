using Client.Controls;
using Client.Envir;

using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using D2D1AlphaMode = SharpDX.Direct2D1.AlphaMode;
using D2D1PixelFormat = SharpDX.Direct2D1.PixelFormat;
using D3D11DataBox = SharpDX.DataBox;
using D3D11MapFlags = SharpDX.Direct3D11.MapFlags;
using D3DDeviceContext = SharpDX.Direct3D11.DeviceContext;
using D3DFeatureLevel = SharpDX.Direct3D.FeatureLevel;
using Device = SharpDX.Direct3D11.Device;
using DrawingBitmap = System.Drawing.Bitmap;
using DrawingInterpolationMode = System.Drawing.Drawing2D.InterpolationMode;
using DrawingPixelFormat = System.Drawing.Imaging.PixelFormat;
using Factory = SharpDX.DXGI.Factory1;

namespace Client.Rendering.SharpDXD3D11
{
    public sealed class SharpD3D11TextureResource : IDisposable
    {
        public RenderTextureFormat Format { get; }
        public Texture2D Texture { get; }
        public Texture2D StagingTexture { get; }
        public Bitmap1 Bitmap { get; }

        public SharpD3D11TextureResource(RenderTextureFormat format, Texture2D texture, Texture2D stagingTexture, Bitmap1 bitmap)
        {
            Format = format;
            Texture = texture;
            StagingTexture = stagingTexture;
            Bitmap = bitmap;
        }

        public void Dispose()
        {
            Bitmap?.Dispose();
            StagingTexture?.Dispose();
            Texture?.Dispose();
        }
    }

    public sealed class SharpD3D11RenderTarget : IDisposable
    {
        public Texture2D Texture { get; }
        public RenderTargetView RenderTargetView { get; }
        public Bitmap1 TargetBitmap { get; }

        public SharpD3D11RenderTarget(Texture2D texture, RenderTargetView renderTargetView, Bitmap1 targetBitmap)
        {
            Texture = texture;
            RenderTargetView = renderTargetView;
            TargetBitmap = targetBitmap;
        }

        public void Dispose()
        {
            TargetBitmap?.Dispose();
            RenderTargetView?.Dispose();
            Texture?.Dispose();
        }
    }

    public static class SharpDXD3D11Manager
    {
        private static readonly Size MinimumResolution = new Size(1024, 768);

        public static Graphics Graphics { get; private set; } = Graphics.FromHwnd(IntPtr.Zero);

        public static List<Size> ValidResolutions { get; } = new List<Size>();
        public static List<DXControl> ControlList { get; } = new List<DXControl>();
        public static List<MirImage> TextureList { get; } = new List<MirImage>();
        public static List<DXSound> SoundList { get; } = new List<DXSound>();

        public static Factory Factory { get; private set; }
        public static Device Device { get; private set; }
        public static SwapChain SwapChain { get; private set; }
        public static D3DDeviceContext Context => Device?.ImmediateContext;

        public static SharpDX.Direct2D1.Factory1 D2DFactory { get; private set; }
        public static SharpDX.Direct2D1.Device D2DDevice { get; private set; }
        public static SharpDX.Direct2D1.DeviceContext D2DContext { get; private set; }
        public static BitmapInterpolationMode InterpolationMode { get; private set; } = BitmapInterpolationMode.NearestNeighbor;

        public static SharpD3D11RenderTarget CurrentTarget { get; private set; }
        private static SharpD3D11RenderTarget _backBufferTarget;
        public static SharpD3D11RenderTarget ScratchTarget { get; private set; }

        public static float Opacity { get; private set; } = 1F;
        public static bool Blending { get; private set; }
        public static float BlendRate { get; private set; } = 1F;
        public static BlendMode BlendMode { get; private set; } = BlendMode.NORMAL;
        public static CompositeMode CurrentCompositeMode { get; private set; } = CompositeMode.SourceOver;

        public static byte[] PalleteData { get; private set; }
        public static SharpD3D11TextureResource ColourPallete { get; private set; }

        public const int LightWidth = 1024;
        public const int LightHeight = 768;
        public static byte[] LightData { get; set; }
        public static SharpD3D11TextureResource LightTexture { get; private set; }
        public static SharpD3D11TextureResource PoisonTexture { get; private set; }

        public static SharpDXD3D11SpriteRenderer SpriteRenderer { get; private set; }

        private static bool _resetRequested;

        static SharpDXD3D11Manager()
        {
            ConfigureGraphics(Graphics);
        }

        public static void Create()
        {
            if (Device != null)
                return;

            D2DFactory = new SharpDX.Direct2D1.Factory1();

            Factory = new Factory();

            ApplyWindowStyle();

            if (CEnvir.Target != null && CEnvir.Target.ClientSize != Config.GameSize)
                CEnvir.Target.ClientSize = Config.GameSize;

            using Adapter adapter = Factory.GetAdapter1(0);
            using Output output = adapter.GetOutput(0);
            ModeDescription[] modes = output.GetDisplayModeList(Format.B8G8R8A8_UNorm, DisplayModeEnumerationFlags.Scaling);
            foreach (ModeDescription mode in modes)
            {
                Size size = new Size(mode.Width, mode.Height);
                if (size.Width < MinimumResolution.Width || size.Height < MinimumResolution.Height)
                    continue;

                if (!ValidResolutions.Contains(size))
                    ValidResolutions.Add(size);
            }
            ValidResolutions.Sort((s1, s2) => (s1.Width * s1.Height).CompareTo(s2.Width * s2.Height));

            Size targetSize = CEnvir.Target?.ClientSize ?? Config.GameSize;

            SwapChainDescription swapChainDescription = new SwapChainDescription
            {
                ModeDescription = new ModeDescription(targetSize.Width, targetSize.Height, new Rational(60, 1), Format.B8G8R8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Usage = Usage.RenderTargetOutput,
                BufferCount = 1,
                OutputHandle = CEnvir.Target.Handle,
                IsWindowed = !Config.FullScreen,
                SwapEffect = SwapEffect.Discard,
                Flags = SwapChainFlags.AllowModeSwitch
            };

            DeviceCreationFlags creationFlags = DeviceCreationFlags.BgraSupport;
            Device = new Device(adapter, creationFlags, new[] { D3DFeatureLevel.Level_11_0 });
            SwapChain = new SwapChain(Factory, Device, swapChainDescription);

            using (var dxgiDevice = Device.QueryInterface<SharpDX.DXGI.Device>())
            {
                D2DDevice = new SharpDX.Direct2D1.Device(D2DFactory, dxgiDevice);
                D2DContext = new SharpDX.Direct2D1.DeviceContext(D2DDevice, DeviceContextOptions.None)
                {
                    UnitMode = UnitMode.Pixels,
                    PrimitiveBlend = PrimitiveBlend.SourceOver
                };
            }

            SpriteRenderer = new SharpDXD3D11SpriteRenderer(Device);

            CreateTargets();
            LoadTextures();
            CenterOnSelectedMonitor();
        }

        public static void ConfigureGraphics(Graphics graphics)
        {
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = DrawingInterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.TextContrast = 0;
        }

        public static void RequestReset()
        {
            _resetRequested = true;
        }

        public static void AttemptReset()
        {
            if (!_resetRequested)
                return;

            _resetRequested = false;
            RecreateSwapChain(Config.GameSize);
        }

        public static void ToggleFullScreen()
        {
            if (SwapChain == null)
                return;

            bool isFullScreen = SwapChain.IsFullScreen;
            SwapChain.IsFullScreen = !isFullScreen;
            Config.FullScreen = SwapChain.IsFullScreen;
            ApplyWindowStyle();
            RequestReset();
        }

        public static void SetResolution(Size size)
        {
            if (CEnvir.Target.ClientSize == size && GetBackBufferSize() == size)
                return;

            Config.GameSize = size;

            if (CEnvir.Target != null && CEnvir.Target.ClientSize != size)
                CEnvir.Target.ClientSize = size;

            ApplyWindowStyle();
            RequestReset();
        }

        public static void SetGameWindowToMonitor(int monitorIndex)
        {
            if (monitorIndex < 0 || monitorIndex >= Screen.AllScreens.Length)
                monitorIndex = 0;

            Screen screen = Screen.AllScreens[monitorIndex];

            CEnvir.Target.Location = screen.Bounds.Location;
            RequestReset();
        }

        public static void CenterOnSelectedMonitor()
        {
            if (CEnvir.Target == null)
                return;

            int index = 0;

            Screen screen = Screen.AllScreens[index];
            int x = Math.Max(screen.Bounds.X, screen.Bounds.X + (screen.Bounds.Width - CEnvir.Target.Width) / 2);
            int y = Math.Max(screen.Bounds.Y, screen.Bounds.Y + (screen.Bounds.Height - CEnvir.Target.Height) / 2);
            CEnvir.Target.Location = new Point(x, y);
        }

        private static void ApplyWindowStyle()
        {
            if (CEnvir.Target == null)
                return;

            CEnvir.Target.FormBorderStyle = (Config.FullScreen || Config.Borderless) ? FormBorderStyle.None : FormBorderStyle.FixedDialog;
            CEnvir.Target.MaximizeBox = false;
        }

        public static void ColorFill(RenderSurface surface, Rectangle rect, System.Drawing.Color color)
        {
            if (!surface.IsValid)
                return;

            // Ensure the surface is a render target
            if (surface.NativeHandle is not SharpD3D11RenderTarget target)
                return;

            var d2d = D2DContext;

            // Use the existing render target
            d2d.Target = target.TargetBitmap;

            // Convert color
            var c = new RawColor4(
                color.R / 255f,
                color.G / 255f,
                color.B / 255f,
                color.A / 255f);

            using var brush = new SolidColorBrush(d2d, c);

            // Apply clip
            d2d.PushAxisAlignedClip(
                new RawRectangleF(rect.Left, rect.Top, rect.Right, rect.Bottom),
                AntialiasMode.Aliased);

            // Fill the clipped region
            d2d.FillRectangle(
                new RawRectangleF(rect.Left, rect.Top, rect.Right, rect.Bottom),
                brush);

            // Remove clip
            d2d.PopAxisAlignedClip();
        }

        public static void SetBlend(bool value, float rate = 1F, BlendMode mode = BlendMode.NORMAL)
        {
            Blending = value;
            BlendRate = rate;
            BlendMode = mode;

            if (D2DContext == null)
                return;

            if (!Blending)
            {
                CurrentCompositeMode = CompositeMode.SourceOver;
                D2DContext.PrimitiveBlend = PrimitiveBlend.SourceOver;
                return;
            }

            CurrentCompositeMode = GetCompositeModeForBlend(mode);
            D2DContext.PrimitiveBlend = GetPrimitiveBlendForMode(mode);
        }

        private static CompositeMode GetCompositeModeForBlend(BlendMode mode)
        {
            switch (mode)
            {
                case BlendMode.NONE:
                    return CompositeMode.SourceOver;
                case BlendMode.NORMAL:
                    return CompositeMode.SourceOver;
                case BlendMode.LIGHT:
                    return CompositeMode.SourceOver;
                case BlendMode.LIGHTINV:
                    return CompositeMode.SourceOver;
                case BlendMode.INVNORMAL:
                    return CompositeMode.SourceOver;
                case BlendMode.INVLIGHT:
                    return CompositeMode.SourceOver;
                case BlendMode.INVLIGHTINV:
                    return CompositeMode.SourceOver;
                case BlendMode.INVCOLOR:
                    return CompositeMode.SourceOver;
                case BlendMode.INVBACKGROUND:
                    return CompositeMode.SourceOver;
                case BlendMode.COLORFY:
                    return CompositeMode.SourceOver;
                case BlendMode.MASK:
                    return CompositeMode.DestinationOut;
                case BlendMode.HIGHLIGHT:
                    return CompositeMode.SourceOver;
                case BlendMode.EFFECTMASK:
                    return CompositeMode.SourceAtop;
                case BlendMode.LIGHTMAP:
                    // Direct2D's CompositeMode lacks a multiply option; fall back to a destination mask that
                    // mirrors the D3D9 lightmap behavior of modulating the existing target with the source alpha.
                    return CompositeMode.DestinationIn;
                default:
                    return CompositeMode.SourceOver;
            }
        }

        private static PrimitiveBlend GetPrimitiveBlendForMode(BlendMode mode)
        {
            switch (mode)
            {
                case BlendMode.NONE:
                    return PrimitiveBlend.SourceOver;
                case BlendMode.NORMAL:
                    return PrimitiveBlend.SourceOver;
                case BlendMode.LIGHT:
                    return PrimitiveBlend.Add;
                case BlendMode.LIGHTINV:
                    return PrimitiveBlend.Add;
                case BlendMode.INVNORMAL:
                    return PrimitiveBlend.SourceOver;
                case BlendMode.INVLIGHT:
                    return PrimitiveBlend.Add;
                case BlendMode.INVLIGHTINV:
                    return PrimitiveBlend.Add;
                case BlendMode.INVCOLOR:
                    return PrimitiveBlend.SourceOver;
                case BlendMode.INVBACKGROUND:
                    return PrimitiveBlend.SourceOver;
                case BlendMode.COLORFY:
                    return PrimitiveBlend.Add;
                case BlendMode.MASK:
                    return PrimitiveBlend.SourceOver;
                case BlendMode.HIGHLIGHT:
                    return PrimitiveBlend.Add;
                case BlendMode.EFFECTMASK:
                    return PrimitiveBlend.SourceOver;
                case BlendMode.LIGHTMAP:
                    return PrimitiveBlend.SourceOver;
                default:
                    return PrimitiveBlend.SourceOver;
            }
        }

        public static void SetOpacity(float opacity)
        {
            if (Math.Abs(Opacity - opacity) < float.Epsilon)
                return;

            Opacity = opacity;
        }

        public static void SetTextureFilter(TextureFilterMode mode)
        {
            InterpolationMode = mode switch
            {
                TextureFilterMode.Linear => BitmapInterpolationMode.Linear,
                TextureFilterMode.None => BitmapInterpolationMode.NearestNeighbor,
                _ => BitmapInterpolationMode.NearestNeighbor
            };
        }

        public static void FlushSprite()
        {
            D2DContext?.Flush();
        }

        public static void MemoryClear()
        {
            if (D2DContext != null)
                D2DContext.Target = _backBufferTarget?.TargetBitmap;

            for (int i = ControlList.Count - 1; i >= 0; i--)
            {
                if (CEnvir.Now < ControlList[i].ExpireTime)
                    continue;

                ControlList[i].TextureValid = false;
            }

            for (int i = TextureList.Count - 1; i >= 0; i--)
            {
                if (CEnvir.Now < TextureList[i].ExpireTime)
                    continue;

                TextureList[i].DisposeTexture();
            }

            for (int i = SoundList.Count - 1; i >= 0; i--)
            {
                if (CEnvir.Now < SoundList[i].ExpireTime)
                    continue;

                SoundList[i].DisposeSoundBuffer();
            }
        }

        public static void Unload()
        {
            for (int i = ControlList.Count - 1; i >= 0; i--)
                ControlList[i].DisposeTexture();

            ControlList.Clear();

            for (int i = TextureList.Count - 1; i >= 0; i--)
                TextureList[i].DisposeTexture();

            TextureList.Clear();

            for (int i = SoundList.Count - 1; i >= 0; i--)
                SoundList[i].DisposeSoundBuffer();

            SoundList.Clear();

            DisposeTargets();

            SpriteRenderer?.Dispose();
            SpriteRenderer = null;

            ColourPallete?.Dispose();
            ColourPallete = null;

            LightTexture?.Dispose();
            LightTexture = null;

            PoisonTexture?.Dispose();
            PoisonTexture = null;

            D2DContext?.Dispose();
            D2DContext = null;

            D2DDevice?.Dispose();
            D2DDevice = null;

            D2DFactory?.Dispose();
            D2DFactory = null;

            SwapChain?.Dispose();
            SwapChain = null;

            Device?.Dispose();
            Device = null;

            Factory?.Dispose();
            Factory = null;
        }

        public static void SetRenderTarget(SharpD3D11RenderTarget target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (CurrentTarget == target)
                return;

            CurrentTarget = target;
            Context.OutputMerger.SetRenderTargets(target.RenderTargetView);
            D2DContext.Target = target.TargetBitmap;
        }

        public static void BeginDraw(Color clearColor)
        {
            if (Context == null || D2DContext == null || CurrentTarget == null)
                return;

            RawColor4 clear = new RawColor4(clearColor.R / 255f, clearColor.G / 255f, clearColor.B / 255f, clearColor.A / 255f);
            Context.ClearRenderTargetView(CurrentTarget.RenderTargetView, clear);
            D2DContext.BeginDraw();
            D2DContext.Transform = new RawMatrix3x2 { M11 = 1, M22 = 1 };
            D2DContext.Clear(clear);
        }

        public static void EndDraw()
        {
            if (D2DContext == null || SwapChain == null)
                return;

            D2DContext.EndDraw();
            SwapChain.Present(Config.VSync ? 1 : 0, PresentFlags.None);
        }

        public static SharpD3D11RenderTarget CreateRenderTarget(Size size)
        {
            Texture2D texture = new Texture2D(Device, new Texture2DDescription
            {
                Width = size.Width,
                Height = size.Height,
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Usage = ResourceUsage.Default,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.Shared
            });

            RenderTargetView view = new RenderTargetView(Device, texture);
            using Surface surface = texture.QueryInterface<Surface>();
            Bitmap1 bitmap = new Bitmap1(D2DContext, surface, new BitmapProperties1(new D2D1PixelFormat(Format.B8G8R8A8_UNorm, D2D1AlphaMode.Premultiplied), 96, 96, BitmapOptions.Target));

            return new SharpD3D11RenderTarget(texture, view, bitmap);
        }

        public static void ReleaseRenderTarget(SharpD3D11RenderTarget renderTarget)
        {
            renderTarget?.Dispose();
        }

        public static SharpD3D11TextureResource CreateTexture(Size size, RenderTextureFormat format, RenderTextureUsage usage, RenderTexturePool pool)
        {
            Format dxFormat = ConvertFormat(format);

            BindFlags bindFlags = BindFlags.ShaderResource;
            if (usage == RenderTextureUsage.RenderTarget)
                bindFlags |= BindFlags.RenderTarget;

            ResourceUsage resourceUsage = usage == RenderTextureUsage.Dynamic ? ResourceUsage.Dynamic : ResourceUsage.Default;
            CpuAccessFlags cpuAccessFlags = usage == RenderTextureUsage.Dynamic ? CpuAccessFlags.Write : CpuAccessFlags.None;

            ResourceOptionFlags optionFlags = usage == RenderTextureUsage.Dynamic ? ResourceOptionFlags.None : ResourceOptionFlags.Shared;

            Texture2D texture = new Texture2D(Device, new Texture2DDescription
            {
                Width = size.Width,
                Height = size.Height,
                ArraySize = 1,
                BindFlags = bindFlags,
                Usage = resourceUsage,
                CpuAccessFlags = cpuAccessFlags,
                Format = dxFormat,
                SampleDescription = new SampleDescription(1, 0),
                MipLevels = 1,
                OptionFlags = optionFlags
            });


            Texture2D staging = new Texture2D(Device, new Texture2DDescription
            {
                Width = size.Width,
                Height = size.Height,
                ArraySize = 1,
                BindFlags = BindFlags.None,
                Usage = ResourceUsage.Staging,
                CpuAccessFlags = CpuAccessFlags.Read | CpuAccessFlags.Write,
                Format = dxFormat,
                SampleDescription = new SampleDescription(1, 0),
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None
            });

            using Surface surface = texture.QueryInterface<Surface>();
            Bitmap1 bitmap = new Bitmap1(D2DContext, surface, new BitmapProperties1(new D2D1PixelFormat(dxFormat, D2D1AlphaMode.Premultiplied), 96, 96, BitmapOptions.None));

            return new SharpD3D11TextureResource(format, texture, staging, bitmap);
        }

        public static void ReleaseTexture(SharpD3D11TextureResource texture)
        {
            texture?.Dispose();
        }

        public static TextureLock LockTexture(SharpD3D11TextureResource texture, TextureLockMode mode)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            MapMode mapMode = mode == TextureLockMode.ReadOnly ? MapMode.Read : MapMode.Write;

            // Block-compressed handler (DXT1 / DXT5)
            if ((texture.Format == RenderTextureFormat.Dxt1 ||
                 texture.Format == RenderTextureFormat.Dxt5) &&
                mode != TextureLockMode.ReadOnly)
            {
                return CreateBlockCompressedWriteLock(texture, mapMode);
            }

            // Read-only: copy GPU → staging
            if (mode == TextureLockMode.ReadOnly)
                Context.CopyResource(texture.Texture, texture.StagingTexture);

            // Map staging texture
            D3D11DataBox box = Context.MapSubresource(texture.StagingTexture, 0, mapMode, D3D11MapFlags.None);

            int width = texture.Texture.Description.Width;
            int height = texture.Texture.Description.Height;

            // A8R8G8B8 WRITE PATH (row-by-row safe)
            if (mode != TextureLockMode.ReadOnly &&
                texture.Format == RenderTextureFormat.A8R8G8B8)
            {
                int rowSize = width * 4;
                int totalSize = rowSize * height;

                byte[] buffer = new byte[totalSize];
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                IntPtr ptr = handle.AddrOfPinnedObject();

                // Return a lock pointing to the tightly-packed buffer (caller fills it)
                return TextureLock.From(ptr, rowSize, () =>
                {
                    try
                    {
                        // Re-map for writing the final data into staging
                        D3D11DataBox writeBox = Context.MapSubresource(
                            texture.StagingTexture, 0, mapMode, D3D11MapFlags.None);

                        try
                        {
                            IntPtr dest = writeBox.DataPointer;
                            int destPitch = writeBox.RowPitch;

                            // Copy each row respecting RowPitch
                            for (int y = 0; y < height; y++)
                            {
                                int srcOffset = y * rowSize;
                                Marshal.Copy(buffer, srcOffset, dest, rowSize);
                                dest += destPitch;
                            }
                        }
                        finally
                        {
                            Context.UnmapSubresource(texture.StagingTexture, 0);
                        }

                        // Copy staging → GPU
                        Context.CopyResource(texture.StagingTexture, texture.Texture);
                    }
                    finally
                    {
                        handle.Free();
                    }
                });
            }

            // All other formats OR read-only path:
            // Direct pointer access into mapped staging texture.
            return TextureLock.From(box.DataPointer, box.RowPitch, () =>
            {
                Context.UnmapSubresource(texture.StagingTexture, 0);

                if (mode != TextureLockMode.ReadOnly)
                    Context.CopyResource(texture.StagingTexture, texture.Texture);
            });
        }

        private static TextureLock CreateBlockCompressedWriteLock(SharpD3D11TextureResource texture, MapMode mapMode)
        {
            // Texture dimensions in pixels
            int width = texture.Texture.Description.Width;
            int height = texture.Texture.Description.Height;

            // DXT1:  8 bytes per 4x4 block
            // DXT5: 16 bytes per 4x4 block
            int blockSize = texture.Format == RenderTextureFormat.Dxt1 ? 8 : 16;

            // Number of blocks
            int blocksX = Math.Max(1, (width + 3) / 4);
            int blocksY = Math.Max(1, (height + 3) / 4);

            // Size of user buffer (tightly packed, no padding)
            int tightRowSize = blocksX * blockSize;
            int dataSize = tightRowSize * blocksY;

            // Managed buffer the caller will write into via Marshal.Copy
            byte[] buffer = new byte[dataSize];
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            IntPtr ptr = handle.AddrOfPinnedObject();

            // We return a TextureLock whose DataPointer points to our managed buffer.
            // On Dispose, we copy that into the mapped staging texture honoring RowPitch.
            return TextureLock.From(ptr, tightRowSize, () =>
            {
                try
                {
                    // Map staging texture (BC1 / BC3)
                    D3D11DataBox box = Context.MapSubresource(texture.StagingTexture, 0, mapMode, D3D11MapFlags.None);

                    try
                    {
                        IntPtr destPtr = box.DataPointer;
                        int destPitch = box.RowPitch; // bytes per row of BLOCKS (can be > tightRowSize)

                        // Copy row by row to account for any driver padding
                        for (int row = 0; row < blocksY; row++)
                        {
                            int srcOffset = row * tightRowSize;
                            Marshal.Copy(buffer, srcOffset, destPtr, tightRowSize);
                            destPtr += destPitch;
                        }
                    }
                    finally
                    {
                        Context.UnmapSubresource(texture.StagingTexture, 0);
                    }

                    // Push staging → GPU texture
                    Context.CopyResource(texture.StagingTexture, texture.Texture);
                }
                finally
                {
                    handle.Free();
                }
            });
        }

        private static unsafe void PremultiplyAlpha(IntPtr dataPointer, int rowPitch, int width, int height)
        {
            byte* row = (byte*)dataPointer;

            for (int y = 0; y < height; y++)
            {
                byte* rowStart = row;
                byte* pixel = rowStart;

                byte lastB = 0, lastG = 0, lastR = 0;
                bool hasColor = false;

                for (int x = 0; x < width; x++)
                {
                    byte alpha = pixel[3];

                    if (alpha > 0)
                    {
                        lastB = pixel[0];
                        lastG = pixel[1];
                        lastR = pixel[2];
                        hasColor = true;
                    }
                    else if (hasColor)
                    {
                        pixel[0] = lastB;
                        pixel[1] = lastG;
                        pixel[2] = lastR;
                    }

                    pixel += 4;
                }

                if (width > 0)
                {
                    pixel = rowStart + (width - 1) * 4;
                    lastB = lastG = lastR = 0;
                    hasColor = false;

                    for (int x = width - 1; x >= 0; x--)
                    {
                        byte alpha = pixel[3];

                        if (alpha > 0)
                        {
                            lastB = pixel[0];
                            lastG = pixel[1];
                            lastR = pixel[2];
                            hasColor = true;
                        }
                        else if (hasColor)
                        {
                            pixel[0] = lastB;
                            pixel[1] = lastG;
                            pixel[2] = lastR;
                        }

                        pixel -= 4;
                    }
                }

                pixel = rowStart;

                for (int x = 0; x < width; x++)
                {
                    byte alpha = pixel[3];

                    pixel[0] = (byte)(pixel[0] * alpha / 255);
                    pixel[1] = (byte)(pixel[1] * alpha / 255);
                    pixel[2] = (byte)(pixel[2] * alpha / 255);

                    pixel += 4;
                }

                row += rowPitch;
            }
        }

        private static void PremultiplyAlpha(Span<byte> data, int width, int height)
        {
            int stride = width * 4;

            for (int y = 0; y < height; y++)
            {
                Span<byte> row = data.Slice(y * stride, stride);
                byte lastB = 0, lastG = 0, lastR = 0;
                bool hasColor = false;

                for (int x = 0; x < stride; x += 4)
                {
                    byte alpha = row[x + 3];

                    if (alpha > 0)
                    {
                        lastB = row[x + 0];
                        lastG = row[x + 1];
                        lastR = row[x + 2];
                        hasColor = true;
                    }
                    else if (hasColor)
                    {
                        row[x + 0] = lastB;
                        row[x + 1] = lastG;
                        row[x + 2] = lastR;
                    }
                }

                if (stride > 0)
                {
                    lastB = lastG = lastR = 0;
                    hasColor = false;

                    for (int x = stride - 4; x >= 0; x -= 4)
                    {
                        byte alpha = row[x + 3];

                        if (alpha > 0)
                        {
                            lastB = row[x + 0];
                            lastG = row[x + 1];
                            lastR = row[x + 2];
                            hasColor = true;
                        }
                        else if (hasColor)
                        {
                            row[x + 0] = lastB;
                            row[x + 1] = lastG;
                            row[x + 2] = lastR;
                        }
                    }
                }

                for (int x = 0; x < stride; x += 4)
                {
                    byte alpha = row[x + 3];

                    row[x + 0] = (byte)(row[x + 0] * alpha / 255);
                    row[x + 1] = (byte)(row[x + 1] * alpha / 255);
                    row[x + 2] = (byte)(row[x + 2] * alpha / 255);
                }
            }
        }

        private static void DecodeRgb565(ushort color, Span<byte> destination)
        {
            byte r = (byte)(((color >> 11) & 0x1F) * 255 / 31);
            byte g = (byte)(((color >> 5) & 0x3F) * 255 / 63);
            byte b = (byte)((color & 0x1F) * 255 / 31);

            destination[0] = b;
            destination[1] = g;
            destination[2] = r;
            destination[3] = 255;
        }

        public static Color HSLToRGB(float h, float s, float l)
        {
            float r, g, b;

            if (Math.Abs(s) < float.Epsilon)
            {
                r = g = b = l;
            }
            else
            {
                float q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                float p = 2 * l - q;
                r = HueToRGB(p, q, h + 1f / 3f);
                g = HueToRGB(p, q, h);
                b = HueToRGB(p, q, h - 1f / 3f);
            }

            return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }

        private static float HueToRGB(float p, float q, float t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1f / 6f) return p + (q - p) * 6f * t;
            if (t < 1f / 2f) return q;
            if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
            return p;
        }

        public static Size GetBackBufferSize()
        {
            if (_backBufferTarget?.Texture == null)
                return Size.Empty;

            return new Size(_backBufferTarget.Texture.Description.Width, _backBufferTarget.Texture.Description.Height);
        }

        public static void ClearColourPalette()
        {
            ColourPallete?.Dispose();
            ColourPallete = null;
        }

        private static void CreateTargets()
        {
            DisposeTargets();

            _backBufferTarget = CreateBackBufferTarget();
            ScratchTarget = CreateRenderTarget(GetBackBufferSize());
            SetRenderTarget(_backBufferTarget);
        }

        private static SharpD3D11RenderTarget CreateBackBufferTarget()
        {
            Texture2D backBuffer = SwapChain.GetBackBuffer<Texture2D>(0);
            RenderTargetView view = new RenderTargetView(Device, backBuffer);

            using Surface surface = backBuffer.QueryInterface<Surface>();
            Bitmap1 bitmap = new Bitmap1(D2DContext, surface, new BitmapProperties1(new D2D1PixelFormat(Format.B8G8R8A8_UNorm, D2D1AlphaMode.Premultiplied), 96, 96, BitmapOptions.Target | BitmapOptions.CannotDraw));

            return new SharpD3D11RenderTarget(backBuffer, view, bitmap);
        }

        private static void DisposeTargets()
        {
            if (Context != null)
                Context.OutputMerger.SetRenderTargets((RenderTargetView)null);

            if (D2DContext != null)
                D2DContext.Target = null;

            ScratchTarget?.Dispose();
            ScratchTarget = null;

            _backBufferTarget?.Dispose();
            _backBufferTarget = null;

            CurrentTarget = null;
        }

        private static void RecreateSwapChain(Size size)
        {
            DisposeTargets();

            ApplyWindowStyle();

            if (CEnvir.Target != null && CEnvir.Target.ClientSize != size)
                CEnvir.Target.ClientSize = size;

            Size targetSize = CEnvir.Target?.ClientSize ?? size;

            if (SwapChain.IsFullScreen)
            {
                ModeDescription targetDescription = SwapChain.Description.ModeDescription;
                targetDescription.Width = targetSize.Width;
                targetDescription.Height = targetSize.Height;
                targetDescription.RefreshRate = targetDescription.RefreshRate.Numerator == 0 ? new Rational(60, 1) : targetDescription.RefreshRate;
                targetDescription.Scaling = DisplayModeScaling.Unspecified;

                SwapChain.ResizeTarget(ref targetDescription);
            }

            SwapChain.ResizeBuffers(1, targetSize.Width, targetSize.Height, Format.B8G8R8A8_UNorm, SwapChainFlags.AllowModeSwitch);
            CreateTargets();
        }

        private static void LoadTextures()
        {
            const string path = @".\\Data\\Pallete.png";
            if (File.Exists(path))
            {
                using DrawingBitmap pallete = new DrawingBitmap(path);
                BitmapData data = pallete.LockBits(new Rectangle(Point.Empty, pallete.Size), ImageLockMode.ReadOnly, DrawingPixelFormat.Format32bppArgb);

                PalleteData = new byte[pallete.Width * pallete.Height * 4];
                Marshal.Copy(data.Scan0, PalleteData, 0, PalleteData.Length);

                pallete.UnlockBits(data);

                ColourPallete = CreateTexture(new Size(pallete.Width, pallete.Height), RenderTextureFormat.A8R8G8B8, RenderTextureUsage.None, RenderTexturePool.Managed);
                using TextureLock textureLock = LockTexture(ColourPallete, TextureLockMode.Discard);
                Marshal.Copy(PalleteData, 0, textureLock.DataPointer, PalleteData.Length);
            }

            if (LightData == null)
                LightData = LightGenerator.CreateLightData(LightWidth, LightHeight);

            LightTexture = CreateTexture(new Size(LightWidth, LightHeight), RenderTextureFormat.A8R8G8B8, RenderTextureUsage.None, RenderTexturePool.Managed);
            using (TextureLock textureLock = LockTexture(LightTexture, TextureLockMode.Discard))
            {
                Marshal.Copy(LightData, 0, textureLock.DataPointer, LightData.Length);
            }

            PoisonTexture = CreateTexture(new Size(48, 48), RenderTextureFormat.A8R8G8B8, RenderTextureUsage.None, RenderTexturePool.Managed);
            using TextureLock poisonLock = LockTexture(PoisonTexture, TextureLockMode.Discard);
            Span<byte> poisonData = stackalloc byte[48 * 48 * 4];
            Marshal.Copy(poisonData.ToArray(), 0, poisonLock.DataPointer, poisonData.Length);
        }

        private static Format ConvertFormat(RenderTextureFormat format)
        {
            return format switch
            {
                RenderTextureFormat.A8R8G8B8 => Format.B8G8R8A8_UNorm,
                RenderTextureFormat.Dxt1 => Format.BC1_UNorm,
                RenderTextureFormat.Dxt5 => Format.BC3_UNorm,
                _ => Format.B8G8R8A8_UNorm
            };
        }
    }
}
