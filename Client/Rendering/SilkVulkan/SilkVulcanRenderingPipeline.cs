using Client.Controls;
using Client.Envir;
using Client.Helpers;
using Silk.NET.Core;
using Silk.NET.Shaderc;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using static Client.Rendering.RenderingPipelineManager;
using Buffer = Silk.NET.Vulkan.Buffer;
using ClientBlendMode = Client.Rendering.BlendMode;

using VkImage = Silk.NET.Vulkan.Image;
using VkImageLayout = Silk.NET.Vulkan.ImageLayout;
using VkImageType = Silk.NET.Vulkan.ImageType;
using VkSemaphore = Silk.NET.Vulkan.Semaphore;

namespace Client.Rendering.SilkVulkan
{
    public sealed unsafe class SilkVulkanRenderingPipeline : IRenderingPipeline
    {
        private const int MaxFramesInFlight = 2;
        private const int LightWidth = 1024;
        private const int LightHeight = 768;
        private const int PoisonSize = 6;
        private const uint MaxSpriteBatchInstances = 8192;
        private const int EnumCurrentSettings = -1;
        private const int CdsFullscreen = 0x00000004;
        private const int DispChangeSuccessful = 0;
        private const int DmPelsWidth = 0x00080000;
        private const int DmPelsHeight = 0x00100000;
        private const ulong DynamicVertexBufferSize = 16 * 1024 * 1024;
        private const uint MaxTextureDescriptors = 8192;
        private static readonly Size MinimumResolution = new Size(1024, 768);
        private static bool _forceNextSwapchainFifoPresentMode;

        private readonly List<WeakReference<DXControl>> _controlCache = new();
        private readonly List<MirImage> _textureCache = new();
        private readonly List<DXSound> _soundCache = new();
        private readonly List<SilkVulkanRenderTarget> _renderTargets = new();
        private readonly List<SilkVulkanTextureResource> _textures = new();
        private readonly Queue<DeferredDisposal> _deferredDisposals = new();
        private readonly Dictionary<ClientBlendMode, Pipeline> _texturePipelines = new();
        private readonly List<DescriptorPool> _descriptorPools = new();

        private RenderingPipelineContext _context;
        private Graphics _graphics;
        private Vk _vk;
        private KhrSurface _surfaceApi;
        private KhrWin32Surface _win32SurfaceApi;
        private KhrSwapchain _swapchainApi;
        private Instance _instance;
        private SurfaceKHR _surface;
        private PhysicalDevice _physicalDevice;
        private PhysicalDeviceMemoryProperties _memoryProperties;
        private Device _device;
        private Silk.NET.Vulkan.Queue _graphicsQueue;
        private uint _graphicsQueueFamilyIndex;
        private bool _supportsWideLines;
        private SwapchainKHR _swapchain;
        private Format _swapchainFormat;
        private Extent2D _swapchainExtent;
        private VkImage[] _swapchainImages = Array.Empty<VkImage>();
        private SilkVulkanRenderTarget[] _backBufferTargets = Array.Empty<SilkVulkanRenderTarget>();
        private RenderPass _renderPass;
        private CommandPool _frameCommandPool;
        private CommandPool _uploadCommandPool;
        private DescriptorSetLayout _textureDescriptorSetLayout;
        private DescriptorPool _descriptorPool;
        private Sampler _pointSampler;
        private Sampler _linearSampler;
        private PipelineLayout _texturePipelineLayout;
        private PipelineLayout _linePipelineLayout;
        private Pipeline _linePipeline;
        private FrameResources[] _frames;
        private int _frameIndex;
        private FrameResources _activeFrame;
        private CommandBuffer _activeCommandBuffer;
        private bool _frameActive;
        private bool _renderPassActive;
        private SilkVulkanRenderTarget _currentTarget;
        private SilkVulkanRenderTarget _recordingTarget;
        private SilkVulkanRenderTarget _currentBackBufferTarget;
        private SilkVulkanRenderTarget _scratchTarget;
        private SilkVulkanTextureResource _colourPalette;
        private SilkVulkanTextureResource _lightTexture;
        private SilkVulkanTextureResource _poisonTexture;
        private byte[] _paletteData;
        private byte[] _lightData;
        private float _opacity = 1F;
        private bool _blending;
        private float _blendRate = 1F;
        private ClientBlendMode _blendMode = ClientBlendMode.NORMAL;
        private float _lineWidth = 1F;
        private TextureFilterMode _textureFilter = TextureFilterMode.Point;
        private Pipeline _boundPipeline;
        private ClientBlendMode? _boundTextureBlendMode;
        private float _boundTextureBlendRate = float.NaN;
        private DescriptorSet _boundDescriptorSet;
        private SilkVulkanTextureResource _spriteBatchTexture;
        private SilkVulkanRenderTarget _spriteBatchTarget;
        private ClientBlendMode _spriteBatchBlendMode;
        private float _spriteBatchBlendRate;
        private ulong _spriteBatchOffset;
        private uint _spriteBatchInstanceCount;
        private bool _displayModeChanged;
        private string _displayModeDeviceName;
        private Size _displayModeSize;

        public string Id => RenderingPipelineIds.SilkVulkan;

        internal static void ForceNextSwapchainFifoPresentMode()
        {
            _forceNextSwapchainFifoPresentMode = true;
        }

        public void Initialize(RenderingPipelineContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _graphics = Graphics.FromHwnd(IntPtr.Zero);
            ConfigureGraphics(_graphics);

            ApplyWindowStyle();
            ApplyWindowBounds(true);

            _vk = Vk.GetApi();
            CreateInstance();
            CreateSurface();
            PickPhysicalDevice();
            CreateLogicalDevice();
            CreateCommandPools();
            CreateDescriptorResources();
            CreateFrameResources();
            CreateSwapchain();
            CreateRenderPass();
            CreateSwapchainFramebuffers();
            CreateGraphicsPipelines();

            Size size = GetTargetSize();
            _scratchTarget = CreateRenderTargetCore(size);
            LoadTextures();
            _currentTarget = _backBufferTargets.Length > 0 ? _backBufferTargets[0] : null;
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

            Application.Idle += Tick;
            try
            {
                Application.Run(form);
            }
            finally
            {
                Application.Idle -= Tick;
            }
        }

        public bool RenderFrame(Action drawScene)
        {
            if (drawScene == null)
                throw new ArgumentNullException(nameof(drawScene));

            if (_backBufferTargets.Length == 0)
                return false;

            FrameResources frame = _frames[_frameIndex];

            try
            {
                Fence inFlightFence = frame.InFlightFence;
                Check(_vk.WaitForFences(_device, 1, in inFlightFence, true, ulong.MaxValue), "wait for frame fence");
                FlushDeferredDisposals();

                ResizeBackBufferIfNeeded();

                uint imageIndex = 0;
                Result acquire = _swapchainApi.AcquireNextImage(_device, _swapchain, ulong.MaxValue, frame.ImageAvailableSemaphore, default, ref imageIndex);
                if (acquire == Result.ErrorOutOfDateKhr)
                {
                    RecreateSwapchain();
                    return false;
                }
                Check(acquire, "acquire Vulkan swapchain image", Result.SuboptimalKhr);

                _activeFrame = frame;
                _activeCommandBuffer = frame.CommandBuffer;
                _activeFrame.VertexOffset = 0;
                _currentBackBufferTarget = _backBufferTargets[imageIndex];
                _currentTarget = _currentBackBufferTarget;
                _recordingTarget = null;
                _renderPassActive = false;
                _boundPipeline = default;
                _boundTextureBlendMode = null;
                _boundTextureBlendRate = float.NaN;
                _boundDescriptorSet = default;
                ResetSpriteBatch();
                _frameActive = true;

                Check(_vk.ResetCommandBuffer(_activeCommandBuffer, 0), "reset Vulkan command buffer");
                CommandBufferBeginInfo beginInfo = new CommandBufferBeginInfo
                {
                    SType = StructureType.CommandBufferBeginInfo,
                    Flags = CommandBufferUsageFlags.CommandBufferUsageOneTimeSubmitBit
                };
                Check(_vk.BeginCommandBuffer(_activeCommandBuffer, in beginInfo), "begin Vulkan command buffer");

                BeginRenderPass(_currentTarget);
                Clear(RenderClearFlags.Target, Color.Black, 0, 0);
                drawScene();
                EndRenderPass();

                TransitionTarget(_currentBackBufferTarget, VkImageLayout.PresentSrcKhr, AccessFlags.AccessColorAttachmentWriteBit, 0, PipelineStageFlags.PipelineStageColorAttachmentOutputBit, PipelineStageFlags.PipelineStageBottomOfPipeBit);
                Check(_vk.EndCommandBuffer(_activeCommandBuffer), "end Vulkan command buffer");

                PipelineStageFlags waitStage = PipelineStageFlags.PipelineStageColorAttachmentOutputBit;
                VkSemaphore imageAvailableSemaphore = frame.ImageAvailableSemaphore;
                VkSemaphore renderFinishedSemaphore = frame.RenderFinishedSemaphore;
                CommandBuffer commandBuffer = _activeCommandBuffer;
                SubmitInfo submitInfo = new SubmitInfo
                {
                    SType = StructureType.SubmitInfo,
                    WaitSemaphoreCount = 1,
                    PWaitSemaphores = &imageAvailableSemaphore,
                    PWaitDstStageMask = &waitStage,
                    CommandBufferCount = 1,
                    PCommandBuffers = &commandBuffer,
                    SignalSemaphoreCount = 1,
                    PSignalSemaphores = &renderFinishedSemaphore
                };
                Check(_vk.ResetFences(_device, 1, in inFlightFence), "reset frame fence");
                Check(_vk.QueueSubmit(_graphicsQueue, 1, in submitInfo, inFlightFence), "submit Vulkan frame");

                SwapchainKHR swapchain = _swapchain;
                PresentInfoKHR presentInfo = new PresentInfoKHR
                {
                    SType = StructureType.PresentInfoKhr,
                    WaitSemaphoreCount = 1,
                    PWaitSemaphores = &renderFinishedSemaphore,
                    SwapchainCount = 1,
                    PSwapchains = &swapchain,
                    PImageIndices = &imageIndex
                };

                Result present = _swapchainApi.QueuePresent(_graphicsQueue, in presentInfo);
                if (present == Result.ErrorOutOfDateKhr || present == Result.SuboptimalKhr)
                    RecreateSwapchain();
                else
                    Check(present, "present Vulkan frame");

                _frameIndex = (_frameIndex + 1) % _frames.Length;
                return true;
            }
            catch (Exception ex)
            {
                CEnvir.SaveException(ex);
                Thread.Sleep(1);
                return false;
            }
            finally
            {
                _frameActive = false;
                _renderPassActive = false;
                _recordingTarget = null;
                _activeCommandBuffer = default;
                _activeFrame = null;
            }
        }

        public void ToggleFullScreen()
        {
            Config.FullScreen = !Config.FullScreen;

            if (DXConfigWindow.ActiveConfig?.FullScreenCheckBox != null)
                DXConfigWindow.ActiveConfig.FullScreenCheckBox.Checked = Config.FullScreen;

            ApplyWindowStyle();
            if (!ApplyWindowBounds(true))
            {
                Config.FullScreen = !Config.FullScreen;
                if (DXConfigWindow.ActiveConfig?.FullScreenCheckBox != null)
                    DXConfigWindow.ActiveConfig.FullScreenCheckBox.Checked = Config.FullScreen;

                ApplyWindowStyle();
                ApplyWindowBounds(true);
            }

            ResizeBackBufferIfNeeded(true);
        }

        public void SetResolution(Size size)
        {
            bool needsResize = _context.RenderTarget.ClientSize != size;
            if (!needsResize && size == Config.GameSize)
                return;

            Config.GameSize = size;
            if (needsResize && !Config.FullScreen)
                _context.RenderTarget.ClientSize = size;

            ApplyWindowStyle();
            ApplyWindowBounds();
            ResizeBackBufferIfNeeded(true);
        }

        public void SetTargetMonitor(int monitorIndex)
        {
            //if (monitorIndex < 0 || monitorIndex >= Screen.AllScreens.Length)
                monitorIndex = 0;

            //Config.SelectedMonitorIndex = monitorIndex;
            ApplyWindowStyle();
            ApplyWindowBounds(true);
            ResizeBackBufferIfNeeded(true);
        }

        public void CenterOnSelectedMonitor()
        {
            if (_context?.RenderTarget == null)
                return;

            Screen selectedScreen = GetSelectedScreen();
            Rectangle bounds = selectedScreen.Bounds;
            int x = bounds.X + (bounds.Width - _context.RenderTarget.Width) / 2;
            int y = bounds.Y + (bounds.Height - _context.RenderTarget.Height) / 2;
            _context.RenderTarget.Location = new Point(x, y);
        }

        public void ResetDevice()
        {
            ApplyWindowStyle();
            ApplyWindowBounds();
            ResizeBackBufferIfNeeded(true);
        }

        public void OnSceneChanged(bool isGameScene)
        {
            if (!isGameScene)
                return;

            ApplyWindowBounds();
            ResizeBackBufferIfNeeded(true);
        }

        public IReadOnlyList<Size> GetSupportedResolutions()
        {
            List<Size> sizes = new List<Size>();
            Screen screen = GetSelectedScreen();

            for (int modeIndex = 0; ; modeIndex++)
            {
                DevMode mode = CreateDevMode();
                if (!EnumDisplaySettings(screen.DeviceName, modeIndex, ref mode))
                    break;

                Size size = new Size((int)mode.dmPelsWidth, (int)mode.dmPelsHeight);
                if (size.Width < MinimumResolution.Width || size.Height < MinimumResolution.Height)
                    continue;

                if (!sizes.Contains(size))
                    sizes.Add(size);
            }

            if (sizes.Count == 0)
            {
                foreach (Screen fallbackScreen in Screen.AllScreens)
                {
                    Size size = fallbackScreen.Bounds.Size;
                    if (size.Width >= MinimumResolution.Width && size.Height >= MinimumResolution.Height && !sizes.Contains(size))
                        sizes.Add(size);
                }
            }

            if (!sizes.Contains(Config.GameSize))
                sizes.Add(Config.GameSize);

            sizes.Sort((s1, s2) => (s1.Width * s1.Height).CompareTo(s2.Width * s2.Height));
            return sizes;
        }

        public Size MeasureText(string text, Font font)
        {
            return TextRenderer.MeasureText(_graphics, text, font);
        }

        public Size MeasureText(string text, Font font, Size proposedSize, TextFormatFlags format)
        {
            return TextRenderer.MeasureText(_graphics, text, font, proposedSize, format);
        }

        public float GetHorizontalDpi()
        {
            return _graphics.DpiX;
        }

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

        public Color ConvertHslToRgb(float h, float s, float l)
        {
            float r, g, b;

            if (s == 0)
            {
                r = g = b = l;
            }
            else
            {
                float q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                float p = 2 * l - q;
                r = HueToRgb(p, q, h + 1f / 3f);
                g = HueToRgb(p, q, h);
                b = HueToRgb(p, q, h - 1f / 3f);
            }

            return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }

        public void SetOpacity(float opacity)
        {
            _opacity = opacity;
        }

        public float GetOpacity()
        {
            return _opacity;
        }

        public void ColorFill(RenderSurface surface, Rectangle rectangle, Color color)
        {
            if (!surface.IsValid)
                throw new ArgumentException("A valid surface handle is required.", nameof(surface));

            if (surface.NativeHandle is not SilkVulkanRenderTarget target)
                throw new ArgumentException("Surface handle must wrap a Silk.NET Vulkan render target.", nameof(surface));

            if (rectangle.Width <= 0 || rectangle.Height <= 0)
                return;

            FlushSpriteBatch();
            BeginRenderPass(target);

            float alpha = color.A / 255F;
            ClearAttachment attachment = new ClearAttachment
            {
                AspectMask = ImageAspectFlags.ImageAspectColorBit,
                ColorAttachment = 0,
                ClearValue = CreateClearValue(color.R / 255F * alpha, color.G / 255F * alpha, color.B / 255F * alpha, alpha)
            };

            ClearRect clearRect = ToClearRect(rectangle, target.Size);
            if (clearRect.Rect.Extent.Width == 0 || clearRect.Rect.Extent.Height == 0)
                return;

            _vk.CmdClearAttachments(_activeCommandBuffer, 1, in attachment, 1, in clearRect);
        }

        public void SetBlend(bool enabled, float rate, ClientBlendMode mode)
        {
            _blending = enabled;
            _blendRate = rate;
            _blendMode = mode;
        }

        public bool IsBlending()
        {
            return _blending;
        }

        public float GetBlendRate()
        {
            return _blendRate;
        }

        public ClientBlendMode GetBlendMode()
        {
            return _blendMode;
        }

        public float GetLineWidth()
        {
            return _lineWidth;
        }

        public void SetLineWidth(float width)
        {
            if (width > 0)
                _lineWidth = width;
        }

