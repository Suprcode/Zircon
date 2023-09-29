using System;
using System.Drawing;
using System.Windows.Forms;
using Client.Envir;
using Library;
using SlimDX;
using SlimDX.Direct3D9;

namespace Client.Controls
{
    public class DXButton : DXImageControl
    {
        #region Properites

        #region HasFocus

        public bool HasFocus
        {
            get => _HasFocus;
            set
            {
                if (_HasFocus == value) return;

                bool oldValue = _HasFocus;
                _HasFocus = value;

                OnHasFocusChanged(oldValue, value);
            }
        }
        private bool _HasFocus;
        public event EventHandler<EventArgs> HasFocusChanged;
        public virtual void OnHasFocusChanged(bool oValue, bool nValue)
        {
            UpdateDisplayArea();
            HasFocusChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Pressed

        public bool Pressed
        {
            get => _Pressed;
            set
            {
                if (_Pressed == value) return;

                bool oldValue = _Pressed;
                _Pressed = value;

                OnPressedChanged(oldValue, value);
            }
        }
        private bool _Pressed;
        public event EventHandler<EventArgs> PressedChanged;
        public virtual void OnPressedChanged(bool oValue, bool nValue)
        {
            UpdateForeColour();

            PressedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region CanBePressed

        public bool CanBePressed
        {
            get => _CanBePressed;
            set
            {
                if (_CanBePressed == value) return;

                bool oldValue = _CanBePressed;
                _CanBePressed = value;

                OnCanBePressedChanged(oldValue, value);
            }
        }
        private bool _CanBePressed;
        public event EventHandler<EventArgs> CanBePressedChanged;
        public virtual void OnCanBePressedChanged(bool oValue, bool nValue)
        {
            CanBePressedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region RightAligned

        public bool RightAligned
        {
            get => _RightAligned;
            set
            {
                if (_RightAligned == value) return;

                bool oldValue = _RightAligned;
                _RightAligned = value;

                OnRightAlignedChanged(oldValue, value);
            }
        }
        private bool _RightAligned;
        public event EventHandler<EventArgs> RightAlignedChanged;
        public virtual void OnRightAlignedChanged(bool oValue, bool nValue)
        {
            RightAlignedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region ButtonType

        public ButtonType ButtonType
        {
            get => _ButtonType;
            set
            {
                if (_ButtonType == value) return;

                ButtonType oldValue = _ButtonType;
                _ButtonType = value;

                OnButtonTypeChanged(oldValue, value);
            }
        }
        private ButtonType _ButtonType;
        public event EventHandler<EventArgs> ButtonTypeChanged;
        public virtual void OnButtonTypeChanged(ButtonType oValue, ButtonType nValue)
        {
            if (Label == null) return;
            switch (nValue)
            {
                case ButtonType.SmallButton:
                    Label.Location = new Point(0, -1);
                    break;
                default:
                    Label.Location = new Point(0, 0);
                    break;
            }

            ButtonTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region HoverIndex

        public int HoverIndex
        {
            get => _HoverIndex;
            set
            {
                if (_HoverIndex == value) return;

                int oldValue = _HoverIndex;
                _HoverIndex = value;

                OnHoverIndexChanged(oldValue, value);
            }
        }
        private int _HoverIndex;
        public event EventHandler<EventArgs> HoverIndexChanged;
        public virtual void OnHoverIndexChanged(int oValue, int nValue)
        {
            TextureValid = false;
            UpdateDisplayArea();
            HoverIndexChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region PressedIndex

        public int PressedIndex
        {
            get => _PressedIndex;
            set
            {
                if (_PressedIndex == value) return;

                int oldValue = _PressedIndex;
                _PressedIndex = value;

                OnPressedIndexChanged(oldValue, value);
            }
        }
        private int _PressedIndex;
        public event EventHandler<EventArgs> PressedIndexChanged;
        public virtual void OnPressedIndexChanged(int oValue, int nValue)
        {
            TextureValid = false;
            UpdateDisplayArea();
            PressedIndexChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXLabel Label { get; private set; }

        public override void OnIsEnabledChanged(bool oValue, bool nValue)
        {
            base.OnIsEnabledChanged(oValue, nValue);

            UpdateForeColour();
            UpdateDisplayArea();
        }
        public override void OnDisplayAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnDisplayAreaChanged(oValue, nValue);

            if (Label == null) return;

            Label.Size = DisplayArea.Size;
        }
        public override void OnOpacityChanged(float oValue, float nValue)
        {
            base.OnOpacityChanged(oValue, nValue);

            if (Label == null) return;

            Label.Opacity = Opacity;
        }

        #endregion

        public DXButton()
        {
            ForeColour = Color.White;
            Sound = SoundIndex.ButtonA;
            CanBePressed = true;
            ForeColour = new Color4(0.85F, 0.85F, 0.85F).ToColor();

            Label = new DXLabel
            {
                Location = new Point(0, -1),
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false,
                Parent = this,
            };
        }

        #region Methods
        
        protected internal override void UpdateDisplayArea()
        {
            Rectangle area = new Rectangle(Location, Size);

            if (Parent != null)
                area.Offset(Parent.DisplayArea.Location);

            if (HasFocus && MouseControl == this && !Pressed && IsEnabled && CanBePressed) area.Y++;

            DisplayArea = area;
        }
        protected override void DrawMirTexture()
        {
            Texture texture = null;

            if (Library == null)
            {
                DXManager.SetOpacity(Opacity);
                
                Surface oldSurface = DXManager.CurrentSurface;
                DXManager.SetSurface(DXManager.ScratchSurface);
                DXManager.Device.Clear(ClearFlags.Target, 0, 0, 0);

                switch (ButtonType)
                {
                    case ButtonType.Default:
                        DrawDefault();
                        break;
                    case ButtonType.SelectedTab:
                        DrawSelectedTab();
                        break;
                    case ButtonType.DeselectedTab:
                        DrawDeselectedTab();
                        break;
                    case ButtonType.SmallButton:
                        DrawSmallButton();
                        break;
                    case ButtonType.AddButton:
                        InterfaceLibrary.Draw(241, 0, 0, Color.White, false, 1F, ImageType.Image);
                        break;
                    case ButtonType.RemoveButton:
                        InterfaceLibrary.Draw(242, 0, 0, Color.White, false, 1F, ImageType.Image);
                        break;
                }

                DXManager.SetSurface(oldSurface);

                texture = DXManager.ScratchTexture;
            }
            else
            {
                var index = Index;

                if (HoverIndex > 0 && MouseControl == this && IsEnabled && CanBePressed)
                    index = HoverIndex;

                if (PressedIndex > 0 && Pressed && IsEnabled)
                    index = PressedIndex;

                if (index > 0)
                {
                    MirImage image = Library.CreateImage(index, ImageType.Image);
                    texture = image.Image;
                    image.ExpireTime = CEnvir.Now + Config.CacheDuration;
                }
            }

            if (texture == null) return;

            bool oldBlend = DXManager.Blending;
            float oldRate = DXManager.BlendRate;

            if (Blend)
                DXManager.SetBlend(true, ImageOpacity, BlendMode);
            else
                DXManager.SetOpacity(Opacity);

            PresentTexture(texture, Parent, DisplayArea, ForeColour, this, 0, Pressed ? 1 : 0);
            
            if (Blend)
                DXManager.SetBlend(oldBlend, oldRate, BlendMode);
            else
                DXManager.SetOpacity(1F);

        }

        public override void OnFocus()
        {
            base.OnFocus();

            HasFocus = true;
        }
        public override void OnLostFocus()
        {
            base.OnFocus();

            HasFocus = false;
        }

        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            UpdateForeColour();
            UpdateDisplayArea();
        }
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            UpdateForeColour();
            UpdateDisplayArea();
        }

        public void UpdateForeColour()
        {
            if (!IsEnabled)
                ForeColour = new Color4(0.2F, 0.2F, 0.2F).ToColor();
            else
                ForeColour = MouseControl == this || Pressed ? new Color4(1F, 1F, 1F).ToColor() : new Color4(0.85F, 0.85F, 0.85F).ToColor();
        }

        private void DrawDefault()
        {
            Size s = InterfaceLibrary.GetSize(16);

            int x = s.Width;
            s = InterfaceLibrary.GetSize(18);
            InterfaceLibrary.Draw(18, x, 0, Color.White, new Rectangle(0, 0, Size.Width - x * 2, s.Height), 1f, ImageType.Image);

            InterfaceLibrary.Draw(16, 0, 0, Color.White, false, 1F, ImageType.Image);

            s = InterfaceLibrary.GetSize(17);
            InterfaceLibrary.Draw(17, Size.Width - s.Width, 0, Color.White, false, 1F, ImageType.Image);
        }
        private void DrawSelectedTab()
        {
            Size s = InterfaceLibrary.GetSize(56);
            InterfaceLibrary.Draw(56, 0, 0, Color.White, false, 1F, ImageType.Image);

            int x = s.Width;
            s = InterfaceLibrary.GetSize(58);
            InterfaceLibrary.Draw(58, x, 0, Color.White, new Rectangle(0, 0, Size.Width - x * 2, s.Height), 1f, ImageType.Image);

            s = InterfaceLibrary.GetSize(57);
            InterfaceLibrary.Draw(57, Size.Width - s.Width, 0, Color.White, false, 1F, ImageType.Image);
        }
        private void DrawDeselectedTab()
        {
            Size s = InterfaceLibrary.GetSize(53);
            InterfaceLibrary.Draw(53, 0, 0, Color.White, false, 1F, ImageType.Image);

            int x = s.Width;
            s = InterfaceLibrary.GetSize(55);
            InterfaceLibrary.Draw(55, x, 0, Color.White, new Rectangle(0, 0, Size.Width - x * 2, s.Height), 1f, ImageType.Image);


            s = InterfaceLibrary.GetSize(54);
            InterfaceLibrary.Draw(54, Size.Width - s.Width, 0, Color.White, false, 1F, ImageType.Image);
        }

        private void DrawSmallButton()
        {
            Size s = InterfaceLibrary.GetSize(41);
            InterfaceLibrary.Draw(41, 0, 0, Color.White, false, 1F, ImageType.Image);

            int x = s.Width;
            s = InterfaceLibrary.GetSize(43);
            InterfaceLibrary.Draw(43, x, 0, Color.White, new Rectangle(0, 0, Size.Width - x * 2, s.Height), 1f, ImageType.Image);


            s = InterfaceLibrary.GetSize(42);
            InterfaceLibrary.Draw(42, Size.Width - s.Width, 0, Color.White, false, 1F, ImageType.Image);
        }

        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _HasFocus = false;
                _Pressed = false;
                _CanBePressed = false;
                _RightAligned = false;
                _ButtonType = 0;

                _HoverIndex = 0;
                _PressedIndex = 0;

                if (Label != null)
                {
                    if (!Label.IsDisposed)
                        Label.Dispose();

                    Label = null;
                }

                HasFocusChanged = null;
                CanBePressedChanged = null;
                PressedChanged = null;
                RightAlignedChanged = null;
                ButtonTypeChanged = null;
                HoverIndexChanged = null;
                PressedIndexChanged = null;
            }
        }
        #endregion
    }

    public enum ButtonType
    {
        Default,
        SelectedTab,
        DeselectedTab,
        SmallButton,
        AddButton,
        RemoveButton
    }
}
