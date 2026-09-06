using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpDX.Mathematics.Interop;
using D2D = SharpDX.Direct2D1;
using DW = SharpDX.DirectWrite;

namespace Client.Controls
{
    // Opaque Windows-style hint labels can retain ClearType colour coverage.
    internal static class DirectWriteHintRenderer
    {
        private static readonly object RenderLock = new object();
        private static DW.Factory writeFactory;
        private static D2D.Factory drawingFactory;
        private static D2D.DeviceContextRenderTarget renderTarget;

        static DirectWriteHintRenderer()
        {
            AppDomain.CurrentDomain.ProcessExit += (_, _) => ReleaseResources();
            Application.ApplicationExit += (_, _) => ReleaseResources();
        }

        private static DW.Factory WriteFactory => writeFactory ??= new DW.Factory();

        private static D2D.DeviceContextRenderTarget RenderTarget
        {
            get
            {
                if (renderTarget != null) return renderTarget;

                drawingFactory ??= new D2D.Factory(D2D.FactoryType.MultiThreaded);
                renderTarget = new D2D.DeviceContextRenderTarget(drawingFactory,
                    new D2D.RenderTargetProperties(D2D.RenderTargetType.Software,
                        new D2D.PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, D2D.AlphaMode.Ignore),
                        96F, 96F, D2D.RenderTargetUsage.None, D2D.FeatureLevel.Level_DEFAULT))
                {
                    TextAntialiasMode = D2D.TextAntialiasMode.Cleartype
                };

                return renderTarget;
            }
        }

        public static Size Measure(string text, Font font, float scale, int paddingBottom)
        {
            if (string.IsNullOrEmpty(text)) return Size.Empty;

            lock (RenderLock)
            {
                using var format = CreateFormat(font, scale);
                using var layout = new DW.TextLayout(WriteFactory, text, format, 16384F, 16384F);
                DW.TextMetrics metrics = layout.Metrics;
                return new Size((int)Math.Ceiling(metrics.WidthIncludingTrailingWhitespace / scale) + 6,
                    (int)Math.Ceiling(metrics.Height / scale) + paddingBottom + 2);
            }
        }

        public static Bitmap Render(string text, Font font, float scale, Size size, Color foreground, Color background)
        {
            lock (RenderLock)
            {
                Bitmap bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
                try
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        using Graphics emptyGraphics = Graphics.FromImage(bitmap);
                        emptyGraphics.Clear(background);
                        return bitmap;
                    }

                    using var format = CreateFormat(font, scale);
                    using var layout = new DW.TextLayout(WriteFactory, text, format,
                        Math.Max(1F, size.Width - 6F * scale), size.Height);
                    using Graphics graphics = Graphics.FromImage(bitmap);
                    IntPtr dc = graphics.GetHdc();
                    try
                    {
                        D2D.DeviceContextRenderTarget target = RenderTarget;
                        target.BindDeviceContext(dc, new RawRectangle(0, 0, size.Width, size.Height));
                        using var brush = new D2D.SolidColorBrush(target, ToColour(foreground));
                        target.BeginDraw();
                        target.Clear(ToColour(background));
                        target.DrawTextLayout(new RawVector2((float)Math.Round(3F * scale), (float)Math.Round(scale)),
                            layout, brush, D2D.DrawTextOptions.Clip);
                        target.EndDraw();
                    }
                    finally
                    {
                        graphics.ReleaseHdc(dc);
                    }

                    MakeOpaque(bitmap);
                    return bitmap;
                }
                catch
                {
                    bitmap.Dispose();
                    renderTarget?.Dispose();
                    renderTarget = null;
                    throw;
                }
            }
        }

        private static DW.TextFormat CreateFormat(Font font, float scale) =>
            new DW.TextFormat(WriteFactory, font.FontFamily.Name,
                font.Bold ? DW.FontWeight.Bold : DW.FontWeight.Normal,
                font.Italic ? DW.FontStyle.Italic : DW.FontStyle.Normal,
                DW.FontStretch.Normal,
                (font.Unit == GraphicsUnit.Pixel ? font.Size : font.SizeInPoints * 96F / 72F) * scale)
            {
                WordWrapping = DW.WordWrapping.NoWrap
            };

        private static void MakeOpaque(Bitmap bitmap)
        {
            BitmapData data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            try
            {
                int byteCount = Math.Abs(data.Stride) * data.Height;
                byte[] pixels = new byte[byteCount];
                Marshal.Copy(data.Scan0, pixels, 0, byteCount);
                for (int index = 3; index < pixels.Length; index += 4)
                    pixels[index] = 255;
                Marshal.Copy(pixels, 0, data.Scan0, byteCount);
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
        }

        private static RawColor4 ToColour(Color colour) =>
            new RawColor4(colour.R / 255F, colour.G / 255F, colour.B / 255F, colour.A / 255F);

        private static void ReleaseResources()
        {
            lock (RenderLock)
            {
                renderTarget?.Dispose();
                renderTarget = null;
                drawingFactory?.Dispose();
                drawingFactory = null;
                writeFactory?.Dispose();
                writeFactory = null;
            }
        }
    }
}
