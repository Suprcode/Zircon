using Library;
using Library.SystemModels;
using Server.Envir;
using Server.Views.DirectX;
using Shared.Envir;
using Shared.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using Matrix = System.Numerics.Matrix3x2;
namespace Server.Views
{
    public partial class MapViewer : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public static MapViewer CurrentViewer;
        public DXManager Manager;
        public MapControl Map;

        #region MapRegion

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MapRegion MapRegion
        {
            get { return _MapRegion; }
            set
            {
                if (_MapRegion == value) return;

                MapRegion oldValue = _MapRegion;
                _MapRegion = value;

                OnMapRegionChanged(oldValue, value);
            }
        }
        private MapRegion _MapRegion;
        public event EventHandler<EventArgs> MapRegionChanged;
        public virtual void OnMapRegionChanged(MapRegion oValue, MapRegion nValue)
        {
            Map.Selection.Clear();
            Map.TextureValid = false;

            if (MapRegion == null)
            {
                Map.Width = 0;
                Map.Height = 0;
                Map.Cells = null;
                UpdateScrollBars();
                return;
            }

            if (oValue == null || MapRegion.Map != oValue.Map)
            {
                Map.Load(MapRegion.Map.FileName);
                UpdateScrollBars();
            }

            Map.Selection = MapRegion.GetPoints(Map.Width);

            AttributesButton.Enabled = true;
            BlockedOnlyButton.Enabled = true;
            SelectionButton.Enabled = true;
            SaveButton.Enabled = true;
            CancelButton1.Enabled = true;

            MapRegionChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Map Path

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string MapPath
        {
            get { return _MapPath; }
            set
            {
                if (_MapPath == value) return;

                string oldValue = _MapPath;
                _MapPath = value;

                OnMapPathChanged(oldValue, value);
            }
        }
        private string _MapPath;
        public event EventHandler<EventArgs> MapPathChanged;
        public virtual void OnMapPathChanged(string oValue, string nValue)
        {
            Map.Selection.Clear();
            Map.TextureValid = false;
            MapRegion = null;

            if (oValue != nValue)
            {
                Map.Load(nValue);
                UpdateScrollBars();
            }

            Map.Selection = new HashSet<Point>();

            AttributesButton.Enabled = false;
            BlockedOnlyButton.Enabled = false;
            SelectionButton.Enabled = false;
            SaveButton.Enabled = false;
            CancelButton1.Enabled = false;

            MapPathChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public MapViewer()
        {
            InitializeComponent();

            CurrentViewer = this;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (CurrentViewer == this)
                CurrentViewer = null;

            Manager.Dispose();
            Manager = null;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Manager = new DXManager(DXPanel);
            Manager.Create();
            Map = new MapControl(Manager)
            {
                Size = DXPanel.ClientSize,
            };
            DXPanel.SizeChanged += DXPanel_SizeChanged;
            DXPanel.MouseWheel += DXPanel_MouseWheel;

            UpdateScrollBars();
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (Manager == null) return;

            Manager.ResetDevice();
            if (Map != null)
                Map.Size = DXPanel.ClientSize;


            UpdateScrollBars();
        }

        private void DXPanel_SizeChanged(object sender, EventArgs e)
        {
            if (Manager == null || Map == null) return;

            Manager.ResetDevice();
            Map.Size = DXPanel.ClientSize;
            UpdateScrollBars();
        }

        public void Process()
        {
            if (Map == null || Manager == null)
                return;

            UpdateEnvironment();
            RenderEnvironment();
        }

        private void UpdateEnvironment()
        {
            MapSizeLabel.Caption = string.Format(@"Map Size: {0},{1}", Map.Width, Map.Height);
            PositionLabel.Caption = string.Format(@"Position: {0},{1}", Map.MouseLocation.X, Map.MouseLocation.Y);
            SelectedCellsLabel.Caption = string.Format(@"Selected Cells: {0}", Map.Selection.Count);
        }

        private void RenderEnvironment()
        {
            try
            {
                RenderingPipelineManager.RenderFrame(() =>
                {
                    Manager.RefreshMainSurface();
                    RenderingPipelineManager.Clear(RenderClearFlags.Target, Color.Black, 1, 0);
                    Manager.SetSurface(Manager.MainSurface);
                    Map.Draw();
                });
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());

                Manager.AttemptRecovery();
            }
        }


        public void UpdateScrollBars()
        {
            if (Map.Width == 0 || Map.Height == 0)
            {
                MapVScroll.Enabled = false;
                MapHScroll.Enabled = false;
                return;
            }

            MapVScroll.Enabled = true;
            MapHScroll.Enabled = true;

            int wCount = (int)(DXPanel.ClientSize.Width / (Map.CellWidth));
            int hCount = (int)(DXPanel.ClientSize.Height / (Map.CellHeight));


            MapVScroll.Maximum = Math.Max(0, Map.Height - hCount + 20);
            MapHScroll.Maximum = Math.Max(0, Map.Width - wCount + 20);

            if (MapVScroll.Value >= MapVScroll.Maximum)
                MapVScroll.Value = MapVScroll.Maximum - 1;

            if (MapHScroll.Value >= MapHScroll.Maximum)
                MapHScroll.Value = MapHScroll.Maximum - 1;
        }

        private void MapVScroll_ValueChanged(object sender, EventArgs e)
        {
            Map.StartY = MapVScroll.Value;
        }
        private void MapHScroll_ValueChanged(object sender, EventArgs e)
        {
            Map.StartX = MapHScroll.Value;
        }

        private void ZoomResetButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Map.Zoom = 1;
            UpdateScrollBars();
        }

