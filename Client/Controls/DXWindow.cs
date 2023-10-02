using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Client.Envir;
using Client.UserModels;
using Library;
using SlimDX.Direct3D9;
using Font = System.Drawing.Font;

//Cleaned
namespace Client.Controls
{
    public abstract class DXWindow : DXControl
    {
        #region Properties

        public static List<DXWindow> Windows = new List<DXWindow>();

        #region HasTopBorder

        public bool HasTopBorder
        {
            get => _HasTopBorder;
            set
            {
                if (_HasTopBorder == value) return;

                bool oldValue = _HasTopBorder;
                _HasTopBorder = value;

                OnHasTopBorderChanged(oldValue, value);
            }
        }
        private bool _HasTopBorder;
        public event EventHandler<EventArgs> HasTopBorderChanged;
        public virtual void OnHasTopBorderChanged(bool oValue, bool nValue)
        {
            HasTopBorderChanged?.Invoke(this, EventArgs.Empty);

            UpdateClientArea();
        }

        #endregion
        
        #region HasTitle

        public bool HasTitle
        {
            get => _HasTitle;
            set
            {
                if (_HasTitle == value) return;

                bool oldValue = _HasTitle;
                _HasTitle = value;

                OnHasTitleChanged(oldValue, value);
            }
        }
        private bool _HasTitle;
        public event EventHandler<EventArgs> HasTitleChanged;
        public virtual void OnHasTitleChanged(bool oValue, bool nValue)
        {
            HasTitleChanged?.Invoke(this, EventArgs.Empty);

            UpdateClientArea();
            if (TitleLabel == null) return;
            TitleLabel.Visible = HasTitle;
        }

        #endregion

        #region HasFooter

        public bool HasFooter
        {
            get => _HasFooter;
            set
            {
                if (_HasFooter == value) return;

                bool oldValue = _HasFooter;
                _HasFooter = value;

                OnHasFooterChanged(oldValue, value);
            }
        }
        private bool _HasFooter;
        public event EventHandler<EventArgs> HasFooterChanged;
        public virtual void OnHasFooterChanged(bool oValue, bool nValue)
        {
            HasFooterChanged?.Invoke(this, EventArgs.Empty);

            UpdateClientArea();
        }

        #endregion

        #region ClientArea

        public Rectangle ClientArea
        {
            get => _ClientArea;
            set
            {
                if (_ClientArea == value) return;

                Rectangle oldValue = _ClientArea;
                _ClientArea = value;

                OnClientAreaChanged(oldValue, value);
            }
        }
        private Rectangle _ClientArea;
        public event EventHandler<EventArgs> ClientAreaChanged;
        public virtual void OnClientAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            ClientAreaChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public abstract WindowType Type { get; }
        public abstract bool CustomSize { get; }
        public abstract bool AutomaticVisibility { get; }

        public DXButton CloseButton { get; protected set; }
        public DXLabel TitleLabel { get; protected set; }

        public Texture WindowTexture;
        public Surface WindowSurface;
        public bool WindowValid;

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            UpdateClientArea();
            UpdateLocations();

