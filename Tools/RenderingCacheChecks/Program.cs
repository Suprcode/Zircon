using System.Diagnostics;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using Shared.Rendering;
using R = Shared.Rendering.RenderingPipelineManager;

internal static class Program
{
    private static RenderTexture _art, _text;
    private static readonly Rectangle Content = new(27, 31, 390, 130);
    private static int _failures;

    [STAThread]
    private static int Main(string[] args)
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        if (args.Contains("--switch")) return CheckPipelineSwitch();
        string backend = args.Contains("--vulkan") ? RenderingPipelineIds.SilkVulkan : RenderingPipelineIds.SilkDXD3D11;
        foreach (float windowScale in new[] { 1F, 1.25F, 1.5F, 1.75F, 2F })
        {
            using Form form = new() { ShowInTaskbar = false, ClientSize = new Size(1024, 768) };
            Exception renderError = null;
            var settings = new RenderingHostSettings
            {
                GameSize = new Size(1024, 768), GetActiveSceneSize = () => new Size(1024, 768),
                GetMonitorScale = _ => windowScale, VSync = false, SaveException = ex => renderError = ex
            };
            using var session = R.CreateSession(backend, new RenderingPipelineContext(form, settings));
            _art = MakeTexture(17, 19, false);
            _text = MakeTexture(93, 21, true);
            foreach (float uiScale in new[] { 1F, 1.25F, 1.5F, 1.75F, 2F })
            {
                byte[] direct = null, cached = null, lines = null;
                RenderTargetResource cache = default;
                Rectangle bounds = default;
                void Frame(Action draw)
                {
                    if (!R.RenderFrame(() =>
                    {
                        R.PushUIScale(uiScale, new PointF(13.25F, 7.5F));
                        try { draw(); }
                        finally { R.PopUIScale(); }
                    })) throw new InvalidOperationException("Render failed", renderError);
                }
                Frame(() =>
                {
                    DrawContent(false);
                    if (backend == RenderingPipelineIds.SilkDXD3D11) direct = ReadBackBuffer();
                });
                Frame(() =>
                {
                    bounds = R.GetUIPhysicalBounds(Content);
                    cache = R.RentUICacheTarget(bounds.Size);
                    object previous = R.GetCurrentSurface().NativeHandle;
                    using (R.PushUICacheTarget(cache.Surface, bounds))
                    {
                        R.Clear(RenderClearFlags.Target, Color.FromArgb(0), 0, 0);
                        // Test nested scopes and restoration through an exception.
                        var temporary = R.RentUICacheTarget(new Size(32, 32));
                        try
                        {
                            using (R.PushUICacheTarget(temporary.Surface, new Rectangle(0, 0, 32, 32)))
                                throw new ApplicationException("expected");
                        }
                        catch (ApplicationException) { }
                        if (!ReferenceEquals(R.GetCurrentSurface().NativeHandle, cache.Surface.NativeHandle))
                            throw new Exception("Nested surface was not restored");
                        R.ReturnUICacheTarget(temporary);
                        DrawContent(false);
                    }
                    if (!ReferenceEquals(R.GetCurrentSurface().NativeHandle, previous))
                        throw new Exception("Parent surface was not restored");
                    R.PresentUICache(cache.Texture, bounds, bounds);
                    if (backend == RenderingPipelineIds.SilkDXD3D11) cached = ReadBackBuffer();
                });
                Frame(() =>
                {
                    DrawContent(true);
                    if (backend == RenderingPipelineIds.SilkDXD3D11) lines = ReadBackBuffer();
                });
                if (direct != null)
                {
                    Compare(direct, cached, $"{backend} window={windowScale} ui={uiScale} cached");
                    Compare(direct, lines, $"{backend} window={windowScale} ui={uiScale} borders");
                }
                else Console.WriteLine($"{backend} window={windowScale} ui={uiScale}: draw/context smoke passed (no GPU readback)");

                if (windowScale == 1.5F && uiScale == 1.5F)
                {
                    foreach (string mode in new[] { "lines", "sprites", "cached" })
                    {
                        RenderDiagnostics.StartMeasurement();
                        Frame(() =>
                        {
                            if (mode == "cached") R.PresentUICache(cache.Texture, bounds, bounds);
                            else DrawContent(mode == "lines");
                        });
                        Console.WriteLine($"COUNTERS {backend} {mode}: submissions={RenderDiagnostics.ReadCounter(RenderDiagnostics.Counter.SpriteSubmissions)} sprites={RenderDiagnostics.ReadCounter(RenderDiagnostics.Counter.SpritesSubmitted)} lineBatches={RenderDiagnostics.ReadCounter(RenderDiagnostics.Counter.LineBatches)} lineFlushes={RenderDiagnostics.ReadCounter(RenderDiagnostics.Counter.PendingLineFlushes)} allocations={RenderDiagnostics.ReadCounter(RenderDiagnostics.Counter.CacheAllocations)}");
                        RenderDiagnostics.Enabled = false;
                    }
                    foreach (bool useCache in new[] { false, true })
                    {
                        double[] times = new double[1000];
                        for (int i = -100; i < times.Length; i++)
                        {
                            long start = Stopwatch.GetTimestamp();
                            Frame(() => { if (useCache) R.PresentUICache(cache.Texture, bounds, bounds); else DrawContent(false); });
                            if (i >= 0) times[i] = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
                        }
                        Array.Sort(times);
                        Console.WriteLine($"SYNTHETIC {backend} cached={useCache} median={times[500]:F4}ms p99={times[990]:F4}ms target={R.GetUICacheTargetSize(bounds.Size)} backbuffer={R.GetBackBufferSize()}");
                    }
                }
                R.ReturnUICacheTarget(cache);
                var reused = R.RentUICacheTarget(bounds.Size);
                if (!ReferenceEquals(reused.Surface.NativeHandle, cache.Surface.NativeHandle)) throw new Exception("Pool reuse failed");
                R.ReturnUICacheTarget(reused);
            }
            // Device/resolution generation invalidates borrowed targets and pooled resources.
            for (int i = 1; i <= 80; i++)
            {
                var pooled = R.RentUICacheTarget(new Size(i * 32, 64));
                R.ReturnUICacheTarget(pooled);
            }
            if (RenderDiagnostics.PooledBytes > 64L * 1024 * 1024)
                throw new Exception("Pool exceeded its memory cap");
            var oldKey = R.GetUICacheKey(windowScale, windowScale);
            var borrowed = R.RentUICacheTarget(new Size(96, 64));
            R.SetResolution(new Size(1000, 740));
            if (!R.RenderFrame(() => { })) throw new Exception("Resize failed", renderError);
            if (R.GetUICacheKey(windowScale, windowScale) == oldKey) throw new Exception("Resize retained old key");
            R.ReturnUICacheTarget(borrowed); // Stale return must be harmless.
            if (RenderDiagnostics.CacheBytes != 0 || RenderDiagnostics.PooledBytes != 0)
                throw new Exception("Resize retained cache targets");
            R.ReleaseTexture(_art);
            R.ReleaseTexture(_text);
            CheckSceneResolutionTransitions(settings, form, windowScale);
        }
        Console.WriteLine($"Failures={_failures}");
        return _failures == 0 ? 0 : 1;
    }

    private sealed class SwitchTestForm : Form
    {
        public void RenewRenderTarget() => RecreateHandle();
    }

    private static int CheckPipelineSwitch()
    {
        using SwitchTestForm form = new() { ShowInTaskbar = false, ClientSize = new Size(1024, 768) };
        using TextBox input = new() { Text = "Preserved native input" };
        form.Controls.Add(input);
        int renewals = 0;
        var settings = new RenderingHostSettings
        {
            RecreateRenderTarget = () =>
            {
                if (R.ActivePipelineId != null) throw new Exception("Window renewed before renderer shutdown");
                form.RenewRenderTarget();
                renewals++;
            },
            GameSize = form.ClientSize, GetActiveSceneSize = () => form.ClientSize,
            VSync = true, SaveException = ex => throw new Exception("Switch render failed", ex)
        };
        R.Initialize(RenderingPipelineIds.SilkVulkan, new RenderingPipelineContext(form, settings));
        try
        {
            R.RunMessageLoop(form, () =>
            {
                foreach (string backend in new[] { RenderingPipelineIds.SilkDXD3D11, RenderingPipelineIds.SilkVulkan, RenderingPipelineIds.SilkDXD3D11 })
                {
                    RenderTexture texture = MakeTexture(93, 21, true);
                    RenderTargetResource cache = R.RentUICacheTarget(new Size(128, 128));
                    for (int frame = 0; frame < 3; frame++)
                        if (!R.RenderFrame(() =>
                        {
                            using (R.PushUICacheTarget(cache.Surface, new Rectangle(0, 0, 128, 128)))
                            {
                                R.Clear(RenderClearFlags.Target, Color.Transparent, 0, 0);
                                R.DrawTexture(texture, new Rectangle(0, 0, 93, 21), new RectangleF(10, 10, 93, 21), Color.White);
                            }
                            R.PresentUICache(cache.Texture, new Rectangle(0, 0, 128, 128), new Rectangle(0, 0, 128, 128));
                        }))
                            throw new Exception("Frame failed before switch");
                    R.ReleaseTexture(texture);
                    R.ReturnUICacheTarget(cache);
                    Console.WriteLine($"Switching {R.ActivePipelineId} to {backend}");
                    int previousRenewals = renewals;
                    R.RequestSwitchPipeline(backend);
                    if (!R.ApplyPendingPipelineSwitch() || R.ActivePipelineId != backend)
                        throw new Exception("Requested backend was not activated");
                    if (renewals != previousRenewals + 1 || input.Parent != form || input.Text != "Preserved native input" || !input.IsHandleCreated)
                        throw new Exception("Window handover did not preserve the native input control");
                    if (!R.RenderFrame(() => R.FillRectangle(new Rectangle(10, 10, 100, 100), Color.White)))
                        throw new Exception("Frame failed after switch");
                    Console.WriteLine($"Switched to {backend} and rendered");
                }
                form.Close();
                Application.ExitThread();
            });
        }
        finally { R.Shutdown(); }
        return 0;
    }

    private static void CheckSceneResolutionTransitions(RenderingHostSettings settings, Form form, float windowScale)
    {
        Size fixedSceneSize = settings.ActiveSceneSize;
        foreach (Size gameSize in R.GetSupportedResolutions())
        foreach (bool extended in new[] { false, true })
        {
            Size introSize = extended ? gameSize : fixedSceneSize;
            settings.GetActiveSceneSize = () => gameSize;
            R.SetResolution(gameSize);
            if (!R.RenderFrame(() => { })) throw new Exception("Game resolution failed");

            // DXScene preserves the player's game resolution after requesting the intro size.
            R.SetResolution(introSize);
            settings.GameSize = gameSize;
            settings.GetActiveSceneSize = () => introSize;
            Size expected = form.ClientSize;
            for (int frame = 0; frame < 3; frame++)
            {
                if (!R.RenderFrame(() => { })) throw new Exception("Intro resolution failed");
                if (form.ClientSize != expected || R.GetBackBufferSize() != expected)
                    throw new Exception($"Scene {introSize} after {gameSize}, extended={extended}, scale={windowScale}: expected {expected}, window {form.ClientSize}, buffer {R.GetBackBufferSize()}");
                if (settings.GameSize != gameSize) throw new Exception("Intro overwrote saved game resolution");
            }
        }
        Console.WriteLine($"Fixed and extended scene resolution transitions passed at window scale {windowScale}");
    }

    private static void DrawContent(bool lines)
    {
        for (int i = 0; i < 24; i++)
        {
            Rectangle cell = new(32 + i % 12 * 31, 36 + i / 12 * 55, 29, 49);
            R.FillRectangle(cell, Color.FromArgb(210, 32, 25, 20), true);
            R.DrawTexture(_art, new Rectangle(0, 0, 17, 19), new RectangleF(cell.X + 2, cell.Y + 2, 25, 25), Color.White);
            R.DrawDpiText(_text, new Rectangle(i % 3, 0, 30, 21), new RectangleF(cell.X, cell.Y + 28, 23, 17),
                new PointF(32, 36), (i & 1) != 0, Color.White);
            RectangleF border = R.GetPixelAlignedBorderBounds(cell);
            PointF[] points = { new(border.Left, border.Top), new(border.Right, border.Top), new(border.Right, border.Bottom), new(border.Left, border.Bottom) };
            for (int side = 0; side < 4; side++)
            {
                PointF a = points[side], b = points[(side + 1) % 4];
                // DXControl emits the bottom and left in the same direction as top and right.
                if (side >= 2) (a, b) = (b, a);
                if (lines) R.DrawLine(new[] { new LinePoint(a.X, a.Y), new LinePoint(b.X, b.Y) }, Color.Gold);
                else R.DrawBorderStroke(a, b, 1F, Color.Gold);
            }
        }
        R.DrawTexture(_art, new Rectangle(0, 0, 17, 19), Matrix3x2.CreateScale(.8F), Vector3.Zero, new Vector3(300, 145, 0), Color.White);
    }

    private static RenderTexture MakeTexture(int width, int height, bool text)
    {
        var texture = R.CreateTexture(new Size(width, height), RenderTextureFormat.A8R8G8B8, RenderTextureUsage.None, RenderTexturePool.Managed);
        using Bitmap bitmap = new(width, height, PixelFormat.Format32bppArgb);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.Transparent);
            if (text)
            {
                using Font font = new("Segoe UI", 14, FontStyle.Bold, GraphicsUnit.Pixel);
                graphics.DrawString("Abcd 0123", font, Brushes.White, PointF.Empty);
            }
            else
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                        bitmap.SetPixel(x, y, Color.FromArgb((x + y) % 3 == 0 ? 128 : 255, x * 13, y * 11, 180));
        }
        BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        try
        {
            using TextureLock target = R.LockTexture(texture, TextureLockMode.Discard);
            byte[] row = new byte[width * 4];
            for (int y = 0; y < height; y++)
            {
                Marshal.Copy(data.Scan0 + y * data.Stride, row, 0, row.Length);
                Marshal.Copy(row, 0, target.DataPointer + y * target.Pitch, row.Length);
            }
        }
        finally { bitmap.UnlockBits(data); }
        return texture;
    }

    private static byte[] ReadBackBuffer()
    {
        R.FlushLines();
        R.FlushSprite();
        object surface = R.GetCurrentSurface().NativeHandle;
        object native = surface.GetType().GetProperty("Texture").GetValue(surface);
        Size size = R.GetBackBufferSize();
        using TextureLock data = R.LockTexture(RenderTexture.From(native), TextureLockMode.ReadOnly);
        byte[] result = new byte[size.Width * size.Height * 4];
        for (int y = 0; y < size.Height; y++)
            Marshal.Copy(data.DataPointer + y * data.Pitch, result, y * size.Width * 4, size.Width * 4);
        return result;
    }

    private static void Compare(byte[] expected, byte[] actual, string label)
    {
        if (!expected.Where((_, i) => i % 4 != 3).Any(value => value > 0))
            throw new Exception("Readback contained no coloured pixels");
        int changed = 0, max = 0;
        for (int i = 0; i < expected.Length; i++)
        {
            int delta = Math.Abs(expected[i] - actual[i]);
            if (delta > 1) changed++;
            max = Math.Max(max, delta);
        }
        Console.WriteLine($"{label}: channels differing >1={changed}, max={max}");
        if (changed > 0) _failures++;
    }
}
