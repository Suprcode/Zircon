using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Client.Rendering
{
    public static class LightGenerator
    {
        public static byte[] CreateLightData(int width, int height)
        {
            using (Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb))
            {
                using (Graphics graphics = Graphics.FromImage(image))
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(new Rectangle(0, 0, width, height));
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        graphics.Clear(Color.FromArgb(0, 0, 0, 0));
                        brush.SurroundColors = new[] { Color.FromArgb(0, 0, 0, 0) };
                        brush.CenterColor = Color.FromArgb(255, 200, 200, 200);
                        graphics.FillPath(brush, path);
                        graphics.Save();
                    }
                }

                BitmapData data = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                byte[] result = new byte[width * height * 4];
                Marshal.Copy(data.Scan0, result, 0, result.Length);
                image.UnlockBits(data);

                return result;
            }
        }
    }
}