        private void ZoomInButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Map.Zoom *= 2F;
            if (Map.Zoom > 4F)
                Map.Zoom = 4F;

            UpdateScrollBars();
        }

        private void ZoomOutButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Map.Zoom /= 2;
            if (Map.Zoom < 0.01F)
                Map.Zoom = 0.01F;

            UpdateScrollBars();
        }

        private void AttributesButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Map.DrawAttributes = !Map.DrawAttributes;
        }

        private void DXPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            Map.Radius = Math.Max(0, Map.Radius - e.Delta / SystemInformation.MouseWheelScrollDelta);
        }
        private void DXPanel_MouseDown(object sender, MouseEventArgs e)
        {
            Map.MouseDown(e);
        }

        private void DXPanel_MouseMove(object sender, MouseEventArgs e)
        {
            Map.MouseMove(e);
        }

        private void DXPanel_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void DXPanel_MouseEnter(object sender, EventArgs e)
        {
            Map.MouseEnter();
        }

        private void DXPanel_MouseLeave(object sender, EventArgs e)
        {
            Map.MouseLeave();
        }

        private void SelectionButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Map.DrawSelection = !Map.DrawSelection;
        }

        private void SaveButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Save();
        }

        public void Save()
        {
            if (MapRegion == null) return;

            BitArray bitRegion = null;
            Point[] pointRegion = null;

            if (Map.Selection.Count * 8 * 8 > Map.Width * Map.Height)
            {
                bitRegion = new BitArray(Map.Width * Map.Height);

                foreach (Point point in Map.Selection)
                    bitRegion[point.Y * Map.Width + point.X] = true;
            }
            else
            {
                pointRegion = Map.Selection.ToArray();
            }

            MapRegion.BitRegion = bitRegion;
            MapRegion.PointRegion = pointRegion;

            MapRegion.Size = Map.Selection.Count;
        }

        private void CancelButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MapRegion == null) return;

            Map.Selection = MapRegion.GetPoints(Map.Width);

            Map.TextureValid = false;
        }

        private void BlockedOnlyButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Map.AttributeSelection = !Map.AttributeSelection;
        }
    }


}


namespace Server.Views.DirectX
{
    public class DXManager : IDisposable
    {
        public Graphics Graphics;

        public readonly Control Target;

        public Dictionary<LibraryFile, MirLibrary> LibraryList = new Dictionary<LibraryFile, MirLibrary>();

        public MapViewerSprite Sprite { get; private set; }
        public MapViewerLine Line { get; private set; }

        public RenderSurface CurrentSurface { get; private set; }
        public RenderSurface MainSurface { get; private set; }

        public float Opacity { get; private set; } = 1F;

        public bool Blending { get; private set; }
        public float BlendRate { get; private set; } = 1F;

        public List<MirImage> TextureList { get; } = new List<MirImage>();

        public RenderTexture AttributeTexture;

        public MapControl Map;
        private bool _pipelineInitialized;

        public DXManager(Control target)
        {
            Target = target;

            Graphics = Graphics.FromHwnd(IntPtr.Zero);
            ConfigureGraphics(Graphics);


            foreach (KeyValuePair<LibraryFile, string> pair in Libraries.LibraryList)
            {
                if (!File.Exists(Path.Combine(Config.ClientPath, pair.Value))) continue;

                LibraryList[pair.Key] = new MirLibrary(Path.Combine(Config.ClientPath, pair.Value));
            }
        }

        public void Create()
        {
            RenderingPipelineManager.Initialize(RenderingPipelineIds.SilkDXD3D11, new RenderingPipelineContext(Target, new RenderingHostSettings
            {
                Now = () => SEnvir.Now,
                SaveException = ex => SEnvir.SaveError(ex.ToString()),
                GetActiveSceneSize = () => Target.ClientSize,
                GetGameSize = () => Target.ClientSize,
                SetGameSize = _ => { },
                GetFullScreen = () => false,
                SetFullScreen = _ => { },
                GetBorderless = () => false,
                SetBorderless = _ => { },
                GetVSync = () => true,
                SetVSync = _ => { },
                GetRenderingPipeline = () => RenderingPipelineIds.SilkDXD3D11,
                SetRenderingPipeline = _ => { },
                GetUseD3D11SpriteBatch = () => true,
                SetUseD3D11SpriteBatch = _ => { },
            }));
            _pipelineInitialized = true;

            LoadTextures();
        }

