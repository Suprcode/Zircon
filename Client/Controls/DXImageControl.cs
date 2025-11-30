using Client.Envir;
using Client.Rendering;
using Library;
using System;
using System.Drawing;

//Cleaned
namespace Client.Controls
{
    public class DXImageControl : DXControl
    {
        #region Properties

        #region Blend

        public bool Blend
        {
            get => _Blend;
            set
            {
                if (_Blend == value) return;

                bool oldValue = _Blend;
                _Blend = value;

                OnBlendChanged(oldValue, value);
            }
        }
        private bool _Blend;
        public event EventHandler<EventArgs> BlendChanged;
        public virtual void OnBlendChanged(bool oValue, bool nValue)
        {
            BlendChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region BlendMode

        public BlendMode BlendMode
        {
            get => _BlendMode;
            set
            {
                if (_BlendMode == value) return;

                BlendMode oldValue = _BlendMode;
                _BlendMode = value;

                OnBlendModeChanged(oldValue, value);
            }
        }
        private BlendMode _BlendMode = BlendMode.NORMAL;
        public event EventHandler<EventArgs> BlendModeChanged;
        public virtual void OnBlendModeChanged(BlendMode oValue, BlendMode nValue)
        {
            BlendModeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region DrawImage

        public bool DrawImage
        {
            get => _DrawImage;
            set
            {
                if (_DrawImage == value) return;

                bool oldValue = _DrawImage;
                _DrawImage = value;

                OnDrawImageChanged(oldValue, value);
            }
        }
        private bool _DrawImage;
        public event EventHandler<EventArgs> DrawImageChanged;
        public virtual void OnDrawImageChanged(bool oValue, bool nValue)
        {
            DrawImageChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region FixedSize

        public bool FixedSize
        {
            get => _FixedSize;
            set
            {
                if (_FixedSize == value) return;

                bool oldValue = _FixedSize;
                _FixedSize = value;

                OnFixedSizeChanged(oldValue, value);
            }
        }
        private bool _FixedSize;
        public event EventHandler<EventArgs> FixedSizeChanged;
        public virtual void OnFixedSizeChanged(bool oValue, bool nValue)
        {
            TextureValid = false;
            UpdateDisplayArea();
            FixedSizeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Scale

        #endregion


        #region ImageOpacity

        public float ImageOpacity
        {
            get => _ImageOpacity;
            set
            {
                if (_ImageOpacity == value) return;

                float oldValue = _ImageOpacity;
                _ImageOpacity = value;

                OnImageOpacityChanged(oldValue, value);
            }
        }
        private float _ImageOpacity;
        public event EventHandler<EventArgs> ImageOpacityChanged;
        public virtual void OnImageOpacityChanged(float oValue, float nValue)
        {
            ImageOpacityChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Index

        public int Index
        {
            get => _Index;
            set
            {
                if (_Index == value) return;

                int oldValue = _Index;
                _Index = value;

                OnIndexChanged(oldValue, value);
            }
        }
        private int _Index;
        public event EventHandler<EventArgs> IndexChanged;
        public virtual void OnIndexChanged(int oValue, int nValue)
        {
            TextureValid = false;
            UpdateDisplayArea();
            IndexChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region LibraryFile
        public MirLibrary Library;

        public LibraryFile LibraryFile
        {
            get => _LibraryFile;
            set
            {
                if (_LibraryFile == value) return;

                LibraryFile oldValue = _LibraryFile;
                _LibraryFile = value;

                OnLibraryFileChanged(oldValue, value);
            }
        }
        private LibraryFile _LibraryFile;
        public event EventHandler<EventArgs> LibraryFileChanged;
        public virtual void OnLibraryFileChanged(LibraryFile oValue, LibraryFile nValue)
        {
            CEnvir.LibraryList.TryGetValue(LibraryFile, out Library);

            TextureValid = false;
            UpdateDisplayArea();

            LibraryFileChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region PixelDetect

        public bool PixelDetect
        {
            get => _PixelDetect;
            set
            {
                if (_PixelDetect == value) return;

                bool oldValue = _PixelDetect;
                _PixelDetect = value;

                OnPixelDetectChanged(oldValue, value);
            }
        }
        private bool _PixelDetect;
        public event EventHandler<EventArgs> PixelDetectChanged;
        public virtual void OnPixelDetectChanged(bool oValue, bool nValue)
        {
            PixelDetectChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region UseOffSet

        public bool UseOffSet
        {
            get => _UseOffSet;
            set
            {
                if (_UseOffSet == value) return;

                bool oldValue = _UseOffSet;
                _UseOffSet = value;

                OnUseOffSetChanged(oldValue, value);
            }
        }
        private bool _UseOffSet;
        public event EventHandler<EventArgs> UseOffSetChanged;
        public virtual void OnUseOffSetChanged(bool oValue, bool nValue)
        {
            UpdateDisplayArea();
            UseOffSetChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public override Size Size
        {
            get
            {
                if (Library != null && Index >= 0 && !FixedSize)
                    return Library.GetSize(Index);

                return base.Size;
            }
            set => base.Size = value;
        }

        #endregion

        public bool IntersectParent;

        #region Drop Shadow

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
            DropShadowChanged?.Invoke(this, EventArgs.Empty);
        }

        public Color DropShadowColour
        {
            get => _DropShadowColour;
            set
            {
                if (_DropShadowColour == value) return;

                Color oldValue = _DropShadowColour;
                _DropShadowColour = value;

                OnDropShadowColourChanged(oldValue, value);
            }
        }
        private Color _DropShadowColour = Color.Black;
        public event EventHandler<EventArgs> DropShadowColourChanged;
        public virtual void OnDropShadowColourChanged(Color oValue, Color nValue)
        {
            DropShadowColourChanged?.Invoke(this, EventArgs.Empty);
        }

        public float DropShadowWidth
        {
            get => _DropShadowWidth;
            set
            {
                if (Math.Abs(_DropShadowWidth - value) < float.Epsilon) return;

                float oldValue = _DropShadowWidth;
                _DropShadowWidth = value;

                OnDropShadowWidthChanged(oldValue, value);
            }
        }
        private float _DropShadowWidth = 5f;
        public event EventHandler<EventArgs> DropShadowWidthChanged;
        public virtual void OnDropShadowWidthChanged(float oValue, float nValue)
        {
            DropShadowWidthChanged?.Invoke(this, EventArgs.Empty);
        }

        public float DropShadowOpacity
        {
            get => _DropShadowOpacity;
            set
            {
                if (Math.Abs(_DropShadowOpacity - value) < float.Epsilon) return;

                float oldValue = _DropShadowOpacity;
                _DropShadowOpacity = value;

                OnDropShadowOpacityChanged(oldValue, value);
            }
        }
        private float _DropShadowOpacity = 0.5f;
        public event EventHandler<EventArgs> DropShadowOpacityChanged;
        public virtual void OnDropShadowOpacityChanged(float oValue, float nValue)
        {
            DropShadowOpacityChanged?.Invoke(this, EventArgs.Empty);
        }

        public float DropShadowOpacityExponent
        {
            get => _DropShadowOpacityExponent;
            set
            {
                if (Math.Abs(_DropShadowOpacityExponent - value) < float.Epsilon) return;

                float oldValue = _DropShadowOpacityExponent;
                _DropShadowOpacityExponent = value;

                OnDropShadowOpacityExponentChanged(oldValue, value);
            }
        }
        private float _DropShadowOpacityExponent = 1f;
        public event EventHandler<EventArgs> DropShadowOpacityExponentChanged;
        public virtual void OnDropShadowOpacityExponentChanged(float oValue, float nValue)
        {
            DropShadowOpacityExponentChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXImageControl()
        {
            DrawImage = true;
            Index = -1;
            ImageOpacity = 1F;
            PixelDetect = true;
        }

        #region Methods
        protected override void DrawControl()
        {
            base.DrawControl();

            if (!DrawImage) return;

            DrawMirTexture();
        }
        protected virtual void DrawMirTexture()
        {
            bool oldBlend = RenderingPipelineManager.IsBlending();
            float oldRate = RenderingPipelineManager.GetBlendRate();
            BlendMode previousBlendMode = RenderingPipelineManager.GetBlendMode();
            float previousOpacity = RenderingPipelineManager.GetOpacity();

            MirImage image = Library.CreateImage(Index, ImageType.Image);

            if (image?.Image == null)
            {
                return;
            }

            if (Blend)
            {
                RenderingPipelineManager.SetBlend(true, ImageOpacity, BlendMode);
            }
            else
            {
                RenderingPipelineManager.SetOpacity(ImageOpacity);
            }

            bool dropShadowEnabled = DropShadow;

            Rectangle drawArea = DisplayArea;

            if (dropShadowEnabled)
            {
                float shadowWidth = Math.Max(0, DropShadowWidth);
                float shadowOpacity = Math.Min(1f, Math.Max(0f, DropShadowOpacity));
                float shadowExponent = Math.Max(0.01f, DropShadowOpacityExponent);

                int padding = (int)Math.Ceiling(shadowWidth);
                drawArea = Rectangle.Inflate(DisplayArea, padding, padding);

                RenderingPipelineManager.EnableDropShadowEffect(DropShadowColour, shadowWidth, shadowOpacity, shadowExponent);
            }

            PresentTexture(image.Image, FixedSize ? null : Parent, drawArea, IsEnabled ? ForeColour : Color.FromArgb(75, 75, 75), this, 0, 0, 1f);

            if (dropShadowEnabled)
            {
                RenderingPipelineManager.DisableSpriteShaderEffect();
            }

            if (Blend)
            {
                RenderingPipelineManager.SetBlend(oldBlend, oldRate, previousBlendMode);
            }

            RenderingPipelineManager.SetOpacity(previousOpacity);

            image.ExpireTime = Time.Now + Config.CacheDuration;
        }

        protected internal override void UpdateDisplayArea()
        {
            Rectangle area = new Rectangle(Location, Size);

            if (UseOffSet && Library != null)
                area.Offset(Library.GetOffSet(Index));

            if (Parent != null)
                area.Offset(Parent.DisplayArea.Location);

            DisplayArea = area;
        }


        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Blend = false;
                _DrawImage = false;
                _FixedSize = false;
                _ImageOpacity = 0F;
                _Index = -1;
                Library = null;
                _LibraryFile = LibraryFile.None;
                _PixelDetect = false;
                _UseOffSet = false;
                _DropShadow = false;
                _DropShadowColour = Color.Empty;
                _DropShadowWidth = 0f;
                _DropShadowOpacity = 0f;
                _DropShadowOpacityExponent = 0f;

                BlendChanged = null;
                DrawImageChanged = null;
                FixedSizeChanged = null;
                ImageOpacityChanged = null;
                IndexChanged = null;
                LibraryFileChanged = null;
                PixelDetectChanged = null;
                UseOffSetChanged = null;
            }
        }
        #endregion
    }

}
