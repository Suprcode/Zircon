using System.Drawing;

namespace Client.Rendering
{
    public sealed class DisplayMonitorInfo
    {
        public DisplayMonitorInfo(int index, string deviceName, bool primary, Rectangle bounds)
        {
            Index = index;
            DeviceName = deviceName;
            Primary = primary;
            Bounds = bounds;
        }

        public int Index { get; }
        public string DeviceName { get; }
        public bool Primary { get; }
        public Rectangle Bounds { get; }

        public string DisplayName
        {
            get
            {
                string primaryText = Primary ? " (Primary)" : string.Empty;

                return $"Monitor {Index + 1}{primaryText} - {Bounds.Width} x {Bounds.Height}";
            }
        }

        public override bool Equals(object obj)
        {
            return obj is DisplayMonitorInfo other && string.Equals(DeviceName, other.DeviceName, System.StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return System.StringComparer.OrdinalIgnoreCase.GetHashCode(DeviceName ?? string.Empty);
        }
    }
}