        public void DrawLine(IReadOnlyList<LinePoint> points, Color colour)
        {
            if (points == null || points.Count < 2)
                return;

            FlushSpriteBatch();
            BeginRenderPass(_currentTarget);

            TextureVertex* vertices = AllocateVertices(points.Count, out ulong offset);
            for (int i = 0; i < points.Count; i++)
            {
                vertices[i] = new TextureVertex
                {
                    X = SnapLineCoordinate(points[i].X),
                    Y = SnapLineCoordinate(points[i].Y)
                };
            }

            BindPipeline(_linePipeline);
            _vk.CmdSetLineWidth(_activeCommandBuffer, _supportsWideLines ? _lineWidth : 1F);

            PushConstants push = new PushConstants
            {
                Viewport = new Vector2(_currentTarget.Size.Width, _currentTarget.Size.Height),
                Colour = new Vector4(colour.R / 255F, colour.G / 255F, colour.B / 255F, colour.A / 255F * _opacity)
            };
            _vk.CmdPushConstants(_activeCommandBuffer, _linePipelineLayout, ShaderStageFlags.ShaderStageVertexBit | ShaderStageFlags.ShaderStageFragmentBit, 0, (uint)sizeof(PushConstants), &push);

            Buffer buffer = _activeFrame.VertexBuffer.Buffer;
            _vk.CmdBindVertexBuffers(_activeCommandBuffer, 0, 1, in buffer, in offset);
            _vk.CmdDraw(_activeCommandBuffer, (uint)points.Count, 1, 0, 0);
        }

        public void DrawTexture(RenderTexture texture, Rectangle sourceRectangle, RectangleF destinationRectangle, Color colour)
        {
            DrawTextureCore(texture, sourceRectangle, destinationRectangle, Matrix3x2.Identity, colour);
        }

        public void DrawTexture(RenderTexture texture, Rectangle? sourceRectangle, Matrix3x2 transform, Vector3 center, Vector3 translation, Color colour)
        {
            if (!TryGetTexture(texture, out SilkVulkanTextureResource resource))
                return;

            Rectangle source = sourceRectangle ?? new Rectangle(0, 0, resource.Size.Width, resource.Size.Height);
            RectangleF destination = new RectangleF(0, 0, source.Width, source.Height);
            Matrix3x2 finalTransform = Matrix3x2.CreateTranslation(translation.X - center.X, translation.Y - center.Y) * transform;
            DrawTextureCore(texture, source, destination, finalTransform, colour);
        }

        public void DrawTextureBlend(RenderTexture texture, Rectangle? sourceRectangle, Matrix3x2 transform, Vector3 center, Vector3 translation, Color colour, float blendRate, ClientBlendMode mode)
        {
            bool oldBlend = _blending;
            float oldBlendRate = _blendRate;
            ClientBlendMode oldBlendMode = _blendMode;

            _blending = true;
            _blendRate = blendRate;
            _blendMode = mode;

            try
            {
                DrawTexture(texture, sourceRectangle, transform, center, translation, colour);
            }
            finally
            {
                _blending = oldBlend;
                _blendRate = oldBlendRate;
                _blendMode = oldBlendMode;
            }
        }

        public RenderSurface GetCurrentSurface()
        {
            return RenderSurface.From(_currentTarget);
        }

