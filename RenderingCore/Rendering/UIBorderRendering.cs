using System;
using System.Drawing;

namespace Shared.Rendering
{
    public static partial class RenderingPipelineManager
    {
        /// <summary>Axis-aligned stroke with the same physical endpoints as the line path.</summary>
        public static void DrawBorderStroke(PointF start, PointF end, float width, Color colour)
        {
            // Other offscreen surfaces use their own projection; retain their established line path.
            if (_activePipeline?.IsPresentationSurface != true && !IsUICacheSurface)
            {
                DrawLine(new[] { new LinePoint(start.X, start.Y), new LinePoint(end.X, end.Y) }, colour);
                return;
            }
            Size logical = Settings.ActiveSceneSize, physical = GetBackBufferSize();
            float sx = physical.Width / (float)Math.Max(1, logical.Width);
            float sy = physical.Height / (float)Math.Max(1, logical.Height);
            start = ScaleUIPoint(start);
            end = ScaleUIPoint(end);
            float x1 = MathF.Floor(start.X * sx) + .5F, y1 = MathF.Floor(start.Y * sy) + .5F;
            float x2 = MathF.Floor(end.X * sx) + .5F, y2 = MathF.Floor(end.Y * sy) + .5F;
            width = Math.Max(1F, width);
            RectangleF pixels = Math.Abs(x2 - x1) >= Math.Abs(y2 - y1)
                ? new RectangleF(Math.Min(x1, x2), y1 - width / 2F, Math.Max(width, Math.Abs(x2 - x1)), width)
                : new RectangleF(x1 - width / 2F, Math.Min(y1, y2), width, Math.Max(width, Math.Abs(y2 - y1)));
            _activePipeline.DrawTexture(GetSolidFillTexture(), new Rectangle(0, 0, 1, 1),
                new RectangleF(pixels.X / sx, pixels.Y / sy, pixels.Width / sx, pixels.Height / sy), colour);
        }
    }
}
