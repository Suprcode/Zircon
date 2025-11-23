using Client.Envir;
using System;

namespace Client.Rendering
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

        SoundType SoundType { get; }

        void Stop();

        void UpdateFlags();
    }
}