        private void LoadTextures()
        {
            Sprite = new MapViewerSprite();
            Line = new MapViewerLine { Width = 1F };

            MainSurface = RenderingPipelineManager.GetCurrentSurface();
            CurrentSurface = MainSurface;

            AttributeTexture = RenderingPipelineManager.CreateTexture(new Size(48, 32), RenderTextureFormat.A8R8G8B8, RenderTextureUsage.None, RenderTexturePool.Managed);
            byte[] data = new byte[48 * 32 * 4];

            for (int i = 0; i < data.Length; i += 4)
            {
                data[i] = 255;
                data[i + 1] = 255;
                data[i + 2] = 255;
                data[i + 3] = 255;
            }

            using (TextureLock textureLock = RenderingPipelineManager.LockTexture(AttributeTexture, TextureLockMode.Discard))
                System.Runtime.InteropServices.Marshal.Copy(data, 0, textureLock.DataPointer, data.Length);

        }
        
        public void SetSurface(RenderSurface surface)
        {
            if (CurrentSurface.Equals(surface)) return;

            Sprite?.Flush();
            CurrentSurface = surface;
            RenderingPipelineManager.SetSurface(surface);
        }

        public void RestoreMainSurface()
        {
            if (!MainSurface.IsValid)
                return;

            Sprite?.Flush();
            CurrentSurface = MainSurface;
            RenderingPipelineManager.SetSurface(MainSurface);
        }

        public void RefreshMainSurface()
        {
            MainSurface = RenderingPipelineManager.GetCurrentSurface();
            CurrentSurface = MainSurface;
        }
        public void SetOpacity(float opacity)
        {
            if (Opacity == opacity)
                return;

            Opacity = opacity;
            RenderingPipelineManager.SetOpacity(opacity);
        }
        public void SetBlend(bool value, float rate = 1F)
        {
            if (value == Blending && Math.Abs(rate - BlendRate) < 0.0001F) return;

            Blending = value;
            BlendRate = rate;
            RenderingPipelineManager.SetBlend(value, rate);
        }
        public void SetColour(int colour)
        {
        }

        public void ResetDevice()
        {
            Map?.DisposeTexture();
            RenderingPipelineManager.ResetDevice();
            MainSurface = RenderingPipelineManager.GetCurrentSurface();
            CurrentSurface = MainSurface;
        }
        public void AttemptReset()
        {
        }
        public void AttemptRecovery()
        {
            MainSurface = RenderingPipelineManager.GetCurrentSurface();
            CurrentSurface = MainSurface;
        }

        public static void ConfigureGraphics(Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            graphics.TextContrast = 0;
        }

        #region IDisposable Support

        public bool IsDisposed { get; private set; }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsDisposed = true;

                RenderingPipelineManager.ReleaseTexture(AttributeTexture);
                AttributeTexture = default;
                Sprite = null;
                Line = null;
                CurrentSurface = default;
                MainSurface = default;

                Map?.DisposeTexture();

                if (Graphics != null)
                {
                    Graphics.Dispose();
                    Graphics = null;
                }

                foreach (KeyValuePair<LibraryFile, MirLibrary> library in LibraryList)
                    library.Value.Dispose();

                Opacity = 0;
                Blending = false;
                BlendRate = 0;

