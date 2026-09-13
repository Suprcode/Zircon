using System;
using System.Diagnostics;

namespace Shared.Rendering
{
    /// <summary>Render-thread counters. Calls compile away in release builds.</summary>
    public static class RenderDiagnostics
    {
        public enum Counter
        {
            ControlsVisited, ControlsUncached, CachePresented, CacheRebuilt, CacheInvalidations,
            SpritesQueued, SpriteSubmissions, SpritesSubmitted, LineBatches, LineVertices,
            PendingLineFlushes, TargetSwitches, TargetClears, CacheAllocations, CacheReleases, Count
        }

        public static bool Enabled { get; set; } =
            Environment.GetEnvironmentVariable("ZIRCON_RENDER_METRICS") == "1";
        public static bool ForceUncached { get; set; } =
            Environment.GetEnvironmentVariable("ZIRCON_FORCE_UNCACHED") == "1";
        public static string LatestSnapshot { get; private set; } = string.Empty;
        private static readonly long[] Counts = new long[(int)Counter.Count];
        private static readonly double[] FrameTimes = new double[131072];
        private static readonly double[] SceneTimes = new double[131072];
        private static int _frames;
        private static long _started, _interval;
        private static long _sceneStarted;
        private static double _sceneMilliseconds;
        public static long CacheBytes { get; internal set; }
        public static long PooledBytes { get; internal set; }

        public static long ReadCounter(Counter counter) => Counts[(int)counter];

        public static void StartMeasurement()
        {
            Array.Clear(Counts);
            _frames = 0;
            _interval = 0;
            Enabled = true;
        }

        [Conditional("DEBUG")]
        internal static void BeginScene()
        {
            if (Enabled) _sceneStarted = Stopwatch.GetTimestamp();
        }

        [Conditional("DEBUG")]
        internal static void EndScene()
        {
            if (Enabled) _sceneMilliseconds = Stopwatch.GetElapsedTime(_sceneStarted).TotalMilliseconds;
        }

        [Conditional("DEBUG")]
        public static void Count(Counter counter, long amount = 1)
        {
            if (Enabled) Counts[(int)counter] += amount;
        }

        [Conditional("DEBUG")]
        internal static void BeginFrame()
        {
            if (!Enabled) return;
            _started = Stopwatch.GetTimestamp();
            if (_interval == 0) _interval = _started;
        }

        [Conditional("DEBUG")]
        internal static void EndFrame()
        {
            if (!Enabled) return;
            long now = Stopwatch.GetTimestamp();
            if (_frames < FrameTimes.Length)
            {
                FrameTimes[_frames] = (now - _started) * 1000.0 / Stopwatch.Frequency;
                SceneTimes[_frames++] = _sceneMilliseconds;
            }
            if (now - _interval < Stopwatch.Frequency || _frames == 0) return;
            Array.Sort(FrameTimes, 0, _frames);
            Array.Sort(SceneTimes, 0, _frames);
            var output = new System.Text.StringBuilder(512);
            output.Append($"Render wall ms median={FrameTimes[_frames / 2]:F4} p99={FrameTimes[Math.Min(_frames - 1, (int)(_frames * .99))]:F4} scene CPU median={SceneTimes[_frames / 2]:F4} frames={_frames}");
            for (int i = 0; i < Counts.Length; i++)
                output.Append($" {(Counter)i}/frame={Counts[i] / (double)_frames:F2}");
            output.Append($" sprites/submission={Counts[(int)Counter.SpritesSubmitted] / (double)Math.Max(1, Counts[(int)Counter.SpriteSubmissions]):F2} cacheBytes={CacheBytes} pooledBytes={PooledBytes} {RenderingPipelineManager.DiagnosticPresentation}");
            LatestSnapshot = output.ToString();
            Debug.WriteLine(LatestSnapshot);
            Array.Clear(Counts);
            _frames = 0;
            _interval = now;
        }
    }
}
