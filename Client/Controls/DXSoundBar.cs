using Client.Envir;
using Library;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Client.Controls
{
    public class DXSoundBar : DXControl
    {
        #region Constants
        private const int OuterBarIndex = 4743;
        private const int InnerBarIndex = 4742;

        private const int ButtonWidth = 16;
        private const int IconPadding = 4;
        #endregion

        #region Properties
        public DXImageControl SoundIcon;
        public DXImageControl OuterBar;
        public DXControl InnerBar;
        public DXButton SliderButton;

        public bool Muted
        {
            get => _Muted;
            set
            {
                if (_Muted == value) return;

                bool oldValue = _Muted;
                _Muted = value;

                OnMutedChanged(oldValue, value);
            }
        }
        private bool _Muted;
        public event EventHandler<EventArgs> MutedChanged;
        public virtual void OnMutedChanged(bool oValue, bool nValue)
        {
            UpdateSoundIcon();
            MutedChanged?.Invoke(this, EventArgs.Empty);
        }

        public int Value
        {
            get => _Value;
            set
            {
                value = Math.Max(0, Math.Min(100, value));

                if (_Value == value) return;

                int oldValue = _Value;
                _Value = value;

                OnValueChanged(oldValue, value);
            }
        }
        private int _Value;
        public event EventHandler<EventArgs> ValueChanged;
        public virtual void OnValueChanged(int oValue, int nValue)
        {
            UpdateButtonLocation();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXHScrollBar ScrollBar;

        public DXSoundBar()
        {
            Size = new Size(180, 18);

            OuterBar = new DXImageControl
            {
                LibraryFile = LibraryFile.GameInter,
                Index = OuterBarIndex,
                Parent = this,
                Location = new Point(ButtonWidth + IconPadding, 3)
            };

            InnerBar = new DXControl
            {
                Parent = OuterBar,
                Location = new Point(2, 2),
                Size = OuterBar.Library.GetSize(InnerBarIndex),
            };
            InnerBar.BeforeDraw += (o, e) =>
            {
                float percent = Math.Min(1, Math.Max(0, Value / 100f));

                if (percent == 0) return;

                MirImage image = OuterBar.Library.CreateImage(InnerBarIndex, ImageType.Image);

                if (image == null) return;

                PresentTexture(image.Image, OuterBar, new Rectangle(InnerBar.DisplayArea.X, InnerBar.DisplayArea.Y, (int)(image.Width * percent), image.Height), Color.White, InnerBar);
            };

            ScrollBar = new DXHScrollBar
            {
                Parent = this,
                BackColour = Color.Empty,
                Border = false,
                Size = new Size(195, 18),
                Location = new Point(5, 0),
                Value = 0,
                MinValue = 0,
                MaxValue = 100,
                Change = 1,
                LeftButton = { Visible = false, IsControl = false, },
                RightButton = { Visible = false, IsControl = false, },
                PositionBar = { Index = 4746, PressedIndex = 4745, HoverIndex = 4746, LibraryFile = LibraryFile.GameInter }
            };
            ScrollBar.ValueChanged += (o, e) => UpdateValue();

            SoundIcon = new DXImageControl
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 4741,
                Parent = this,
                Location = new Point(0, 1)
            };
            SoundIcon.MouseClick += (o, e) =>
            {
                if (e.Button != MouseButtons.Left) return;
                Muted = !Muted;
            };
        }

        public void UpdateValue()
        {
            Value = ScrollBar.Value;
        }

        private void UpdateButtonLocation()
        {
            ScrollBar.Value = Value;
        }

        private void UpdateSoundIcon()
        {
            if (Muted)
            {
                SoundIcon.Index = 4740;
            }
            else
            {
                SoundIcon.Index = 4741;
            }
        }
    }
}