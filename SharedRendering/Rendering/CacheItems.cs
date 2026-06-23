using System;

namespace Shared.Rendering
{
    public interface ITextureCacheItem
    {
        DateTime ExpireTime { get; }

        void DisposeTexture();
    }

    public interface ISoundCacheItem
    {
        DateTime ExpireTime { get; }

        void DisposeSoundBuffer();

        void Stop();

        void UpdateFlags();
    }
}