        public void SetSurface(RenderSurface surface)
        {
            if (surface.NativeHandle is not SilkVulkanRenderTarget target)
                throw new ArgumentException("Surface handle must wrap a Silk.NET Vulkan render target.", nameof(surface));

            if (_currentTarget == target)
                return;

            FlushSpriteBatch();
            EndRenderPass();
            _currentTarget = target;
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

        public RenderTargetResource CreateRenderTarget(Size size)
        {
            SilkVulkanRenderTarget target = CreateRenderTargetCore(size);
            _renderTargets.Add(target);
            return RenderTargetResource.From(RenderTexture.From(target.Texture), RenderSurface.From(target));
        }

        public void ReleaseRenderTarget(RenderTargetResource renderTarget)
        {
            if (renderTarget.Surface.NativeHandle is not SilkVulkanRenderTarget target || target.IsBackBuffer)
                return;

            FlushSpriteBatch();
            _renderTargets.Remove(target);
            if (_currentTarget == target)
                _currentTarget = _currentBackBufferTarget;

            DeferDispose(target);
        }

        public Size GetBackBufferSize()
        {
            return new Size((int)_swapchainExtent.Width, (int)_swapchainExtent.Height);
        }

        public void Clear(RenderClearFlags flags, Color colour, float z, int stencil, params Rectangle[] regions)
        {
            if ((flags & RenderClearFlags.Target) == 0 || _currentTarget == null)
                return;

            FlushSpriteBatch();
            BeginRenderPass(_currentTarget);

            float alpha = colour.A / 255F * _opacity;
            ClearAttachment attachment = new ClearAttachment
            {
                AspectMask = ImageAspectFlags.ImageAspectColorBit,
                ColorAttachment = 0,
                ClearValue = CreateClearValue(colour.R / 255F * alpha, colour.G / 255F * alpha, colour.B / 255F * alpha, alpha)
            };

            if (regions != null && regions.Length > 0)
            {
                ClearRect[] clearRects = new ClearRect[regions.Length];
                for (int i = 0; i < regions.Length; i++)
                    clearRects[i] = ToClearRect(regions[i], _currentTarget.Size);

                fixed (ClearRect* rectPtr = clearRects)
                    _vk.CmdClearAttachments(_activeCommandBuffer, 1, in attachment, (uint)clearRects.Length, rectPtr);
            }
            else
            {
                ClearRect rect = ToClearRect(new Rectangle(Point.Empty, _currentTarget.Size), _currentTarget.Size);
                _vk.CmdClearAttachments(_activeCommandBuffer, 1, in attachment, 1, in rect);
            }
        }

        public void FlushSprite()
        {
            FlushSpriteBatch();
        }

        public void RegisterControlCache(ITextureCacheItem control)
        {
            if (control is not DXControl dxControl)
                return;

            for (int i = 0; i < _controlCache.Count; i++)
            {
                if (_controlCache[i].TryGetTarget(out DXControl target) && target == dxControl)
                    return;
            }

            _controlCache.Add(new WeakReference<DXControl>(dxControl));
        }

        public void UnregisterControlCache(ITextureCacheItem control)
        {
            if (control is not DXControl dxControl)
                return;

            for (int i = _controlCache.Count - 1; i >= 0; i--)
            {
                if (!_controlCache[i].TryGetTarget(out DXControl target) || target == dxControl)
                    _controlCache.RemoveAt(i);
            }
        }

        public RenderTexture CreateTexture(Size size, RenderTextureFormat format, RenderTextureUsage usage, RenderTexturePool pool)
        {
            SilkVulkanTextureResource texture = CreateTextureCore(size, format, keepCpuData: usage == RenderTextureUsage.Dynamic, transitionToReadable: false);
            _textures.Add(texture);
            return RenderTexture.From(texture);
        }

        public void ReleaseTexture(RenderTexture texture)
        {
            if (texture.NativeHandle is not SilkVulkanTextureResource resource)
                return;

            FlushSpriteBatch();
            _textures.Remove(resource);
            DeferDispose(resource);
        }

        public TextureLock LockTexture(RenderTexture texture, TextureLockMode mode)
        {
            if (texture.NativeHandle is not SilkVulkanTextureResource resource)
                throw new InvalidOperationException("Silk.NET Vulkan texture handle expected.");

            FlushSpriteBatch();

            if (resource.Format == RenderTextureFormat.Dxt1 && mode != TextureLockMode.ReadOnly)
                return CreateCompressedTextureWriteLock(resource, 8, DecompressDxt1);

            if (resource.Format == RenderTextureFormat.Dxt5 && mode != TextureLockMode.ReadOnly)
                return CreateCompressedTextureWriteLock(resource, 16, DecompressDxt5);

            byte[] data = resource.Data;
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            return TextureLock.From(handle.AddrOfPinnedObject(), resource.Size.Width * 4, () =>
            {
                try
                {
                    if (mode != TextureLockMode.ReadOnly)
                    {
                        PremultiplyAlpha(data, resource.Size.Width, resource.Size.Height);
                        UploadTexture(resource, data);
                    }
                }
                finally
                {
                    handle.Free();
                }
            });
        }

        public void RegisterTextureCache(ITextureCacheItem texture)
        {
            if (texture is MirImage image && !_textureCache.Contains(image))
                _textureCache.Add(image);
        }

        public void UnregisterTextureCache(ITextureCacheItem texture)
        {
            if (texture is MirImage image)
                _textureCache.Remove(image);
        }

        public void RegisterSoundCache(ISoundCacheItem sound)
        {
            if (sound is DXSound dxSound && !_soundCache.Contains(dxSound))
                _soundCache.Add(dxSound);
        }

        public void UnregisterSoundCache(ISoundCacheItem sound)
        {
            if (sound is DXSound dxSound)
                _soundCache.Remove(dxSound);
        }

        public void MemoryClear()
        {
            DateTime now = CEnvir.Now;

            for (int i = _controlCache.Count - 1; i >= 0; i--)
            {
                if (!_controlCache[i].TryGetTarget(out DXControl control))
                {
                    _controlCache.RemoveAt(i);
                    continue;
                }

                if (now < control.ExpireTime)
                    continue;

                control.DisposeTexture();
            }

            for (int i = _textureCache.Count - 1; i >= 0; i--)
            {
                MirImage image = _textureCache[i];
                if (image == null || image.IsDisposed)
                {
                    _textureCache.RemoveAt(i);
                    continue;
                }

                if (now < image.ExpireTime)
                    continue;

                image.DisposeTexture();
            }

            for (int i = _soundCache.Count - 1; i >= 0; i--)
            {
                DXSound sound = _soundCache[i];
                if (sound == null)
                {
                    _soundCache.RemoveAt(i);
                    continue;
                }

                if (now < sound.ExpireTime)
                    continue;

                sound.DisposeSoundBuffer();
            }
        }

        public IReadOnlyList<ISoundCacheItem> GetRegisteredSoundCaches()
        {
            if (_soundCache.Count == 0)
                return Array.Empty<ISoundCacheItem>();

            return _soundCache.ConvertAll<ISoundCacheItem>(sound => sound);
        }

        public RenderTexture GetColourPaletteTexture()
        {
            return RenderTexture.From(_colourPalette);
        }

        public byte[] GetColourPaletteData()
        {
            return _paletteData;
        }

        public RenderTexture GetLightTexture()
        {
            return RenderTexture.From(_lightTexture);
        }

        public Size GetLightTextureSize()
        {
            return new Size(LightWidth, LightHeight);
        }

        public RenderTexture GetPoisonTexture()
        {
            return RenderTexture.From(_poisonTexture);
        }

        public Size GetPoisonTextureSize()
        {
            return new Size(PoisonSize, PoisonSize);
        }

        public TextureFilterMode GetTextureFilter()
        {
            return _textureFilter;
        }

        public void SetTextureFilter(TextureFilterMode mode)
        {
            if (_textureFilter == mode)
                return;

            FlushSpriteBatch();
            _textureFilter = mode;
        }

        public void Shutdown()
        {
            RestoreDisplayMode();

            if (_device.Handle != IntPtr.Zero)
                _vk.DeviceWaitIdle(_device);

            DisposeRegisteredCaches();
            FlushDeferredDisposals(true);

            foreach (SilkVulkanRenderTarget target in _renderTargets)
                target.Dispose();
            _renderTargets.Clear();

            _scratchTarget?.Dispose();
            _scratchTarget = null;

            _colourPalette?.Dispose();
            _lightTexture?.Dispose();
            _poisonTexture?.Dispose();

            foreach (SilkVulkanTextureResource texture in _textures)
                texture.Dispose();
            _textures.Clear();

            DestroySwapchainResources();

            if (_frames != null)
            {
                foreach (FrameResources frame in _frames)
                    frame?.Dispose(_vk, _device);
            }

            foreach (Pipeline pipeline in _texturePipelines.Values)
            {
                if (pipeline.Handle != 0)
                    _vk.DestroyPipeline(_device, pipeline, null);
            }
            _texturePipelines.Clear();

            if (_linePipeline.Handle != 0)
                _vk.DestroyPipeline(_device, _linePipeline, null);
            if (_texturePipelineLayout.Handle != 0)
                _vk.DestroyPipelineLayout(_device, _texturePipelineLayout, null);
            if (_linePipelineLayout.Handle != 0)
                _vk.DestroyPipelineLayout(_device, _linePipelineLayout, null);
            if (_renderPass.Handle != 0)
                _vk.DestroyRenderPass(_device, _renderPass, null);
            if (_pointSampler.Handle != 0)
                _vk.DestroySampler(_device, _pointSampler, null);
            if (_linearSampler.Handle != 0)
                _vk.DestroySampler(_device, _linearSampler, null);
            foreach (DescriptorPool descriptorPool in _descriptorPools)
            {
                if (descriptorPool.Handle != 0)
                    _vk.DestroyDescriptorPool(_device, descriptorPool, null);
            }
            _descriptorPools.Clear();
            _descriptorPool = default;
            if (_textureDescriptorSetLayout.Handle != 0)
                _vk.DestroyDescriptorSetLayout(_device, _textureDescriptorSetLayout, null);
            if (_frameCommandPool.Handle != 0)
                _vk.DestroyCommandPool(_device, _frameCommandPool, null);
            if (_uploadCommandPool.Handle != 0)
                _vk.DestroyCommandPool(_device, _uploadCommandPool, null);

            if (_device.Handle != IntPtr.Zero)
                _vk.DestroyDevice(_device, null);

            if (_surface.Handle != 0 && _surfaceApi != null)
                _surfaceApi.DestroySurface(_instance, _surface, null);

            _swapchainApi?.Dispose();
            _surfaceApi?.Dispose();
            _win32SurfaceApi?.Dispose();

            if (_instance.Handle != IntPtr.Zero)
                _vk.DestroyInstance(_instance, null);

            _graphics?.Dispose();
            _vk?.Dispose();
        }

        private void DisposeRegisteredCaches()
        {
            for (int i = _controlCache.Count - 1; i >= 0; i--)
            {
                if (_controlCache[i].TryGetTarget(out DXControl control))
                    control.DisposeTexture();
                else
                    _controlCache.RemoveAt(i);
            }
            _controlCache.Clear();

            for (int i = _textureCache.Count - 1; i >= 0; i--)
                _textureCache[i]?.DisposeTexture();
            _textureCache.Clear();

            for (int i = _soundCache.Count - 1; i >= 0; i--)
                _soundCache[i]?.DisposeSoundBuffer();
            _soundCache.Clear();
        }

        private void DrawTextureCore(RenderTexture texture, Rectangle sourceRectangle, RectangleF destinationRectangle, Matrix3x2 transform, Color colour)
        {
            if (!TryGetTexture(texture, out SilkVulkanTextureResource resource))
                return;

            if (destinationRectangle.Width <= 0 || destinationRectangle.Height <= 0 || sourceRectangle.Width <= 0 || sourceRectangle.Height <= 0)
                return;

            EnsureTextureReadable(resource);
            BeginRenderPass(_currentTarget);

            Vector2 p0 = Vector2.Transform(new Vector2(destinationRectangle.Left, destinationRectangle.Top), transform);
            Vector2 p1 = Vector2.Transform(new Vector2(destinationRectangle.Right, destinationRectangle.Top), transform);
            Vector2 p2 = Vector2.Transform(new Vector2(destinationRectangle.Right, destinationRectangle.Bottom), transform);
            Vector2 p3 = Vector2.Transform(new Vector2(destinationRectangle.Left, destinationRectangle.Bottom), transform);

            float u0 = sourceRectangle.Left / (float)resource.Size.Width;
            float u1 = sourceRectangle.Right / (float)resource.Size.Width;
            float v0 = sourceRectangle.Top / (float)resource.Size.Height;
            float v1 = sourceRectangle.Bottom / (float)resource.Size.Height;

            if (resource.FlipVerticallyWhenSampling)
            {
                v0 = 1F - sourceRectangle.Top / (float)resource.Size.Height;
                v1 = 1F - sourceRectangle.Bottom / (float)resource.Size.Height;
            }

            ClientBlendMode blendMode = GetAppliedBlendMode();
            float opacity = _opacity * (colour.A / 255F);
            if (_blending && _blendMode != ClientBlendMode.NONE && AppliesBlendRateToVertexColour(_blendMode))
                opacity *= _blendRate;

            Vector4 vertexColour = new Vector4(colour.R / 255F, colour.G / 255F, colour.B / 255F, opacity);

            SpriteShaderEffectRequest? effect = RenderingPipelineManager.GetSpriteShaderEffect();
            if (effect.HasValue)
            {
                Vector4 sourceUv = new Vector4(Math.Min(u0, u1), Math.Min(v0, v1), Math.Max(u0, u1), Math.Max(v0, v1));

                switch (effect.Value.Kind)
                {
                    case SpriteShaderEffectKind.Grayscale:
                        DrawSpriteImmediate(resource, blendMode, GetBlendConstantRate(blendMode), p0, p1, p2, p3, u0, u1, v0, v1, vertexColour,
                            CreateTexturePushConstants(1, sourceUv, Vector4.Zero, 0F, resource.Size));
                        return;
                    case SpriteShaderEffectKind.Outline:
                        OutlineEffectSettings outline = effect.Value.Outline;
                        float thickness = Math.Max(0.5F, outline.Thickness);
                        RectangleF outlineDestination = destinationRectangle;
                        outlineDestination.Inflate(thickness, thickness);

                        Vector2 op0 = Vector2.Transform(new Vector2(outlineDestination.Left, outlineDestination.Top), transform);
                        Vector2 op1 = Vector2.Transform(new Vector2(outlineDestination.Right, outlineDestination.Top), transform);
                        Vector2 op2 = Vector2.Transform(new Vector2(outlineDestination.Right, outlineDestination.Bottom), transform);
                        Vector2 op3 = Vector2.Transform(new Vector2(outlineDestination.Left, outlineDestination.Bottom), transform);

                        float ou0 = u0 - thickness / resource.Size.Width;
                        float ou1 = u1 + thickness / resource.Size.Width;
                        float ov0 = v0;
                        float ov1 = v1;
                        float vPad = thickness / resource.Size.Height;

                        if (ov0 <= ov1)
                        {
                            ov0 -= vPad;
                            ov1 += vPad;
                        }
                        else
                        {
                            ov0 += vPad;
                            ov1 -= vPad;
                        }

                        Color outlineColour = outline.Colour;
                        Vector4 outlineVector = new Vector4(outlineColour.R / 255F, outlineColour.G / 255F, outlineColour.B / 255F, outlineColour.A / 255F);
                        DrawSpriteImmediate(resource, blendMode, GetBlendConstantRate(blendMode), op0, op1, op2, op3, ou0, ou1, ov0, ov1, vertexColour,
                            CreateTexturePushConstants(2, sourceUv, outlineVector, thickness, resource.Size));
                        break;
                    case SpriteShaderEffectKind.DropShadow:
                        DropShadowEffectSettings dropShadow = effect.Value.DropShadow;
                        float shadowWidth = Math.Max(0.5F, dropShadow.Width);
                        RectangleF shadowDestination = destinationRectangle;
                        shadowDestination.Inflate(shadowWidth, shadowWidth);

                        Vector2 sp0 = Vector2.Transform(new Vector2(shadowDestination.Left, shadowDestination.Top), transform);
                        Vector2 sp1 = Vector2.Transform(new Vector2(shadowDestination.Right, shadowDestination.Top), transform);
                        Vector2 sp2 = Vector2.Transform(new Vector2(shadowDestination.Right, shadowDestination.Bottom), transform);
                        Vector2 sp3 = Vector2.Transform(new Vector2(shadowDestination.Left, shadowDestination.Bottom), transform);

                        RectangleF shadowBounds = dropShadow.VisibleBounds ?? destinationRectangle;
                        Color shadowColour = dropShadow.Colour;
                        Vector4 shadowVector = new Vector4(shadowColour.R / 255F, shadowColour.G / 255F, shadowColour.B / 255F, shadowColour.A / 255F);
                        Vector4 shadowBoundsVector = new Vector4(shadowBounds.Left, shadowBounds.Top, shadowBounds.Right, shadowBounds.Bottom);

                        DrawSpriteImmediate(resource, ClientBlendMode.NONE, 0F, sp0, sp1, sp2, sp3, u0, u1, v0, v1, vertexColour,
                            CreateTexturePushConstants(3, shadowBoundsVector, shadowVector, shadowWidth, dropShadow.StartOpacity));
                        break;
                }
            }

            QueueSprite(resource, blendMode, GetBlendConstantRate(blendMode), p0, p1, p2, p3, u0, u1, v0, v1, vertexColour);
        }

        private bool TryGetTexture(RenderTexture texture, out SilkVulkanTextureResource resource)
        {
            if (texture.NativeHandle is SilkVulkanTextureResource vulkanTexture)
            {
                resource = vulkanTexture;
                return true;
            }

            resource = null;
            return false;
        }

        private void CreateInstance()
        {
            byte* appName = ToUtf8("Xtreme");
            byte* engineName = ToUtf8("Zircon");
            byte* surfaceExtension = ToUtf8(KhrSurface.ExtensionName);
            byte* win32SurfaceExtension = ToUtf8(KhrWin32Surface.ExtensionName);

            try
            {
                byte*[] extensions = { surfaceExtension, win32SurfaceExtension };
                fixed (byte** extensionPtr = extensions)
                {
                    ApplicationInfo appInfo = new ApplicationInfo
                    {
                        SType = StructureType.ApplicationInfo,
                        PApplicationName = appName,
                        ApplicationVersion = Vk.MakeVersion(1, 0, 0),
                        PEngineName = engineName,
                        EngineVersion = Vk.MakeVersion(1, 0, 0),
                        ApiVersion = Vk.Version10
                    };

                    InstanceCreateInfo createInfo = new InstanceCreateInfo
                    {
                        SType = StructureType.InstanceCreateInfo,
                        PApplicationInfo = &appInfo,
                        EnabledExtensionCount = (uint)extensions.Length,
                        PpEnabledExtensionNames = extensionPtr
                    };

                    Check(_vk.CreateInstance(in createInfo, null, out _instance), "create Vulkan instance");
                }
            }
            finally
            {
                FreeUtf8(appName);
                FreeUtf8(engineName);
                FreeUtf8(surfaceExtension);
                FreeUtf8(win32SurfaceExtension);
            }

            if (!_vk.TryGetInstanceExtension(_instance, out _surfaceApi))
                throw new InvalidOperationException("VK_KHR_surface is unavailable.");

            if (!_vk.TryGetInstanceExtension(_instance, out _win32SurfaceApi))
                throw new InvalidOperationException("VK_KHR_win32_surface is unavailable.");
        }

        private void CreateSurface()
        {
            Win32SurfaceCreateInfoKHR createInfo = new Win32SurfaceCreateInfoKHR
            {
                SType = StructureType.Win32SurfaceCreateInfoKhr,
                Hinstance = GetModuleHandle(null),
                Hwnd = _context.RenderTarget.Handle
            };

            Check(_win32SurfaceApi.CreateWin32Surface(_instance, in createInfo, null, out _surface), "create Win32 Vulkan surface");
        }

        private void PickPhysicalDevice()
        {
            uint count = 0;
            Check(_vk.EnumeratePhysicalDevices(_instance, ref count, null), "count Vulkan physical devices");
            if (count == 0)
                throw new InvalidOperationException("No Vulkan-capable physical devices were found.");

            PhysicalDevice[] devices = new PhysicalDevice[count];
            fixed (PhysicalDevice* devicePtr = devices)
                Check(_vk.EnumeratePhysicalDevices(_instance, ref count, devicePtr), "enumerate Vulkan physical devices");

            PhysicalDevice selectedDevice = default;
            uint selectedQueueFamily = 0;
            long selectedScore = long.MinValue;
            string requestedAdapterId = string.Empty; //Config.GraphicsAdapter ?? string.Empty;

            for (int i = 0; i < devices.Length; i++)
            {
                PhysicalDevice device = devices[i];
                if (TrySelectQueueFamily(device, out uint queueFamilyIndex))
                {
                    GraphicsAdapterInfo adapter = CreateGraphicsAdapterInfo(_vk, device, i);
                    if (!string.IsNullOrWhiteSpace(requestedAdapterId) &&
                        string.Equals(adapter.Id, requestedAdapterId, StringComparison.OrdinalIgnoreCase))
                    {
                        selectedDevice = device;
                        selectedQueueFamily = queueFamilyIndex;
                        selectedScore = long.MaxValue;
                        break;
                    }

                    long score = ScorePhysicalDevice(adapter);
                    if (score > selectedScore)
                    {
                        selectedDevice = device;
                        selectedQueueFamily = queueFamilyIndex;
                        selectedScore = score;
                    }
                }
            }

            if (selectedDevice.Handle == 0)
                throw new InvalidOperationException("No Vulkan device supports graphics and presentation for this window.");

            _physicalDevice = selectedDevice;
            _graphicsQueueFamilyIndex = selectedQueueFamily;
            _vk.GetPhysicalDeviceMemoryProperties(_physicalDevice, out _memoryProperties);

            PhysicalDeviceFeatures features;
            _vk.GetPhysicalDeviceFeatures(_physicalDevice, out features);
            _supportsWideLines = features.WideLines;
        }

        public static IReadOnlyList<GraphicsAdapterInfo> GetAvailableGraphicsAdapters()
        {
            Vk vk = Vk.GetApi();
            Instance instance = default;

            try
            {
                byte* appName = ToUtf8("Legend of Mir 3");
                byte* engineName = ToUtf8("Zircon");
                try
                {
                    ApplicationInfo appInfo = new ApplicationInfo
                    {
                        SType = StructureType.ApplicationInfo,
                        PApplicationName = appName,
                        ApplicationVersion = Vk.MakeVersion(1, 0, 0),
                        PEngineName = engineName,
                        EngineVersion = Vk.MakeVersion(1, 0, 0),
                        ApiVersion = Vk.Version12
                    };

                    InstanceCreateInfo createInfo = new InstanceCreateInfo
                    {
                        SType = StructureType.InstanceCreateInfo,
                        PApplicationInfo = &appInfo
                    };

                    Check(vk.CreateInstance(in createInfo, null, out instance), "create Vulkan adapter enumeration instance");
                }
                finally
                {
                    FreeUtf8(appName);
                    FreeUtf8(engineName);
                }

                uint count = 0;
                Check(vk.EnumeratePhysicalDevices(instance, ref count, null), "count Vulkan physical devices");
                if (count == 0)
                    return Array.Empty<GraphicsAdapterInfo>();

                PhysicalDevice[] devices = new PhysicalDevice[count];
                fixed (PhysicalDevice* devicePtr = devices)
                    Check(vk.EnumeratePhysicalDevices(instance, ref count, devicePtr), "enumerate Vulkan physical devices");

                List<GraphicsAdapterInfo> adapters = new List<GraphicsAdapterInfo>(devices.Length);
                for (int i = 0; i < devices.Length; i++)
                    adapters.Add(CreateGraphicsAdapterInfo(vk, devices[i], i));

                adapters.Sort((x, y) => ScorePhysicalDevice(y).CompareTo(ScorePhysicalDevice(x)));
                return adapters;
            }
            catch
            {
                return Array.Empty<GraphicsAdapterInfo>();
            }
            finally
            {
                if (instance.Handle != 0)
                    vk.DestroyInstance(instance, null);

                vk.Dispose();
            }
        }

        private static GraphicsAdapterInfo CreateGraphicsAdapterInfo(Vk vk, PhysicalDevice device, int index)
        {
            vk.GetPhysicalDeviceProperties(device, out PhysicalDeviceProperties properties);
            vk.GetPhysicalDeviceMemoryProperties(device, out PhysicalDeviceMemoryProperties memoryProperties);

            string name = GetPhysicalDeviceName(properties);
            string type = GetPhysicalDeviceTypeName(properties.DeviceType);
            ulong deviceLocalMemory = GetDeviceLocalMemoryMegabytes(memoryProperties);
            string id = $"{properties.VendorID:X8}:{properties.DeviceID:X8}:{properties.DeviceType}:{name}";

            return new GraphicsAdapterInfo(id, name, type, deviceLocalMemory);
        }

        private static long ScorePhysicalDevice(GraphicsAdapterInfo adapter)
        {
            long score = adapter.Type switch
            {
                "Discrete GPU" => 1_000_000,
                "Integrated GPU" => 100_000,
                "Virtual GPU" => 50_000,
                "CPU" => -1_000_000,
                _ => 0
            };

            score += (long)Math.Min(adapter.DedicatedMemoryMegabytes, 262_144UL);
            return score;
        }

        private static ulong GetDeviceLocalMemoryMegabytes(PhysicalDeviceMemoryProperties memoryProperties)
        {
            ulong deviceLocalMemory = 0;
            for (uint i = 0; i < memoryProperties.MemoryHeapCount; i++)
            {
                MemoryHeap heap = memoryProperties.MemoryHeaps[(int)i];
                if ((heap.Flags & MemoryHeapFlags.MemoryHeapDeviceLocalBit) != 0)
                    deviceLocalMemory += heap.Size;
            }

            return deviceLocalMemory / (1024UL * 1024UL);
        }

        private static string GetPhysicalDeviceTypeName(PhysicalDeviceType type)
        {
            return type switch
            {
                PhysicalDeviceType.DiscreteGpu => "Discrete GPU",
                PhysicalDeviceType.IntegratedGpu => "Integrated GPU",
                PhysicalDeviceType.VirtualGpu => "Virtual GPU",
                PhysicalDeviceType.Cpu => "CPU",
                _ => "Other GPU"
            };
        }

        private static string GetPhysicalDeviceName(PhysicalDeviceProperties properties)
        {
            byte* namePtr = properties.DeviceName;
            return Marshal.PtrToStringAnsi((IntPtr)namePtr) ?? "Unknown Vulkan GPU";
        }

        private bool TrySelectQueueFamily(PhysicalDevice device, out uint queueFamilyIndex)
        {
            queueFamilyIndex = 0;

            uint count = 0;
            _vk.GetPhysicalDeviceQueueFamilyProperties(device, ref count, null);
            if (count == 0)
                return false;

            QueueFamilyProperties[] families = new QueueFamilyProperties[count];
            fixed (QueueFamilyProperties* familyPtr = families)
                _vk.GetPhysicalDeviceQueueFamilyProperties(device, ref count, familyPtr);

            for (uint i = 0; i < families.Length; i++)
            {
                if ((families[i].QueueFlags & QueueFlags.QueueGraphicsBit) == 0)
                    continue;

                Bool32 presentSupported = false;
                _surfaceApi.GetPhysicalDeviceSurfaceSupport(device, i, _surface, out presentSupported);
                if (!presentSupported)
                    continue;

                if (!HasSwapchainSupport(device))
                    continue;

                queueFamilyIndex = i;
                return true;
            }

            return false;
        }

        private bool HasSwapchainSupport(PhysicalDevice device)
        {
            byte* extensionName = ToUtf8(KhrSwapchain.ExtensionName);
            try
            {
                if (!_vk.IsDeviceExtensionPresent(device, (string)KhrSwapchain.ExtensionName))
                    return false;
            }
            finally
            {
                FreeUtf8(extensionName);
            }

            uint formatCount = 0;
            _surfaceApi.GetPhysicalDeviceSurfaceFormats(device, _surface, ref formatCount, null);
            uint presentModeCount = 0;
            _surfaceApi.GetPhysicalDeviceSurfacePresentModes(device, _surface, ref presentModeCount, null);
            return formatCount > 0 && presentModeCount > 0;
        }

        private void CreateLogicalDevice()
        {
            float priority = 1F;
            byte* swapchainExtension = ToUtf8(KhrSwapchain.ExtensionName);

            try
            {
                byte*[] extensions = { swapchainExtension };
                fixed (byte** extensionPtr = extensions)
                {
                    DeviceQueueCreateInfo queueCreateInfo = new DeviceQueueCreateInfo
                    {
                        SType = StructureType.DeviceQueueCreateInfo,
                        QueueFamilyIndex = _graphicsQueueFamilyIndex,
                        QueueCount = 1,
                        PQueuePriorities = &priority
                    };

                    PhysicalDeviceFeatures features = new PhysicalDeviceFeatures
                    {
                        WideLines = _supportsWideLines
                    };

                    DeviceCreateInfo createInfo = new DeviceCreateInfo
                    {
                        SType = StructureType.DeviceCreateInfo,
                        QueueCreateInfoCount = 1,
                        PQueueCreateInfos = &queueCreateInfo,
                        EnabledExtensionCount = (uint)extensions.Length,
                        PpEnabledExtensionNames = extensionPtr,
                        PEnabledFeatures = &features
                    };

                    Check(_vk.CreateDevice(_physicalDevice, in createInfo, null, out _device), "create Vulkan device");
                }
            }
            finally
            {
                FreeUtf8(swapchainExtension);
            }

            _vk.GetDeviceQueue(_device, _graphicsQueueFamilyIndex, 0, out _graphicsQueue);

            if (!_vk.TryGetDeviceExtension(_instance, _device, out _swapchainApi))
                throw new InvalidOperationException("VK_KHR_swapchain is unavailable.");
        }

        private void CreateCommandPools()
        {
            CommandPoolCreateInfo framePoolInfo = new CommandPoolCreateInfo
            {
                SType = StructureType.CommandPoolCreateInfo,
                QueueFamilyIndex = _graphicsQueueFamilyIndex,
                Flags = CommandPoolCreateFlags.CommandPoolCreateResetCommandBufferBit
            };
            Check(_vk.CreateCommandPool(_device, in framePoolInfo, null, out _frameCommandPool), "create Vulkan frame command pool");

            CommandPoolCreateInfo uploadPoolInfo = new CommandPoolCreateInfo
            {
                SType = StructureType.CommandPoolCreateInfo,
                QueueFamilyIndex = _graphicsQueueFamilyIndex,
                Flags = CommandPoolCreateFlags.CommandPoolCreateTransientBit
            };
            Check(_vk.CreateCommandPool(_device, in uploadPoolInfo, null, out _uploadCommandPool), "create Vulkan upload command pool");
        }

        private void CreateDescriptorResources()
        {
            DescriptorSetLayoutBinding textureBinding = new DescriptorSetLayoutBinding
            {
                Binding = 0,
                DescriptorCount = 1,
                DescriptorType = DescriptorType.CombinedImageSampler,
                StageFlags = ShaderStageFlags.ShaderStageFragmentBit
            };
            DescriptorSetLayoutCreateInfo textureLayoutInfo = new DescriptorSetLayoutCreateInfo
            {
                SType = StructureType.DescriptorSetLayoutCreateInfo,
                BindingCount = 1,
                PBindings = &textureBinding
            };
            Check(_vk.CreateDescriptorSetLayout(_device, in textureLayoutInfo, null, out _textureDescriptorSetLayout), "create Vulkan texture descriptor layout");

            _descriptorPool = CreateDescriptorPool();
            _descriptorPools.Add(_descriptorPool);

            _pointSampler = CreateSampler(Filter.Nearest);
            _linearSampler = CreateSampler(Filter.Linear);
        }

        private DescriptorPool CreateDescriptorPool()
        {
            DescriptorPoolSize* poolSizes = stackalloc DescriptorPoolSize[2];
            poolSizes[0] = new DescriptorPoolSize
            {
                Type = DescriptorType.CombinedImageSampler,
                DescriptorCount = MaxTextureDescriptors
            };
            poolSizes[1] = new DescriptorPoolSize
            {
                Type = DescriptorType.UniformBuffer,
                DescriptorCount = (uint)MaxFramesInFlight
            };
            DescriptorPoolCreateInfo poolInfo = new DescriptorPoolCreateInfo
            {
                SType = StructureType.DescriptorPoolCreateInfo,
                Flags = DescriptorPoolCreateFlags.DescriptorPoolCreateFreeDescriptorSetBit,
                MaxSets = MaxTextureDescriptors + (uint)MaxFramesInFlight,
                PoolSizeCount = 2,
                PPoolSizes = poolSizes
            };
            Check(_vk.CreateDescriptorPool(_device, in poolInfo, null, out DescriptorPool descriptorPool), "create Vulkan descriptor pool");
            return descriptorPool;
        }

        private Sampler CreateSampler(Filter filter)
        {
            SamplerCreateInfo createInfo = new SamplerCreateInfo
            {
                SType = StructureType.SamplerCreateInfo,
                MagFilter = filter,
                MinFilter = filter,
                MipmapMode = SamplerMipmapMode.Nearest,
                AddressModeU = SamplerAddressMode.ClampToEdge,
                AddressModeV = SamplerAddressMode.ClampToEdge,
                AddressModeW = SamplerAddressMode.ClampToEdge,
                MaxLod = 0F
            };
            Check(_vk.CreateSampler(_device, in createInfo, null, out Sampler sampler), "create Vulkan sampler");
            return sampler;
        }

        private void CreateFrameResources()
        {
            _frames = new FrameResources[MaxFramesInFlight];
            for (int i = 0; i < _frames.Length; i++)
            {
                CommandBufferAllocateInfo allocateInfo = new CommandBufferAllocateInfo
                {
                    SType = StructureType.CommandBufferAllocateInfo,
                    CommandPool = _frameCommandPool,
                    Level = CommandBufferLevel.Primary,
                    CommandBufferCount = 1
                };
                Check(_vk.AllocateCommandBuffers(_device, in allocateInfo, out CommandBuffer commandBuffer), "allocate Vulkan command buffer");

                SilkVulkanBufferResource vertexBuffer = CreateBuffer(DynamicVertexBufferSize, BufferUsageFlags.BufferUsageVertexBufferBit, MemoryPropertyFlags.MemoryPropertyHostVisibleBit | MemoryPropertyFlags.MemoryPropertyHostCoherentBit, true);

                SemaphoreCreateInfo semaphoreInfo = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
                Check(_vk.CreateSemaphore(_device, in semaphoreInfo, null, out VkSemaphore imageAvailable), "create Vulkan image semaphore");
                Check(_vk.CreateSemaphore(_device, in semaphoreInfo, null, out VkSemaphore renderFinished), "create Vulkan render semaphore");

                FenceCreateInfo fenceInfo = new FenceCreateInfo
                {
                    SType = StructureType.FenceCreateInfo,
                    Flags = FenceCreateFlags.FenceCreateSignaledBit
                };
                Check(_vk.CreateFence(_device, in fenceInfo, null, out Fence fence), "create Vulkan frame fence");

                _frames[i] = new FrameResources(commandBuffer, vertexBuffer, imageAvailable, renderFinished, fence);
            }
        }

        private void CreateSwapchain()
        {
            SurfaceCapabilitiesKHR capabilities;
            _surfaceApi.GetPhysicalDeviceSurfaceCapabilities(_physicalDevice, _surface, out capabilities);

            SurfaceFormatKHR surfaceFormat = ChooseSurfaceFormat();
            PresentModeKHR presentMode = ChoosePresentMode();
            Extent2D extent = ChooseSwapExtent(capabilities);
            uint imageCount = capabilities.MinImageCount + 1;
            if (capabilities.MaxImageCount > 0 && imageCount > capabilities.MaxImageCount)
                imageCount = capabilities.MaxImageCount;

            SwapchainCreateInfoKHR createInfo = new SwapchainCreateInfoKHR
            {
                SType = StructureType.SwapchainCreateInfoKhr,
                Surface = _surface,
                MinImageCount = imageCount,
                ImageFormat = surfaceFormat.Format,
                ImageColorSpace = surfaceFormat.ColorSpace,
                ImageExtent = extent,
                ImageArrayLayers = 1,
                ImageUsage = ImageUsageFlags.ImageUsageColorAttachmentBit,
                ImageSharingMode = SharingMode.Exclusive,
                PreTransform = capabilities.CurrentTransform,
                CompositeAlpha = CompositeAlphaFlagsKHR.CompositeAlphaOpaqueBitKhr,
                PresentMode = presentMode,
                Clipped = true,
                OldSwapchain = _swapchain
            };

            Check(_swapchainApi.CreateSwapchain(_device, in createInfo, null, out SwapchainKHR newSwapchain), "create Vulkan swapchain");
            if (_swapchain.Handle != 0)
                _swapchainApi.DestroySwapchain(_device, _swapchain, null);

            _swapchain = newSwapchain;
            _swapchainFormat = surfaceFormat.Format;
            _swapchainExtent = extent;

            uint actualImageCount = 0;
            Check(_swapchainApi.GetSwapchainImages(_device, _swapchain, ref actualImageCount, null), "count Vulkan swapchain images");
            _swapchainImages = new VkImage[actualImageCount];
            fixed (VkImage* imagePtr = _swapchainImages)
                Check(_swapchainApi.GetSwapchainImages(_device, _swapchain, ref actualImageCount, imagePtr), "get Vulkan swapchain images");
        }

        private SurfaceFormatKHR ChooseSurfaceFormat()
        {
            uint count = 0;
            _surfaceApi.GetPhysicalDeviceSurfaceFormats(_physicalDevice, _surface, ref count, null);
            SurfaceFormatKHR[] formats = new SurfaceFormatKHR[count];
            fixed (SurfaceFormatKHR* formatPtr = formats)
                _surfaceApi.GetPhysicalDeviceSurfaceFormats(_physicalDevice, _surface, ref count, formatPtr);

            foreach (SurfaceFormatKHR format in formats)
            {
                if (format.Format == Format.B8G8R8A8Unorm && format.ColorSpace == ColorSpaceKHR.ColorSpaceSrgbNonlinearKhr)
                    return format;
            }

            throw new InvalidOperationException("The Vulkan surface does not expose B8G8R8A8Unorm, which is required by the current client texture pipeline.");
        }

        private PresentModeKHR ChoosePresentMode()
        {
            uint count = 0;
            _surfaceApi.GetPhysicalDeviceSurfacePresentModes(_physicalDevice, _surface, ref count, null);
            PresentModeKHR[] modes = new PresentModeKHR[count];
            fixed (PresentModeKHR* modePtr = modes)
                _surfaceApi.GetPhysicalDeviceSurfacePresentModes(_physicalDevice, _surface, ref count, modePtr);

            bool forceFifo = _forceNextSwapchainFifoPresentMode;
            _forceNextSwapchainFifoPresentMode = false;

            if (forceFifo || (Config.VSync && Config.FullScreen))
                return PresentModeKHR.PresentModeFifoKhr;

            // Windowed mailbox is still tear-free, but avoids FIFO blocking during renderer handoff.
            foreach (PresentModeKHR mode in modes)
            {
                if (mode == PresentModeKHR.PresentModeMailboxKhr)
                    return mode;
            }

            if (Config.VSync)
                return PresentModeKHR.PresentModeFifoKhr;

            foreach (PresentModeKHR mode in modes)
            {
                if (mode == PresentModeKHR.PresentModeImmediateKhr)
                    return mode;
            }

            return PresentModeKHR.PresentModeFifoKhr;
        }

        private Extent2D ChooseSwapExtent(SurfaceCapabilitiesKHR capabilities)
        {
            if (capabilities.CurrentExtent.Width != uint.MaxValue)
                return capabilities.CurrentExtent;

            Size size = GetTargetSize();
            return new Extent2D
            {
                Width = Math.Min(Math.Max((uint)size.Width, capabilities.MinImageExtent.Width), capabilities.MaxImageExtent.Width),
                Height = Math.Min(Math.Max((uint)size.Height, capabilities.MinImageExtent.Height), capabilities.MaxImageExtent.Height)
            };
        }

        private void CreateRenderPass()
        {
            AttachmentDescription colourAttachment = new AttachmentDescription
            {
                Format = _swapchainFormat,
                Samples = SampleCountFlags.SampleCount1Bit,
                LoadOp = AttachmentLoadOp.Load,
                StoreOp = AttachmentStoreOp.Store,
                StencilLoadOp = AttachmentLoadOp.DontCare,
                StencilStoreOp = AttachmentStoreOp.DontCare,
                InitialLayout = VkImageLayout.ColorAttachmentOptimal,
                FinalLayout = VkImageLayout.ColorAttachmentOptimal
            };

            AttachmentReference colourReference = new AttachmentReference
            {
                Attachment = 0,
                Layout = VkImageLayout.ColorAttachmentOptimal
            };

            SubpassDescription subpass = new SubpassDescription
            {
                PipelineBindPoint = PipelineBindPoint.Graphics,
                ColorAttachmentCount = 1,
                PColorAttachments = &colourReference
            };

            RenderPassCreateInfo renderPassInfo = new RenderPassCreateInfo
            {
                SType = StructureType.RenderPassCreateInfo,
                AttachmentCount = 1,
                PAttachments = &colourAttachment,
                SubpassCount = 1,
                PSubpasses = &subpass
            };

            Check(_vk.CreateRenderPass(_device, in renderPassInfo, null, out _renderPass), "create Vulkan render pass");
        }

        private void CreateSwapchainFramebuffers()
        {
            _backBufferTargets = new SilkVulkanRenderTarget[_swapchainImages.Length];
            Size size = new Size((int)_swapchainExtent.Width, (int)_swapchainExtent.Height);
            for (uint i = 0; i < _swapchainImages.Length; i++)
            {
                ImageView imageView = CreateImageView(_swapchainImages[i], _swapchainFormat);
                Framebuffer framebuffer = CreateFramebuffer(imageView, size);
                _backBufferTargets[i] = new SilkVulkanRenderTarget(_vk, _device, size, _swapchainImages[i], imageView, framebuffer, null, true, i, true);
            }
        }

        private void CreateGraphicsPipelines()
        {
            byte[] textureVertexShader = SilkVulkanShaderCompiler.Compile(TextureVertexShaderSource, ShaderKind.VertexShader, "vulkan_texture.vert");
            byte[] textureFragmentShader = SilkVulkanShaderCompiler.Compile(TextureFragmentShaderSource, ShaderKind.FragmentShader, "vulkan_texture.frag");
            byte[] lineVertexShader = SilkVulkanShaderCompiler.Compile(LineVertexShaderSource, ShaderKind.VertexShader, "vulkan_line.vert");
            byte[] lineFragmentShader = SilkVulkanShaderCompiler.Compile(LineFragmentShaderSource, ShaderKind.FragmentShader, "vulkan_line.frag");

            ShaderModule textureVertexModule = CreateShaderModule(textureVertexShader);
            ShaderModule textureFragmentModule = CreateShaderModule(textureFragmentShader);
            ShaderModule lineVertexModule = CreateShaderModule(lineVertexShader);
            ShaderModule lineFragmentModule = CreateShaderModule(lineFragmentShader);

            try
            {
                PushConstantRange pushConstantRange = new PushConstantRange
                {
                    StageFlags = ShaderStageFlags.ShaderStageVertexBit | ShaderStageFlags.ShaderStageFragmentBit,
                    Offset = 0,
                    Size = (uint)sizeof(PushConstants)
                };

                DescriptorSetLayout textureSetLayout = _textureDescriptorSetLayout;
                PipelineLayoutCreateInfo textureLayoutInfo = new PipelineLayoutCreateInfo
                {
                    SType = StructureType.PipelineLayoutCreateInfo,
                    SetLayoutCount = 1,
                    PSetLayouts = &textureSetLayout,
                    PushConstantRangeCount = 1,
                    PPushConstantRanges = &pushConstantRange
                };
                Check(_vk.CreatePipelineLayout(_device, in textureLayoutInfo, null, out _texturePipelineLayout), "create Vulkan texture pipeline layout");

                PipelineLayoutCreateInfo lineLayoutInfo = new PipelineLayoutCreateInfo
                {
                    SType = StructureType.PipelineLayoutCreateInfo,
                    PushConstantRangeCount = 1,
                    PPushConstantRanges = &pushConstantRange
                };
                Check(_vk.CreatePipelineLayout(_device, in lineLayoutInfo, null, out _linePipelineLayout), "create Vulkan line pipeline layout");

                _texturePipelines[ClientBlendMode.NONE] = CreateTexturePipeline(textureVertexModule, textureFragmentModule, ClientBlendMode.NONE);
                _texturePipelines[ClientBlendMode.NORMAL] = CreateTexturePipeline(textureVertexModule, textureFragmentModule, ClientBlendMode.NORMAL);
                _texturePipelines[ClientBlendMode.COLORFY] = CreateTexturePipeline(textureVertexModule, textureFragmentModule, ClientBlendMode.COLORFY);
                _texturePipelines[ClientBlendMode.EFFECTMASK] = CreateTexturePipeline(textureVertexModule, textureFragmentModule, ClientBlendMode.EFFECTMASK);
                _texturePipelines[ClientBlendMode.HIGHLIGHT] = CreateTexturePipeline(textureVertexModule, textureFragmentModule, ClientBlendMode.HIGHLIGHT);
                _texturePipelines[ClientBlendMode.MASK] = CreateTexturePipeline(textureVertexModule, textureFragmentModule, ClientBlendMode.MASK);
                _texturePipelines[ClientBlendMode.LIGHTMAP] = CreateTexturePipeline(textureVertexModule, textureFragmentModule, ClientBlendMode.LIGHTMAP);
                _texturePipelines[ClientBlendMode.INVLIGHT] = CreateTexturePipeline(textureVertexModule, textureFragmentModule, ClientBlendMode.INVLIGHT);
                _linePipeline = CreateLinePipeline(lineVertexModule, lineFragmentModule);
            }
            finally
            {
                _vk.DestroyShaderModule(_device, textureVertexModule, null);
                _vk.DestroyShaderModule(_device, textureFragmentModule, null);
                _vk.DestroyShaderModule(_device, lineVertexModule, null);
                _vk.DestroyShaderModule(_device, lineFragmentModule, null);
            }
        }

        private Pipeline CreateTexturePipeline(ShaderModule vertexModule, ShaderModule fragmentModule, ClientBlendMode blendMode)
        {
            PipelineColorBlendAttachmentState blendAttachment = CreateBlendAttachment(blendMode);
            return CreatePipeline(vertexModule, fragmentModule, _texturePipelineLayout, PrimitiveTopology.TriangleList, blendAttachment, VertexInputKind.SpriteInstance);
        }

        private Pipeline CreateLinePipeline(ShaderModule vertexModule, ShaderModule fragmentModule)
        {
            PipelineColorBlendAttachmentState blendAttachment = CreateBlendAttachment(ClientBlendMode.NONE);
            return CreatePipeline(vertexModule, fragmentModule, _linePipelineLayout, PrimitiveTopology.LineStrip, blendAttachment, VertexInputKind.Position);
        }

        private Pipeline CreatePipeline(ShaderModule vertexModule, ShaderModule fragmentModule, PipelineLayout layout, PrimitiveTopology topology, PipelineColorBlendAttachmentState blendAttachment, VertexInputKind vertexInputKind)
        {
            byte* entryPoint = ToUtf8("main");
            try
            {
                PipelineShaderStageCreateInfo* shaderStages = stackalloc PipelineShaderStageCreateInfo[2];
                shaderStages[0] = new PipelineShaderStageCreateInfo
                {
                    SType = StructureType.PipelineShaderStageCreateInfo,
                    Stage = ShaderStageFlags.ShaderStageVertexBit,
                    Module = vertexModule,
                    PName = entryPoint
                };
                shaderStages[1] = new PipelineShaderStageCreateInfo
                {
                    SType = StructureType.PipelineShaderStageCreateInfo,
                    Stage = ShaderStageFlags.ShaderStageFragmentBit,
                    Module = fragmentModule,
                    PName = entryPoint
                };

                VertexInputBindingDescription binding = CreateVertexInputBinding(vertexInputKind);
                VertexInputAttributeDescription* attributes = stackalloc VertexInputAttributeDescription[5];
                uint attributeCount = CreateVertexInputAttributes(vertexInputKind, attributes);

                PipelineVertexInputStateCreateInfo vertexInput = new PipelineVertexInputStateCreateInfo
                {
                    SType = StructureType.PipelineVertexInputStateCreateInfo,
                    VertexBindingDescriptionCount = 1,
                    PVertexBindingDescriptions = &binding,
                    VertexAttributeDescriptionCount = attributeCount,
                    PVertexAttributeDescriptions = attributes
                };

                PipelineInputAssemblyStateCreateInfo inputAssembly = new PipelineInputAssemblyStateCreateInfo
                {
                    SType = StructureType.PipelineInputAssemblyStateCreateInfo,
                    Topology = topology
                };

                PipelineViewportStateCreateInfo viewportState = new PipelineViewportStateCreateInfo
                {
                    SType = StructureType.PipelineViewportStateCreateInfo,
                    ViewportCount = 1,
                    ScissorCount = 1
                };

                PipelineRasterizationStateCreateInfo rasterizer = new PipelineRasterizationStateCreateInfo
                {
                    SType = StructureType.PipelineRasterizationStateCreateInfo,
                    PolygonMode = PolygonMode.Fill,
                    CullMode = CullModeFlags.CullModeNone,
                    FrontFace = FrontFace.Clockwise,
                    LineWidth = 1F
                };

                PipelineMultisampleStateCreateInfo multisampling = new PipelineMultisampleStateCreateInfo
                {
                    SType = StructureType.PipelineMultisampleStateCreateInfo,
                    RasterizationSamples = SampleCountFlags.SampleCount1Bit
                };

                PipelineColorBlendStateCreateInfo colorBlending = new PipelineColorBlendStateCreateInfo
                {
                    SType = StructureType.PipelineColorBlendStateCreateInfo,
                    AttachmentCount = 1,
                    PAttachments = &blendAttachment
                };

                DynamicState* dynamicStates = stackalloc DynamicState[3];
                dynamicStates[0] = DynamicState.Viewport;
                dynamicStates[1] = DynamicState.Scissor;
                dynamicStates[2] = DynamicState.LineWidth;
                PipelineDynamicStateCreateInfo dynamicState = new PipelineDynamicStateCreateInfo
                {
                    SType = StructureType.PipelineDynamicStateCreateInfo,
                    DynamicStateCount = 3,
                    PDynamicStates = dynamicStates
                };

                GraphicsPipelineCreateInfo pipelineInfo = new GraphicsPipelineCreateInfo
                {
                    SType = StructureType.GraphicsPipelineCreateInfo,
                    StageCount = 2,
                    PStages = shaderStages,
                    PVertexInputState = &vertexInput,
                    PInputAssemblyState = &inputAssembly,
                    PViewportState = &viewportState,
                    PRasterizationState = &rasterizer,
                    PMultisampleState = &multisampling,
                    PColorBlendState = &colorBlending,
                    PDynamicState = &dynamicState,
                    Layout = layout,
                    RenderPass = _renderPass,
                    Subpass = 0
                };

                Check(_vk.CreateGraphicsPipelines(_device, default, 1, in pipelineInfo, null, out Pipeline pipeline), "create Vulkan graphics pipeline");
                return pipeline;
            }
            finally
            {
                FreeUtf8(entryPoint);
            }
        }

        private static VertexInputBindingDescription CreateVertexInputBinding(VertexInputKind kind)
        {
            return new VertexInputBindingDescription
            {
                Binding = 0,
                Stride = kind == VertexInputKind.SpriteInstance ? (uint)sizeof(SpriteInstance) : (uint)sizeof(TextureVertex),
                InputRate = kind == VertexInputKind.SpriteInstance ? VertexInputRate.Instance : VertexInputRate.Vertex
            };
        }

        private static uint CreateVertexInputAttributes(VertexInputKind kind, VertexInputAttributeDescription* attributes)
        {
            attributes[0] = new VertexInputAttributeDescription
            {
                Binding = 0,
                Location = 0,
                Format = kind == VertexInputKind.SpriteInstance ? Format.R32G32B32A32Sfloat : Format.R32G32Sfloat,
                Offset = 0
            };

            if (kind == VertexInputKind.Position)
                return 1;

            attributes[1] = new VertexInputAttributeDescription
            {
                Binding = 0,
                Location = 1,
                Format = kind == VertexInputKind.SpriteInstance ? Format.R32G32B32A32Sfloat : Format.R32G32Sfloat,
                Offset = kind == VertexInputKind.SpriteInstance ? 16U : 8U
            };

            if (kind == VertexInputKind.PositionTexture)
                return 2;

            attributes[2] = new VertexInputAttributeDescription
            {
                Binding = 0,
                Location = 2,
                Format = Format.R32G32B32A32Sfloat,
                Offset = 32
            };
            attributes[3] = new VertexInputAttributeDescription
            {
                Binding = 0,
                Location = 3,
                Format = Format.R32G32B32A32Sfloat,
                Offset = 48
            };
            attributes[4] = new VertexInputAttributeDescription
            {
                Binding = 0,
                Location = 4,
                Format = Format.R32G32B32A32Sfloat,
                Offset = 64
            };

            return 5;
        }

        private PipelineColorBlendAttachmentState CreateBlendAttachment(ClientBlendMode blendMode)
        {
            PipelineColorBlendAttachmentState attachment = new PipelineColorBlendAttachmentState
            {
                BlendEnable = true,
                ColorBlendOp = BlendOp.Add,
                AlphaBlendOp = BlendOp.Add,
                ColorWriteMask = ColorComponentFlags.ColorComponentRBit |
                                 ColorComponentFlags.ColorComponentGBit |
                                 ColorComponentFlags.ColorComponentBBit |
                                 ColorComponentFlags.ColorComponentABit
            };

            switch (blendMode)
            {
                case ClientBlendMode.NONE:
                    attachment.SrcColorBlendFactor = BlendFactor.One;
                    attachment.DstColorBlendFactor = BlendFactor.OneMinusSrcAlpha;
                    attachment.SrcAlphaBlendFactor = BlendFactor.One;
                    attachment.DstAlphaBlendFactor = BlendFactor.OneMinusSrcAlpha;
                    break;
                case ClientBlendMode.COLORFY:
                    attachment.SrcColorBlendFactor = BlendFactor.SrcAlpha;
                    attachment.DstColorBlendFactor = BlendFactor.One;
                    attachment.SrcAlphaBlendFactor = BlendFactor.SrcAlpha;
                    attachment.DstAlphaBlendFactor = BlendFactor.One;
                    break;
                case ClientBlendMode.EFFECTMASK:
                    attachment.SrcColorBlendFactor = BlendFactor.DstAlpha;
                    attachment.DstColorBlendFactor = BlendFactor.One;
                    attachment.SrcAlphaBlendFactor = BlendFactor.DstAlpha;
                    attachment.DstAlphaBlendFactor = BlendFactor.One;
                    break;
                case ClientBlendMode.HIGHLIGHT:
                    attachment.SrcColorBlendFactor = BlendFactor.ConstantColor;
                    attachment.DstColorBlendFactor = BlendFactor.One;
                    attachment.SrcAlphaBlendFactor = BlendFactor.ConstantAlpha;
                    attachment.DstAlphaBlendFactor = BlendFactor.One;
                    break;
                case ClientBlendMode.MASK:
                    attachment.SrcColorBlendFactor = BlendFactor.Zero;
                    attachment.DstColorBlendFactor = BlendFactor.OneMinusSrcAlpha;
                    attachment.SrcAlphaBlendFactor = BlendFactor.Zero;
                    attachment.DstAlphaBlendFactor = BlendFactor.OneMinusSrcAlpha;
                    break;
                case ClientBlendMode.LIGHTMAP:
                    attachment.SrcColorBlendFactor = BlendFactor.Zero;
                    attachment.DstColorBlendFactor = BlendFactor.SrcColor;
                    attachment.SrcAlphaBlendFactor = BlendFactor.Zero;
                    attachment.DstAlphaBlendFactor = BlendFactor.SrcAlpha;
                    break;
                case ClientBlendMode.INVLIGHT:
                    attachment.SrcColorBlendFactor = BlendFactor.ConstantColor;
                    attachment.DstColorBlendFactor = BlendFactor.OneMinusSrcColor;
                    attachment.SrcAlphaBlendFactor = BlendFactor.ConstantAlpha;
                    attachment.DstAlphaBlendFactor = BlendFactor.OneMinusSrcAlpha;
                    break;
                default:
                    attachment.SrcColorBlendFactor = BlendFactor.OneMinusDstColor;
                    attachment.DstColorBlendFactor = BlendFactor.One;
                    attachment.SrcAlphaBlendFactor = BlendFactor.OneMinusDstAlpha;
                    attachment.DstAlphaBlendFactor = BlendFactor.One;
                    break;
            }

            return attachment;
        }

        private ShaderModule CreateShaderModule(byte[] spirv)
        {
            fixed (byte* code = spirv)
            {
                ShaderModuleCreateInfo createInfo = new ShaderModuleCreateInfo
                {
                    SType = StructureType.ShaderModuleCreateInfo,
                    CodeSize = (nuint)spirv.Length,
                    PCode = (uint*)code
                };
                Check(_vk.CreateShaderModule(_device, in createInfo, null, out ShaderModule module), "create Vulkan shader module");
                return module;
            }
        }

        private SilkVulkanTextureResource CreateTextureCore(Size size, RenderTextureFormat format, bool flipVerticallyWhenSampling = false, bool keepCpuData = false, bool transitionToReadable = true)
        {
            Size safeSize = new Size(Math.Max(size.Width, 1), Math.Max(size.Height, 1));
            VkImage image = CreateImage(safeSize, Format.B8G8R8A8Unorm, ImageUsageFlags.ImageUsageSampledBit | ImageUsageFlags.ImageUsageTransferDstBit | ImageUsageFlags.ImageUsageColorAttachmentBit);
            DeviceMemory memory = AllocateAndBindImageMemory(image, MemoryPropertyFlags.MemoryPropertyDeviceLocalBit);
            ImageView view = CreateImageView(image, Format.B8G8R8A8Unorm);
            SilkVulkanTextureResource resource = new SilkVulkanTextureResource(_vk, _device, safeSize, format, image, memory, view, null, VkImageLayout.Undefined, true, keepCpuData, flipVerticallyWhenSampling);
            AllocateTextureDescriptor(resource);

            if (transitionToReadable)
                TransitionTextureImmediate(resource, VkImageLayout.ShaderReadOnlyOptimal, 0, AccessFlags.AccessShaderReadBit, PipelineStageFlags.PipelineStageTopOfPipeBit, PipelineStageFlags.PipelineStageFragmentShaderBit);

            return resource;
        }

        private SilkVulkanRenderTarget CreateRenderTargetCore(Size size)
        {
            SilkVulkanTextureResource texture = CreateTextureCore(size, RenderTextureFormat.A8R8G8B8);
            Framebuffer framebuffer = CreateFramebuffer(texture.ImageView, texture.Size);
            return new SilkVulkanRenderTarget(_vk, _device, texture.Size, texture.Image, texture.ImageView, framebuffer, texture, false, 0, false)
            {
                Layout = texture.Layout
            };
        }

        private void EnsureScratchTargetSize()
        {
            Size required = _currentTarget?.Size ?? GetTargetSize();
            if (required.Width <= 0 || required.Height <= 0)
                return;

            if (_scratchTarget != null &&
                _scratchTarget.Size.Width >= required.Width &&
                _scratchTarget.Size.Height >= required.Height)
            {
                return;
            }

            if (_currentTarget == _scratchTarget)
                return;

            FlushSpriteBatch();
            EndRenderPass();

            SilkVulkanRenderTarget oldScratch = _scratchTarget;
            _scratchTarget = CreateRenderTargetCore(required);
            DeferDispose(oldScratch);
        }

        private VkImage CreateImage(Size size, Format format, ImageUsageFlags usage)
        {
            ImageCreateInfo createInfo = new ImageCreateInfo
            {
                SType = StructureType.ImageCreateInfo,
                ImageType = VkImageType.Type2D,
                Extent = new Extent3D((uint)size.Width, (uint)size.Height, 1),
                MipLevels = 1,
                ArrayLayers = 1,
                Format = format,
                Tiling = ImageTiling.Optimal,
                InitialLayout = VkImageLayout.Undefined,
                Usage = usage,
                Samples = SampleCountFlags.SampleCount1Bit,
                SharingMode = SharingMode.Exclusive
            };
            Check(_vk.CreateImage(_device, in createInfo, null, out VkImage image), "create Vulkan image");
            return image;
        }

        private DeviceMemory AllocateAndBindImageMemory(VkImage image, MemoryPropertyFlags properties)
        {
            _vk.GetImageMemoryRequirements(_device, image, out MemoryRequirements requirements);
            DeviceMemory memory = AllocateMemory(requirements, properties);
            Check(_vk.BindImageMemory(_device, image, memory, 0), "bind Vulkan image memory");
            return memory;
        }

        private ImageView CreateImageView(VkImage image, Format format)
        {
            ImageViewCreateInfo createInfo = new ImageViewCreateInfo
            {
                SType = StructureType.ImageViewCreateInfo,
                Image = image,
                ViewType = ImageViewType.Type2D,
                Format = format,
                SubresourceRange = new ImageSubresourceRange
                {
                    AspectMask = ImageAspectFlags.ImageAspectColorBit,
                    BaseMipLevel = 0,
                    LevelCount = 1,
                    BaseArrayLayer = 0,
                    LayerCount = 1
                }
            };
            Check(_vk.CreateImageView(_device, in createInfo, null, out ImageView imageView), "create Vulkan image view");
            return imageView;
        }

        private Framebuffer CreateFramebuffer(ImageView imageView, Size size)
        {
            FramebufferCreateInfo createInfo = new FramebufferCreateInfo
            {
                SType = StructureType.FramebufferCreateInfo,
                RenderPass = _renderPass,
                AttachmentCount = 1,
                PAttachments = &imageView,
                Width = (uint)Math.Max(size.Width, 1),
                Height = (uint)Math.Max(size.Height, 1),
                Layers = 1
            };
            Check(_vk.CreateFramebuffer(_device, in createInfo, null, out Framebuffer framebuffer), "create Vulkan framebuffer");
            return framebuffer;
        }

        private void AllocateTextureDescriptor(SilkVulkanTextureResource resource)
        {
            DescriptorSetLayout layout = _textureDescriptorSetLayout;

            foreach (DescriptorPool descriptorPool in _descriptorPools)
            {
                Result result = TryAllocateDescriptorSet(descriptorPool, layout, out DescriptorSet allocatedDescriptorSet);
                if (result == Result.Success)
                {
                    resource.DescriptorSet = allocatedDescriptorSet;
                    resource.DescriptorPool = descriptorPool;
                    UpdateTextureDescriptor(resource);
                    return;
                }

                if (!IsDescriptorPoolExhausted(result))
                    Check(result, "allocate Vulkan texture descriptor");
            }

            _descriptorPool = CreateDescriptorPool();
            _descriptorPools.Add(_descriptorPool);

            Check(TryAllocateDescriptorSet(_descriptorPool, layout, out DescriptorSet newDescriptorSet), "allocate Vulkan texture descriptor");
            DescriptorSet descriptorSet = newDescriptorSet;
            resource.DescriptorSet = descriptorSet;
            resource.DescriptorPool = _descriptorPool;
            UpdateTextureDescriptor(resource);
        }

        private Result TryAllocateDescriptorSet(DescriptorPool descriptorPool, DescriptorSetLayout layout, out DescriptorSet descriptorSet)
        {
            DescriptorSetAllocateInfo allocateInfo = new DescriptorSetAllocateInfo
            {
                SType = StructureType.DescriptorSetAllocateInfo,
                DescriptorPool = descriptorPool,
                DescriptorSetCount = 1,
                PSetLayouts = &layout
            };

            return _vk.AllocateDescriptorSets(_device, in allocateInfo, out descriptorSet);
        }

        private static bool IsDescriptorPoolExhausted(Result result)
        {
            return result == Result.ErrorOutOfPoolMemory ||
                   result == Result.ErrorFragmentedPool;
        }

        private void UpdateTextureDescriptor(SilkVulkanTextureResource resource)
        {
            DescriptorImageInfo imageInfo = new DescriptorImageInfo
            {
                Sampler = _textureFilter == TextureFilterMode.Linear ? _linearSampler : _pointSampler,
                ImageView = resource.ImageView,
                ImageLayout = VkImageLayout.ShaderReadOnlyOptimal
            };

            WriteDescriptorSet write = new WriteDescriptorSet
            {
                SType = StructureType.WriteDescriptorSet,
                DstSet = resource.DescriptorSet,
                DstBinding = 0,
                DescriptorCount = 1,
                DescriptorType = DescriptorType.CombinedImageSampler,
                PImageInfo = &imageInfo
            };
            _vk.UpdateDescriptorSets(_device, 1, in write, 0, (CopyDescriptorSet*)null);
            resource.AppliedFilter = _textureFilter;
        }

        private void UploadTexture(SilkVulkanTextureResource resource)
        {
            UploadTexture(resource, resource.Data);
        }

        private void UploadTexture(SilkVulkanTextureResource resource, byte[] data)
        {
            ulong uploadSize = (ulong)data.Length;
            SilkVulkanBufferResource staging = CreateBuffer(uploadSize, BufferUsageFlags.BufferUsageTransferSrcBit, MemoryPropertyFlags.MemoryPropertyHostVisibleBit | MemoryPropertyFlags.MemoryPropertyHostCoherentBit, true);
            Marshal.Copy(data, 0, staging.MappedPointer, data.Length);

            if (_frameActive && _activeFrame != null && _activeCommandBuffer.Handle != 0)
            {
                FlushSpriteBatch();
                EndRenderPass();
                RecordTextureUpload(_activeCommandBuffer, resource, staging.Buffer);
                DeferDispose(staging);
            }
            else
            {
                using (staging)
                {
                    ExecuteImmediate(commandBuffer => RecordTextureUpload(commandBuffer, resource, staging.Buffer));
                }
            }

            resource.ReleaseCpuDataIfUnused();
        }

        private void RecordTextureUpload(CommandBuffer commandBuffer, SilkVulkanTextureResource resource, Buffer stagingBuffer)
        {
            TransitionTexture(commandBuffer, resource, VkImageLayout.TransferDstOptimal, 0, AccessFlags.AccessTransferWriteBit, PipelineStageFlags.PipelineStageTopOfPipeBit, PipelineStageFlags.PipelineStageTransferBit);

            BufferImageCopy region = new BufferImageCopy
            {
                ImageSubresource = new ImageSubresourceLayers
                {
                    AspectMask = ImageAspectFlags.ImageAspectColorBit,
                    MipLevel = 0,
                    BaseArrayLayer = 0,
                    LayerCount = 1
                },
                ImageExtent = new Extent3D((uint)resource.Size.Width, (uint)resource.Size.Height, 1)
            };
            _vk.CmdCopyBufferToImage(commandBuffer, stagingBuffer, resource.Image, VkImageLayout.TransferDstOptimal, 1, in region);
            TransitionTexture(commandBuffer, resource, VkImageLayout.ShaderReadOnlyOptimal, AccessFlags.AccessTransferWriteBit, AccessFlags.AccessShaderReadBit, PipelineStageFlags.PipelineStageTransferBit, PipelineStageFlags.PipelineStageFragmentShaderBit);
        }

        private void LoadTextures()
        {
            _paletteData = ColourPaletteHelper.CreatePaletteData();
            _colourPalette = CreateTextureCore(new Size(ColourPaletteHelper.PaletteWidth, ColourPaletteHelper.PaletteHeight), RenderTextureFormat.A8R8G8B8);
            System.Buffer.BlockCopy(_paletteData, 0, _colourPalette.Data, 0, _paletteData.Length);
            UploadTexture(_colourPalette);

            _lightData ??= LightGenerator.CreateLightData(LightWidth, LightHeight);
            _lightTexture = CreateTextureCore(new Size(LightWidth, LightHeight), RenderTextureFormat.A8R8G8B8);
            System.Buffer.BlockCopy(_lightData, 0, _lightTexture.Data, 0, _lightData.Length);
            UploadTexture(_lightTexture);

            _poisonTexture = CreateTextureCore(new Size(PoisonSize, PoisonSize), RenderTextureFormat.A8R8G8B8);
            for (int y = 0; y < PoisonSize; y++)
            {
                for (int x = 0; x < PoisonSize; x++)
                {
                    Color colour = x == 0 || y == 0 || x == PoisonSize - 1 || y == PoisonSize - 1 ? Color.Black : Color.White;
                    int index = (y * PoisonSize + x) * 4;
                    _poisonTexture.Data[index] = colour.B;
                    _poisonTexture.Data[index + 1] = colour.G;
                    _poisonTexture.Data[index + 2] = colour.R;
                    _poisonTexture.Data[index + 3] = colour.A;
                }
            }
            UploadTexture(_poisonTexture);
        }

        private TextureLock CreateCompressedTextureWriteLock(SilkVulkanTextureResource resource, int blockSize, Action<byte[], byte[], int, int> decompress)
        {
            int blockCountX = Math.Max(1, (resource.Size.Width + 3) / 4);
            int blockCountY = Math.Max(1, (resource.Size.Height + 3) / 4);
            int compressedSize = blockCountX * blockCountY * blockSize;
            byte[] compressed = new byte[compressedSize];
            GCHandle handle = GCHandle.Alloc(compressed, GCHandleType.Pinned);

            return TextureLock.From(handle.AddrOfPinnedObject(), compressedSize, () =>
            {
                try
                {
                    byte[] data = resource.Data;
                    decompress(compressed, data, resource.Size.Width, resource.Size.Height);
                    PremultiplyAlpha(data, resource.Size.Width, resource.Size.Height);
                    UploadTexture(resource, data);
                }
                finally
                {
                    handle.Free();
                }
            });
        }

        private void BeginRenderPass(SilkVulkanRenderTarget target)
        {
            if (!_frameActive || target == null)
                return;

            if (_renderPassActive && _recordingTarget == target)
                return;

            EndRenderPass();
            TransitionTarget(target, VkImageLayout.ColorAttachmentOptimal, 0, AccessFlags.AccessColorAttachmentWriteBit, PipelineStageFlags.PipelineStageTopOfPipeBit, PipelineStageFlags.PipelineStageColorAttachmentOutputBit);

            ClearValue clearValue = CreateClearValue(0F, 0F, 0F, 0F);
            RenderPassBeginInfo beginInfo = new RenderPassBeginInfo
            {
                SType = StructureType.RenderPassBeginInfo,
                RenderPass = _renderPass,
                Framebuffer = target.Framebuffer,
                RenderArea = new Rect2D(new Offset2D(0, 0), new Extent2D((uint)target.Size.Width, (uint)target.Size.Height)),
                ClearValueCount = 1,
                PClearValues = &clearValue
            };

            _vk.CmdBeginRenderPass(_activeCommandBuffer, in beginInfo, SubpassContents.Inline);

            Viewport viewport = new Viewport(0, 0, target.Size.Width, target.Size.Height, 0F, 1F);
            Rect2D scissor = new Rect2D(new Offset2D(0, 0), new Extent2D((uint)target.Size.Width, (uint)target.Size.Height));
            _vk.CmdSetViewport(_activeCommandBuffer, 0, 1, in viewport);
            _vk.CmdSetScissor(_activeCommandBuffer, 0, 1, in scissor);

            _renderPassActive = true;
            _recordingTarget = target;
            _boundPipeline = default;
            _boundTextureBlendMode = null;
            _boundTextureBlendRate = float.NaN;
            _boundDescriptorSet = default;
        }

        private void EndRenderPass()
        {
            if (!_renderPassActive)
                return;

            FlushSpriteBatch();
            _vk.CmdEndRenderPass(_activeCommandBuffer);
            _renderPassActive = false;
            _recordingTarget = null;
            _boundPipeline = default;
            _boundTextureBlendMode = null;
            _boundTextureBlendRate = float.NaN;
            _boundDescriptorSet = default;
        }

        private void TransitionTarget(SilkVulkanRenderTarget target, VkImageLayout newLayout, AccessFlags srcAccess, AccessFlags dstAccess, PipelineStageFlags srcStage, PipelineStageFlags dstStage)
        {
            if (target == null || target.Layout == newLayout)
                return;

            ImageMemoryBarrier barrier = CreateImageBarrier(target.Image, target.Layout, newLayout, srcAccess, dstAccess);
            _vk.CmdPipelineBarrier(_activeCommandBuffer, srcStage, dstStage, 0, 0, (MemoryBarrier*)null, 0, (BufferMemoryBarrier*)null, 1, in barrier);
            target.Layout = newLayout;
            if (target.Texture != null)
                target.Texture.Layout = newLayout;
        }

        private void TransitionTextureImmediate(SilkVulkanTextureResource texture, VkImageLayout newLayout, AccessFlags srcAccess, AccessFlags dstAccess, PipelineStageFlags srcStage, PipelineStageFlags dstStage)
        {
            ExecuteImmediate(commandBuffer => TransitionTexture(commandBuffer, texture, newLayout, srcAccess, dstAccess, srcStage, dstStage));
        }

        private void TransitionTexture(CommandBuffer commandBuffer, SilkVulkanTextureResource texture, VkImageLayout newLayout, AccessFlags srcAccess, AccessFlags dstAccess, PipelineStageFlags srcStage, PipelineStageFlags dstStage)
        {
            if (texture.Layout == newLayout)
                return;

            ImageMemoryBarrier barrier = CreateImageBarrier(texture.Image, texture.Layout, newLayout, srcAccess, dstAccess);
            _vk.CmdPipelineBarrier(commandBuffer, srcStage, dstStage, 0, 0, (MemoryBarrier*)null, 0, (BufferMemoryBarrier*)null, 1, in barrier);
            texture.Layout = newLayout;
            SyncRenderTargetLayout(texture, newLayout);
        }

        private void SyncRenderTargetLayout(SilkVulkanTextureResource texture, VkImageLayout layout)
        {
            if (_scratchTarget?.Texture == texture)
                _scratchTarget.Layout = layout;

            foreach (SilkVulkanRenderTarget target in _renderTargets)
            {
                if (target.Texture == texture)
                    target.Layout = layout;
            }
        }

        private ImageMemoryBarrier CreateImageBarrier(VkImage image, VkImageLayout oldLayout, VkImageLayout newLayout, AccessFlags srcAccess, AccessFlags dstAccess)
        {
            return new ImageMemoryBarrier
            {
                SType = StructureType.ImageMemoryBarrier,
                OldLayout = oldLayout,
                NewLayout = newLayout,
                SrcAccessMask = srcAccess,
                DstAccessMask = dstAccess,
                SrcQueueFamilyIndex = Vk.QueueFamilyIgnored,
                DstQueueFamilyIndex = Vk.QueueFamilyIgnored,
                Image = image,
                SubresourceRange = new ImageSubresourceRange
                {
                    AspectMask = ImageAspectFlags.ImageAspectColorBit,
                    BaseMipLevel = 0,
                    LevelCount = 1,
                    BaseArrayLayer = 0,
                    LayerCount = 1
                }
            };
        }

        private void EnsureTextureReadable(SilkVulkanTextureResource resource)
        {
            if (resource.Layout == VkImageLayout.ShaderReadOnlyOptimal)
                return;

            FlushSpriteBatch();
            EndRenderPass();
            TransitionTexture(_activeCommandBuffer, resource, VkImageLayout.ShaderReadOnlyOptimal, AccessFlags.AccessColorAttachmentWriteBit, AccessFlags.AccessShaderReadBit, PipelineStageFlags.PipelineStageColorAttachmentOutputBit, PipelineStageFlags.PipelineStageFragmentShaderBit);
        }

        private void QueueSprite(SilkVulkanTextureResource resource, ClientBlendMode blendMode, float blendRate, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float u0, float u1, float v0, float v1, Vector4 colour)
        {
            if (_spriteBatchInstanceCount > 0 &&
                (_spriteBatchTexture != resource ||
                 _spriteBatchTarget != _currentTarget ||
                 _spriteBatchBlendMode != blendMode ||
                 _spriteBatchBlendRate != blendRate ||
                 _spriteBatchInstanceCount >= MaxSpriteBatchInstances))
            {
                FlushSpriteBatch();
            }

            if (!HasFrameSpace((ulong)sizeof(SpriteInstance), 16))
                FlushSpriteBatch();

            SpriteInstance* instances = AllocateSpriteInstances(1, out ulong offset);
            if (_spriteBatchInstanceCount == 0)
            {
                _spriteBatchTexture = resource;
                _spriteBatchTarget = _currentTarget;
                _spriteBatchBlendMode = blendMode;
                _spriteBatchBlendRate = blendRate;
                _spriteBatchOffset = offset;
            }

            instances[0] = new SpriteInstance(
                new Vector4(p0.X, p1.X, p2.X, p3.X),
                new Vector4(p0.Y, p1.Y, p2.Y, p3.Y),
                new Vector4(u0, u1, u1, u0),
                new Vector4(v0, v0, v1, v1),
                colour);

            _spriteBatchInstanceCount++;
        }

        private void DrawSpriteImmediate(SilkVulkanTextureResource resource, ClientBlendMode blendMode, float blendRate, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float u0, float u1, float v0, float v1, Vector4 colour, PushConstants push)
        {
            FlushSpriteBatch();

            if (!HasFrameSpace((ulong)sizeof(SpriteInstance), 16))
                throw new InvalidOperationException("The Vulkan dynamic sprite instance buffer is full for this frame.");

            SpriteInstance* instances = AllocateSpriteInstances(1, out ulong offset);
            instances[0] = new SpriteInstance(
                new Vector4(p0.X, p1.X, p2.X, p3.X),
                new Vector4(p0.Y, p1.Y, p2.Y, p3.Y),
                new Vector4(u0, u1, u1, u0),
                new Vector4(v0, v0, v1, v1),
                colour);

            BindTexturePipeline(blendMode, blendRate);
            BindTexture(resource);

            _vk.CmdPushConstants(_activeCommandBuffer, _texturePipelineLayout, ShaderStageFlags.ShaderStageVertexBit | ShaderStageFlags.ShaderStageFragmentBit, 0, (uint)sizeof(PushConstants), &push);

            Buffer buffer = _activeFrame.VertexBuffer.Buffer;
            _vk.CmdBindVertexBuffers(_activeCommandBuffer, 0, 1, in buffer, in offset);
            _vk.CmdDraw(_activeCommandBuffer, 6, 1, 0, 0);
        }

        private void FlushSpriteBatch()
        {
            if (_spriteBatchInstanceCount == 0)
                return;

            if (!_renderPassActive || _spriteBatchTarget == null || _spriteBatchTexture == null)
            {
                ResetSpriteBatch();
                return;
            }

            BindTexturePipeline(_spriteBatchBlendMode, _spriteBatchBlendRate);
            BindTexture(_spriteBatchTexture);

            PushConstants push = new PushConstants
            {
                Viewport = new Vector2(_spriteBatchTarget.Size.Width, _spriteBatchTarget.Size.Height),
                Colour = Vector4.One,
                SourceUv = new Vector4(0F, 0F, 1F, 1F),
                OutlineColour = Vector4.Zero,
                Effect = Vector4.Zero
            };
            _vk.CmdPushConstants(_activeCommandBuffer, _texturePipelineLayout, ShaderStageFlags.ShaderStageVertexBit | ShaderStageFlags.ShaderStageFragmentBit, 0, (uint)sizeof(PushConstants), &push);

            Buffer buffer = _activeFrame.VertexBuffer.Buffer;
            _vk.CmdBindVertexBuffers(_activeCommandBuffer, 0, 1, in buffer, in _spriteBatchOffset);
            _vk.CmdDraw(_activeCommandBuffer, 6, _spriteBatchInstanceCount, 0, 0);
            ResetSpriteBatch();
        }

        private void ResetSpriteBatch()
        {
            _spriteBatchTexture = null;
            _spriteBatchTarget = null;
            _spriteBatchBlendMode = ClientBlendMode.NONE;
            _spriteBatchBlendRate = 0F;
            _spriteBatchOffset = 0;
            _spriteBatchInstanceCount = 0;
        }

        private PushConstants CreateTexturePushConstants(float effectMode, Vector4 sourceUv, Vector4 outlineColour, float outlineThickness, Size textureSize)
        {
            return new PushConstants
            {
                Viewport = new Vector2(_currentTarget.Size.Width, _currentTarget.Size.Height),
                Colour = Vector4.One,
                SourceUv = sourceUv,
                OutlineColour = outlineColour,
                Effect = new Vector4(effectMode, outlineThickness, textureSize.Width, textureSize.Height)
            };
        }

        private PushConstants CreateTexturePushConstants(float effectMode, Vector4 effectData, Vector4 effectColour, float effectWidth, float effectOpacity)
        {
            return new PushConstants
            {
                Viewport = new Vector2(_currentTarget.Size.Width, _currentTarget.Size.Height),
                Colour = Vector4.One,
                SourceUv = effectData,
                OutlineColour = effectColour,
                Effect = new Vector4(effectMode, effectWidth, effectOpacity, 0F)
            };
        }

        private void BindTexturePipeline(ClientBlendMode blendMode, float blendRate)
        {
            if (!_texturePipelines.TryGetValue(blendMode, out Pipeline pipeline))
                pipeline = _texturePipelines[ClientBlendMode.NORMAL];

            BindPipeline(pipeline);
            if (_boundTextureBlendMode != blendMode || _boundTextureBlendRate != blendRate)
            {
                _boundTextureBlendMode = blendMode;
                _boundTextureBlendRate = blendRate;
                if (RequiresBlendConstants(blendMode))
                {
                    float* constants = stackalloc float[4] { blendRate, blendRate, blendRate, blendRate };
                    _vk.CmdSetBlendConstants(_activeCommandBuffer, constants);
                }
            }
        }

        private void BindPipeline(Pipeline pipeline)
        {
            if (_boundPipeline.Handle == pipeline.Handle)
                return;

            _vk.CmdBindPipeline(_activeCommandBuffer, PipelineBindPoint.Graphics, pipeline);
            _boundPipeline = pipeline;
        }

        private void BindTexture(SilkVulkanTextureResource resource)
        {
            if (resource.AppliedFilter != _textureFilter)
                UpdateTextureDescriptor(resource);

            if (_boundDescriptorSet.Handle == resource.DescriptorSet.Handle)
                return;

            DescriptorSet set = resource.DescriptorSet;
            _vk.CmdBindDescriptorSets(_activeCommandBuffer, PipelineBindPoint.Graphics, _texturePipelineLayout, 0, 1, in set, 0, null);
            _boundDescriptorSet = set;
        }

        private ClientBlendMode GetAppliedBlendMode()
        {
            if (!_blending || _blendMode == ClientBlendMode.NONE)
                return ClientBlendMode.NONE;

            return _blendMode;
        }

        private float GetBlendConstantRate(ClientBlendMode blendMode)
        {
            return RequiresBlendConstants(blendMode) ? _blendRate : 0F;
        }

        private static bool RequiresBlendConstants(ClientBlendMode blendMode)
        {
            return blendMode == ClientBlendMode.HIGHLIGHT || blendMode == ClientBlendMode.INVLIGHT;
        }

        private static bool AppliesBlendRateToVertexColour(ClientBlendMode blendMode)
        {
            return blendMode == ClientBlendMode.COLORFY ||
                   blendMode == ClientBlendMode.MASK ||
                   blendMode == ClientBlendMode.EFFECTMASK ||
                   blendMode == ClientBlendMode.LIGHTMAP;
        }

        private TextureVertex* AllocateVertices(int vertexCount, out ulong offset)
        {
            ulong bytes = (ulong)(vertexCount * sizeof(TextureVertex));
            ulong alignedOffset = Align(_activeFrame.VertexOffset, 16);
            if (alignedOffset + bytes > _activeFrame.VertexBuffer.Size)
                throw new InvalidOperationException("The Vulkan dynamic vertex buffer is full for this frame.");

            offset = alignedOffset;
            _activeFrame.VertexOffset = alignedOffset + bytes;
            return (TextureVertex*)((byte*)_activeFrame.VertexBuffer.MappedPointer + alignedOffset);
        }

        private SpriteInstance* AllocateSpriteInstances(int instanceCount, out ulong offset)
        {
            ulong bytes = (ulong)(instanceCount * sizeof(SpriteInstance));
            ulong alignedOffset = Align(_activeFrame.VertexOffset, 16);
            if (alignedOffset + bytes > _activeFrame.VertexBuffer.Size)
                throw new InvalidOperationException("The Vulkan dynamic sprite instance buffer is full for this frame.");

            offset = alignedOffset;
            _activeFrame.VertexOffset = alignedOffset + bytes;
            return (SpriteInstance*)((byte*)_activeFrame.VertexBuffer.MappedPointer + alignedOffset);
        }

        private bool HasFrameSpace(ulong bytes, ulong alignment)
        {
            return Align(_activeFrame.VertexOffset, alignment) + bytes <= _activeFrame.VertexBuffer.Size;
        }

        private void DeferDispose(IDisposable resource)
        {
            if (resource == null)
                return;

            _deferredDisposals.Enqueue(new DeferredDisposal(resource, MaxFramesInFlight));
        }

        private void FlushDeferredDisposals(bool force = false)
        {
            int count = _deferredDisposals.Count;
            for (int i = 0; i < count; i++)
            {
                DeferredDisposal disposal = _deferredDisposals.Dequeue();
                if (!force && disposal.FramesRemaining > 1)
                {
                    disposal.FramesRemaining--;
                    _deferredDisposals.Enqueue(disposal);
                    continue;
                }

                disposal.Resource.Dispose();
            }
        }

        private SilkVulkanBufferResource CreateBuffer(ulong size, BufferUsageFlags usage, MemoryPropertyFlags properties, bool map)
        {
            BufferCreateInfo createInfo = new BufferCreateInfo
            {
                SType = StructureType.BufferCreateInfo,
                Size = size,
                Usage = usage,
                SharingMode = SharingMode.Exclusive
            };
            Check(_vk.CreateBuffer(_device, in createInfo, null, out Buffer buffer), "create Vulkan buffer");
            _vk.GetBufferMemoryRequirements(_device, buffer, out MemoryRequirements requirements);
            DeviceMemory memory = AllocateMemory(requirements, properties);
            Check(_vk.BindBufferMemory(_device, buffer, memory, 0), "bind Vulkan buffer memory");

            IntPtr mappedPointer = IntPtr.Zero;
            if (map)
            {
                void* mapped = null;
                Check(_vk.MapMemory(_device, memory, 0, size, 0, ref mapped), "map Vulkan buffer memory");
                mappedPointer = (IntPtr)mapped;
            }

            return new SilkVulkanBufferResource(_vk, _device, buffer, memory, size, mappedPointer);
        }

        private DeviceMemory AllocateMemory(MemoryRequirements requirements, MemoryPropertyFlags properties)
        {
            MemoryAllocateInfo allocateInfo = new MemoryAllocateInfo
            {
                SType = StructureType.MemoryAllocateInfo,
                AllocationSize = requirements.Size,
                MemoryTypeIndex = FindMemoryType(requirements.MemoryTypeBits, properties)
            };
            Check(_vk.AllocateMemory(_device, in allocateInfo, null, out DeviceMemory memory), "allocate Vulkan memory");
            return memory;
        }

        private uint FindMemoryType(uint typeFilter, MemoryPropertyFlags properties)
        {
            for (uint i = 0; i < _memoryProperties.MemoryTypeCount; i++)
            {
                if ((typeFilter & (1U << (int)i)) == 0)
                    continue;

                if ((_memoryProperties.MemoryTypes[(int)i].PropertyFlags & properties) == properties)
                    return i;
            }

            throw new InvalidOperationException($"No Vulkan memory type supports {properties}.");
        }

        private void ExecuteImmediate(Action<CommandBuffer> record)
        {
            CommandBufferAllocateInfo allocateInfo = new CommandBufferAllocateInfo
            {
                SType = StructureType.CommandBufferAllocateInfo,
                CommandPool = _uploadCommandPool,
                Level = CommandBufferLevel.Primary,
                CommandBufferCount = 1
            };
            Check(_vk.AllocateCommandBuffers(_device, in allocateInfo, out CommandBuffer commandBuffer), "allocate Vulkan upload command buffer");

            try
            {
                CommandBufferBeginInfo beginInfo = new CommandBufferBeginInfo
                {
                    SType = StructureType.CommandBufferBeginInfo,
                    Flags = CommandBufferUsageFlags.CommandBufferUsageOneTimeSubmitBit
                };
                Check(_vk.BeginCommandBuffer(commandBuffer, in beginInfo), "begin Vulkan upload command buffer");
                record(commandBuffer);
                Check(_vk.EndCommandBuffer(commandBuffer), "end Vulkan upload command buffer");

                SubmitInfo submitInfo = new SubmitInfo
                {
                    SType = StructureType.SubmitInfo,
                    CommandBufferCount = 1,
                    PCommandBuffers = &commandBuffer
                };
                Check(_vk.QueueSubmit(_graphicsQueue, 1, in submitInfo, default), "submit Vulkan upload command buffer");
                Check(_vk.QueueWaitIdle(_graphicsQueue), "wait for Vulkan upload queue");
            }
            finally
            {
                _vk.FreeCommandBuffers(_device, _uploadCommandPool, 1, in commandBuffer);
            }
        }

        private void ResizeBackBufferIfNeeded(bool force = false)
        {
            Size size = GetTargetSize();
            if (!force && size.Width == _swapchainExtent.Width && size.Height == _swapchainExtent.Height)
                return;

            RecreateSwapchain();
            _scratchTarget?.Dispose();
            _scratchTarget = CreateRenderTargetCore(size);
        }

        private void RecreateSwapchain()
        {
            if (_device.Handle == IntPtr.Zero)
                return;

            _vk.DeviceWaitIdle(_device);
            DestroySwapchainResources();
            CreateSwapchain();
            CreateSwapchainFramebuffers();
            _currentTarget = _backBufferTargets.Length > 0 ? _backBufferTargets[0] : null;
        }

        private void DestroySwapchainResources()
        {
            if (_backBufferTargets != null)
            {
                foreach (SilkVulkanRenderTarget target in _backBufferTargets)
                    target?.Dispose();
            }

            _backBufferTargets = Array.Empty<SilkVulkanRenderTarget>();
            _swapchainImages = Array.Empty<VkImage>();

            if (_swapchain.Handle != 0)
            {
                _swapchainApi.DestroySwapchain(_device, _swapchain, null);
                _swapchain = default;
            }
        }

        private Size GetTargetSize()
        {
            Size size = _context.RenderTarget.ClientSize;
            return new Size(Math.Max(size.Width, 1), Math.Max(size.Height, 1));
        }

        private static ClearValue CreateClearValue(float r, float g, float b, float a)
        {
            return new ClearValue
            {
                Color = new ClearColorValue
                {
                    Float32_0 = r,
                    Float32_1 = g,
                    Float32_2 = b,
                    Float32_3 = a
                }
            };
        }

        private static ClearRect ToClearRect(Rectangle region, Size targetSize)
        {
            Rectangle clipped = Rectangle.Intersect(region, new Rectangle(Point.Empty, targetSize));
            return new ClearRect
            {
                Rect = new Rect2D
                {
                    Offset = new Offset2D(clipped.X, clipped.Y),
                    Extent = new Extent2D((uint)Math.Max(clipped.Width, 0), (uint)Math.Max(clipped.Height, 0))
                },
                BaseArrayLayer = 0,
                LayerCount = 1
            };
        }

        private static ulong Align(ulong value, ulong alignment)
        {
            return (value + alignment - 1) & ~(alignment - 1);
        }

        private static float SnapLineCoordinate(float value)
        {
            return (float)Math.Floor(value) + 0.5F;
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

        private void ApplyWindowStyle()
        {
            if (_context?.RenderTarget == null)
                return;

            _context.RenderTarget.FormBorderStyle = (Config.FullScreen || Config.Borderless) ? FormBorderStyle.None : FormBorderStyle.FixedDialog;
            _context.RenderTarget.MaximizeBox = false;
        }

        private bool ApplyWindowBounds(bool forceCenter = false)
        {
            if (_context?.RenderTarget == null)
                return true;

            if (Config.FullScreen)
            {
                if (!ApplyFullscreenDisplayMode())
                    return false;

                Screen screen = GetSelectedScreen();
                _context.RenderTarget.Bounds = new Rectangle(screen.Bounds.Location, Config.GameSize);
                return true;
            }

            RestoreDisplayMode();

            if (_context.RenderTarget.ClientSize != Config.GameSize)
                _context.RenderTarget.ClientSize = Config.GameSize;

            if (forceCenter || !IsWindowOnVisibleScreen())
                CenterOnSelectedMonitor();

            return true;
        }

        private bool ApplyFullscreenDisplayMode()
        {
            Screen screen = GetSelectedScreen();
            string deviceName = screen.DeviceName;

            if (_displayModeChanged &&
                string.Equals(_displayModeDeviceName, deviceName, StringComparison.OrdinalIgnoreCase) &&
                _displayModeSize == Config.GameSize)
                return true;

            RestoreDisplayMode();

            if (!TryGetDisplayMode(deviceName, Config.GameSize, out DevMode mode))
                return false;

            mode.dmFields |= DmPelsWidth | DmPelsHeight;

            int result = ChangeDisplaySettingsEx(deviceName, ref mode, IntPtr.Zero, CdsFullscreen, IntPtr.Zero);
            if (result != DispChangeSuccessful)
                return false;

            _displayModeChanged = true;
            _displayModeDeviceName = deviceName;
            _displayModeSize = Config.GameSize;
            return true;
        }

        private static bool TryGetDisplayMode(string deviceName, Size size, out DevMode displayMode)
        {
            for (int modeIndex = 0; ; modeIndex++)
            {
                DevMode mode = CreateDevMode();
                if (!EnumDisplaySettings(deviceName, modeIndex, ref mode))
                    break;

                if (mode.dmPelsWidth != (uint)size.Width || mode.dmPelsHeight != (uint)size.Height)
                    continue;

                displayMode = mode;
                return true;
            }

            displayMode = default;
            return false;
        }

        private void RestoreDisplayMode()
        {
            if (!_displayModeChanged)
                return;

            ChangeDisplaySettingsEx(_displayModeDeviceName, IntPtr.Zero, IntPtr.Zero, 0, IntPtr.Zero);
            _displayModeChanged = false;
            _displayModeDeviceName = null;
            _displayModeSize = Size.Empty;
        }

        private bool IsWindowOnVisibleScreen()
        {
            Rectangle currentBounds = _context.RenderTarget.Bounds;

            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(currentBounds))
                    return true;
            }

            return false;
        }

