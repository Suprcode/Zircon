using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Client.Envir;
using SlimDX;
using SlimDX.Direct3D9;
using Font = System.Drawing.Font;

//Cleaned
namespace Client.Controls
{
    public class DXTextBox : DXControl
    {
        #region Static

        public static DXTextBox ActiveTextBox
        {
            get => _ActiveTextBox;
            set
            {
                if (_ActiveTextBox == value) return;

                /*
                if (value == null)
                    if (_ActiveTextBox != null && _ActiveTextBox.KeepFocus) return;
                */

                var oldValue = _ActiveTextBox;
                _ActiveTextBox = value;

                oldValue?.OnDeactivated();
                _ActiveTextBox?.OnActivated();
            }
        }
        private static DXTextBox _ActiveTextBox;

        #endregion

        #region Properties

        #region Editable

        public bool Editable
        {
            get => _Editable;
            set
            {
                if (_Editable == value) return;

                bool oldValue = _Editable;
                _Editable = value;

                OnEditableChanged(oldValue, value);
            }
        }
        private bool _Editable;
        public event EventHandler<EventArgs> EditableChanged;
        public virtual void OnEditableChanged(bool oValue, bool nValue)
        {
            EditableChanged?.Invoke(this, EventArgs.Empty);

            CheckFocus();
        }

        #endregion
        
        #region Font

        public Font Font
        {
            get => _Font;
            set
            {
                if (_Font == value) return;

                Font oldValue = _Font;
                _Font = value;

                OnFontChanged(oldValue, value);
            }
        }
        private Font _Font;
        public event EventHandler<EventArgs> FontChanged;
        public virtual void OnFontChanged(Font oValue, Font nValue)
        {
            FontChanged?.Invoke(this, EventArgs.Empty);

            TextBox.Font = Font;
        }

        #endregion

        #region KeepFocus

        public bool KeepFocus
        {
            get => _KeepFocus;
            set
            {
                if (_KeepFocus == value) return;

                bool oldValue = _KeepFocus;
                _KeepFocus = value;

                OnKeepFocusChanged(oldValue, value);
            }
        }
        private bool _KeepFocus;
        public event EventHandler<EventArgs> KeepFocusChanged;
        public virtual void OnKeepFocusChanged(bool oValue, bool nValue)
        {
            KeepFocusChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region MaxLength

        public int MaxLength
        {
            get => _MaxLength;
            set
            {
                if (_MaxLength == value) return;

                int oldValue = _MaxLength;
                _MaxLength = value;

                OnMaxLengthChanged(oldValue, value);
            }
        }
        private int _MaxLength;
        public event EventHandler<EventArgs> MaxLengthChanged;
        public virtual void OnMaxLengthChanged(int oValue, int nValue)
        {
            MaxLengthChanged?.Invoke(this, EventArgs.Empty);

            TextBox.MaxLength = MaxLength;
        }

        #endregion
        
        #region Password

        public bool Password
        {
            get => _Password;
            set
            {
                if (_Password == value) return;

                bool oldValue = _Password;
                _Password = value;

                OnPasswordChanged(oldValue, value);
            }
        }
        private bool _Password;
        public event EventHandler<EventArgs> PasswordChanged;
        public virtual void OnPasswordChanged(bool oValue, bool nValue)
        {
            PasswordChanged?.Invoke(this, EventArgs.Empty);

            TextBox.UseSystemPasswordChar = Password;
        }

        #endregion

        #region ReadOnly

        public bool ReadOnly
        {
            get => _ReadOnly;
            set
            {
                if (_ReadOnly == value) return;

                bool oldValue = _ReadOnly;
                _ReadOnly = value;

                OnReadOnlyChanged(oldValue, value);
            }
        }
        private bool _ReadOnly;
        public event EventHandler<EventArgs> ReadOnlyChanged;
        public virtual void OnReadOnlyChanged(bool oValue, bool nValue)
        {
            ReadOnlyChanged?.Invoke(this, EventArgs.Empty);
            TextBox.ReadOnly = ReadOnly;
        }

        #endregion

        #region TextBox

        public MirTextBox TextBox
        {
            get => _TextBox;
            private set
            {
                if (_TextBox == value) return;

                MirTextBox oldValue = _TextBox;
                _TextBox = value;

                OnTextBoxChanged(oldValue, value);
            }
        }
        private MirTextBox _TextBox;
        public event EventHandler<EventArgs> TextBoxChanged;
        public virtual void OnTextBoxChanged(MirTextBox oValue, MirTextBox nValue)
        {
            TextBoxChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public override void OnBackColourChanged(Color oValue, Color nValue)
        {
            base.OnBackColourChanged(oValue, nValue);

            TextBox.BackColor = BackColour;
        }
        public override void OnForeColourChanged(Color oValue, Color nValue)
        {
            base.OnForeColourChanged(oValue, nValue);

            if (TextBox == null) return;

            TextBox.ForeColor = ForeColour;
        }
        public override void OnDisplayAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnDisplayAreaChanged(oValue, nValue);

            if (TextBox == null || !TextBox.Visible) return;

            TextBox.Location = DisplayArea.Location;
        }
        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            if (TextBox == null) return;

            TextBox.Size = Size;
        }
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            CheckFocus();
        }
        public override void OnIsEnabledChanged(bool oValue, bool nValue)
        {
            base.OnIsEnabledChanged(oValue, nValue);

            CheckFocus();
        }

