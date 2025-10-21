using Client.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Client.Controls
{
    public class DXMapInfoControl : DXControl
    {
        private sealed class BorderAnimation
        {
            public DateTime StartTime { get; set; }
        }

        private readonly List<BorderAnimation> _activeAnimations = [];

        public int BorderAnimationLayerCount = 4;
        public int StartInnerInflation = 50;
        public int TargetInnerInflation = 2;
        public int StartLayerGap = 10;
        public int TargetLayerGap = 2;
        public float InnerLayerOpacity = 1f;
        public float OuterLayerOpacity = 0.25f;
        public TimeSpan BorderAnimationDuration = TimeSpan.FromMilliseconds(750);

        public bool IsBorderAnimationActive => _activeAnimations.Count > 0;

        public DXMapInfoControl()
        {
            DrawTexture = true;
        }

        public void PlayBorderAnimation()
        {
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
            Surface oldSurface = DXManager.CurrentSurface;
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

                    DXManager.SetSurface(DXManager.ScratchSurface);
                    DXManager.Device.Clear(ClearFlags.Target, 0, 0, 0);
                    DrawBorderLayerToScratch(inflation, strokeWidth, layerColour);
                    DXManager.SetSurface(oldSurface);

                    PresentTexture(DXManager.ScratchTexture, Parent, Rectangle.Inflate(DisplayArea, inflation, inflation), Color.White, this);
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

            Rectangle rectangle = new(x, y, width, height);
            DXManager.Device.ColorFill(DXManager.ScratchSurface, rectangle, new Color4(colour));
        }

        private Color GetLayerColour(int layer)
        {
            Color baseColour = BorderColour == Color.Empty ? Color.White : BorderColour;
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
    }
}