        private static Screen GetSelectedScreen()
        {
            //int index = Config.SelectedMonitorIndex;
            //if (index < 0 || index >= Screen.AllScreens.Length)
            //{
            //    index = 0;
            //    Config.SelectedMonitorIndex = 0;
            //}

            int index = 0;

            return Screen.AllScreens[index];
        }

        private static DevMode CreateDevMode()
        {
            return new DevMode { dmSize = (ushort)Marshal.SizeOf<DevMode>() };
        }

        private static byte* ToUtf8(string value)
        {
            return (byte*)Marshal.StringToHGlobalAnsi(value);
        }

        private static void FreeUtf8(byte* value)
        {
            if (value != null)
                Marshal.FreeHGlobal((IntPtr)value);
        }

        private static void Check(Result result, string operation, params Result[] allowedResults)
        {
            if (result == Result.Success)
                return;

            foreach (Result allowed in allowedResults)
            {
                if (result == allowed)
                    return;
            }

            throw new InvalidOperationException($"Failed to {operation}: {result}.");
        }

        private static void PremultiplyAlpha(byte[] data, int width, int height)
        {
            int stride = width * 4;

            for (int y = 0; y < height; y++)
            {
                int row = y * stride;
                byte lastB = 0, lastG = 0, lastR = 0;
                bool hasColor = false;

                for (int x = 0; x < stride && row + x + 3 < data.Length; x += 4)
                {
                    byte alpha = data[row + x + 3];

                    if (alpha > 0)
                    {
                        lastB = data[row + x];
                        lastG = data[row + x + 1];
                        lastR = data[row + x + 2];
                        hasColor = true;
                    }
                    else if (hasColor)
                    {
                        data[row + x] = lastB;
                        data[row + x + 1] = lastG;
                        data[row + x + 2] = lastR;
                    }
                }

                lastB = lastG = lastR = 0;
                hasColor = false;

                for (int x = stride - 4; x >= 0 && row + x + 3 < data.Length; x -= 4)
                {
                    byte alpha = data[row + x + 3];

                    if (alpha > 0)
                    {
                        lastB = data[row + x];
                        lastG = data[row + x + 1];
                        lastR = data[row + x + 2];
                        hasColor = true;
                    }
                    else if (hasColor)
                    {
                        data[row + x] = lastB;
                        data[row + x + 1] = lastG;
                        data[row + x + 2] = lastR;
                    }
                }

                for (int x = 0; x < stride && row + x + 3 < data.Length; x += 4)
                {
                    int alpha = data[row + x + 3];
                    data[row + x] = (byte)(data[row + x] * alpha / 255);
                    data[row + x + 1] = (byte)(data[row + x + 1] * alpha / 255);
                    data[row + x + 2] = (byte)(data[row + x + 2] * alpha / 255);
                }
            }
        }

