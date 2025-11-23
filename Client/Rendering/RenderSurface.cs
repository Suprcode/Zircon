using System;

namespace Client.Rendering
{
    public readonly struct RenderSurface
    {
        private RenderSurface(object nativeHandle)
        {
            NativeHandle = nativeHandle;
        }

        public object? NativeHandle { get; }

        public bool IsValid => NativeHandle != null;

        public static RenderSurface From(object nativeHandle)
        {
            if (nativeHandle == null)
                throw new ArgumentNullException(nameof(nativeHandle));

            return new RenderSurface(nativeHandle);
        }
    }
}
