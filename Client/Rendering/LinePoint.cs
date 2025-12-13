using System.Runtime.CompilerServices;

namespace Client.Rendering
{
    public readonly struct LinePoint
    {
        public float X { get; }
        public float Y { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LinePoint(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
