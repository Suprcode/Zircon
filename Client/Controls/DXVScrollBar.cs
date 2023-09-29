using System;
using System.Drawing;
using System.Windows.Forms;
using Client.Envir;
using Library;
using SlimDX;

//Cleaned
namespace Client.Controls
{
    public sealed class DXVScrollBar : DXControl
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


        private int ScrollHeight => Size.Height - 50;

        public int Change = 10;

        public DXButton UpButton, DownButton, PositionBar;

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            if (ScrollHeight < 0)
                return;

            DownButton.Location = new Point(UpButton.Location.X, Size.Height - 13);

            UpdateScrollBar();
        }

        #endregion

        public DXVScrollBar()
        {
            Border = true;
            BorderColour = Color.FromArgb(198, 166, 99);
            DrawTexture = true;
            BackColour = Color.Black;

            UpButton = new DXButton
            {
                Index = 44,
                LibraryFile = LibraryFile.Interface,
                Location = new Point(1, 1),
                Enabled = false,
                Parent = this,
            };
            UpButton.MouseClick += (o, e) => Value -= Change;
            UpButton.MouseWheel += DoMouseWheel;

            DownButton = new DXButton
            {
                Index = 46,
                LibraryFile = LibraryFile.Interface,
                Location = new Point(UpButton.Location.X, 0),
                Enabled = false,
                Parent = this,
            };
            DownButton.MouseClick += (o, e) => Value += Change;
            DownButton.MouseWheel += DoMouseWheel;

            PositionBar = new DXButton
            {
                Index = 45,
                LibraryFile = LibraryFile.Interface,
                Location = new Point(UpButton.Location.X, UpButton.Size.Height + 4), // | - Space - Button - Space - | - Space - Bar 
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
            UpButton.Enabled = Value > MinValue;
            DownButton.Enabled = Value < MaxValue - VisibleSize;
            PositionBar.Enabled = MaxValue - MinValue > VisibleSize;

            if (MaxValue - MinValue - VisibleSize != 0)
                PositionBar.Location = new Point(UpButton.Location.X, 16 + (int) (ScrollHeight*(Value/(float) (MaxValue - MinValue - VisibleSize))));

            if (HideWhenNoScroll)
                Visible = UpButton.Enabled || DownButton.Enabled;
        }

        public void DoMouseWheel(object sender, MouseEventArgs e)
        {
            Value -= e.Delta/SystemInformation.MouseWheelScrollDelta * Change;
        }

        protected internal override void UpdateBorderInformation()
        {
            BorderInformation = null;
            if (!Border || DisplayArea.Width == 0 || DisplayArea.Height == 0) return;

            BorderInformation = new[]
            {
                new Vector2(0, 0),
                new Vector2(Size.Width + 1, 0),
                new Vector2(Size.Width + 1, Size.Height + 1),
                new Vector2(0, Size.Height + 1),
                new Vector2(0, 0),
                new Vector2(0,  14),
                new Vector2(Size.Width + 1, 14),
                new Vector2(Size.Width + 1, Size.Height - 13),
                new Vector2(0, Size.Height - 13),

            };
        }

        private void PositionBar_Moving(object sender, MouseEventArgs e)
        {
            Value = (int) Math.Round((PositionBar.Location.Y - 16)*(MaxValue - MinValue - VisibleSize)/(float) ScrollHeight);

            if (MaxValue - MinValue - VisibleSize == 0) return;

            PositionBar.Location = new Point(UpButton.Location.X, 16 + (int) (ScrollHeight*(Value/(float) (MaxValue - MinValue - VisibleSize))));
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Value = (int) Math.Round((e.Location.Y - DisplayArea.Top - 32)*(MaxValue - MinValue - VisibleSize)/(float) ScrollHeight);
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

                if (UpButton != null)
                {
                    if (!UpButton.IsDisposed)
                        UpButton.Dispose();

                    UpButton = null;
                }

                if (DownButton != null)
                {
                    if (!DownButton.IsDisposed)
                        DownButton.Dispose();

                    DownButton = null;
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