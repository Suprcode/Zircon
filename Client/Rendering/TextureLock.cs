using System;

namespace Client.Rendering
{
    public sealed class TextureLock : IDisposable
    {
        private readonly Action _dispose;
        private bool _disposed;

        private TextureLock(IntPtr dataPointer, int pitch, Action dispose)
        {
            DataPointer = dataPointer;
            Pitch = pitch;
            _dispose = dispose;
        }

        public IntPtr DataPointer { get; }

        public int Pitch { get; }

        public static TextureLock From(IntPtr dataPointer, int pitch, Action dispose)
        {
            if (dataPointer == IntPtr.Zero)
                throw new ArgumentException("A valid data pointer is required.", nameof(dataPointer));

            if (dispose == null)
                throw new ArgumentNullException(nameof(dispose));

            return new TextureLock(dataPointer, pitch, dispose);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _dispose();
        }

    }
}
