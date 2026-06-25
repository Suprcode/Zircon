using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Shared.Rendering
{
    internal sealed class DisplayModeManager
    {
        private const int EnumCurrentSettings = -1;
        private const int CdsFullscreen = 0x00000004;
        private const int DispChangeSuccessful = 0;
        private const int DmPelsWidth = 0x00080000;
        private const int DmPelsHeight = 0x00100000;

        private bool _changed;
        private string _deviceName;
        private Size _size;

        public bool IsActive(Screen screen, Size size)
        {
            return screen != null &&
                   _changed &&
                   string.Equals(_deviceName, screen.DeviceName, StringComparison.OrdinalIgnoreCase) &&
                   _size == size;
        }

        public bool Apply(Screen screen, Size size)
        {
            if (screen == null)
                return false;

            string deviceName = screen.DeviceName;

            if (IsActive(screen, size))
                return true;

            Restore();

            if (!TryGetMode(deviceName, size, out DevMode mode))
                return false;

            mode.dmFields = DmPelsWidth | DmPelsHeight;

            int result = ChangeDisplaySettingsEx(deviceName, ref mode, IntPtr.Zero, CdsFullscreen, IntPtr.Zero);
            if (result != DispChangeSuccessful)
                return false;

            _changed = true;
            _deviceName = deviceName;
            _size = size;
            return true;
        }

        public void Restore()
        {
            if (!_changed)
                return;

            ChangeDisplaySettingsEx(_deviceName, IntPtr.Zero, IntPtr.Zero, 0, IntPtr.Zero);
            _changed = false;
            _deviceName = null;
            _size = Size.Empty;
        }

        public static Rectangle GetBounds(Screen screen)
        {
            if (screen == null)
                return Rectangle.Empty;

            return GetBounds(screen.DeviceName, screen.Bounds);
        }

        public static Rectangle GetBounds(string deviceName, Rectangle fallbackBounds)
        {
            if (string.IsNullOrWhiteSpace(deviceName))
                return fallbackBounds;

            DevMode mode = CreateDevMode();
            if (!EnumDisplaySettings(deviceName, EnumCurrentSettings, ref mode))
                return fallbackBounds;

            return new Rectangle(mode.dmPositionX, mode.dmPositionY, (int)mode.dmPelsWidth, (int)mode.dmPelsHeight);
        }

        public static Screen GetScreenByDeviceName(string deviceName, Screen fallbackScreen)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (string.Equals(screen.DeviceName, deviceName, StringComparison.OrdinalIgnoreCase))
                    return screen;
            }

            return fallbackScreen ?? Screen.PrimaryScreen;
        }

        public static List<Size> GetSupportedSizes(Screen screen, Size minimumSize, Size configuredSize)
        {
            List<Size> sizes = new List<Size>();

            if (screen != null)
            {
                for (int modeIndex = 0; ; modeIndex++)
                {
                    DevMode mode = CreateDevMode();
                    if (!EnumDisplaySettings(screen.DeviceName, modeIndex, ref mode))
                        break;

                    Size size = new Size((int)mode.dmPelsWidth, (int)mode.dmPelsHeight);
                    if (size.Width < minimumSize.Width || size.Height < minimumSize.Height)
                        continue;

                    if (!sizes.Contains(size))
                        sizes.Add(size);
                }
            }

            if (sizes.Count == 0)
            {
                foreach (Screen fallbackScreen in Screen.AllScreens)
                {
                    Size size = fallbackScreen.Bounds.Size;
                    if (size.Width >= minimumSize.Width && size.Height >= minimumSize.Height && !sizes.Contains(size))
                        sizes.Add(size);
                }
            }

            if (!sizes.Contains(configuredSize))
                sizes.Add(configuredSize);

            sizes.Sort((s1, s2) => (s1.Width * s1.Height).CompareTo(s2.Width * s2.Height));
            return sizes;
        }

        private static bool TryGetMode(string deviceName, Size size, out DevMode displayMode)
        {
            for (int modeIndex = 0; ; modeIndex++)
            {
                DevMode mode = CreateDevMode();
                if (!EnumDisplaySettings(deviceName, modeIndex, ref mode))
                    break;

                if (mode.dmPelsWidth != (uint)size.Width || mode.dmPelsHeight != (uint)size.Height)
                    continue;

                displayMode = mode;
                return true;
            }

            displayMode = default;
            return false;
        }

        private static DevMode CreateDevMode()
        {
            return new DevMode { dmSize = (ushort)Marshal.SizeOf<DevMode>() };
        }

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DevMode devMode);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int ChangeDisplaySettingsEx(string deviceName, ref DevMode devMode, IntPtr hwnd, int flags, IntPtr lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int ChangeDisplaySettingsEx(string deviceName, IntPtr devMode, IntPtr hwnd, int flags, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct DevMode
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public ushort dmSpecVersion;
            public ushort dmDriverVersion;
            public ushort dmSize;
            public ushort dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public uint dmDisplayOrientation;
            public uint dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            public ushort dmLogPixels;
            public uint dmBitsPerPel;
            public uint dmPelsWidth;
            public uint dmPelsHeight;
            public uint dmDisplayFlags;
            public uint dmDisplayFrequency;
            public uint dmICMMethod;
            public uint dmICMIntent;
            public uint dmMediaType;
            public uint dmDitherType;
            public uint dmReserved1;
            public uint dmReserved2;
            public uint dmPanningWidth;
            public uint dmPanningHeight;
        }
    }
}
