using Client.Envir;
using Shared.Rendering;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
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

        public override Size Size
        {
            get => base.Size;
            set
            {
                if (TextBox != null && !TextBox.Multiline)
                    value.Height = Math.Max(1, (int)Math.Ceiling(TextBox.PreferredHeight / (CEnvir.Target?.TextRasterScale ?? 1F)));
                base.Size = value;
            }
        }

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

            UpdateNativeFont();
            Size = Size;
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

            SynchronizeNativeBounds();
        }
        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            if (TextBox == null) return;

            SynchronizeNativeBounds();
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
                AutoSize = false,
                Parent = CEnvir.Target,
                BackColor = Color.Black,
                ForeColor = Color.White,
            };


            Border = true;
            BorderColour = Constants.PrimaryColour;


            Font = new Font(Config.FontName, CEnvir.FontSize(10F));
            Editable = true;
        }

        #region Methods

        private RenderTexture _textBoxTextureHandle;
        private Font _nativeFont;
        private Font _nativeSourceFont;
        private float _nativeFontScale;

        private void UpdateNativeFont()
        {
            if (TextBox == null || Font == null) return;
            float scale = CEnvir.Target?.TextRasterScale ?? 1F;
            if (_nativeSourceFont == Font && _nativeFontScale == scale) return;
            Font previous = _nativeFont;
            _nativeFont = RenderingPipelineManager.CreatePixelFont(Font, scale);
            _nativeSourceFont = Font;
            _nativeFontScale = scale;
            TextBox.Font = _nativeFont;
            previous?.Dispose();
        }

        private void SynchronizeNativeBounds()
        {
            if (TextBox == null || CEnvir.Target == null) return;
            UpdateNativeFont();
            float scale = CEnvir.Target.TextRasterScale;
            int left = (int)Math.Round(DisplayArea.Left * scale);
            int top = (int)Math.Round(DisplayArea.Top * scale);
            int right = Border
                ? (int)Math.Floor(DisplayArea.Right * scale)
                : (int)Math.Round(DisplayArea.Right * scale);
            int bottom = Border
                ? (int)Math.Floor(DisplayArea.Bottom * scale)
                : (int)Math.Round(DisplayArea.Bottom * scale);

            // A live WinForms text box is composited above the rendered UI. Keep its trailing
            // edges inside a bordered DXTextBox so it cannot cover the border at fractional DPI.
            Rectangle bounds = Rectangle.FromLTRB(
                left,
                top,
                Math.Max(left + 1, right),
                Math.Max(top + 1, bottom));
            if (TextBox.Bounds != bounds)
                TextBox.Bounds = bounds;
        }

        private int NativeMouseLocation(MouseEventArgs e)
        {
            float scale = CEnvir.Target?.TextRasterScale ?? 1F;
            int x = (int)Math.Round((e.X - DisplayArea.X) * scale);
            int y = (int)Math.Round((e.Y - DisplayArea.Y) * scale);
            return (x & 0xffff) | (y & 0xffff) << 16;
        }

        protected override void CreateTexture()
        {
            SynchronizeNativeBounds();
            if (!ControlTexture.IsValid || TextBox.Size != TextureSize)
            {
                DisposeTexture();
                TextureSize = TextBox.Size;
                _textBoxTextureHandle = RenderingPipelineManager.CreateTexture(TextureSize, RenderTextureFormat.A8R8G8B8, RenderTextureUsage.None, RenderTexturePool.Managed);

                ControlTexture = _textBoxTextureHandle;
                RenderingPipelineManager.RegisterControlCache(this);
            }

            using (TextureLock textureLock = RenderingPipelineManager.LockTexture(_textBoxTextureHandle, TextureLockMode.Discard))
            using (Bitmap image = new Bitmap(TextureSize.Width, TextureSize.Height, textureLock.Pitch, PixelFormat.Format32bppArgb, textureLock.DataPointer))
            {
                TextBox.DrawToBitmap(image, new Rectangle(Point.Empty, TextureSize));
            }

            TextureValid = true;
            ExpireTime = CEnvir.Now + Config.CacheDuration;
        }
        public override void DisposeTexture()
        {
            if (_textBoxTextureHandle.IsValid)
            {
                RenderingPipelineManager.ReleaseTexture(_textBoxTextureHandle);
                _textBoxTextureHandle = default;
            }

            base.DisposeTexture();
        }
        public virtual void OnActivated()
        {
            SynchronizeNativeBounds();
            if (TextBox.Visible != Editable)
                TextBox.Visible = Editable;

            if (TextBox.Visible && CEnvir.Target.ActiveControl != TextBox)
                CEnvir.Target.ActiveControl = TextBox;
        }
        public virtual void OnDeactivated()
        {
            if (TextBox.Visible)
            {
                TextureValid = false;
                InvalidateParentChildCache();
            }

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
            Rectangle area = new Rectangle(Location, Size);

            if (Parent != null)
                area.Offset(Parent.DisplayArea.Location);

            DisplayArea = area;
        }


        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!TextBox.Visible) return;

            int location = NativeMouseLocation(e);

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

            int location = NativeMouseLocation(e);


            SendMessage(TextBox.Handle, 0x200, e.Clicks, location);
        }
        public override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (!TextBox.Visible) return;


            int location = NativeMouseLocation(e);

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
            SynchronizeNativeBounds();
            if (TextBox.Visible) return;
            if (!DrawTexture)
            {
                return;
            }

            if (!TextureValid)
            {
                CreateTexture();
            }

            float oldOpacity = RenderingPipelineManager.GetOpacity();

            RenderingPipelineManager.SetOpacity(Opacity);

            Rectangle clipped = Rectangle.Intersect(DisplayArea, ClipArea);
            if (clipped.Width > 0 && clipped.Height > 0)
            {
                float sx = TextureSize.Width / (float)DisplayArea.Width;
                float sy = TextureSize.Height / (float)DisplayArea.Height;
                Rectangle source = Rectangle.FromLTRB(
                    (int)Math.Round((clipped.Left - DisplayArea.Left) * sx),
                    (int)Math.Round((clipped.Top - DisplayArea.Top) * sy),
                    (int)Math.Round((clipped.Right - DisplayArea.Left) * sx),
                    (int)Math.Round((clipped.Bottom - DisplayArea.Top) * sy));
                RenderingPipelineManager.DrawDpiText(ControlTexture, source, clipped, DisplayArea.Location, false,
                    IsEnabled ? Color.White : Color.FromArgb(75, 75, 75));
            }

            RenderingPipelineManager.SetOpacity(oldOpacity);

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
                _nativeFont?.Dispose();
                _nativeFont = null;
                _nativeSourceFont = null;

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
            private string _SuggestionText = string.Empty;
            #endregion

            public MirTextBox(DXTextBox owner)
            {
                Owner = owner;
            }

            #region Methods

            public void SetSuggestion(string value)
            {
                value ??= string.Empty;
                if (_SuggestionText == value) return;

                _SuggestionText = value;
                Invalidate();
            }

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
                if (e.Handled) return;

                CEnvir.Shift = e.Shift;
                CEnvir.Alt = e.Alt;
                CEnvir.Ctrl = e.Control;

                if (e.Alt && e.KeyCode == Keys.Enter)
                {
                    RenderingPipelineManager.ToggleFullScreen();
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

                if (!string.IsNullOrEmpty(_SuggestionText))
                    Invalidate();

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
                if (e.IsInputKey) return;

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
                Owner.InvalidateParentChildCache();
            }

            protected override void OnMouseUp(MouseEventArgs mevent)
            {
                base.OnMouseUp(mevent);

                if (!string.IsNullOrEmpty(_SuggestionText))
                    Invalidate();
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                const int WM_PAINT = 0x000F;
                if (m.Msg != WM_PAINT || string.IsNullOrEmpty(_SuggestionText) ||
                    SelectionLength != 0 || SelectionStart != TextLength) return;

                const int EM_POSFROMCHAR = 0x00D6;
                long position = SendMessage(Handle, EM_POSFROMCHAR, TextLength, 0).ToInt64();
                int x = (short)(position & 0xFFFF);
                int y = (short)((position >> 16) & 0xFFFF);

                if ((x < 0 || y < 0) && TextLength > 0)
                {
                    position = SendMessage(Handle, EM_POSFROMCHAR, TextLength - 1, 0).ToInt64();
                    x = (short)(position & 0xFFFF);
                    y = (short)((position >> 16) & 0xFFFF);

                    if (x >= 0 && y >= 0)
                        x += TextRenderer.MeasureText(Text.Substring(TextLength - 1), Font, Size.Empty,
                            TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Width;
                }

                if (x < 0 || y < 0) return;

                using (Graphics graphics = CreateGraphics())
                {
                    TextRenderer.DrawText(graphics, _SuggestionText, Font, new Point(x + 1, y),
                        Color.Gray, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
                }
            }
            protected override void OnSizeChanged(EventArgs e)
            {
                base.OnSizeChanged(e);

                if (Owner == null) return;
                Owner.TextureValid = false;
            }

            #endregion

            #region IDisposable

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    _SuggestionText = null;
                    Owner = null;
                }
            }

            #endregion
        }
    }
}