                if (_pipelineInitialized)
                {
                    RenderingPipelineManager.Shutdown();
                    _pipelineInitialized = false;
                }


            }

        }

        ~DXManager()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }

    public sealed class MapViewerSprite
    {
        public Matrix Transform = Matrix.Identity;

        public void Flush()
        {
            RenderingPipelineManager.FlushSprite();
        }

        public void Draw(RenderTexture texture, Vector3 center, Vector3 position, Color colour)
        {
            Draw(texture, null, center, position, colour);
        }

        public void Draw(RenderTexture texture, Rectangle source, Vector3 center, Vector3 position, Color colour)
        {
            Draw(texture, (Rectangle?)source, center, position, colour);
        }

        public void Draw(RenderTexture texture, Rectangle? source, Vector3 center, Vector3 position, Color colour)
        {
            if (!texture.IsValid)
                return;

            Matrix transform = Matrix.CreateTranslation(position.X, position.Y) * Transform;
            RenderingPipelineManager.DrawTexture(texture, source, transform, center, Vector3.Zero, colour);
        }
    }

    public sealed class MapViewerLine
    {
        public float Width
        {
            get { return RenderingPipelineManager.GetLineWidth(); }
            set { RenderingPipelineManager.SetLineWidth(value); }
        }

        public void Draw(IReadOnlyList<Vector2> points, Color colour)
        {
            if (points == null || points.Count == 0)
                return;

            List<LinePoint> linePoints = new List<LinePoint>(points.Count);
            foreach (Vector2 point in points)
                linePoints.Add(new LinePoint(point.X, point.Y));

            RenderingPipelineManager.DrawLine(linePoints, colour);
        }
    }

    public class MapControl : IDisposable
    {
        public DXManager Manager;

        public MapControl(DXManager manager)
        {
            Manager = manager;
            Zoom = 1;
        }

        #region Size

        public Size Size
        {
            get { return _Size; }
            set
            {
                if (_Size == value) return;

                Size oldValue = _Size;
                _Size = value;

                OnSizeChanged(oldValue, value);
            }
        }
        private Size _Size;
        public event EventHandler<EventArgs> SizeChanged;
        public virtual void OnSizeChanged(Size oValue, Size nValue)
        {
            TextureValid = false;

            SizeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public Cell[,] Cells;
        public int Width, Height;

        #region StartX

        public int StartX
        {
            get { return _StartX; }
            set
            {
                if (_StartX == value) return;

                int oldValue = _StartX;
                _StartX = value;

                OnStartXChanged(oldValue, value);
            }
        }
        private int _StartX;
        public event EventHandler<EventArgs> StartXChanged;
        public virtual void OnStartXChanged(int oValue, int nValue)
        {
            StartXChanged?.Invoke(this, EventArgs.Empty);
            TextureValid = false;
        }

        #endregion

        #region StartY

        public int StartY
        {
            get { return _StartY; }
            set
            {
                if (_StartY == value) return;

                int oldValue = _StartY;
                _StartY = value;

                OnStartYChanged(oldValue, value);
            }
        }
        private int _StartY;
        public event EventHandler<EventArgs> StartYChanged;
        public virtual void OnStartYChanged(int oValue, int nValue)
        {
            StartYChanged?.Invoke(this, EventArgs.Empty);

            TextureValid = false;
        }

        #endregion

        #region DrawAttributes

        public bool DrawAttributes
        {
            get { return _DrawAttributes; }
            set
            {
                if (_DrawAttributes == value) return;

                bool oldValue = _DrawAttributes;
                _DrawAttributes = value;

                OnDrawAttributesChanged(oldValue, value);
            }
        }
        private bool _DrawAttributes;
        public event EventHandler<EventArgs> DrawAttributesChanged;
        public virtual void OnDrawAttributesChanged(bool oValue, bool nValue)
        {
            DrawAttributesChanged?.Invoke(this, EventArgs.Empty);
            TextureValid = false;
        }

        #endregion

        #region DrawSelection

        public bool DrawSelection
        {
            get { return _DrawSelection; }
            set
            {
                if (_DrawSelection == value) return;

                bool oldValue = _DrawSelection;
                _DrawSelection = value;

                OnDrawSelectionChanged(oldValue, value);
            }
        }
        private bool _DrawSelection;
        public event EventHandler<EventArgs> DrawSelectionChanged;
        public virtual void OnDrawSelectionChanged(bool oValue, bool nValue)
        {
            DrawSelectionChanged?.Invoke(this, EventArgs.Empty);
            TextureValid = false;
        }

        #endregion


        #region AttributeSelection

        public bool AttributeSelection
        {
            get { return _AttributeSelection; }
            set
            {
                if (_AttributeSelection == value) return;

                bool oldValue = _AttributeSelection;
                _AttributeSelection = value;

                OnAttributeSelectionChanged(oldValue, value);
            }
        }
        private bool _AttributeSelection;
        public event EventHandler<EventArgs> AttributeSelectionChanged;
        public virtual void OnAttributeSelectionChanged(bool oValue, bool nValue)
        {
            AttributeSelectionChanged?.Invoke(this, EventArgs.Empty);
            TextureValid = false;
        }

        #endregion

        public HashSet<Point> Selection = new HashSet<Point>();


        //Zoom to handle
        public const int BaseCellWidth = 48;
        public const int BaseCellHeight = 32;

        public float CellWidth => BaseCellWidth * Zoom;
        public float CellHeight => BaseCellHeight * Zoom;


        #region Zoom

        public float Zoom
        {
            get { return _Zoom; }
            set
            {
                if (_Zoom == value) return;

                float oldValue = _Zoom;
                _Zoom = value;

                OnZoomChanged(oldValue, value);
            }
        }
        private float _Zoom;
        public event EventHandler<EventArgs> ZoomChanged;
        public virtual void OnZoomChanged(float oValue, float nValue)
        {
            ZoomChanged?.Invoke(this, EventArgs.Empty);
            TextureValid = false;
        }

        #endregion

        #region Animation

        public int Animation
        {
            get { return _Animation; }
            set
            {
                if (_Animation == value) return;

                int oldValue = _Animation;
                _Animation = value;

                OnAnimationChanged(oldValue, value);
            }
        }
        private int _Animation;
        public event EventHandler<EventArgs> AnimationChanged;
        public virtual void OnAnimationChanged(int oValue, int nValue)
        {
            AnimationChanged?.Invoke(this, EventArgs.Empty);
            TextureValid = false;
        }

        #endregion

        #region Border

        public bool Border
        {
            get { return _Border; }
            set
            {
                if (_Border == value) return;

                bool oldValue = _Border;
                _Border = value;

                OnBorderChanged(oldValue, value);
            }
        }
        private bool _Border;
        public event EventHandler<EventArgs> BorderChanged;
        public virtual void OnBorderChanged(bool oValue, bool nValue)
        {
            BorderChanged?.Invoke(this, EventArgs.Empty);
            TextureValid = false;
        }

        #endregion


        #region MouseLocation

        public Point MouseLocation
        {
            get { return _MouseLocation; }
            set
            {
                if (_MouseLocation == value) return;

                Point oldValue = _MouseLocation;
                _MouseLocation = value;

                OnMouseLocationChanged(oldValue, value);
            }
        }
        private Point _MouseLocation;
        public event EventHandler<EventArgs> MouseLocationChanged;
        public virtual void OnMouseLocationChanged(Point oValue, Point nValue)
        {
            MouseLocationChanged?.Invoke(this, EventArgs.Empty);
            TextureValid = false;
        }

        #endregion

        #region Radius

        public int Radius
        {
            get { return _Radius; }
            set
            {
                if (_Radius == value) return;

                int oldValue = _Radius;
                _Radius = value;

                OnRadiusChanged(oldValue, value);
            }
        }
        private int _Radius;
        public event EventHandler<EventArgs> RadiusChanged;
        public virtual void OnRadiusChanged(int oValue, int nValue)
        {
            RadiusChanged?.Invoke(this, EventArgs.Empty);
            TextureValid = false;
        }

        #endregion




        #region Texture
        public bool TextureValid { get; set; }
        public RenderTexture ControlTexture { get; set; }
        public Size TextureSize { get; set; }
        public RenderSurface ControlSurface { get; set; }
        public RenderTargetResource ControlRenderTarget { get; set; }
        public DateTime ExpireTime { get; protected set; }

        protected virtual void CreateTexture()
        {
            if (!ControlTexture.IsValid || Size != TextureSize)
            {
                DisposeTexture();
                TextureSize = Size;
                ControlRenderTarget = RenderingPipelineManager.CreateRenderTarget(TextureSize);
                ControlTexture = ControlRenderTarget.Texture;
                ControlSurface = ControlRenderTarget.Surface;
                Manager.Map = this;
            }

            RenderSurface previous = Manager.CurrentSurface;
            Manager.SetSurface(ControlSurface);

            try
            {
                RenderingPipelineManager.Clear(RenderClearFlags.Target, Color.Black, 0, 0);

                TextureFilterMode oldTextureFilter = RenderingPipelineManager.GetTextureFilter();
                RenderingPipelineManager.SetTextureFilter(Zoom < 1F ? TextureFilterMode.Linear : TextureFilterMode.Point);

                try
                {
                    OnClearTexture();
                }
                finally
                {
                    Manager.Sprite.Flush();
                    RenderingPipelineManager.SetTextureFilter(oldTextureFilter);
                }
            }
            finally
            {
                Manager.SetSurface(previous);
            }

            TextureValid = true;
        }
        protected virtual void OnClearTexture()
        {
            DrawFloor();

            //DrawObjects();

            //DrawPlacements();
        }

        public virtual void DisposeTexture()
        {
            bool wasCurrentSurface = ControlSurface.IsValid && Manager.CurrentSurface.Equals(ControlSurface);

            if (wasCurrentSurface)
                Manager.RestoreMainSurface();

            RenderingPipelineManager.ReleaseRenderTarget(ControlRenderTarget);
            ControlRenderTarget = default;
            ControlTexture = default;
            ControlSurface = default;

            TextureSize = Size.Empty;
            ExpireTime = DateTime.MinValue;
            TextureValid = false;

            if (Manager.Map == this)
                Manager.Map = null;
        }

        #endregion


        public void Draw()
        {
            if (Size.Width <= 0 || Size.Height <= 0) return;

            TextureFilterMode oldTextureFilter = RenderingPipelineManager.GetTextureFilter();
            RenderingPipelineManager.SetTextureFilter(Zoom < 1F ? TextureFilterMode.Linear : TextureFilterMode.Point);

            try
            {
                DrawFloor();
            }
            finally
            {
                Manager.Sprite.Flush();
                RenderingPipelineManager.SetTextureFilter(oldTextureFilter);
            }
        }
        protected virtual void DrawControl()
        {
            if (!TextureValid)
            {
                CreateTexture();

                if (!TextureValid) return;
            }

            float oldOpacity = Manager.Opacity;

            Manager.SetOpacity(1F);

            Manager.Sprite.Draw(ControlTexture, Vector3.Zero, Vector3.Zero, Color.White);

            Manager.SetOpacity(oldOpacity);
        }

        public void DrawFloor()
        {
            int minX = Math.Max(0, StartX - 1);
            int maxX = Math.Min(Width - 1, StartX + (int)Math.Ceiling(Size.Width / CellWidth));

            int minY = Math.Max(0, StartY - 1);
            int maxY = Math.Min(Height - 1, StartY + (int)Math.Ceiling(Size.Height / CellHeight));

            Matrix scale = Matrix.CreateScale(Zoom, Zoom);

            try
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (y % 2 != 0) continue;

                    float drawY = (y - StartY) * BaseCellHeight;
                    float scaledDrawY = drawY * Zoom;

                    for (int x = minX; x <= maxX; x++)
                    {
                        if (x % 2 != 0) continue;

                        float drawX = (x - StartX) * BaseCellWidth;
                        float scaledDrawX = drawX * Zoom;

                        Cell tile = Cells[x, y];

                        if (!Libraries.KROrder.TryGetValue(tile.BackFile, out LibraryFile file)) continue;
                        if (!Manager.LibraryList.TryGetValue(file, out MirLibrary library)) continue;

                        library.Draw(tile.BackImage, scaledDrawX, scaledDrawY, Color.White, false, 1F, ImageType.Image, Zoom);
                    }
                }

                maxY = Math.Min(Height - 1, StartY + 20 + (int)Math.Ceiling(Size.Height / CellHeight));
                for (int y = minY; y <= maxY; y++)
                {
                    float drawY = (y - StartY + 1) * BaseCellHeight;
                    float scaledDrawY = drawY * Zoom;

                    for (int x = minX; x <= maxX; x++)
                    {
                        float drawX = (x - StartX) * BaseCellWidth;
                        float scaledDrawX = drawX * Zoom;

                        Cell cell = Cells[x, y];

                        if (Libraries.KROrder.TryGetValue(cell.MiddleFile, out LibraryFile file) && file != LibraryFile.Tilesc && Manager.LibraryList.TryGetValue(file, out MirLibrary library))
                        {
                            int index = cell.MiddleImage - 1;
                            bool blend = false;

                            if (cell.MiddleAnimationFrame > 1 && cell.MiddleAnimationFrame < 255)
                            {
                                blend = cell.MiddleAnimationBlend;
                                index += Animation % cell.MiddleAnimationCount;
                            }

                            DrawMapLayer(library, index, blend, scaledDrawX, scaledDrawY);
                        }

                        if (Libraries.KROrder.TryGetValue(cell.FrontFile, out file) && file != LibraryFile.Tilesc && Manager.LibraryList.TryGetValue(file, out library))
                        {
                            int index = cell.FrontImage - 1;
                            bool blend = false;

                            if (cell.FrontAnimationFrame > 1 && cell.FrontAnimationFrame < 255)
                            {
                                blend = cell.FrontAnimationBlend;
                                index += Animation % cell.FrontAnimationCount;
                            }

                            DrawMapLayer(library, index, blend, scaledDrawX, scaledDrawY);
                        }
                    }
                }

                maxY = Math.Min(Height - 1, StartY + (int)Math.Ceiling(Size.Height / CellHeight));

                if (DrawAttributes || DrawSelection)
                {
                    Manager.SetOpacity(0.35F);
                    for (int y = minY; y <= maxY; y++)
                    {
                        float drawY = (y - StartY) * BaseCellHeight;

                        for (int x = minX; x <= maxX; x++)
                        {
                            float drawX = (x - StartX) * BaseCellWidth;

                            Cell tile = Cells[x, y];

                            if (tile.Flag != AttributeSelection)
                            {
                                if (!DrawAttributes) continue;

                                Manager.Sprite.Transform = Matrix.CreateTranslation(drawX, drawY) * scale;
                                Manager.Sprite.Draw(Manager.AttributeTexture, Vector3.Zero, Vector3.Zero, Color.Red);
                            }
                            else
                            {
                                if (!DrawSelection) continue;
                                if (!Selection.Contains(new Point(x, y))) continue;

                                Manager.Sprite.Transform = Matrix.CreateTranslation(drawX, drawY) * scale;
                                Manager.Sprite.Draw(Manager.AttributeTexture, Vector3.Zero, Vector3.Zero, Color.Yellow);
                            }
                        }
                    }
                    Manager.Sprite.Flush();
                }

                Manager.SetOpacity(1F);
                if (Border)
                {
                    Manager.Line.Draw(new[]
                    {
                        new Vector2((MouseLocation.X - StartX)*CellWidth, (MouseLocation.Y - StartY)*CellHeight),
                        new Vector2((MouseLocation.X - StartX)*CellWidth + CellWidth, (MouseLocation.Y - StartY)*CellHeight),
                        new Vector2((MouseLocation.X - StartX)*CellWidth + CellWidth, (MouseLocation.Y - StartY)*CellHeight + CellHeight),
                        new Vector2((MouseLocation.X - StartX)*CellWidth, (MouseLocation.Y - StartY)*CellHeight + CellHeight),
                        new Vector2((MouseLocation.X - StartX)*CellWidth, (MouseLocation.Y - StartY)*CellHeight),
                    }, Color.Lime);


                    if (Radius > 0)
                        Manager.Line.Draw(new[]
                        {
                            new Vector2((MouseLocation.X - StartX - Radius)*CellWidth, (MouseLocation.Y - StartY - Radius)*CellHeight),
                            new Vector2((MouseLocation.X - StartX + Radius)*CellWidth + CellWidth, (MouseLocation.Y - StartY- Radius)*CellHeight),
                            new Vector2((MouseLocation.X - StartX + Radius)*CellWidth + CellWidth, (MouseLocation.Y - StartY + Radius)*CellHeight + CellHeight),
                            new Vector2((MouseLocation.X - StartX - Radius)*CellWidth, (MouseLocation.Y - StartY + Radius)*CellHeight + CellHeight),
                            new Vector2((MouseLocation.X - StartX - Radius)*CellWidth, (MouseLocation.Y - StartY - Radius)*CellHeight),
                        }, Color.Lime);
                }
            }
            finally
            {
                Manager.Sprite.Transform = Matrix.Identity;
            }
        }

        private void DrawMapLayer(MirLibrary library, int index, bool blend, float scaledDrawX, float scaledDrawY)
        {
            Size size = library.GetSize(index);

            if ((size.Width != CellWidth || size.Height != CellHeight) && (size.Width != CellWidth * 2 || size.Height != CellHeight * 2))
            {
                if (blend)
                    library.DrawBlend(index, Zoom, Color.White, scaledDrawX, scaledDrawY - size.Height * Zoom, 0F, 0.5F, ImageType.Image);
                else
                    library.Draw(index, scaledDrawX, scaledDrawY - size.Height * Zoom, Color.White, false, 1F, ImageType.Image, Zoom);

                return;
            }

            library.Draw(index, scaledDrawX, scaledDrawY - BaseCellHeight * Zoom, Color.White, false, 1F, ImageType.Image, Zoom);
        }

        public void Load(string fileName)
        {
            try
            {
                string path = null;

                if (Path.IsPathRooted(fileName))
                {
                    path = fileName;
                }
                else
                {
                    path = Path.Combine(Config.MapPath, fileName + ".map");
                }

                if (!File.Exists(path)) return;

                using (MemoryStream mStream = new MemoryStream(File.ReadAllBytes(path)))
                using (BinaryReader reader = new BinaryReader(mStream))
                {
                    mStream.Seek(22, SeekOrigin.Begin);
                    Width = reader.ReadInt16();
                    Height = reader.ReadInt16();

                    mStream.Seek(28, SeekOrigin.Begin);

                    Cells = new Cell[Width, Height];
                    for (int x = 0; x < Width; x++)
                        for (int y = 0; y < Height; y++)
                            Cells[x, y] = new Cell();

                    for (int x = 0; x < Width / 2; x++)
                        for (int y = 0; y < Height / 2; y++)
                        {
                            Cells[(x * 2), (y * 2)].BackFile = reader.ReadByte();
                            Cells[(x * 2), (y * 2)].BackImage = reader.ReadUInt16();
                        }

                    for (int x = 0; x < Width; x++)
                        for (int y = 0; y < Height; y++)
                        {
                            byte flag = reader.ReadByte();
                            Cells[x, y].MiddleAnimationFrame = reader.ReadByte();

                            byte value = reader.ReadByte();
                            Cells[x, y].FrontAnimationFrame = value == 255 ? 0 : value;

                            Cells[x, y].FrontFile = reader.ReadByte();
                            Cells[x, y].MiddleFile = reader.ReadByte();

                            Cells[x, y].MiddleImage = reader.ReadUInt16() + 1;
                            Cells[x, y].FrontImage = reader.ReadUInt16() + 1;

                            mStream.Seek(3, SeekOrigin.Current);

                            Cells[x, y].Light = (byte)(reader.ReadByte() & 0x0F) * 2;

                            mStream.Seek(1, SeekOrigin.Current);

                            Cells[x, y].Flag = ((flag & 0x01) != 1) || ((flag & 0x02) != 2);
                        }
                }
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
            TextureValid = false;
        }


        #region IDisposable Support

        public bool IsDisposed { get; private set; }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsDisposed = true;

                bool wasCurrentSurface = ControlSurface.IsValid && Manager.CurrentSurface.Equals(ControlSurface);

                if (wasCurrentSurface)
                    Manager.RestoreMainSurface();

                RenderingPipelineManager.ReleaseRenderTarget(ControlRenderTarget);
                ControlRenderTarget = default;
                ControlTexture = default;
                ControlSurface = default;

                _Size = Size.Empty;

                TextureValid = false;
                TextureSize = Size.Empty;
                ExpireTime = DateTime.MinValue;

                if (Manager?.Map == this)
                    Manager.Map = null;
            }

        }

        ~MapControl()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public void MouseDown(MouseEventArgs e)
        {


            switch (e.Button)
            {
                case MouseButtons.Left:

                    for (int y = MouseLocation.Y - Radius; y <= MouseLocation.Y + Radius; y++)
                        for (int x = MouseLocation.X - Radius; x <= MouseLocation.X + Radius; x++)
                        {
                            if (x < 0 || x >= Width || y < 0 || y >= Height || Cells[x, y].Flag != AttributeSelection) continue;

                            Selection.Add(new Point(x, y));
                        }


                    break;
                case MouseButtons.Right:

                    for (int y = MouseLocation.Y - Radius; y <= MouseLocation.Y + Radius; y++)
                        for (int x = MouseLocation.X - Radius; x <= MouseLocation.X + Radius; x++)
                        {
                            if (x < 0 || x >= Width || y < 0 || y >= Height || Cells[x, y].Flag != AttributeSelection) continue;

                            Selection.Remove(new Point(x, y));
                        }
                    break;
                case MouseButtons.Middle:
                    if (MouseLocation.X < 0 || MouseLocation.X >= Width || MouseLocation.Y < 0 || MouseLocation.Y >= Height) return;
                    if (Cells[MouseLocation.X, MouseLocation.Y].Flag != AttributeSelection) return;

                    HashSet<Point> doneList = new HashSet<Point> { MouseLocation };
                    Queue<Point> todoList = new Queue<Point>();

                    todoList.Enqueue(MouseLocation);

                    if (Selection.Contains(MouseLocation)) //removing
                    {
                        while (todoList.Count > 0)
                        {
                            Point p = todoList.Dequeue();

                            for (int i = 0; i < 8; i++)
                            {
                                Point nPoint = Functions.Move(p, (MirDirection)i);

                                if (nPoint.X < 0 || nPoint.X >= Width || nPoint.Y < 0 || nPoint.Y >= Height) continue;

                                if (Cells[nPoint.X, nPoint.Y].Flag != AttributeSelection) continue;

                                if (doneList.Contains(nPoint)) continue;

                                if (!Selection.Contains(nPoint)) continue;

                                doneList.Add(nPoint);
                                todoList.Enqueue(nPoint);
                            }

                            Selection.Remove(p);
                        }

                    }
                    else
                    {
                        while (todoList.Count > 0)
                        {
                            Point p = todoList.Dequeue();

                            for (int i = 0; i < 8; i++)
                            {
                                Point nPoint = Functions.Move(p, (MirDirection)i);

                                if (nPoint.X < 0 || nPoint.X >= Width || nPoint.Y < 0 || nPoint.Y >= Height) continue;

                                if (Cells[nPoint.X, nPoint.Y].Flag != AttributeSelection) continue;

                                if (doneList.Contains(nPoint)) continue;

                                if (Selection.Contains(nPoint)) continue;

                                doneList.Add(nPoint);
                                todoList.Enqueue(nPoint);
                            }

                            Selection.Add(p);
                        }
                    }

                    break;
            }
            TextureValid = false;
        }
        public void MouseMove(MouseEventArgs e)
        {
            MouseLocation = new Point(Math.Min(Width, Math.Max(0, (int)(e.X / CellWidth) + StartX)), Math.Min(Height, Math.Max(0, (int)(e.Y / CellHeight) + StartY)));

            switch (e.Button)
            {
                case MouseButtons.Left:
                    for (int y = MouseLocation.Y - Radius; y <= MouseLocation.Y + Radius; y++)
                        for (int x = MouseLocation.X - Radius; x <= MouseLocation.X + Radius; x++)
                        {
                            if (x < 0 || x >= Width || y < 0 || y >= Height || Cells[x, y].Flag != AttributeSelection) continue;

                            Selection.Add(new Point(x, y));
                        }
                    break;
                case MouseButtons.Right:

                    for (int y = MouseLocation.Y - Radius; y <= MouseLocation.Y + Radius; y++)
                        for (int x = MouseLocation.X - Radius; x <= MouseLocation.X + Radius; x++)
                        {
                            if (x < 0 || x >= Width || y < 0 || y >= Height || Cells[x, y].Flag != AttributeSelection) continue;

                            Selection.Remove(new Point(x, y));
                        }
                    break;
            }
        }

        public void MouseEnter()
        {
            Border = true;
        }
        public void MouseLeave()
        {
            Border = false;
        }



        public sealed class Cell
        {
            private const int FrontFrameMask = 0x0F;
            private const int FrontBlendBit = 0x80;
            private const int MiddleFrameMask = 0x0F;
            private const int MiddleBlendBit = 0x80;

            public int BackFile;
            public int BackImage;

            public int MiddleFile;
            public int MiddleImage;

            public int FrontFile;
            public int FrontImage;

            public int FrontAnimationFrame;
            public int FrontAnimationTick;
            public int FrontAnimationCount => FrontAnimationFrame & FrontFrameMask;
            public bool FrontAnimationBlend => (FrontAnimationFrame & FrontBlendBit) != 0;

            public int MiddleAnimationFrame;
            public int MiddleAnimationTick;
            public int MiddleAnimationCount => MiddleAnimationFrame & MiddleFrameMask;
            public bool MiddleAnimationBlend => (MiddleAnimationFrame & MiddleBlendBit) != 0;

            public int Light;

            public bool Flag;
        }

    }


}
