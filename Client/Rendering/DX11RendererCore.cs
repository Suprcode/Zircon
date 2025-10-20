using System;
using System.Collections.Generic;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D9.Device;
using Device11 = SlimDX.Direct3D11.Device;
using Resource11 = SlimDX.Direct3D11.Resource;

namespace Client.Rendering
{
    /// <summary>
    /// SlimDX Direct3D11 implementation of <see cref="IRendererCore"/>.
    /// Provides a very small subset of functionality that allows the client
    /// to render a blank frame using Direct3D11 while the higher level systems
    /// are still based on Direct3D9 constructs.
    /// </summary>
    public sealed class DX11RendererCore : IRendererCore
    {
        private Device11 _device;
        private SwapChain _swapChain;
        private DeviceContext _context;
        private RenderTargetView _renderTargetView;
        private Texture2D _depthStencil;
        private DepthStencilView _depthStencilView;
        private bool _useVSync;

        public Device Device => null;

        public void InitializeDevice(PresentParameters parameters, IntPtr windowHandle)
        {
            DisposeDevice();

            var swapChainDescription = new SwapChainDescription
            {
                BufferCount = 1,
                Usage = Usage.RenderTargetOutput,
                OutputHandle = windowHandle,
                IsWindowed = parameters.Windowed,
                ModeDescription = new ModeDescription(
                    Math.Max(1, parameters.BackBufferWidth),
                    Math.Max(1, parameters.BackBufferHeight),
                    new Rational(60, 1),
                    Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Flags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard
            };

            Device11.CreateWithSwapChain(
                DriverType.Hardware,
                DeviceCreationFlags.BgraSupport,
                swapChainDescription,
                out _device,
                out _swapChain);

            _context = _device.ImmediateContext;
            _useVSync = parameters.PresentationInterval != PresentInterval.Immediate;

            CreateTargets(parameters.BackBufferWidth, parameters.BackBufferHeight);
        }

        public void ResetDevice(PresentParameters parameters)
        {
            if (_swapChain == null)
                return;

            DisposeTargets();

            _swapChain.ResizeBuffers(
                1,
                Math.Max(1, parameters.BackBufferWidth),
                Math.Max(1, parameters.BackBufferHeight),
                Format.R8G8B8A8_UNorm,
                SwapChainFlags.AllowModeSwitch);

            _swapChain.IsWindowed = parameters.Windowed;

            _useVSync = parameters.PresentationInterval != PresentInterval.Immediate;

            CreateTargets(parameters.BackBufferWidth, parameters.BackBufferHeight);
        }

        public Result TestCooperativeLevel()
        {
            return Result.Success;
        }

        public void BeginFrame()
        {
            if (_context == null)
                return;

            _context.OutputMerger.SetTargets(_depthStencilView, _renderTargetView);
        }

        public void EndFrame()
        {
            // No explicit EndScene equivalent in Direct3D11.
        }

        public void Present()
        {
            _swapChain?.Present(_useVSync ? 1 : 0, PresentFlags.None);
        }

        public Sprite CreateSprite()
        {
            return null;
        }

        public Line CreateLine()
        {
            return null;
        }

        public Surface GetBackBuffer(int swapChain, int backBuffer)
        {
            return null;
        }

        public Texture CreateTexture(int width, int height, int levels, Usage usage, Format format, Pool pool)
        {
            return null;
        }

        public Texture CreateRenderTarget(int width, int height, Format format)
        {
            return null;
        }

        public IEnumerable<DisplayMode> GetDisplayModes(Format format)
        {
            yield break;
        }

        public void Clear(ClearFlags flags, Color colour, float z, int stencil)
        {
            if (_context == null)
                return;

            if (_renderTargetView != null && flags.HasFlag(ClearFlags.Target))
            {
                var color4 = new Color4(colour);
                _context.ClearRenderTargetView(_renderTargetView, color4);
            }

            if (_depthStencilView != null && (flags.HasFlag(ClearFlags.ZBuffer) || flags.HasFlag(ClearFlags.Stencil)))
            {
                DepthStencilClearFlags clearFlags = 0;

                if (flags.HasFlag(ClearFlags.ZBuffer))
                    clearFlags |= DepthStencilClearFlags.Depth;

                if (flags.HasFlag(ClearFlags.Stencil))
                    clearFlags |= DepthStencilClearFlags.Stencil;

                _context.ClearDepthStencilView(_depthStencilView, clearFlags, z, (byte)stencil);
            }
        }

        public void Dispose()
        {
            DisposeDevice();
        }

        private void CreateTargets(int width, int height)
        {
            if (_device == null || _context == null || _swapChain == null)
                return;

            using (var backBuffer = Resource11.FromSwapChain<Texture2D>(_swapChain, 0))
            {
                _renderTargetView = new RenderTargetView(_device, backBuffer);
            }

            var depthDescription = new Texture2DDescription
            {
                Width = Math.Max(1, width),
                Height = Math.Max(1, height),
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.D24_UNorm_S8_UInt,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };

            _depthStencil = new Texture2D(_device, depthDescription);
            _depthStencilView = new DepthStencilView(_device, _depthStencil);

            _context.Rasterizer.SetViewports(new Viewport(0, 0, Math.Max(1, width), Math.Max(1, height), 0.0f, 1.0f));
            _context.OutputMerger.SetTargets(_depthStencilView, _renderTargetView);
        }

        private void DisposeTargets()
        {
            _context?.OutputMerger.SetTargets(null as DepthStencilView, null as RenderTargetView);

            _depthStencilView?.Dispose();
            _depthStencilView = null;

            _depthStencil?.Dispose();
            _depthStencil = null;

            _renderTargetView?.Dispose();
            _renderTargetView = null;
        }

        private void DisposeDevice()
        {
            DisposeTargets();

            if (_context != null)
            {
                _context.ClearState();
                _context.Flush();
                _context.Dispose();
                _context = null;
            }

            _swapChain?.Dispose();
            _swapChain = null;

            _device?.Dispose();
            _device = null;
        }
    }
}
