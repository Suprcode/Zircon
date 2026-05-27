using System;
using System.Runtime.InteropServices;

namespace Client.Helpers
{
    public static class ColourPaletteHelper
    {
        public const int PaletteWidth = 200;
        public const int PaletteHeight = 149;

        public static byte[] CreatePaletteData(int width = PaletteWidth, int height = PaletteHeight)
        {
            byte[] data = new byte[width * height * 4];

            for (int y = 0; y < height; y++)
            {
                float hue = height <= 1 ? 0F : y / (float)(height - 1);

                for (int x = 0; x < width; x++)
                {
                    float saturation = width <= 1 ? 0F : x / (float)(width - 1);
                    HsvToRgb(hue, saturation, 1F, out byte red, out byte green, out byte blue);

                    int index = (y * width + x) * 4;
                    data[index] = blue;
                    data[index + 1] = green;
                    data[index + 2] = red;
                    data[index + 3] = byte.MaxValue;
                }
            }

            return data;
        }

        public static void CopyPaletteData(byte[] data, IntPtr destination, int destinationPitch, int width = PaletteWidth, int height = PaletteHeight)
        {
            int rowLength = width * 4;

            for (int y = 0; y < height; y++)
            {
                int sourceIndex = y * rowLength;
                IntPtr target = IntPtr.Add(destination, y * destinationPitch);
                Marshal.Copy(data, sourceIndex, target, rowLength);
            }
        }

        private static void HsvToRgb(float hue, float saturation, float value, out byte red, out byte green, out byte blue)
        {
            if (saturation <= 0)
            {
                red = green = blue = (byte)(value * 255F);
                return;
            }

            float scaledHue = (hue * 6F) % 6F;
            int sector = (int)scaledHue;
            float fraction = scaledHue - sector;

            float p = value * (1F - saturation);
            float q = value * (1F - saturation * fraction);
            float t = value * (1F - saturation * (1F - fraction));

            float r = value, g = value, b = value;

            switch (sector)
            {
                case 0:
                    r = value;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = value;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = value;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = value;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = value;
                    break;
                default:
                    r = value;
                    g = p;
                    b = q;
                    break;
            }

            red = (byte)(r * 255F);
            green = (byte)(g * 255F);
            blue = (byte)(b * 255F);
        }
    }
}