        public MouseButtons Button;
        public DateTime ClickTime;

        public bool NeedFocus;
        #endregion

        public DXTextBox()
        {
            DrawTexture = true;

            TextBox = new MirTextBox(this)
            {
                Visible = false,
                BorderStyle = BorderStyle.None,
                Parent = CEnvir.Target,
                BackColor = Color.Black,
                ForeColor = Color.White,
            };


            Border = true;
            BorderColour = Color.FromArgb(198, 166, 99);


            Font = new Font(Config.FontName, CEnvir.FontSize(10F));
            Editable = true;
        }

        #region Methods

        protected override void CreateTexture()
        {
            if (ControlTexture == null || DisplayArea.Size != TextureSize)
            {
                DisposeTexture();
                TextureSize = DisplayArea.Size;
                ControlTexture = new Texture(DXManager.Device, TextureSize.Width, TextureSize.Height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
                DXManager.ControlList.Add(this);
            }

            DataRectangle rect = ControlTexture.LockRectangle(0, LockFlags.Discard);

            using (Bitmap image = new Bitmap(DisplayArea.Width, DisplayArea.Height, rect.Pitch, PixelFormat.Format32bppArgb, rect.Data.DataPointer))
                TextBox.DrawToBitmap(image, new Rectangle(Point.Empty, Size.Round(DisplayArea.Size)));

            ControlTexture.UnlockRectangle(0);
            rect.Data.Dispose();

            TextureValid = true;
            ExpireTime = CEnvir.Now + Config.CacheDuration;
        }
        
        public virtual void OnActivated()
        {
            if (TextBox.Visible != Editable)
                TextBox.Visible = Editable;

            if (TextBox.Location != DisplayArea.Location)
                TextBox.Location = DisplayArea.Location;

            if (TextBox.Visible && CEnvir.Target.ActiveControl != TextBox)
                CEnvir.Target.ActiveControl = TextBox;
        }
        public virtual void OnDeactivated()
        {
            if (TextBox.Visible)
                TextureValid = false;

            TextBox.Visible = false;
            CEnvir.Target.ActiveControl = null;
        }

        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            if (IsEnabled && Editable)
                CEnvir.Target.Cursor = Cursors.IBeam;
        }
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            CEnvir.Target.Cursor = Cursors.Default;
        }

        protected internal override void UpdateDisplayArea()
        {
            Rectangle area = new Rectangle(Location, TextBox.Size);

            if (Parent != null)
                area.Offset(Parent.DisplayArea.Location);

            DisplayArea = area;
        }


        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!TextBox.Visible ) return;

            int location = (e.X - DisplayArea.X) | (e.Y - DisplayArea.Y) << 16;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    SendMessage(TextBox.Handle, 0x201, e.Clicks, location);
                    break;
                case MouseButtons.Right:
                    SendMessage(TextBox.Handle, 0xA4, e.Clicks, location);
                    break;
            }
        }
        public override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (CEnvir.Target.ActiveControl == TextBox) return;

            int location = (e.X - DisplayArea.X) | (e.Y - DisplayArea.Y) << 16;


            SendMessage(TextBox.Handle, 0x200, e.Clicks, location);
        }
        public override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (!TextBox.Visible) return;


            int location = (e.X - DisplayArea.X) | (e.Y - DisplayArea.Y) << 16;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    SendMessage(TextBox.Handle, 0x202, e.Clicks, location);
                    break;
                case MouseButtons.Right:
                    SendMessage(TextBox.Handle, 0xA5, e.Clicks, location);
                    break;
            }
        }

        public bool CanFocus()
        {
            return IsVisible && Editable && IsEnabled;
        }
        public void SetFocus()
        {
            if (!CanFocus())
                NeedFocus = true;
            else
            {
                ActiveTextBox = this;
                TextBox.SelectAll();
            }
        }
        public void CheckFocus()
        {
            if (TextBox == null) return;

            if (CanFocus())
            {
                if (!NeedFocus) return;

                NeedFocus = false;
                SetFocus();

            }
            else if (ActiveTextBox == this)
                ActiveTextBox = null;
        }

        protected override void DrawControl()
        {
            if (!DrawTexture) return;

            if (!TextureValid) CreateTexture();

            float oldOpacity = DXManager.Opacity;

            DXManager.SetOpacity(Opacity);

            PresentTexture(ControlTexture, Parent, DisplayArea, IsEnabled ? Color.White : Color.FromArgb(75, 75, 75), this);

            DXManager.SetOpacity(oldOpacity);

            ExpireTime = CEnvir.Now + Config.CacheDuration;
        }
        #endregion
        
        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Editable = false;
                _Font?.Dispose();
                _Font = null;
                _KeepFocus = false;
                _MaxLength = 0;
                _Password = false;
                _ReadOnly = false;

                if (_TextBox != null)
                {
                    if (!_TextBox.IsDisposed)
                        _TextBox.Dispose();
                    _TextBox = null;
                }

                Button = MouseButtons.None;
                ClickTime = DateTime.MinValue;
                NeedFocus = false;

                EditableChanged = null;
                FontChanged = null;
                KeepFocusChanged = null;
                MaxLengthChanged = null;
                PasswordChanged = null; 
                ReadOnlyChanged = null;
                TextBoxChanged = null;
            }

            if (_ActiveTextBox == this) _ActiveTextBox = null;
        }
        #endregion

        public class MirTextBox : TextBox
        {
            #region Properties
            public DXTextBox Owner;
            #endregion
            
            public MirTextBox(DXTextBox owner)
            {
                Owner = owner;
            }

            #region Methods

            public void NextTextBox()
            {
                DXTextBox next = null;
                bool found = false;

                foreach (DXControl control in Owner.Parent.Controls)
                {
                    if (!(control is DXTextBox)) continue;

                    if (!found)
                    {
                        if (control == Owner)
                            found = true;
                        else if (next == null)
                        {
                            next = (DXTextBox)control;

                            if (!next.CanFocus())
                                next = null;
                        }

                        continue;
                    }

                    next = (DXTextBox)control;
                    break;
                }

                next?.SetFocus();
            }
            public void PreviousTextBox()
            {
                DXTextBox previous = null;
                bool found = false;

                for (int i = Owner.Parent.Controls.Count - 1; i >= 0; i--)
                {
                    DXControl control = Owner.Parent.Controls[i];
                    if (!(control is DXTextBox)) continue;

                    if (!found)
                    {
                        if (control == Owner)
                            found = true;
                        else if (previous == null)
                        {
                            previous = (DXTextBox)control;
                            if (!previous.CanFocus())
                                previous = null;
                        }

                        continue;
                    }

                    previous = (DXTextBox)control;
                    break;
                }

                previous?.SetFocus();
            }

            protected override void OnMouseClick(MouseEventArgs e)
            {
                base.OnMouseClick(e);

                if (Owner == null) return;

                if (e.Button == Owner.Button && Owner.ClickTime.AddMilliseconds(SystemInformation.DoubleClickTime) >= CEnvir.Now)
                    SelectAll();
                else
                {
                    Owner.Button = e.Button;
                    Owner.ClickTime = CEnvir.Now;
                }
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                base.OnKeyDown(e);

                if (Owner == null) return;

                CEnvir.Shift = e.Shift;
                CEnvir.Alt = e.Alt;
                CEnvir.Ctrl = e.Control;

                if (e.Alt && e.KeyCode == Keys.Enter)
                {
                    DXManager.ToggleFullScreen();
                    return;
                }

                switch (e.KeyCode)
                {
                    case Keys.F1:
                    case Keys.F2:
                    case Keys.F3:
                    case Keys.F4:
                    case Keys.F5:
                    case Keys.F6:
                    case Keys.F7:
                    case Keys.F8:
                    case Keys.F9:
                    case Keys.F10:
                    case Keys.F11:
                    case Keys.F12:
                    case Keys.Tab:
                    case Keys.Escape:

                        ActiveScene?.OnKeyDown(e);

                        //Program.Target.CMain_KeyUp(Program.This, e); <-- Was KeyUp
                        break;
                }
            }
            protected override void OnKeyUp(KeyEventArgs e)
            {
                base.OnKeyUp(e);

                if (Owner == null) return;

                CEnvir.Shift = e.Shift;
                CEnvir.Alt = e.Alt;
                CEnvir.Ctrl = e.Control;

                switch (e.KeyCode)
                {
                    case Keys.F1:
                    case Keys.F2:
                    case Keys.F3:
                    case Keys.F4:
                    case Keys.F5:
                    case Keys.F6:
                    case Keys.F7:
                    case Keys.F8:
                    case Keys.F9:
                    case Keys.F10:
                    case Keys.F11:
                    case Keys.F12:
                    case Keys.Tab:
                    case Keys.Escape:

                        ActiveScene?.OnKeyUp(e);

                        //Program.Target.CMain_KeyUp(Program.This, e);
                        break;
                }
            }
            protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
            {
                base.OnPreviewKeyDown(e);

                if (Owner?.Parent == null) return;

                if (e.KeyCode != Keys.Tab || AcceptsTab) return;

                e.IsInputKey = false;

                if (e.Shift)
                    PreviousTextBox();
                else
                    NextTextBox();
            }
            protected override void OnTextChanged(EventArgs e)
            {
                base.OnTextChanged(e);

                if (Owner == null) return;

                Owner.TextureValid = false;
            }
            protected override void OnSizeChanged(EventArgs e)
            {
                base.OnSizeChanged(e);

                if (Owner == null) return;
                Owner.Size = Size;

                Owner.TextureValid = false;
            }

            #endregion

            #region IDisposable

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    Owner = null;
                }
            }

            #endregion
        }
    }
}
