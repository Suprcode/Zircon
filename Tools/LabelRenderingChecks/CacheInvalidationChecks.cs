using Client;
using Client.Controls;
using System.Reflection;

internal static class CacheInvalidationChecks
{
    internal static void Run()
    {
        // Exercise the real client traversal without requiring a graphics device.
        MethodInfo invalidate = typeof(TargetForm).Assembly.GetType("Client.Program", true)
            .GetMethod("InvalidateControlTree", BindingFlags.Static | BindingFlags.NonPublic)!;
        using var root = new CountingControl();
        using var live = new CountingControl { Parent = root };
        var disposed = new CountingControl();
        disposed.Dispose();
        int disposedCalls = disposed.Invalidations;

        // A retained snapshot or overlay reference can outlive the control's disposal.
        root.AddStaleChild(disposed);
        try
        {
            foreach (bool skipWorld in new[] { false, true })
            {
                var visited = new HashSet<DXControl>();
                int previousCalls = live.Invalidations;
                invalidate.Invoke(null, new object[] { root, visited, skipWorld });
                invalidate.Invoke(null, new object[] { disposed, visited, skipWorld });
                invalidate.Invoke(null, new object[] { live, visited, skipWorld });
                if (live.Invalidations != previousCalls + 1 || disposed.Invalidations != disposedCalls)
                    throw new Exception("Cache invalidation skipped a live control or revisited a disposed control.");
            }
        }
        finally
        {
            // Remove the synthetic stale entry before normal parent disposal.
            root.RemoveStaleChild(disposed);
        }
        Console.WriteLine("Full/UI cache invalidation passed with retained disposed controls and duplicate roots.");
    }

    private sealed class CountingControl : DXControl
    {
        internal int Invalidations;

        // Expose only the synthetic stale-entry setup needed by this regression check.
        internal void AddStaleChild(DXControl child) => Controls.Insert(0, child);
        internal void RemoveStaleChild(DXControl child) => Controls.Remove(child);

        public override void DisposeTexture()
        {
            // Count calls to verify that later live controls are still reached exactly once.
            Invalidations++;
            base.DisposeTexture();
        }
    }
}
