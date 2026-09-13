using System;
using System.Drawing;
using System.Numerics;

namespace Shared.Rendering
{
    public static partial class RenderingPipelineManager
    {
        private static UICacheContext _uiCacheContext;
        private static bool _presentingUICache;
        internal static bool PresentingUICache => _presentingUICache;
        internal static bool ForcePointSampling => DrawingDpiText || _presentingUICache;
        internal static bool IsUICacheSurface => _uiCacheContext != null &&
            ReferenceEquals(_uiCacheContext.Surface.NativeHandle, _activePipeline?.GetCurrentSurface().NativeHandle);
        public static bool CanCacheUI => SupportsCachedRenderTargets &&
            (_activePipeline?.IsPresentationSurface == true || IsUICacheSurface) &&
            !IsBlending() && GetOpacity() == 1F && !_spriteShaderEffect.HasValue;
        internal static bool CacheUsesFractionalScale => IsUICacheSurface &&
            (IsFractional(_uiCacheContext.ScaleX) || IsFractional(_uiCacheContext.ScaleY));
        private static bool IsFractional(float value) => Math.Abs(value - MathF.Round(value)) > .001F;

        /// <summary>Converts the presentation coordinate system into physical target pixels.
        /// Intermediate control textures keep their own coordinate system.</summary>
        private sealed class UICacheContext : IDisposable
        {
            public readonly RenderSurface Surface;
            public readonly Rectangle Bounds;
            public readonly float ScaleX, ScaleY;
            private readonly RenderSurface _previousSurface;
            private readonly UICacheContext _previousContext;
            private bool _disposed;

            public UICacheContext(RenderSurface surface, Rectangle bounds)
            {
                Surface = surface;
                Bounds = bounds;
                Size logical = Settings.ActiveSceneSize;
                Size physical = GetBackBufferSize();
                ScaleX = physical.Width / (float)Math.Max(1, logical.Width);
                ScaleY = physical.Height / (float)Math.Max(1, logical.Height);
                _previousSurface = GetCurrentSurface();
                _previousContext = _uiCacheContext;
                // Flush using the old context, then bind before activating the new context.
                SetSurface(surface);
                _uiCacheContext = this;
            }

            public void Dispose()
            {
                if (_disposed) return;
                try { SetSurface(_previousSurface); }
                finally { _uiCacheContext = _previousContext; _disposed = true; }
            }
        }

        public static IDisposable PushUICacheTarget(RenderSurface surface, Rectangle physicalBounds)
        {
            if (!CanCacheUI) throw new InvalidOperationException("The current surface cannot reproduce UI presentation.");
            return new UICacheContext(surface, physicalBounds);
        }

        internal static RectangleF MapUICacheDestination(RectangleF destination, Size source)
        {
            if (!IsUICacheSurface) return destination;
            destination = AlignTextDestination(destination, GetBackBufferSize(), source);
            destination = AlignBorderBackground(destination, GetBackBufferSize());
            var context = _uiCacheContext;
            return new RectangleF(destination.X * context.ScaleX - context.Bounds.X,
                destination.Y * context.ScaleY - context.Bounds.Y,
                destination.Width * context.ScaleX, destination.Height * context.ScaleY);
        }

        internal static Matrix3x2 MapUICacheTransform(Matrix3x2 transform)
        {
            if (!IsUICacheSurface) return transform;
            var context = _uiCacheContext;
            return transform * Matrix3x2.CreateScale(context.ScaleX, context.ScaleY) *
                Matrix3x2.CreateTranslation(-context.Bounds.X, -context.Bounds.Y);
        }

        private static PointF MapUICachePoint(PointF point)
        {
            if (!IsUICacheSurface) return point;
            return new PointF(point.X * _uiCacheContext.ScaleX - _uiCacheContext.Bounds.X,
                point.Y * _uiCacheContext.ScaleY - _uiCacheContext.Bounds.Y);
        }

        public static Rectangle GetUIPhysicalBounds(Rectangle logicalBounds)
        {
            RectangleF scaled = ScaleUIRectangle(logicalBounds);
            Size physical = GetBackBufferSize(), logical = Settings.ActiveSceneSize;
            float sx = physical.Width / (float)Math.Max(1, logical.Width);
            float sy = physical.Height / (float)Math.Max(1, logical.Height);
            return Rectangle.Intersect(new Rectangle(Point.Empty, physical), Rectangle.FromLTRB(
                (int)MathF.Floor(scaled.Left * sx), (int)MathF.Floor(scaled.Top * sy),
                (int)MathF.Ceiling(scaled.Right * sx), (int)MathF.Ceiling(scaled.Bottom * sy)));
        }

        /// <summary>Copy premultiplied physical pixels without reapplying the UI transform.</summary>
        public static void PresentUICache(RenderTexture texture, Rectangle targetBounds, Rectangle visibleBounds)
        {
            Rectangle pixels = Rectangle.Intersect(targetBounds, visibleBounds);
            if (pixels.Width <= 0 || pixels.Height <= 0) return;
            Size logical = Settings.ActiveSceneSize, physical = GetBackBufferSize();
            Rectangle source = new(pixels.X - targetBounds.X, pixels.Y - targetBounds.Y, pixels.Width, pixels.Height);
            RectangleF destination = new(pixels.X * (float)logical.Width / physical.Width,
                pixels.Y * (float)logical.Height / physical.Height,
                pixels.Width * (float)logical.Width / physical.Width,
                pixels.Height * (float)logical.Height / physical.Height);
            bool previous = _presentingUICache;
            _presentingUICache = true;
            try { _activePipeline.DrawTexture(texture, source, destination, Color.White); }
            finally { _presentingUICache = previous; }
        }
    }
}
