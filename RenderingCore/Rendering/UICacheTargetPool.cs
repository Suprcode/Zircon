using System.Collections.Generic;
using System.Drawing;

namespace Shared.Rendering
{
    public static partial class RenderingPipelineManager
    {
        private sealed class CacheTarget
        {
            public RenderTargetResource Resource;
            public PipelineSession Owner;
            public Size Size;
            public bool Pooled;
            public long Bytes => (long)Size.Width * Size.Height * 4;
        }

        private static readonly Dictionary<object, CacheTarget> UICacheTargets = new();
        private static readonly List<CacheTarget> UICachePool = new();
        private const long MaxPooledBytes = 64L * 1024 * 1024;
        private const int MaxPooledTargets = 32;
        public static Size GetUICacheTargetSize(Size size) =>
            new(((size.Width + 31) / 32) * 32, ((size.Height + 31) / 32) * 32);

        public static RenderTargetResource RentUICacheTarget(Size size)
        {
            size = GetUICacheTargetSize(size);
            for (int i = 0; i < UICachePool.Count; i++)
            {
                CacheTarget item = UICachePool[i];
                if (item.Owner != _activeSession || item.Size != size) continue;
                UICachePool.RemoveAt(i);
                item.Pooled = false;
                RenderDiagnostics.PooledBytes -= item.Bytes;
                return item.Resource; // Cleared by the cache build before any drawing.
            }
            var target = new CacheTarget { Resource = CreateRenderTarget(size), Owner = _activeSession, Size = size };
            UICacheTargets.Add(target.Resource.Surface.NativeHandle, target);
            RenderDiagnostics.CacheBytes += target.Bytes;
            RenderDiagnostics.Count(RenderDiagnostics.Counter.CacheAllocations);
            return target.Resource;
        }

        public static void ReturnUICacheTarget(RenderTargetResource resource)
        {
            if (!resource.IsValid || !UICacheTargets.TryGetValue(resource.Surface.NativeHandle, out CacheTarget target) || target.Pooled)
                return;
            if (target.Owner == _activeSession && !target.Owner.IsDisposed &&
                UICachePool.Count < MaxPooledTargets && RenderDiagnostics.PooledBytes + target.Bytes <= MaxPooledBytes)
            {
                target.Pooled = true;
                UICachePool.Add(target);
                RenderDiagnostics.PooledBytes += target.Bytes;
                return;
            }
            ReleaseUICacheTarget(target);
        }

        private static void ReleaseUICacheTarget(CacheTarget target)
        {
            target.Owner.Pipeline.ReleaseRenderTarget(target.Resource);
            UICacheTargets.Remove(target.Resource.Surface.NativeHandle);
            if (target.Pooled)
            {
                UICachePool.Remove(target);
                RenderDiagnostics.PooledBytes -= target.Bytes;
            }
            RenderDiagnostics.CacheBytes -= target.Bytes;
            RenderDiagnostics.Count(RenderDiagnostics.Counter.CacheReleases);
        }

        private static void ReleaseUICacheTargets(PipelineSession owner)
        {
            foreach (CacheTarget target in new List<CacheTarget>(UICacheTargets.Values))
                if (target.Owner == owner) ReleaseUICacheTarget(target);
        }
    }
}