            if (Settings != null && IsResizing)
            {
                Settings.Size = nValue;
                Settings.Location = Location;
            }
        }
        public override void OnParentChanged(DXControl oValue, DXControl nValue)
        {
            base.OnParentChanged(oValue, nValue);

            if (Parent == null) return;

            UpdateClientArea();

            UpdateLocations();
        }
        public override void OnLocationChanged(Point oValue, Point nValue)
        {
            base.OnLocationChanged(oValue, nValue);

            if (Settings != null && IsMoving)
                Settings.Location = nValue;
        }

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);
            
            if (IsVisible)
                BringToFront();

            if (Settings != null && AutomaticVisibility)
                Settings.Visible = nValue;
        }

        public WindowSetting Settings;

        #endregion
        
        protected DXWindow()
        {
            Windows.Add(this);

            DrawTexture = true;
            BackColour = Color.FromArgb(16, 8, 8);
            HasTitle = true;
            Movable = true;
            HasTopBorder = true;
            Sort = true;

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
            };
            CloseButton.MouseClick += (o, e) => Visible = false;

            TitleLabel = new DXLabel
            {
                Text = "Window",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                Visible = HasTitle,
                IsControl = false,
            };
            TitleLabel.SizeChanged += (o, e) => TitleLabel.Location = new Point((Size.Width - TitleLabel.Size.Width) / 2, 8);
        }

        #region Methods

        public override void ResolutionChanged()
        {
            Settings = null;

            base.ResolutionChanged();

            DisposeTexture();
        }

        protected override void CreateTexture()
        {
            base.CreateTexture();

            if (WindowTexture == null || DisplayArea.Size != TextureSize)
            {
                WindowTexture = new Texture(DXManager.Device, DXManager.Parameters.BackBufferWidth, DXManager.Parameters.BackBufferHeight, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                WindowSurface = WindowTexture.GetSurfaceLevel(0);
                WindowValid = false;
            }
        }

        public override void DisposeTexture()
        {
            base.DisposeTexture();

            if (WindowTexture != null)
            {
                if (!WindowTexture.Disposed)
                    WindowTexture.Dispose();

                WindowTexture = null;
            }

            if (WindowSurface != null)
            {
                if (!WindowSurface.Disposed)
                    WindowSurface.Dispose();

                WindowSurface = null;
            }
        }

        private void UpdateLocations()
        {
            if (CloseButton != null)
                CloseButton.Location = new Point(DisplayArea.Width - CloseButton.Size.Width - 3, 3);

            if (TitleLabel != null)
                TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 8);
        }

        protected internal override void UpdateDisplayArea()
        {
            base.UpdateDisplayArea();

            WindowValid = false;
        }
        
        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (CloseButton.Visible)
                    {
                        CloseButton.InvokeMouseClick();
                        if (!Config.EscapeCloseAll)
                            e.Handled = true;
                    }
                    break;
            }
        }

        public void UpdateClientArea()
        {
            ClientArea = GetClientArea(Size);
        }

        public void SetClientSize(Size clientSize)
        {
            Size = GetSize(clientSize);
        }

        public Size GetSize(Size clientSize)
        {
            int w = 3 + 6 + 6 + 3; //Border Padding Padding Border
            int h = 6 + 6; //Padding Padding


            if (!HasTopBorder)
                h += NoFooterSize;
            else if (HasTitle)
                h += HeaderSize;
            else
                h += HeaderBarSize;

            if (!HasFooter)
                h += NoFooterSize;
            else
                h += FooterSize;

            return new Size(clientSize.Width + w, clientSize.Height + h);
        }

        public Rectangle GetClientArea(Size size)
        {
            int x = 6 + 3;
            int y = 6;

            if (!HasTopBorder)
                y += NoFooterSize;
            else if (HasTitle)
                y += HeaderSize;
            else
                y += HeaderBarSize;

            int w = size.Width - x * 2;
            int h = size.Height - y - 6;


            if (!HasFooter)
                h -= NoFooterSize;
            else
                h -= FooterSize;

            return new Rectangle(x, y, w, h);
        }

        public override void Draw()
        {
            if (!IsVisible || Size.Width == 0 || Size.Height == 0) return;

            OnBeforeDraw();
            DrawControl();
            DrawWindow();
            OnBeforeChildrenDraw();
            DrawChildControls();
            DrawBorder();
            OnAfterDraw();
        }
        protected void DrawWindow()
        {
            if (InterfaceLibrary == null) return;
            
            if (!WindowValid)
            {
                Surface oldSurface = DXManager.CurrentSurface;
                DXManager.SetSurface(WindowSurface);
                DXManager.Device.Clear(ClearFlags.Target, 0, 0, 0);

                DrawEdges();

                DXManager.SetSurface(oldSurface);
                WindowValid = true;
            }

            float oldOpacity = DXManager.Opacity;

            DXManager.SetOpacity(Opacity);
            PresentTexture(WindowTexture, Parent, DisplayArea, ForeColour, this);

            DXManager.SetOpacity(oldOpacity);
        }
        
        private void DrawEdges()
        {
            Size s;

            if (HasTopBorder)
            {
                s = InterfaceLibrary.GetSize(0);
                InterfaceLibrary.Draw(0, 0, 0, Color.White, new Rectangle(0, 0, Size.Width, s.Height), 1f, ImageType.Image);
            }
            else
            {
                s = InterfaceLibrary.GetSize(2);
                InterfaceLibrary.Draw(2, 0, 0, Color.White, new Rectangle(0, 0, Size.Width, s.Height), 1f, ImageType.Image);
            }

            int y = s.Height;

            s = InterfaceLibrary.GetSize(1);
            int x = s.Width;
            InterfaceLibrary.Draw(1, 0, y, Color.White, new Rectangle(0, 0, s.Width, Size.Height - y), 1f, ImageType.Image);
            InterfaceLibrary.Draw(1, Size.Width - s.Width, y, Color.White, new Rectangle(0, 0, s.Width, Size.Height - y), 1F, ImageType.Image);


            if (HasTitle)
            {
                s = InterfaceLibrary.GetSize(3);
                InterfaceLibrary.Draw(3, x, y, Color.White, new Rectangle(0, 0, Size.Width - x * 2, Size.Height), 1f, ImageType.Image);

                y += s.Height;

                InterfaceLibrary.Draw(4, 0, y - 3, Color.White, false, 1F, ImageType.Image);

                s = InterfaceLibrary.GetSize(5);
                InterfaceLibrary.Draw(5, Size.Width - s.Width, y - 3, Color.White, false, 1F, ImageType.Image);
            }

            if (HasTopBorder)
            {
                //2X Corner
                InterfaceLibrary.Draw(11, 0, 0, Color.White, false, 1F, ImageType.Image);

                s = InterfaceLibrary.GetSize(12);
                InterfaceLibrary.Draw(12, Size.Width - s.Width, 0, Color.White, false, 1F, ImageType.Image);
            }
            else
            {
                //2X Corner
                InterfaceLibrary.Draw(25, 0, 0, Color.White, false, 1F, ImageType.Image);

                s = InterfaceLibrary.GetSize(26);
                InterfaceLibrary.Draw(26, Size.Width - s.Width, 0, Color.White, false, 1F, ImageType.Image);
            }

            if (!HasFooter)
            {
                s = InterfaceLibrary.GetSize(2);
                InterfaceLibrary.Draw(2, 0, Size.Height - s.Height, Color.White, new Rectangle(0, 0, Size.Width, s.Height), 1f, ImageType.Image);

                s = InterfaceLibrary.GetSize(8);
                InterfaceLibrary.Draw(8, 0, Size.Height - s.Height, Color.White, false, 1F, ImageType.Image);

                s = InterfaceLibrary.GetSize(9);
                InterfaceLibrary.Draw(9, Size.Width - s.Width, Size.Height - s.Height, Color.White, false, 1F, ImageType.Image);
            }
            else
            {
                s = InterfaceLibrary.GetSize(0);
                InterfaceLibrary.Draw(0, 0, Size.Height - s.Height, Color.White, new Rectangle(0, 0, Size.Width, s.Height), 1f, ImageType.Image);

                y = s.Height;

                s = InterfaceLibrary.GetSize(10);
                InterfaceLibrary.Draw(10, x, Size.Height - s.Height - y, Color.White, new Rectangle(0, 0, Size.Width - x * 2, s.Height), 1f, ImageType.Image);

                y += s.Height;

                s = InterfaceLibrary.GetSize(2);
                InterfaceLibrary.Draw(2, 0, Size.Height - y - s.Height, Color.White, new Rectangle(0, 0, Size.Width, s.Height), 1f, ImageType.Image);

                y += s.Height;

                s = InterfaceLibrary.GetSize(6);
                InterfaceLibrary.Draw(6, 0, Size.Height - y - s.Height + 3, Color.White, false, 1F, ImageType.Image);

                s = InterfaceLibrary.GetSize(7);
                InterfaceLibrary.Draw(7, Size.Width - s.Width, Size.Height - y - s.Height + 3, Color.White, false, 1F, ImageType.Image);


                s = InterfaceLibrary.GetSize(13);
                InterfaceLibrary.Draw(13, 0, Size.Height - s.Height, Color.White, false, 1F, ImageType.Image);

                s = InterfaceLibrary.GetSize(14);
                InterfaceLibrary.Draw(14, Size.Width - s.Width, Size.Height - s.Height, Color.White, false, 1F, ImageType.Image);
            }
        }

        public void LoadSettings()
        {
            if (Type == WindowType.None || !CEnvir.Loaded) return;

            Settings = CEnvir.WindowSettings.Binding.FirstOrDefault(x => x.Resolution == Config.GameSize && x.Window == Type);

            if (Settings != null)
            {
                ApplySettings();
                return;
            }

            UpdateSettings();
        }

        public void UpdateSettings()
        {
            Settings ??= CEnvir.WindowSettings.CreateNewObject();

            Settings.Resolution = Config.GameSize;
            Settings.Window = Type;
            Settings.Size = Size;
            Settings.Visible = Visible;
            Settings.Location = Location;
        }

        public virtual void ApplySettings()
        {
            if (Settings == null) return;

            Location = Settings.Location;

            if (AutomaticVisibility)
                Visible = Settings.Visible;

            if (CustomSize)
                Size = Settings.Size;
        }

        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _HasTopBorder = false;
                _HasTitle = false;
                _HasFooter = false;
                _ClientArea = Rectangle.Empty;

                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();
                    CloseButton = null;
                }

                if (TitleLabel != null)
                {
                    if (!TitleLabel.IsDisposed)
                        TitleLabel.Dispose();
                    TitleLabel = null;
                }

                HasTopBorderChanged = null;
                HasTitleChanged = null;
                HasFooterChanged = null;
                ClientAreaChanged = null;

                if (WindowTexture != null)
                {
                    if (!WindowTexture.Disposed)
                        WindowTexture.Dispose();

                    WindowTexture = null;
                }

                if (WindowSurface != null)
                {
                    if (!WindowSurface.Disposed)
                        WindowSurface.Dispose();

                    WindowSurface = null;
                }

                WindowValid = false;
                Settings = null;
                Windows.Remove(this);

            }
        }
        #endregion
    }
}
