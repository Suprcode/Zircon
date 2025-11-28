using System.Drawing;

namespace Client.Rendering
{
    public enum SpriteEffectType
    {
        None,
        Outline
    }

    public sealed class SpriteEffectRequest
    {
        public SpriteEffectType Type { get; }
        public Color Colour { get; }
        public float Thickness { get; }

        public SpriteEffectRequest(SpriteEffectType type, Color colour, float thickness)
        {
            Type = type;
            Colour = colour;
            Thickness = thickness;
        }
    }
}
