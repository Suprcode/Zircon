using System;

namespace Client.Rendering
{
    public readonly struct RenderTexture
    {
        private RenderTexture(object nativeHandle)
        {
            NativeHandle = nativeHandle;
        }

        public object? NativeHandle { get; }

        public bool IsValid => NativeHandle != null;

        public static RenderTexture From(object nativeHandle)
        {
            if (nativeHandle == null)
                throw new ArgumentNullException(nameof(nativeHandle));

            return new RenderTexture(nativeHandle);
        }
    }
}
