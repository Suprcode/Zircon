using Client.Envir;
using Client.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Client.Controls
{
    public class DXMapInfoControl : DXControl
    {
        private sealed class BorderAnimation
        {
            public DateTime StartTime { get; set; }
        }

        private readonly List<BorderAnimation> _activeAnimations = [];

        private static readonly ConditionalWeakTable<DXControl, DXMapInfoControl> OverlayTable = new();

        private DXControl _overlayTarget;
        private bool _isOverlayInstance;

        public int BorderAnimationLayerCount = 4;
        public int StartInnerInflation = 50;
        public int TargetInnerInflation = 2;
        public int StartLayerGap = 10;
        public int TargetLayerGap = 2;
        public float InnerLayerOpacity = 1f;
        public float OuterLayerOpacity = 0.25f;
        public TimeSpan BorderAnimationDuration = TimeSpan.FromMilliseconds(750);
        public Color AnimationColour;

        public bool IsBorderAnimationActive => _activeAnimations.Count > 0;

        public DXMapInfoControl()
        {
            DrawTexture = true;
            PassThrough = false;
        }

        public DXControl OverlayTarget
        {
            get => _overlayTarget;
            set
            {
                if (_overlayTarget == value) return;

                if (_overlayTarget != null)
                    DetachOverlayTarget(_overlayTarget);

                _overlayTarget = value;

                if (_overlayTarget != null)
                {
                    AttachOverlayTarget(_overlayTarget);
                    SynchroniseWithOverlayTarget();
                }
                else if (_isOverlayInstance)
                {
                    Parent = null;
                }
            }
        }

        public static DXMapInfoControl GetOverlay(DXControl target)
        {
            if (target == null || target.IsDisposed)
                return null;

            if (!OverlayTable.TryGetValue(target, out DXMapInfoControl overlay) || overlay.IsDisposed)
            {
                overlay = CreateOverlayInstance(target);
                OverlayTable.Remove(target);
                OverlayTable.Add(target, overlay);
            }
            else
            {
                overlay.OverlayTarget = target;
            }

            //overlay.BringToFront();

            return overlay;
        }

        private static DXMapInfoControl CreateOverlayInstance(DXControl target)
        {
            DXMapInfoControl overlay = new DXMapInfoControl
            {
                DrawTexture = false,
                BackColour = Color.Empty,
                Border = false,
                PassThrough = true,
                BorderSize = 3f
            };

            overlay._isOverlayInstance = true;
            overlay.OverlayTarget = target;

            return overlay;
        }

        public void PlayBorderAnimation(Color animationColour)
        {
            AnimationColour = animationColour;

            _activeAnimations.Add(new BorderAnimation { StartTime = CEnvir.Now });
        }

        public override void Process()
        {
            base.Process();

            if (_activeAnimations.Count == 0) return;

            DateTime now = CEnvir.Now;
            bool removed = false;

            for (int i = _activeAnimations.Count - 1; i >= 0; i--)
            {
                BorderAnimation animation = _activeAnimations[i];
                if (now - animation.StartTime >= BorderAnimationDuration)
                {
                    _activeAnimations.RemoveAt(i);
                    removed = true;
                }
            }

            if (removed)
            {
                UpdateBorderInformation();
            }
        }

        protected override void DrawBorder()
        {
            if (DrawBorderAnimation()) return;

            base.DrawBorder();
        }

        private bool DrawBorderAnimation()
        {
            if (_activeAnimations.Count == 0) return false;

            bool drew = false;
            RenderSurface oldSurface = RenderingPipelineManager.GetCurrentSurface();
            int strokeWidth = Math.Max(1, (int)Math.Round(BorderSize));

            DateTime now = CEnvir.Now;
            double duration = BorderAnimationDuration.TotalMilliseconds;
            if (duration <= 0) return false;

            foreach (BorderAnimation animation in _activeAnimations)
            {
                double elapsed = (now - animation.StartTime).TotalMilliseconds;
                if (elapsed < 0) continue;

                float progress = (float)Math.Min(1f, elapsed / duration);

                for (int layer = BorderAnimationLayerCount - 1; layer >= 0; layer--)
                {
                    float startInflation = StartInnerInflation + layer * StartLayerGap;
                    float targetInflation = TargetInnerInflation + layer * TargetLayerGap;
                    float inflationValue = Lerp(startInflation, targetInflation, progress);
                    if (inflationValue < 0f) continue;

                    int inflation = Math.Max(0, (int)Math.Round(inflationValue));
                    Color layerColour = GetLayerColour(layer);

                    RenderingPipelineManager.SetSurface(RenderingPipelineManager.GetScratchSurface());
                    RenderingPipelineManager.Clear(RenderClearFlags.Target, Color.FromArgb(0, 0, 0, 0), 0f, 0);
                    DrawBorderLayerToScratch(inflation, strokeWidth, layerColour);
                    RenderingPipelineManager.SetSurface(oldSurface);

                    RenderTexture scratchHandle = RenderingPipelineManager.GetScratchTexture();

                    PresentTexture(scratchHandle, Parent, Rectangle.Inflate(DisplayArea, inflation, inflation), Color.White, this);
                    drew = true;
                }
            }

            return drew;
        }

        private void DrawBorderLayerToScratch(int inflation, int strokeWidth, Color layerColour)
        {
            int effectiveInflation = Math.Max(0, inflation);
            int width = Size.Width + effectiveInflation * 2;
            int height = Size.Height + effectiveInflation * 2;

            if (width <= 0 || height <= 0) return;

            int topBottomThickness = Math.Max(1, Math.Min(strokeWidth, height));

            FillScratchRectangle(0, 0, width, topBottomThickness, layerColour);

            if (height <= topBottomThickness)
                return;

            FillScratchRectangle(0, height - topBottomThickness, width, topBottomThickness, layerColour);

            int verticalHeight = height - topBottomThickness * 2;
            if (verticalHeight <= 0) return;

            int sideThickness = Math.Max(1, Math.Min(strokeWidth, width));

            FillScratchRectangle(0, topBottomThickness, sideThickness, verticalHeight, layerColour);
            if (width > sideThickness)
                FillScratchRectangle(width - sideThickness, topBottomThickness, sideThickness, verticalHeight, layerColour);
        }

        private static void FillScratchRectangle(int x, int y, int width, int height, Color colour)
        {
            if (width <= 0 || height <= 0) return;

            RenderingPipelineManager.ColorFill(RenderingPipelineManager.GetScratchSurface(), new Rectangle(x, y, x + width, y + height), colour);
        }

        private Color GetLayerColour(int layer)
        {
            Color baseColour = AnimationColour == Color.Empty ? Color.White : AnimationColour;
            float baseAlpha = baseColour.A / 255f;
            if (baseAlpha <= 0f)
                baseAlpha = 1f;

            float t = BorderAnimationLayerCount <= 1 ? 0f : layer / (float)(BorderAnimationLayerCount - 1);
            float opacity = Lerp(InnerLayerOpacity, OuterLayerOpacity, t);
            opacity = Math.Clamp(opacity, 0f, 1f);

            byte alpha = (byte)Math.Max(0, Math.Min(255, (int)Math.Round(255f * baseAlpha * opacity)));
            return Color.FromArgb(alpha, baseColour);
        }

        private static float Lerp(float from, float to, float t)
        {
            return from + (to - from) * Math.Min(1f, Math.Max(0f, t));
        }

        private void AttachOverlayTarget(DXControl target)
        {
            target.LocationChanged += OverlayTargetBoundsChanged;
            target.SizeChanged += OverlayTargetBoundsChanged;
            target.VisibleChanged += OverlayTargetVisibilityChanged;
            target.IsVisibleChanged += OverlayTargetVisibilityChanged;
            target.OpacityChanged += OverlayTargetOpacityChanged;
            target.ParentChanged += OverlayTargetParentChanged;
            target.Disposing += OverlayTargetDisposing;

            PassThrough = true;

            SynchroniseWithOverlayTarget();
        }

        private void DetachOverlayTarget(DXControl target)
        {
            target.LocationChanged -= OverlayTargetBoundsChanged;
            target.SizeChanged -= OverlayTargetBoundsChanged;
            target.VisibleChanged -= OverlayTargetVisibilityChanged;
            target.IsVisibleChanged -= OverlayTargetVisibilityChanged;
            target.OpacityChanged -= OverlayTargetOpacityChanged;
            target.ParentChanged -= OverlayTargetParentChanged;
            target.Disposing -= OverlayTargetDisposing;

            OverlayTable.Remove(target);
        }

        private void OverlayTargetBoundsChanged(object sender, EventArgs e)
        {
            SynchroniseWithOverlayTarget();
        }

        private void OverlayTargetVisibilityChanged(object sender, EventArgs e)
        {
            if (_overlayTarget == null) return;

            Visible = _overlayTarget.Visible;
            IsVisible = _overlayTarget.IsVisible;
        }

        private void OverlayTargetOpacityChanged(object sender, EventArgs e)
        {
            if (_overlayTarget == null) return;

            Opacity = _overlayTarget.Opacity;
        }

        private void OverlayTargetParentChanged(object sender, EventArgs e)
        {
            if (_overlayTarget == null) return;

            Parent = _overlayTarget.Parent;
            //BringToFront();
        }

        private void OverlayTargetDisposing(object sender, EventArgs e)
        {
            if (sender is DXControl target)
            {
                DetachOverlayTarget(target);
            }

            if (!IsDisposed)
                Dispose();
        }

        private void SynchroniseWithOverlayTarget()
        {
            if (_overlayTarget == null) return;

            if (Parent != _overlayTarget.Parent)
                Parent = _overlayTarget.Parent;

            Location = _overlayTarget.Location;
            Size = _overlayTarget.Size;
            Opacity = _overlayTarget.Opacity;
            Visible = _overlayTarget.Visible;
            IsVisible = _overlayTarget.IsVisible;

            //BringToFront();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _overlayTarget != null)
            {
                DetachOverlayTarget(_overlayTarget);
                _overlayTarget = null;
            }

            base.Dispose(disposing);
        }
    }
}