        private static void DecompressDxt1(byte[] source, byte[] destination, int width, int height)
        {
            Array.Clear(destination, 0, destination.Length);

            int blockCountX = Math.Max(1, (width + 3) / 4);
            int blockCountY = Math.Max(1, (height + 3) / 4);
            int sourceIndex = 0;

            for (int by = 0; by < blockCountY; by++)
            {
                for (int bx = 0; bx < blockCountX; bx++)
                {
                    if (sourceIndex + 8 > source.Length)
                        return;

                    ushort c0 = BitConverter.ToUInt16(source, sourceIndex);
                    ushort c1 = BitConverter.ToUInt16(source, sourceIndex + 2);
                    uint bits = BitConverter.ToUInt32(source, sourceIndex + 4);
                    sourceIndex += 8;

                    Color[] colours = BuildDxt1Palette(c0, c1);
                    for (int py = 0; py < 4; py++)
                    {
                        for (int px = 0; px < 4; px++)
                        {
                            int x = bx * 4 + px;
                            int y = by * 4 + py;
                            if (x >= width || y >= height)
                                continue;

                            int colourIndex = (int)((bits >> (2 * (py * 4 + px))) & 0x3);
                            Color colour = colours[colourIndex];
                            int destinationIndex = (y * width + x) * 4;
                            destination[destinationIndex] = colour.B;
                            destination[destinationIndex + 1] = colour.G;
                            destination[destinationIndex + 2] = colour.R;
                            destination[destinationIndex + 3] = colour.A;
                        }
                    }
                }
            }
        }

