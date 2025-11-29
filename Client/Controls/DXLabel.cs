using Client.Envir;
using Client.Rendering;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Font = System.Drawing.Font;

//Cleaned
namespace Client.Controls
{
    public class DXLabel : DXControl
    {
        #region Static
        public static Size GetSize(string text, Font font, bool outline, int paddingBottom = 0)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Size.Empty;
            }

            Size tempSize = RenderingPipelineManager.MeasureText(text, font);

            if (outline && tempSize.Width > 0 && tempSize.Height > 0)
            {
                tempSize.Width += 2;
                tempSize.Height += 2;
            }

            tempSize.Height += paddingBottom;

            return tempSize;
        }
        public static Size GetHeight(DXLabel label, int width)
        {
            Size tempSize = RenderingPipelineManager.MeasureText(label.Text, label.Font, new Size(width, 2000), label.DrawFormat);

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


        #region PaddingBottom

        public int PaddingBottom
        {
            get => _PaddingBottom;
            set
            {
                if (_PaddingBottom == value) return;

                int oldValue = _PaddingBottom;
                _PaddingBottom = value;

                OnPaddingBottomChanged(oldValue, value);
            }
        }
        private int _PaddingBottom;
        public event EventHandler<EventArgs> PaddingBottomChanged;
        public virtual void OnPaddingBottomChanged(int oValue, int nValue)
        {
            TextureValid = false;
            CreateSize();

            PaddingBottomChanged?.Invoke(this, EventArgs.Empty);
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

            Size = GetSize(Text, Font, Outline, PaddingBottom);
        }

        private RenderTexture _labelTextureHandle;

        protected override void CreateTexture()
        {
            int width = DisplayArea.Width;
            int height = DisplayArea.Height;

            if (!ControlTexture.IsValid || DisplayArea.Size != TextureSize)
            {
                DisposeTexture();
                TextureSize = DisplayArea.Size;
                _labelTextureHandle = RenderingPipelineManager.CreateTexture(TextureSize, RenderTextureFormat.A8R8G8B8, RenderTextureUsage.None, RenderTexturePool.Managed);

                ControlTexture = _labelTextureHandle;
                RenderingPipelineManager.RegisterControlCache(this);
            }

            using (TextureLock textureLock = RenderingPipelineManager.LockTexture(_labelTextureHandle, TextureLockMode.Discard))
            using (Bitmap image = new Bitmap(width, height, textureLock.Pitch, PixelFormat.Format32bppArgb, textureLock.DataPointer))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                RenderingPipelineManager.ConfigureGraphics(graphics);
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
                    TextRenderer.DrawText(graphics, Text, Font, new Rectangle(1, 0, width, height), ForeColour, DrawFormat);
                }
            }
            TextureValid = true;
            ExpireTime = CEnvir.Now + Config.CacheDuration;
        }
        public override void DisposeTexture()
        {
            if (_labelTextureHandle.IsValid)
            {
                RenderingPipelineManager.ReleaseTexture(_labelTextureHandle);
                _labelTextureHandle = default;
            }

            base.DisposeTexture();
        }
        protected override void DrawControl()
        {
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

            bool applyGrayscale = !IsEnabled;

            if (applyGrayscale)
            {
                RenderingPipelineManager.EnableGrayscaleEffect();
            }

            PresentTexture(ControlTexture, Parent, DisplayArea, IsEnabled ? Color.White : Color.FromArgb(75, 75, 75), this);

            if (applyGrayscale)
            {
                RenderingPipelineManager.DisableSpriteShaderEffect();
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
