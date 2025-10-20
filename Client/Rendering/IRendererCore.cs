using System;
using System.Collections.Generic;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;

namespace Client.Rendering
{
    /// <summary>
    /// Describes the rendering device backend used by the client.
    /// Implementations are responsible for managing the device lifecycle,
    /// frame flow and resource creation.
    /// </summary>
    public interface IRendererCore : IDisposable
    {
        Device Device { get; }

        void InitializeDevice(PresentParameters parameters, IntPtr windowHandle);

        void ResetDevice(PresentParameters parameters);

        Result TestCooperativeLevel();

        void BeginFrame();

        void EndFrame();

        void Present();

        Sprite CreateSprite();

        Line CreateLine();

        Surface GetBackBuffer(int swapChain, int backBuffer);

        Texture CreateTexture(int width, int height, int levels, Usage usage, Format format, Pool pool);

        Texture CreateRenderTarget(int width, int height, Format format);

        IEnumerable<DisplayMode> GetDisplayModes(Format format);

        void Clear(ClearFlags flags, Color colour, float z, int stencil);
    }
}