        private static Color[] BuildDxt1Palette(ushort c0, ushort c1)
        {
            Color colour0 = ConvertRgb565(c0);
            Color colour1 = ConvertRgb565(c1);
            Color[] colours = new Color[4];
            colours[0] = colour0;
            colours[1] = colour1;

            if (c0 > c1)
            {
                colours[2] = Color.FromArgb(255, (2 * colour0.R + colour1.R) / 3, (2 * colour0.G + colour1.G) / 3, (2 * colour0.B + colour1.B) / 3);
                colours[3] = Color.FromArgb(255, (colour0.R + 2 * colour1.R) / 3, (colour0.G + 2 * colour1.G) / 3, (colour0.B + 2 * colour1.B) / 3);
            }
            else
            {
                colours[2] = Color.FromArgb(255, (colour0.R + colour1.R) / 2, (colour0.G + colour1.G) / 2, (colour0.B + colour1.B) / 2);
                colours[3] = Color.FromArgb(0, 0, 0, 0);
            }

            return colours;
        }

        private static void DecompressDxt5(byte[] source, byte[] destination, int width, int height)
        {
            Array.Clear(destination, 0, destination.Length);

            int blockCountX = Math.Max(1, (width + 3) / 4);
            int blockCountY = Math.Max(1, (height + 3) / 4);
            int sourceIndex = 0;

            for (int by = 0; by < blockCountY; by++)
            {
                for (int bx = 0; bx < blockCountX; bx++)
                {
                    if (sourceIndex + 16 > source.Length)
                        return;

                    byte alpha0 = source[sourceIndex];
                    byte alpha1 = source[sourceIndex + 1];
                    ulong alphaBits = 0;
                    for (int i = 0; i < 6; i++)
                        alphaBits |= (ulong)source[sourceIndex + 2 + i] << (8 * i);

                    ushort c0 = BitConverter.ToUInt16(source, sourceIndex + 8);
                    ushort c1 = BitConverter.ToUInt16(source, sourceIndex + 10);
                    uint colourBits = BitConverter.ToUInt32(source, sourceIndex + 12);
                    sourceIndex += 16;

                    byte[] alphas = BuildDxt5AlphaPalette(alpha0, alpha1);
                    Color[] colours = BuildDxt5ColourPalette(c0, c1);

                    for (int py = 0; py < 4; py++)
                    {
                        for (int px = 0; px < 4; px++)
                        {
                            int x = bx * 4 + px;
                            int y = by * 4 + py;
                            if (x >= width || y >= height)
                                continue;

                            int texel = py * 4 + px;
                            int alphaIndex = (int)((alphaBits >> (3 * texel)) & 0x7);
                            int colourIndex = (int)((colourBits >> (2 * texel)) & 0x3);
                            Color colour = colours[colourIndex];

                            int destinationIndex = (y * width + x) * 4;
                            destination[destinationIndex] = colour.B;
                            destination[destinationIndex + 1] = colour.G;
                            destination[destinationIndex + 2] = colour.R;
                            destination[destinationIndex + 3] = alphas[alphaIndex];
                        }
                    }
                }
            }
        }

