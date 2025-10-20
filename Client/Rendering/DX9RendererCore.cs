using System;
using System.Collections.Generic;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;

namespace Client.Rendering
{
    /// <summary>
    /// SlimDX Direct3D9 implementation of <see cref="IRendererCore"/>.
    /// </summary>
    public sealed class DX9RendererCore : IRendererCore
    {
        private Direct3D _direct3D;

        public Device Device { get; private set; }

        public void InitializeDevice(PresentParameters parameters, IntPtr windowHandle)
        {
            DisposeDevice();

            _direct3D = new Direct3D();
            Device = new Device(
                _direct3D,
                _direct3D.Adapters.DefaultAdapter.Adapter,
                DeviceType.Hardware,
                windowHandle,
                CreateFlags.HardwareVertexProcessing,
                parameters);
        }

        public void ResetDevice(PresentParameters parameters)
        {
            Device?.Reset(parameters);
        }

        public Result TestCooperativeLevel()
        {
            return Device?.TestCooperativeLevel() ?? Result.Success;
        }

        public void BeginFrame()
        {
            Device?.BeginScene();
        }

        public void EndFrame()
        {
            Device?.EndScene();
        }

        public void Present()
        {
            Device?.Present();
        }

        public Sprite CreateSprite()
        {
            return Device == null ? null : new Sprite(Device);
        }

        public Line CreateLine()
        {
            return Device == null ? null : new Line(Device);
        }

        public Surface GetBackBuffer(int swapChain, int backBuffer)
        {
            return Device?.GetBackBuffer(swapChain, backBuffer);
        }

        public Texture CreateTexture(int width, int height, int levels, Usage usage, Format format, Pool pool)
        {
            return Device == null ? null : new Texture(Device, width, height, levels, usage, format, pool);
        }

        public Texture CreateRenderTarget(int width, int height, Format format)
        {
            return Device == null ? null : new Texture(Device, width, height, 1, Usage.RenderTarget, format, Pool.Default);
        }

        public IEnumerable<DisplayMode> GetDisplayModes(Format format)
        {
            if (_direct3D == null)
                yield break;

            AdapterInformation adapterInfo = _direct3D.Adapters.DefaultAdapter;

            foreach (DisplayMode mode in adapterInfo.GetDisplayModes(format))
                yield return mode;
        }

        public void Clear(ClearFlags flags, Color colour, float z, int stencil)
        {
            Device?.Clear(flags, colour, z, stencil);
        }

        public void Dispose()
        {
            DisposeDevice();
        }

        private void DisposeDevice()
        {
            if (Device != null)
            {
                if (!Device.Disposed)
                    Device.Dispose();

                Device = null;
            }

            if (_direct3D != null)
            {
                if (!_direct3D.Disposed)
                    _direct3D.Dispose();

                _direct3D = null;
            }
        }
    }
}
