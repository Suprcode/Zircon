// Isolate label cache ownership from DirectX. These checks do not validate GPU disposal.
namespace Client.Envir
{
    public static class Config
    {
        public static string HighlightedItems { get; set; } = string.Empty;
        public static bool DrawEffects { get; set; } = true;
    }
}

namespace Client.Controls
{
    public sealed class DXLabel : System.IDisposable
    {
        public static int LiveCount;
        public DXLabel() { LiveCount++; }
        public string Text { get; set; }
        public System.Drawing.Color ForeColour { get; set; }
        public System.Drawing.Color BackColour { get; set; }
        public System.Drawing.Color OutlineColour { get; set; }
        public System.Drawing.Color BorderColour { get; set; }
        public bool Outline { get; set; }
        public bool Border { get; set; }
        public bool IsControl { get; set; }
        public bool IsVisible { get; set; }
        public bool IsDisposed { get; private set; }
        public void Dispose()
        {
            if (IsDisposed) throw new System.Exception("Label disposed twice.");
            IsDisposed = true;
            LiveCount--;
        }
    }
}
