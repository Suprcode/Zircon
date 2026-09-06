using Library;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Client.Controls
{
    public class DXValueBar : DXControl
    {
        private const int OuterBarIndex = 4743;
        private const int InnerBarIndex = 4742;
        private const int MaximumStepCount = 10000;

        private readonly int _stepCount;
        private Func<float, string> _valueFormatter;

        public float Minimum { get; }
        public float Maximum { get; }
        public float Change { get; }

        public DXImageControl OuterBar;
        public DXControl InnerBar;
        public DXHScrollBar ScrollBar;
        public DXLabel ValueLabel;

        public float Value
        {
            get => _Value;
            set
            {
                value = Math.Max(Minimum, Math.Min(Maximum, value));
                value = Minimum + (float)Math.Round((value - Minimum) / Change, MidpointRounding.AwayFromZero) * Change;

                if (_Value == value) return;

                float oldValue = _Value;
                _Value = value;

                OnValueChanged(oldValue, value);
            }
        }
        private float _Value;
        public event EventHandler<EventArgs> ValueChanged;

        public DXValueBar(float minimum, float maximum, float change, Func<float, string> valueFormatter = null)
        {
            float safeMinimum = float.IsFinite(minimum) ? minimum : 0F;
            float safeMaximum = float.IsFinite(maximum) && maximum > safeMinimum ? maximum : safeMinimum + 1F;

            if (safeMaximum <= safeMinimum || !float.IsFinite(safeMaximum - safeMinimum))
            {
                safeMinimum = 0F;
                safeMaximum = 1F;
            }

            Minimum = safeMinimum;
            Maximum = safeMaximum;

            float range = Maximum - Minimum;
            float requestedChange = float.IsFinite(change) && change > 0F ? change : range;
            double requestedStepCount = Math.Round(range / (double)requestedChange);
            _stepCount = (int)Math.Max(1D, Math.Min(MaximumStepCount, requestedStepCount));
            Change = range / _stepCount;

            _valueFormatter = valueFormatter ?? (value => value.ToString("0.##"));
            _Value = Minimum;

            Size = new Size(215, 18);

            OuterBar = new DXImageControl
            {
                LibraryFile = LibraryFile.GameInter,
                Index = OuterBarIndex,
                Parent = this,
                Location = new Point(20, 3)
            };

            InnerBar = new DXControl
            {
                Parent = OuterBar,
                Location = new Point(2, 2),
                Size = OuterBar.Library.GetSize(InnerBarIndex),
            };
            InnerBar.BeforeDraw += DrawInnerBar;

            ScrollBar = new DXHScrollBar
            {
                Parent = this,
                BackColour = Color.Empty,
                Border = false,
                Size = new Size(195, 18),
                Location = new Point(5, 0),
                MinValue = 0,
                MaxValue = _stepCount,
                Change = 1,
                LeftButton = { Visible = false, IsControl = false, },
                RightButton = { Visible = false, IsControl = false, },
                PositionBar = { Index = 4746, PressedIndex = 4745, HoverIndex = 4746, LibraryFile = LibraryFile.GameInter }
            };
            ScrollBar.ValueChanged += ScrollBar_ValueChanged;

            ValueLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(180, 0),
                Size = new Size(35, 18),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Outline = true,
                IsControl = false,
                PassThrough = true,
            };

            UpdateDisplay();
        }

        public virtual void OnValueChanged(float oldValue, float newValue)
        {
            UpdateDisplay();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            Value = Minimum + ScrollBar.Value * Change;
        }

        private void UpdateDisplay()
        {
            if (ScrollBar != null)
                ScrollBar.Value = (int)Math.Round((Value - Minimum) / Change);

            if (ValueLabel != null)
                ValueLabel.Text = _valueFormatter(Value);
        }

        private void DrawInnerBar(object sender, EventArgs e)
        {
            float percent = ScrollBar.Value / (float)_stepCount;
            if (percent <= 0) return;

            if (!OuterBar.Library.TryGetTexture(InnerBarIndex, ImageType.Image, out MirImage image, out var texture, out var sourceRectangle)) return;

            PresentTexture(texture, sourceRectangle, OuterBar,
                new Rectangle(InnerBar.DisplayArea.X, InnerBar.DisplayArea.Y, (int)(image.Width * percent), image.Height),
                Color.White, InnerBar);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            ValueChanged = null;
            _valueFormatter = null;

            if (OuterBar != null)
            {
                if (!OuterBar.IsDisposed)
                    OuterBar.Dispose();

                OuterBar = null;
            }

            if (InnerBar != null)
            {
                InnerBar.BeforeDraw -= DrawInnerBar;

                if (!InnerBar.IsDisposed)
                    InnerBar.Dispose();

                InnerBar = null;
            }

            if (ScrollBar != null)
            {
                ScrollBar.ValueChanged -= ScrollBar_ValueChanged;

                if (!ScrollBar.IsDisposed)
                    ScrollBar.Dispose();

                ScrollBar = null;
            }

            if (ValueLabel != null)
            {
                if (!ValueLabel.IsDisposed)
                    ValueLabel.Dispose();

                ValueLabel = null;
            }
        }
    }
}