        private static byte[] BuildDxt5AlphaPalette(byte alpha0, byte alpha1)
        {
            byte[] alphas = new byte[8];
            alphas[0] = alpha0;
            alphas[1] = alpha1;

            if (alpha0 > alpha1)
            {
                alphas[2] = (byte)((6 * alpha0 + alpha1) / 7);
                alphas[3] = (byte)((5 * alpha0 + 2 * alpha1) / 7);
                alphas[4] = (byte)((4 * alpha0 + 3 * alpha1) / 7);
                alphas[5] = (byte)((3 * alpha0 + 4 * alpha1) / 7);
                alphas[6] = (byte)((2 * alpha0 + 5 * alpha1) / 7);
                alphas[7] = (byte)((alpha0 + 6 * alpha1) / 7);
            }
            else
            {
                alphas[2] = (byte)((4 * alpha0 + alpha1) / 5);
                alphas[3] = (byte)((3 * alpha0 + 2 * alpha1) / 5);
                alphas[4] = (byte)((2 * alpha0 + 3 * alpha1) / 5);
                alphas[5] = (byte)((alpha0 + 4 * alpha1) / 5);
                alphas[6] = 0;
                alphas[7] = 255;
            }

            return alphas;
        }

        private static Color[] BuildDxt5ColourPalette(ushort c0, ushort c1)
        {
            Color colour0 = ConvertRgb565(c0);
            Color colour1 = ConvertRgb565(c1);

            return new[]
            {
                colour0,
                colour1,
                Color.FromArgb(255, (2 * colour0.R + colour1.R) / 3, (2 * colour0.G + colour1.G) / 3, (2 * colour0.B + colour1.B) / 3),
                Color.FromArgb(255, (colour0.R + 2 * colour1.R) / 3, (colour0.G + 2 * colour1.G) / 3, (colour0.B + 2 * colour1.B) / 3)
            };
        }

