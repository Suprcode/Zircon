using System;

namespace Client.Rendering
{
    [Flags]
    public enum RenderClearFlags
    {
        Target = 1 << 0,
        ZBuffer = 1 << 1,
        Stencil = 1 << 2
    }
}
