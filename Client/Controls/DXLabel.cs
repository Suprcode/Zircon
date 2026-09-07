using Client.Envir;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Font = System.Drawing.Font;

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

            // Measure the same pixel font used by the scaled label texture so
            // fractional-DPI hinting cannot clip an automatically sized label.
            float scale = CEnvir.Target?.TextRasterScale ?? 1F;
            if (scale > 1F)
            {
                using Font rasterFont = RenderingPipelineManager.CreatePixelFont(font, scale);
                Size rasterSize = RenderingPipelineManager.MeasureText(text, rasterFont);
                tempSize.Width = Math.Max(tempSize.Width, (int)Math.Ceiling(rasterSize.Width / scale));
                tempSize.Height = Math.Max(tempSize.Height, (int)Math.Ceiling(rasterSize.Height / scale));
            }

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
            float scale = CEnvir.Target?.TextRasterScale ?? 1F;
            if (scale > 1F)
            {
                using Font rasterFont = RenderingPipelineManager.CreatePixelFont(label.Font, scale);
                Size rasterSize = RenderingPipelineManager.MeasureText(label.Text, rasterFont,
                    new Size(Math.Max(1, (int)Math.Ceiling(width * scale)),
                        Math.Max(1, (int)Math.Ceiling(2000 * scale))), label.DrawFormat);
                tempSize.Width = Math.Max(tempSize.Width, (int)Math.Ceiling(rasterSize.Width / scale));
                tempSize.Height = Math.Max(tempSize.Height, (int)Math.Ceiling(rasterSize.Height / scale));
            }

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

        #region Gradient

        public bool Gradient
        {
            get => _Gradient;
            set
            {
                if (_Gradient == value) return;

                bool oldValue = _Gradient;
                _Gradient = value;

                OnGradientChanged(oldValue, value);
            }
        }
        private bool _Gradient;
        public event EventHandler<EventArgs> GradientChanged;
        public virtual void OnGradientChanged(bool oValue, bool nValue)
        {
            TextureValid = false;

            GradientChanged?.Invoke(this, EventArgs.Empty);
        }

        public Color GradientTopColour
        {
            get => _GradientTopColour;
            set
            {
                if (_GradientTopColour == value) return;

                Color oldValue = _GradientTopColour;
                _GradientTopColour = value;

                OnGradientTopColourChanged(oldValue, value);
            }
        }
        private Color _GradientTopColour;
        public event EventHandler<EventArgs> GradientTopColourChanged;
        public virtual void OnGradientTopColourChanged(Color oValue, Color nValue)
        {
            TextureValid = false;

            GradientTopColourChanged?.Invoke(this, EventArgs.Empty);
        }

        public Color GradientBottomColour
        {
            get => _GradientBottomColour;
            set
            {
                if (_GradientBottomColour == value) return;

                Color oldValue = _GradientBottomColour;
                _GradientBottomColour = value;

                OnGradientBottomColourChanged(oldValue, value);
            }
        }
        private Color _GradientBottomColour;
        public event EventHandler<EventArgs> GradientBottomColourChanged;
        public virtual void OnGradientBottomColourChanged(Color oValue, Color nValue)
        {
            TextureValid = false;

            GradientBottomColourChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region AlignRight

        public bool AlignRight
        {
            get => _AlignRight;
            set
            {
                if (_AlignRight == value) return;

                _AlignRight = value;
                TextureValid = false;
            }
        }
        private bool _AlignRight;

        #endregion

        #region SeparateBackground

        public bool SeparateBackground
        {
            get => _SeparateBackground;
            set
            {
                if (_SeparateBackground == value) return;

                bool oldValue = _SeparateBackground;
                _SeparateBackground = value;

                OnSeparateBackgroundChanged(oldValue, value);
            }
        }
        private bool _SeparateBackground;
        public event EventHandler<EventArgs> SeparateBackgroundChanged;
        public virtual void OnSeparateBackgroundChanged(bool oValue, bool nValue)
        {
            TextureValid = false;

            SeparateBackgroundChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region LabelStyle

        public DXLabelStyle LabelStyle
        {
            get => _LabelStyle;
            set
            {
                if (_LabelStyle == value) return;

                DXLabelStyle oldValue = _LabelStyle;
                _LabelStyle = value;

                OnLabelStyleChanged(oldValue, value);
            }
        }
        private DXLabelStyle _LabelStyle;
        public event EventHandler<EventArgs> LabelStyleChanged;
        public virtual void OnLabelStyleChanged(DXLabelStyle oValue, DXLabelStyle nValue)
        {
            UpdateLabelStyle();

            LabelStyleChanged?.Invoke(this, EventArgs.Empty);
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
            DrawFormat = TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix;

            Outline = true;
            ForeColour = Constants.PrimaryColour;
            OutlineColour = Color.Black;
            GradientTopColour = Color.Empty;
            GradientBottomColour = Color.Empty;
        }

        #region Methods
        private void CreateSize()
        {
            if (!AutoSize) return;

            Size = GetSize(Text, Font, Outline, PaddingBottom);
        }

        private void UpdateLabelStyle()
        {
            switch (LabelStyle)
            {
                case DXLabelStyle.Title:
                    Outline = true;
                    OutlineColour = Color.Black;
                    Gradient = true;
                    GradientTopColour = Color.FromArgb(255, 226, 113);
                    GradientBottomColour = Color.FromArgb(226, 171, 55);
                    break;
                case DXLabelStyle.GameStoreTopRank:
                    Outline = true;
                    OutlineColour = Color.Black;
                    Gradient = true;
                    GradientTopColour = Color.FromArgb(245, 248, 255);
                    GradientBottomColour = Color.FromArgb(151, 184, 255);
                    break;
                default:
                    Gradient = false;
                    GradientTopColour = Color.Empty;
                    GradientBottomColour = Color.Empty;
                    break;
            }
        }

        private RenderTexture _labelTextureHandle;
        private float _rasterScale;
        private float RasterScale => CEnvir.Target?.TextRasterScale ?? 1F;

        protected override void CreateTexture()
        {
            float textureScale = RasterScale;

            // Auto-sized bounds must be measured for the same raster scale as the texture.
            // Reusing bounds measured at another fractional scale can clip hinted glyphs.
            if (_rasterScale != textureScale && AutoSize)
                CreateSize();

            _rasterScale = textureScale;
            using Font rasterFont = RenderingPipelineManager.CreatePixelFont(Font, textureScale);
            int width = Math.Max(1, (int)Math.Ceiling(DisplayArea.Width * textureScale));
            int height = Math.Max(1, (int)Math.Ceiling(DisplayArea.Height * textureScale));
            Size requiredTextureSize = new Size(width, height);

            if (!ControlTexture.IsValid || requiredTextureSize != TextureSize)
            {
                DisposeTexture();
                TextureSize = requiredTextureSize;
                _labelTextureHandle = RenderingPipelineManager.CreateTexture(TextureSize, RenderTextureFormat.A8R8G8B8, RenderTextureUsage.None, RenderTexturePool.Managed);

                ControlTexture = _labelTextureHandle;
                RenderingPipelineManager.RegisterControlCache(this);
            }

            using (TextureLock textureLock = RenderingPipelineManager.LockTexture(_labelTextureHandle, TextureLockMode.Discard))
            using (Bitmap image = new Bitmap(width, height, textureLock.Pitch, PixelFormat.Format32bppArgb, textureLock.DataPointer))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                RenderingPipelineManager.ConfigureGraphics(graphics);
                graphics.Clear(SeparateBackground ? Color.Transparent : BackColour);

                int outlineOffset = Math.Max(1, (int)Math.Floor(textureScale));
                int outlineFarOffset = outlineOffset * 2;

                if (Gradient)
                {
                    DrawGradientText(graphics, width, height, outlineOffset, outlineFarOffset, rasterFont);
                }
                else if (Outline)
                {
                    if (AlignRight)
                    {
                        TextFormatFlags format = (DrawFormat & ~(TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis | TextFormatFlags.HorizontalCenter)) |
                                                 TextFormatFlags.Right | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding;
                        TextRenderer.DrawText(graphics, Text, rasterFont, new Rectangle(0, 0, width - outlineOffset, height), OutlineColour, format);
                        TextRenderer.DrawText(graphics, Text, rasterFont, new Rectangle(0, outlineOffset, width - outlineFarOffset, height), OutlineColour, format);
                        TextRenderer.DrawText(graphics, Text, rasterFont, new Rectangle(0, outlineOffset, width, height), OutlineColour, format);
                        TextRenderer.DrawText(graphics, Text, rasterFont, new Rectangle(0, outlineFarOffset, width - outlineOffset, height), OutlineColour, format);
                        TextRenderer.DrawText(graphics, Text, rasterFont, new Rectangle(0, outlineOffset, width - outlineOffset, height), ForeColour, format);
                    }
                    else
                    {
                        TextRenderer.DrawText(graphics, Text, rasterFont, new Rectangle(outlineOffset, 0, width, height), OutlineColour, DrawFormat);
                        TextRenderer.DrawText(graphics, Text, rasterFont, new Rectangle(0, outlineOffset, width, height), OutlineColour, DrawFormat);
                        TextRenderer.DrawText(graphics, Text, rasterFont, new Rectangle(outlineFarOffset, outlineOffset, width, height), OutlineColour, DrawFormat);
                        TextRenderer.DrawText(graphics, Text, rasterFont, new Rectangle(outlineOffset, outlineFarOffset, width, height), OutlineColour, DrawFormat);
                        TextRenderer.DrawText(graphics, Text, rasterFont, new Rectangle(outlineOffset, outlineOffset, width, height), ForeColour, DrawFormat);
                    }
                }
                else
                {
                    TextFormatFlags format = AlignRight
                        ? (DrawFormat & ~(TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis | TextFormatFlags.HorizontalCenter)) |
                          TextFormatFlags.Right | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding
                        : DrawFormat;
                    Rectangle bounds = AlignRight
                        ? new Rectangle(0, 0, width, height)
                        : new Rectangle(outlineOffset, 0, width, height);
                    TextRenderer.DrawText(graphics, Text, rasterFont, bounds, ForeColour, format);
                }
            }
            TextureValid = true;
            ExpireTime = CEnvir.Now + Config.CacheDuration;
        }

        private void DrawGradientText(Graphics graphics, int width, int height, int outlineOffset, int outlineFarOffset, Font rasterFont)
        {
            if (string.IsNullOrEmpty(Text) || width <= 0 || height <= 0) return;

            Color topColour = GradientTopColour.IsEmpty ? ForeColour : GradientTopColour;
            Color bottomColour = GradientBottomColour.IsEmpty ? ForeColour : GradientBottomColour;

            if (Outline)
            {
                TextRenderer.DrawText(graphics, Text, rasterFont, new Rectangle(outlineOffset, 0, width, height), OutlineColour, DrawFormat);
                TextRenderer.DrawText(graphics, Text, rasterFont, new Rectangle(0, outlineOffset, width, height), OutlineColour, DrawFormat);
                TextRenderer.DrawText(graphics, Text, rasterFont, new Rectangle(outlineFarOffset, outlineOffset, width, height), OutlineColour, DrawFormat);
                TextRenderer.DrawText(graphics, Text, rasterFont, new Rectangle(outlineOffset, outlineFarOffset, width, height), OutlineColour, DrawFormat);
            }

            using (Bitmap textMask = new Bitmap(width, height, PixelFormat.Format32bppArgb))
            using (Graphics maskGraphics = Graphics.FromImage(textMask))
            {
                RenderingPipelineManager.ConfigureGraphics(maskGraphics);
                maskGraphics.Clear(Color.Transparent);

                Rectangle textBounds = Outline
                    ? new Rectangle(outlineOffset, outlineOffset, width, height)
                    : new Rectangle(outlineOffset, 0, width, height);

                TextRenderer.DrawText(maskGraphics, Text, rasterFont, textBounds, Color.White, DrawFormat);
                ApplyGradientToMask(textMask, topColour, bottomColour);

                graphics.DrawImageUnscaled(textMask, 0, 0);
            }
        }

        private static void ApplyGradientToMask(Bitmap textMask, Color topColour, Color bottomColour)
        {
            Rectangle bounds = new Rectangle(Point.Empty, textMask.Size);
            BitmapData data = textMask.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            try
            {
                int byteCount = Math.Abs(data.Stride) * data.Height;
                byte[] pixels = new byte[byteCount];
                Marshal.Copy(data.Scan0, pixels, 0, byteCount);

                int gradientHeight = Math.Max(1, data.Height - 1);

                for (int y = 0; y < data.Height; y++)
                {
                    float amount = y / (float)gradientHeight;
                    byte red = (byte)(topColour.R + (bottomColour.R - topColour.R) * amount);
                    byte green = (byte)(topColour.G + (bottomColour.G - topColour.G) * amount);
                    byte blue = (byte)(topColour.B + (bottomColour.B - topColour.B) * amount);

                    int row = y * data.Stride;

                    for (int x = 0; x < data.Width; x++)
                    {
                        int index = row + x * 4;
                        byte alpha = pixels[index + 3];

                        if (alpha == 0) continue;

                        pixels[index] = blue;
                        pixels[index + 1] = green;
                        pixels[index + 2] = red;
                    }
                }

                Marshal.Copy(pixels, 0, data.Scan0, byteCount);
            }
            finally
            {
                textMask.UnlockBits(data);
            }
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

            if (!TextureValid || _rasterScale != RasterScale)
            {
                CreateTexture();
            }

            float oldOpacity = RenderingPipelineManager.GetOpacity();

            RenderingPipelineManager.SetOpacity(Opacity);

            Rectangle clippedArea = Rectangle.Intersect(ClipArea, DisplayArea);
            if (clippedArea.Width > 0 && clippedArea.Height > 0)
            {
                Color colour = IsEnabled ? Color.White : Color.FromArgb(75, 75, 75);

                if (SeparateBackground && BackColour.A > 0)
                {
                    Color background = Color.FromArgb(
                        BackColour.A,
                        BackColour.R * colour.R / 255,
                        BackColour.G * colour.G / 255,
                        BackColour.B * colour.B / 255);

                    RenderingPipelineManager.FillRectangle(clippedArea, background);
                }

                // Use the same nearest physical-pixel boundary as the destination alignment.
                // Mixing floor/ceiling here with a rounded destination shifts the first
                // partially clipped line by one source pixel at fractional DPI scales.
                int sourceLeft = (int)Math.Round((clippedArea.Left - DisplayArea.Left) * _rasterScale);
                int sourceTop = (int)Math.Round((clippedArea.Top - DisplayArea.Top) * _rasterScale);
                int sourceRight = (int)Math.Round((clippedArea.Right - DisplayArea.Left) * _rasterScale);
                int sourceBottom = (int)Math.Round((clippedArea.Bottom - DisplayArea.Top) * _rasterScale);
                Rectangle source = Rectangle.FromLTRB(sourceLeft, sourceTop, Math.Min(TextureSize.Width, sourceRight), Math.Min(TextureSize.Height, sourceBottom));

                // Keep every label in a movable UI subtree on the same physical-pixel grid.
                DXControl alignmentRoot = this;
                while (alignmentRoot.Parent != null && alignmentRoot.Parent != ActiveScene)
                    alignmentRoot = alignmentRoot.Parent;

                Point alignmentOrigin = alignmentRoot.DisplayArea.Location;
                RenderingPipelineManager.DrawDpiText(ControlTexture, source, clippedArea, alignmentOrigin, AlignRight,
                    colour);
            }

            RenderingPipelineManager.SetOpacity(oldOpacity);

            ExpireTime = CEnvir.Now + Config.CacheDuration;
        }

        internal void DrawTextureTo(RectangleF destination)
        {
            if (!DrawTexture) return;

            if (!TextureValid)
                CreateTexture();

            if (!ControlTexture.IsValid || TextureSize.Width <= 0 || TextureSize.Height <= 0)
                return;

            RenderingPipelineManager.DrawTexture(ControlTexture, new Rectangle(Point.Empty, TextureSize), destination, IsEnabled ? Color.White : Color.FromArgb(75, 75, 75));

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
                _AlignRight = false;
                _SeparateBackground = false;
                _Gradient = false;
                _LabelStyle = DXLabelStyle.None;
                _GradientTopColour = Color.Empty;
                _GradientBottomColour = Color.Empty;
                _OutlineColour = Color.Empty;

                AutoSizeChanged = null;
                DrawFormatChanged = null;
                FontChanged = null;
                OutlineChanged = null;
                SeparateBackgroundChanged = null;
                GradientChanged = null;
                LabelStyleChanged = null;
                GradientTopColourChanged = null;
                GradientBottomColourChanged = null;
                OutlineColourChanged = null;
            }
        }
        #endregion
    }

    public enum DXLabelStyle
    {
        None,
        Title,
        GameStoreTopRank
    }
}
