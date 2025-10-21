using System;
using System.Drawing;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;
using Vector2 = SharpDX.Vector2;
using Vector3 = SharpDX.Vector3;

namespace Client.Extensions;

public static class SharpDXExtensions
{
    public static void Draw(this Sprite sprite, Texture texture, Rectangle? sourceRectangle, Vector3? center, Vector3? position, Color color)
    {
        ArgumentNullException.ThrowIfNull(sprite);
        ArgumentNullException.ThrowIfNull(texture);

        RawRectangle? rawRectangle = sourceRectangle.HasValue ? ToRawRectangle(sourceRectangle.Value) : null;
        RawVector3? rawCenter = center.HasValue ? ToRawVector3(center.Value) : null;
        RawVector3? rawPosition = position.HasValue ? ToRawVector3(position.Value) : null;

        sprite.Draw(texture, rawRectangle, rawCenter, rawPosition, ToColor(color));
    }

    public static void Draw(this Sprite sprite, Texture texture, Vector3? center, Vector3? position, Color color)
    {
        sprite.Draw(texture, null, center, position, color);
    }

    public static void Draw(this Sprite sprite, Texture texture, Color color)
    {
        sprite.Draw(texture, null, null, null, color);
    }

    public static void Draw(this Line line, Vector2[] vertexList, Color color)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentNullException.ThrowIfNull(vertexList);

        RawVector2[] raw = new RawVector2[vertexList.Length];
        for (int i = 0; i < vertexList.Length; i++)
        {
            raw[i] = new RawVector2(vertexList[i].X, vertexList[i].Y);
        }

        line.Draw(raw, ToColor(color));
    }

    private static RawRectangle ToRawRectangle(Rectangle rectangle) => new(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);

    private static RawVector3 ToRawVector3(Vector3 vector) => new(vector.X, vector.Y, vector.Z);

    private static ColorBGRA ToColor(Color color) => new(color.R, color.G, color.B, color.A);
}
