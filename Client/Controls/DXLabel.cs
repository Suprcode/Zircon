using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Client.Envir;
using SlimDX;
using SlimDX.Direct3D9;
using Font = System.Drawing.Font;

//Cleaned
namespace Client.Controls
{
    public class DXLabel : DXControl
    {
        #region Static
        public static Size GetSize(string text, Font font, bool outline)
        {
            if (string.IsNullOrEmpty(text))
                return Size.Empty;
            
            Size tempSize = TextRenderer.MeasureText(DXManager.Graphics, text, font);

            if (outline && tempSize.Width > 0 && tempSize.Height > 0)
            {
                tempSize.Width += 2;
                tempSize.Height += 2;
            }

            return tempSize;
        }
        public static Size GetHeight(DXLabel label, int width)
        {
            Size tempSize = TextRenderer.MeasureText(DXManager.Graphics, label.Text, label.Font, new Size(width, 2000), label.DrawFormat);

            if (label.Outline && tempSize.Width > 0 && tempSize.Height > 0)
            {
                tempSize.Width += 2;
                tempSize.Height += 2;
            }

            return tempSize;
        }
        #endregion

        #region Properties

        #region AutoSize

        public bool AutoSize
        {
            get => _AutoSize;
            set
            {
                if (_AutoSize == value) return;

                bool oldValue = _AutoSize;
                _AutoSize = value;

                OnAutoSizeChanged(oldValue, value);
            }
        }
        private bool _AutoSize;
        public event EventHandler<EventArgs> AutoSizeChanged;
        public virtual void OnAutoSizeChanged(bool oValue, bool nValue)
        {
            TextureValid = false;
            CreateSize();

            AutoSizeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        
        #region DrawFormat

        public TextFormatFlags DrawFormat
        {
            get => _DrawFormat;
            set
            {
                if (_DrawFormat == value) return;

                TextFormatFlags oldValue = _DrawFormat;
                _DrawFormat = value;

                OnDrawFormatChanged(oldValue, value);
            }
        }
        private TextFormatFlags _DrawFormat;
        public event EventHandler<EventArgs> DrawFormatChanged;
        public virtual void OnDrawFormatChanged(TextFormatFlags oValue, TextFormatFlags nValue)
        {
            TextureValid = false;

            DrawFormatChanged?.Invoke(this, EventArgs.Empty);
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

            TextureValid = false;
            CreateSize();
        }

        #endregion

        #region Outline

        public bool Outline
        {
            get => _Outline;
            set
            {
                if (_Outline == value) return;

                bool oldValue = _Outline;
                _Outline = value;

                OnOutlineChanged(oldValue, value);
            }
        }
        private bool _Outline;
        public event EventHandler<EventArgs> OutlineChanged;
        public virtual void OnOutlineChanged(bool oValue, bool nValue)
        {
            TextureValid = false;
            CreateSize();

            OutlineChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region DropShadow

        public bool DropShadow
        {
            get => _DropShadow;
            set
            {
                if (_DropShadow == value) return;

                bool oldValue = _DropShadow;
                _DropShadow = value;

                OnDropShadowChanged(oldValue, value);
            }
        }
        private bool _DropShadow;
        public event EventHandler<EventArgs> DropShadowChanged;
        public virtual void OnDropShadowChanged(bool oValue, bool nValue)
        {
            TextureValid = false;
            CreateSize();

            DropShadowChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region OutlineColour

        public Color OutlineColour
        {
            get => _OutlineColour;
            set
            {
                if (_OutlineColour == value) return;

                Color oldValue = _OutlineColour;
                _OutlineColour = value;

                OnOutlineColourChanged(oldValue, value);
            }
        }
        private Color _OutlineColour;
        public event EventHandler<EventArgs> OutlineColourChanged;
        public virtual void OnOutlineColourChanged(Color oValue, Color nValue)
        {
            TextureValid = false;

            OutlineColourChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public override void OnTextChanged(string oValue, string nValue)
        {
            base.OnTextChanged(oValue, nValue);

            TextureValid = false;
            CreateSize();
        }
        public override void OnForeColourChanged(Color oValue, Color nValue)
        {
            base.OnForeColourChanged(oValue, nValue);

            TextureValid = false;
        }
        #endregion

        public DXLabel()
        {
            BackColour = Color.Empty;
            DrawTexture = true;
            AutoSize = true;
            Font = new Font(Config.FontName, CEnvir.FontSize(8F));
            DrawFormat = TextFormatFlags.WordBreak;
            
            Outline = true;
            ForeColour = Color.FromArgb(198, 166, 99);
            OutlineColour = Color.Black;
        }

        #region Methods
        private void CreateSize()
        {
            if (!AutoSize) return;

            Size = GetSize(Text, Font, Outline);
        }

        protected override void CreateTexture()
        {
            int width = DisplayArea.Width;
            int height = DisplayArea.Height;

            if (ControlTexture == null || DisplayArea.Size != TextureSize)
            {
                DisposeTexture();
                TextureSize = DisplayArea.Size;
                ControlTexture = new Texture(DXManager.Device, TextureSize.Width, TextureSize.Height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
                DXManager.ControlList.Add(this);
            }
            
            DataRectangle rect = ControlTexture.LockRectangle(0, LockFlags.Discard);

            using (Bitmap image = new Bitmap(width, height, width*4, PixelFormat.Format32bppArgb, rect.Data.DataPointer))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                DXManager.ConfigureGraphics(graphics);
                graphics.Clear(BackColour);

                if (Outline)
                {
                    TextRenderer.DrawText(graphics, Text, Font, new Rectangle(1, 0, width, height), OutlineColour, DrawFormat);
                    TextRenderer.DrawText(graphics, Text, Font, new Rectangle(0, 1, width, height), OutlineColour, DrawFormat);
                    TextRenderer.DrawText(graphics, Text, Font, new Rectangle(2, 1, width, height), OutlineColour, DrawFormat);
                    TextRenderer.DrawText(graphics, Text, Font, new Rectangle(1, 2, width, height), OutlineColour, DrawFormat);
                    TextRenderer.DrawText(graphics, Text, Font, new Rectangle(1, 1, width, height), ForeColour, DrawFormat);
                }
                else
                {
                    if (DropShadow)
                    {
                        TextRenderer.DrawText(graphics, Text, Font, new Rectangle(2, 1, width, height), Color.Black, DrawFormat);
                    }

                    TextRenderer.DrawText(graphics, Text, Font, new Rectangle(1, 0, width, height), ForeColour, DrawFormat);
                }
            }
            ControlTexture.UnlockRectangle(0);
            rect.Data.Dispose();
            
            TextureValid = true;
            ExpireTime = CEnvir.Now + Config.CacheDuration;
        }
        protected override void DrawControl()
        {
            if (!DrawTexture) return;

            if (!TextureValid) CreateTexture();

            DXManager.SetOpacity(Opacity);
            
            PresentTexture(ControlTexture, Parent, DisplayArea, IsEnabled ? Color.White : Color.FromArgb(75, 75, 75), this);

            ExpireTime = CEnvir.Now + Config.CacheDuration;
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _AutoSize = false;
                _DrawFormat = TextFormatFlags.Default;
                _Font?.Dispose();
                _Font = null;
                _Outline = false;
                _OutlineColour = Color.Empty;

                AutoSizeChanged = null;
                DrawFormatChanged = null;
                FontChanged = null;
                OutlineChanged = null;
                OutlineColourChanged = null;
            }
        }
        #endregion
    }
}
