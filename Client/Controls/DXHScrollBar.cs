using Client.Rendering;
using Library;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Client.Controls
{
    public sealed class DXHScrollBar : DXControl
    {
        #region Properties

        #region Value

        public int Value
        {
            get => _Value;
            set
            {
                if (_Value == value) return;

                int oldValue = _Value;
                _Value = value;

                OnValueChanged(oldValue, value);
            }
        }
        private int _Value;
        public event EventHandler<EventArgs> ValueChanged;
        public void OnValueChanged(int oValue, int nValue)
        {
            if (Value != Math.Max(MinValue, Math.Min(MaxValue - VisibleSize, Value)))
            {
                Value = Math.Max(MinValue, Math.Min(MaxValue - VisibleSize, Value));
                return;
            }

            UpdateScrollBar();

            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region MaxValue

        public int MaxValue
        {
            get => _MaxValue;
            set
            {
                if (_MaxValue == value) return;

                int oldValue = _MaxValue;
                _MaxValue = value;

                OnMaxValueChanged(oldValue, value);
            }
        }
        private int _MaxValue;
        public event EventHandler<EventArgs> MaxValueChanged;
        public void OnMaxValueChanged(int oValue, int nValue)
        {
            if (Value + VisibleSize > MaxValue)
                Value = MaxValue - VisibleSize;

            UpdateScrollBar();

            MaxValueChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region MinValue

        public int MinValue
        {
            get => _MinValue;
            set
            {
                if (_MinValue == value) return;

                int oldValue = _MinValue;
                _MinValue = value;

                OnMinValueChanged(oldValue, value);
            }
        }
        private int _MinValue;
        public event EventHandler<EventArgs> MinValueChanged;
        public void OnMinValueChanged(int oValue, int nValue)
        {
            UpdateScrollBar();

            MinValueChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region VisibleSize

        public int VisibleSize
        {
            get => _VisibleSize;
            set
            {
                if (_VisibleSize == value) return;

                int oldValue = _VisibleSize;
                _VisibleSize = value;

                OnVisibleSizeChanged(oldValue, value);
            }
        }
        private int _VisibleSize;
        public event EventHandler<EventArgs> VisibleSizeChanged;
        public void OnVisibleSizeChanged(int oValue, int nValue)
        {
            UpdateScrollBar();

            VisibleSizeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region HideWhenNoScroll

        public bool HideWhenNoScroll
        {
            get => _HideWhenNoScroll;
            set
            {
                if (_HideWhenNoScroll == value) return;

                bool oldValue = _HideWhenNoScroll;
                _HideWhenNoScroll = value;

                OnHideWhenNoScrollChanged(oldValue, value);
            }
        }
        private bool _HideWhenNoScroll;
        public event EventHandler<EventArgs> HideWhenNoScrollChanged;
        public void OnHideWhenNoScrollChanged(bool oValue, bool nValue)
        {
            UpdateScrollBar();

            HideWhenNoScrollChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion


        private int ScrollWidth => Size.Width - 50;

        public int Change = 10;

        public DXButton LeftButton, RightButton, PositionBar;

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            if (ScrollWidth < 0)
                return;

            RightButton.Location = new Point(Size.Width - 13, LeftButton.Location.Y);

            UpdateScrollBar();
        }

        #endregion

        public DXHScrollBar()
        {
            Border = true;
            BorderColour = Color.FromArgb(198, 166, 99);
            DrawTexture = true;
            BackColour = Color.Black;

            LeftButton = new DXButton
            {
                Index = 44,
                LibraryFile = LibraryFile.Interface,
                Location = new Point(1, 1),
                Enabled = false,
                Parent = this,
            };
            LeftButton.MouseClick += (o, e) => Value -= Change;
            LeftButton.MouseWheel += DoMouseWheel;

            RightButton = new DXButton
            {
                Index = 46,
                LibraryFile = LibraryFile.Interface,
                Location = new Point(0, LeftButton.Location.Y),
                Enabled = false,
                Parent = this,
            };
            RightButton.MouseClick += (o, e) => Value += Change;
            RightButton.MouseWheel += DoMouseWheel;

            PositionBar = new DXButton
            {
                Index = 45,
                LibraryFile = LibraryFile.Interface,
                Location = new Point(LeftButton.Size.Width + 4, LeftButton.Location.Y), // | - Space - Button - Space - | - Space - Bar
                Enabled = false,
                Parent = this,
                Movable = true,
                Sound = SoundIndex.None,
                CanBePressed = false,
            };
            PositionBar.Moving += PositionBar_Moving;
            PositionBar.MouseWheel += DoMouseWheel;


        }

        #region Methods

        private void UpdateScrollBar()
        {
            LeftButton.Enabled = Value > MinValue;
            RightButton.Enabled = Value < MaxValue - VisibleSize;
            PositionBar.Enabled = MaxValue - MinValue > VisibleSize;

            if (MaxValue - MinValue - VisibleSize != 0)
                PositionBar.Location = new Point(16 + (int)(ScrollWidth * (Value / (float)(MaxValue - MinValue - VisibleSize))), LeftButton.Location.Y);

            if (HideWhenNoScroll)
                Visible = LeftButton.Enabled || RightButton.Enabled;
        }

        public void DoMouseWheel(object sender, MouseEventArgs e)
        {
            Value -= e.Delta / SystemInformation.MouseWheelScrollDelta * Change;
        }

        protected internal override void UpdateBorderInformation()
        {
            BorderInformation = null;
            if (!Border || DisplayArea.Width == 0 || DisplayArea.Height == 0)
            {
                return;
            }

            BorderInformation = new[]
            {
                new LinePoint(0, 0),
                new LinePoint(Size.Width + 1, 0),
                new LinePoint(Size.Width + 1, Size.Height + 1),
                new LinePoint(0, Size.Height + 1),
                new LinePoint(0, 0),
                new LinePoint(14, 0),
                new LinePoint(14, Size.Height + 1),
                new LinePoint(Size.Width - 13, Size.Height + 1),
                new LinePoint(Size.Width - 13, 0),

            };
        }

        private void PositionBar_Moving(object sender, MouseEventArgs e)
        {
            Value = (int)Math.Round((PositionBar.Location.X - 16) * (MaxValue - MinValue - VisibleSize) / (float)ScrollWidth);

            if (MaxValue - MinValue - VisibleSize == 0) return;

            PositionBar.Location = new Point(16 + (int)(ScrollWidth * (Value / (float)(MaxValue - MinValue - VisibleSize))), LeftButton.Location.Y);
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Value = (int)Math.Round((e.Location.X - DisplayArea.Left - (PositionBar.Size.Width + (PositionBar.Size.Width / 2))) * (MaxValue - MinValue - VisibleSize) / (float)ScrollWidth);
        }
        public override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            DoMouseWheel(this, e);
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Value = 0;
                ValueChanged = null;

                _MaxValue = 0;
                MaxValueChanged = null;

                _MinValue = 0;
                MinValueChanged = null;

                _VisibleSize = 0;
                VisibleSizeChanged = null;

                Change = 0;

                if (LeftButton != null)
                {
                    if (!LeftButton.IsDisposed)
                        LeftButton.Dispose();

                    LeftButton = null;
                }

                if (RightButton != null)
                {
                    if (!RightButton.IsDisposed)
                        RightButton.Dispose();

                    RightButton = null;
                }

                if (PositionBar != null)
                {
                    if (!PositionBar.IsDisposed)
                        PositionBar.Dispose();

                    PositionBar = null;
                }
            }

        }

        #endregion
    }
}