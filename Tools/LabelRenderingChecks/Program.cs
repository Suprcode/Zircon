using Client;
using Client.Controls;
using Client.Envir;
using Client.Envir.Translations;
using Library;
using Shared.Rendering;
using System.Diagnostics;
using System.Runtime.InteropServices;
using R = Shared.Rendering.RenderingPipelineManager;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        ConfigReader.ConfigObjects[typeof(EnglishMessages)] = new EnglishMessages();
        ConfigReader.ConfigObjects[typeof(ChineseMessages)] = new ChineseMessages();
        Config.WindowScalePercent = Config.UIScalePercent = 100;
        using var form = new TargetForm { ShowInTaskbar = false, ClientSize = new Size(1024, 768) };
        CEnvir.Target = form;
        using var session = R.CreateSession(RenderingPipelineIds.SilkDXD3D11,
            new RenderingPipelineContext(form, new RenderingHostSettings
            {
                GameSize = form.ClientSize,
                GetActiveSceneSize = () => form.ClientSize,
                VSync = false,
                SaveException = ex => throw new Exception("Render failed", ex),
            }));
        using var scene = new TestScene(form.ClientSize);
        DXControl.ActiveScene = scene;
        var labels = Enumerable.Range(0, 64).Select(i => new TestLabel
        {
            Text = $"Test Sword {i}", IsVisible = true, IsControl = false,
            ForeColour = Color.White, BackColour = Color.Empty,
        }).ToArray();
        byte[] reference = null;
        foreach (bool cached in new[] { false, true, false, true })
        {
            RenderTargetResource cache = R.RentUICacheTarget(form.ClientSize);
            Rectangle bounds = new(Point.Empty, form.ClientSize);
            double cpu = 0;
            int builds = TestLabel.Builds;
            var watch = Stopwatch.StartNew();
            for (int frame = 0; frame < 220; frame++)
            {
                CEnvir.Now = DateTime.Now;
                if (frame == 20) { cpu = 0; builds = TestLabel.Builds; watch.Restart(); }
                if (!R.RenderFrame(() =>
                {
                    long started = Stopwatch.GetTimestamp();
                    R.PushUIScale(1F);

                    void DrawNames()
                    {
                        for (int i = 0; i < 2000; i++)
                        {
                            var label = labels[i % labels.Length];
                            label.Location = new Point(i % 20 * 55 - 20, i / 20 % 40 * 20 - 10);
                            R.SetUIScaleOrigin(label.Location);
                            label.Draw();
                        }
                    }
                    if (!cached) DrawNames();
                    else
                    {
                        if (frame % 16 == 0)
                            using (R.PushUICacheTarget(cache.Surface, bounds))
                            {
                                R.Clear(RenderClearFlags.Target, Color.FromArgb(0), 0, 0);
                                DrawNames();
                            }
                        R.PresentUICache(cache.Texture, bounds, bounds);
                    }
                    R.PopUIScale();
                    cpu += Stopwatch.GetElapsedTime(started).TotalMilliseconds;
                    if (frame == 0)
                    {
                        byte[] pixels = ReadBackBuffer();
                        if (reference == null) reference = pixels;
                        else if (!reference.SequenceEqual(pixels)) throw new Exception("Label presentation changed pixels.");
                    }
                })) throw new Exception("Frame failed");
            }
            Console.WriteLine($"cached={cached} CPU={cpu / 200:F4}ms wall={watch.Elapsed.TotalMilliseconds / 200:F4}ms texture-builds={TestLabel.Builds - builds}");
            R.ReturnUICacheTarget(cache);
        }
        foreach (var label in labels) label.Dispose();
        Console.WriteLine("Pixel comparison passed.");
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

    private sealed class TestScene : DXScene
    {
        public TestScene(Size size) : base(size, false) { }
    }

    private sealed class TestLabel : DXLabel
    {
        public static int Builds;
        protected override void CreateTexture() { Builds++; base.CreateTexture(); }
    }
}