        private static Color ConvertRgb565(ushort value)
        {
            int r = ((value >> 11) & 0x1F) * 255 / 31;
            int g = ((value >> 5) & 0x3F) * 255 / 63;
            int b = (value & 0x1F) * 255 / 31;
            return Color.FromArgb(255, r, g, b);
        }

        private static bool AppStillIdle => !PeekMessage(out PeekMsg msg, IntPtr.Zero, 0, 0, 0);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern bool PeekMessage(out PeekMsg msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DevMode devMode);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int ChangeDisplaySettingsEx(string deviceName, ref DevMode devMode, IntPtr hwnd, int flags, IntPtr lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int ChangeDisplaySettingsEx(string deviceName, IntPtr devMode, IntPtr hwnd, int flags, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [StructLayout(LayoutKind.Sequential)]
        private struct TextureVertex
        {
            public float X;
            public float Y;
            public float U;
            public float V;

            public TextureVertex(float x, float y, float u, float v)
            {
                X = x;
                Y = y;
                U = u;
                V = v;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SpriteInstance
        {
            public Vector4 PositionX;
            public Vector4 PositionY;
            public Vector4 TexCoordU;
            public Vector4 TexCoordV;
            public Vector4 Colour;

            public SpriteInstance(Vector4 positionX, Vector4 positionY, Vector4 texCoordU, Vector4 texCoordV, Vector4 colour)
            {
                PositionX = positionX;
                PositionY = positionY;
                TexCoordU = texCoordU;
                TexCoordV = texCoordV;
                Colour = colour;
            }
        }

        private enum VertexInputKind
        {
            Position,
            PositionTexture,
            SpriteInstance
        }

        [StructLayout(LayoutKind.Explicit, Size = 80)]
        private struct PushConstants
        {
            [FieldOffset(0)]
            public Vector2 Viewport;
            [FieldOffset(16)]
            public Vector4 Colour;
            [FieldOffset(32)]
            public Vector4 SourceUv;
            [FieldOffset(48)]
            public Vector4 OutlineColour;
            [FieldOffset(64)]
            public Vector4 Effect;
        }

        private struct DeferredDisposal
        {
            public DeferredDisposal(IDisposable resource, int framesRemaining)
            {
                Resource = resource;
                FramesRemaining = framesRemaining;
            }

            public IDisposable Resource { get; }
            public int FramesRemaining { get; set; }
        }

        private sealed class FrameResources
        {
            public FrameResources(CommandBuffer commandBuffer, SilkVulkanBufferResource vertexBuffer, VkSemaphore imageAvailableSemaphore, VkSemaphore renderFinishedSemaphore, Fence inFlightFence)
            {
                CommandBuffer = commandBuffer;
                VertexBuffer = vertexBuffer;
                ImageAvailableSemaphore = imageAvailableSemaphore;
                RenderFinishedSemaphore = renderFinishedSemaphore;
                InFlightFence = inFlightFence;
            }

            public CommandBuffer CommandBuffer { get; }
            public SilkVulkanBufferResource VertexBuffer { get; }
            public VkSemaphore ImageAvailableSemaphore { get; private set; }
            public VkSemaphore RenderFinishedSemaphore { get; private set; }
            public Fence InFlightFence { get; private set; }
            public ulong VertexOffset { get; set; }

            public void Dispose(Vk vk, Device device)
            {
                VertexBuffer?.Dispose();

                if (ImageAvailableSemaphore.Handle != 0)
                {
                    vk.DestroySemaphore(device, ImageAvailableSemaphore, null);
                    ImageAvailableSemaphore = default;
                }

                if (RenderFinishedSemaphore.Handle != 0)
                {
                    vk.DestroySemaphore(device, RenderFinishedSemaphore, null);
                    RenderFinishedSemaphore = default;
                }

                if (InFlightFence.Handle != 0)
                {
                    vk.DestroyFence(device, InFlightFence, null);
                    InFlightFence = default;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct DevMode
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public ushort dmSpecVersion;
            public ushort dmDriverVersion;
            public ushort dmSize;
            public ushort dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public uint dmDisplayOrientation;
            public uint dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            public ushort dmLogPixels;
            public uint dmBitsPerPel;
            public uint dmPelsWidth;
            public uint dmPelsHeight;
            public uint dmDisplayFlags;
            public uint dmDisplayFrequency;
            public uint dmICMMethod;
            public uint dmICMIntent;
            public uint dmMediaType;
            public uint dmDitherType;
            public uint dmReserved1;
            public uint dmReserved2;
            public uint dmPanningWidth;
            public uint dmPanningHeight;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PeekMsg
        {
            private readonly IntPtr hWnd;
            private readonly Message msg;
            private readonly IntPtr wParam;
            private readonly IntPtr lParam;
            private readonly uint time;
            private readonly Point p;
        }

        private const string TextureVertexShaderSource = @"
#version 450
layout(location = 0) in vec4 iPositionX;
layout(location = 1) in vec4 iPositionY;
layout(location = 2) in vec4 iTexCoordU;
layout(location = 3) in vec4 iTexCoordV;
layout(location = 4) in vec4 iColour;
layout(push_constant) uniform PushConstants
{
    vec2 uViewport;
    vec4 uTint;
    vec4 uSource;
    vec4 uOutlineColour;
    vec4 uEffect;
} pushConstants;
layout(location = 0) out vec2 vTexCoord;
layout(location = 1) out vec4 vColour;
layout(location = 2) out vec2 vScreenPos;
void main()
{
    int corner = gl_VertexIndex == 0 ? 0 :
                 gl_VertexIndex == 1 ? 1 :
                 gl_VertexIndex == 2 ? 2 :
                 gl_VertexIndex == 3 ? 0 :
                 gl_VertexIndex == 4 ? 2 : 3;
    vec2 position = vec2(iPositionX[corner], iPositionY[corner]);
    vec2 ndc = vec2((position.x / pushConstants.uViewport.x) * 2.0 - 1.0,
                    (position.y / pushConstants.uViewport.y) * 2.0 - 1.0);
    gl_Position = vec4(ndc, 0.0, 1.0);
    vTexCoord = vec2(iTexCoordU[corner], iTexCoordV[corner]);
    vColour = iColour;
    vScreenPos = position;
}";

        private const string TextureFragmentShaderSource = @"
#version 450
layout(set = 0, binding = 0) uniform sampler2D uTexture;
layout(push_constant) uniform PushConstants
{
    vec2 uViewport;
    vec4 uTint;
    vec4 uSource;
    vec4 uOutlineColour;
    vec4 uEffect;
} pushConstants;
layout(location = 0) in vec2 vTexCoord;
layout(location = 1) in vec4 vColour;
layout(location = 2) in vec2 vScreenPos;
layout(location = 0) out vec4 outColour;

bool InsideSource(vec2 uv)
{
    vec2 sourceMin = min(pushConstants.uSource.xy, pushConstants.uSource.zw);
    vec2 sourceMax = max(pushConstants.uSource.xy, pushConstants.uSource.zw);
    return uv.x >= sourceMin.x && uv.x <= sourceMax.x &&
           uv.y >= sourceMin.y && uv.y <= sourceMax.y;
}

vec4 SampleSprite(vec2 uv)
{
    if (!InsideSource(uv))
        return vec4(0.0);

    return texture(uTexture, uv);
}

void main()
{
    int effectMode = int(pushConstants.uEffect.x + 0.5);

    if (effectMode == 2)
    {
        vec4 center = SampleSprite(vTexCoord);
        if (center.a > 0.01)
            discard;

        vec2 textureSize = max(pushConstants.uEffect.zw, vec2(1.0));
        float thickness = max(pushConstants.uEffect.y, 1.0);
        vec2 texel = 1.0 / textureSize;
        int radius = int(ceil(thickness));

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                vec2 offset = vec2(float(x), float(y));
                if (dot(offset, offset) > (thickness + 0.5) * (thickness + 0.5))
                    continue;

                if (SampleSprite(vTexCoord + offset * texel).a > 0.01)
                {
                    float alpha = pushConstants.uOutlineColour.a * vColour.a;
                    outColour = vec4(pushConstants.uOutlineColour.rgb * alpha, alpha);
                    return;
                }
            }
        }

        discard;
    }

    if (effectMode == 3)
    {
        vec4 bounds = pushConstants.uSource;
        vec2 boundsMin = min(bounds.xy, bounds.zw);
        vec2 boundsMax = max(bounds.xy, bounds.zw);

        float distLeft = boundsMin.x - vScreenPos.x;
        float distTop = boundsMin.y - vScreenPos.y;
        float distRight = vScreenPos.x - boundsMax.x;
        float distBottom = vScreenPos.y - boundsMax.y;
        float shadowDistance = max(max(distLeft, distTop), max(distRight, distBottom));

        if (shadowDistance <= 0.0)
            discard;

        float shadowSize = max(pushConstants.uEffect.y, 0.0001);
        float maxAlpha = pushConstants.uEffect.z;
        float alpha = clamp(1.0 - shadowDistance / shadowSize, 0.0, 1.0) * maxAlpha * pushConstants.uOutlineColour.a * vColour.a;

        outColour = vec4(pushConstants.uOutlineColour.rgb * alpha, alpha);
        return;
    }

    vec4 texel = texture(uTexture, vTexCoord);

    if (effectMode == 1)
    {
        float gray = dot(texel.rgb, vec3(0.299, 0.587, 0.114));
        outColour = vec4(vec3(gray) * vColour.rgb * vColour.a,
                         texel.a * vColour.a);
        return;
    }

    outColour = vec4(texel.rgb * vColour.rgb * vColour.a,
                     texel.a * vColour.a);
}";

        private const string LineVertexShaderSource = @"
#version 450
layout(location = 0) in vec2 aPosition;
layout(push_constant) uniform PushConstants
{
    vec2 uViewport;
    vec4 uColour;
} pushConstants;
void main()
{
    vec2 ndc = vec2((aPosition.x / pushConstants.uViewport.x) * 2.0 - 1.0,
                    (aPosition.y / pushConstants.uViewport.y) * 2.0 - 1.0);
    gl_Position = vec4(ndc, 0.0, 1.0);
}";

        private const string LineFragmentShaderSource = @"
#version 450
layout(push_constant) uniform PushConstants
{
    vec2 uViewport;
    vec4 uColour;
} pushConstants;
layout(location = 0) out vec4 outColour;
void main()
{
    outColour = vec4(pushConstants.uColour.rgb * pushConstants.uColour.a,
                     pushConstants.uColour.a);
}";
    }
}
