using Library;
using Library.SystemModels;
using Server.Envir;
using Server.Views.DirectX;
using SlimDX;
using SlimDX.Direct3D9;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Blend = SlimDX.Direct3D9.Blend;
using Matrix = SlimDX.Matrix;

namespace Server.Views
{
    public partial class MapViewer : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public static MapViewer CurrentViewer;
        public DXManager Manager;
        public MapControl Map;

        public DateTime AnimationTime;

        #region MapRegion

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
                Map.Load(MapRegion.Map.FileName);

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
                Map.Load(nValue);

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

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

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
            
            DXPanel.MouseWheel += DXPanel_MouseWheel;
            
            UpdateScrollBars();
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (Manager == null) return;

            Manager.ResetDevice();
            Map.Size = DXPanel.ClientSize;

            
            UpdateScrollBars();
        }

        public void Process()
        {
            UpdateEnvironment();
            RenderEnvironment();
        }

        private void UpdateEnvironment()
        {
            if (SEnvir.Now > AnimationTime && Map != null)
            {
                AnimationTime = SEnvir.Now.AddMilliseconds(100);
                Map.Animation++;
            }

        }
        private void RenderEnvironment()
        {
            try
            {
                if (Manager.DeviceLost)
                {
                    Manager.AttemptReset();
                    return;
                }

                Manager.Device.Clear(ClearFlags.Target, Color.Black, 1, 0);
                Manager.Device.BeginScene();
                Manager.Sprite.Begin(SpriteFlags.AlphaBlend);
                Manager.SetSurface(Manager.MainSurface);

                Map.Draw();

                Manager.Sprite.End();
                Manager.Device.EndScene();
                Manager.Device.Present();

            }
            catch (Direct3D9Exception)
            {
                Manager.DeviceLost = true;
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

            if (Map.Selection.Count*8*8 > Map.Width*Map.Height)
            {
                bitRegion = new BitArray(Map.Width*Map.Height);

                foreach (Point point in Map.Selection)
                    bitRegion[point.Y*Map.Width + point.X] = true;
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

        public PresentParameters Parameters { get; private set; }
        public Device Device { get; private set; }
        public Sprite Sprite { get; private set; }
        public Line Line { get; private set; }

        public Surface CurrentSurface { get; private set; }
        public Surface MainSurface { get; private set; }

        public float Opacity { get; private set; } = 1F;

        public bool Blending { get; private set; }
        public float BlendRate { get; private set; } = 1F;

        public bool DeviceLost { get; set; }
        
        public List<MirImage> TextureList { get; } = new List<MirImage>();

        public Texture AttributeTexture;

        public MapControl Map;

        public DXManager(Control target)
        {
            Target = target;

            Graphics = Graphics.FromHwnd(IntPtr.Zero);
            ConfigureGraphics(Graphics);

            
            foreach (KeyValuePair<LibraryFile, string> pair in Libraries.LibraryList)
            {
                if (!File.Exists(Path.Combine(Config.ClientPath, pair.Value))) continue;

                LibraryList[pair.Key] = new MirLibrary(Path.Combine(Config.ClientPath, pair.Value), this);
            }

        }

        public void Create()
        {
            Parameters = new PresentParameters
            {
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                BackBufferFormat = Format.X8R8G8B8,
                PresentationInterval = PresentInterval.Default,
                BackBufferWidth = Target.ClientSize.Width,
                BackBufferHeight = Target.ClientSize.Height,
                PresentFlags = PresentFlags.LockableBackBuffer,
            };

            Direct3D direct3D = new Direct3D();

            Device = new Device(direct3D, direct3D.Adapters.DefaultAdapter.Adapter, DeviceType.Hardware, Target.Handle, CreateFlags.HardwareVertexProcessing, Parameters);

            LoadTextures();

            Device.SetDialogBoxMode(true);
        }

        private unsafe void LoadTextures()
        {
            Sprite = new Sprite(Device);
            Line = new Line(Device) { Width = 1F };

            MainSurface = Device.GetBackBuffer(0, 0);
            CurrentSurface = MainSurface;
            Device.SetRenderTarget(0, MainSurface);

            AttributeTexture = new Texture(Device, 48, 32, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);

            DataRectangle rect = AttributeTexture.LockRectangle(0, LockFlags.Discard);

            int* data = (int*)rect.Data.DataPointer;

            for (int y = 0; y < 32; y++)
                for (int x = 0; x < 48; x++)
                    data[y * 48 + x] = -1;

        }
        private void CleanUp()
        {
            if (Sprite != null)
            {
                if (!Sprite.Disposed)
                    Sprite.Dispose();

                Sprite = null;
            }

            if (Line != null)
            {
                if (!Line.Disposed)
                    Line.Dispose();

                Line = null;
            }

            if (CurrentSurface != null)
            {
                if (!CurrentSurface.Disposed)
                    CurrentSurface.Dispose();

                CurrentSurface = null;
            }

            if (AttributeTexture != null)
            {
                if (!AttributeTexture.Disposed)
                    AttributeTexture.Dispose();

                AttributeTexture = null;
            }


            Map?.DisposeTexture();

            for (int i = TextureList.Count - 1; i >= 0; i--)
                TextureList[i].DisposeTexture();
        }

        public void SetSurface(Surface surface)
        {
            if (CurrentSurface == surface) return;

            Sprite.Flush();
            CurrentSurface = surface;
            Device.SetRenderTarget(0, surface);
        }
        public void SetOpacity(float opacity)
        {
            Device.SetSamplerState(0, SamplerState.MagFilter, 0);

            if (Opacity == opacity)
                return;

            Sprite.Flush();
            Device.SetRenderState(RenderState.AlphaBlendEnable, true);

            if (opacity >= 1 || opacity < 0)
            {
                Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
                Device.SetRenderState(RenderState.SourceBlendAlpha, Blend.One);
                Device.SetRenderState(RenderState.BlendFactor, Color.FromArgb(255, 255, 255, 255).ToArgb());
            }
            else
            {
                Device.SetRenderState(RenderState.SourceBlend, Blend.BlendFactor);
                Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseBlendFactor);
                Device.SetRenderState(RenderState.SourceBlendAlpha, Blend.SourceAlpha);
                Device.SetRenderState(RenderState.BlendFactor, Color.FromArgb((byte)(255 * opacity), (byte)(255 * opacity),
                    (byte)(255 * opacity), (byte)(255 * opacity)).ToArgb());
            }

            Opacity = opacity;
            Sprite.Flush();
        }
        public void SetBlend(bool value, float rate = 1F)
        {
            if (value == Blending) return;

            Blending = value;
            BlendRate = 1F;
            Sprite.Flush();

            Sprite.End();
            if (Blending)
            {
                Sprite.Begin(SpriteFlags.DoNotSaveState);
                Device.SetRenderState(RenderState.AlphaBlendEnable, true);

                Device.SetRenderState(RenderState.SourceBlend, Blend.BlendFactor);
                Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
                Device.SetRenderState(RenderState.BlendFactor, Color.FromArgb((byte)(255 * rate), (byte)(255 * rate), (byte)(255 * rate), (byte)(255 * rate)).ToArgb());
            }
            else
            {
                Sprite.Begin(SpriteFlags.AlphaBlend);
            }


            Device.SetRenderTarget(0, CurrentSurface);
        }
        public void SetColour(int colour)
        {
            Sprite.Flush();

            if (colour == 0)
            {
                Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
                Device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
            }
            else
            {

                Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.SelectArg1);
                Device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Current);
            }

            Sprite.Flush();
        }

        public void ResetDevice()
        {
            CleanUp();

            DeviceLost = true;

            if (Parameters == null || Target.ClientSize.Width == 0 || Target.ClientSize.Height == 0) return;
            
            Parameters.BackBufferWidth = Target.ClientSize.Width;
            Parameters.BackBufferHeight = Target.ClientSize.Height;

            Device.Reset(Parameters);
            LoadTextures();
        }
        public void AttemptReset()
        {
            try
            {
                Result result = Device.TestCooperativeLevel();

                if (result.Code == ResultCode.DeviceLost.Code) return;

                if (result.Code == ResultCode.DeviceNotReset.Code)
                {
                    ResetDevice();
                    return;
                }

                if (result.Code != ResultCode.Success.Code) return;

                DeviceLost = false;
            }
            catch (Exception ex)
            {
                SEnvir.SaveError(ex.ToString());
            }
        }
        public void AttemptRecovery()
        {
            try
            {
                Sprite.End();
            }
            catch
            {
            }

            try
            {
                Device.EndScene();
            }
            catch
            {
            }

            try
            {
                MainSurface = Device.GetBackBuffer(0, 0);
                CurrentSurface = MainSurface;
                Device.SetRenderTarget(0, MainSurface);
            }
            catch
            {
            }
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

                Parameters = null;
                if (Sprite != null)
                {
                    if (!Sprite.Disposed)
                        Sprite.Dispose();

                    Sprite = null;
                }

                if (Line != null)
                {
                    if (!Line.Disposed)
                        Line.Dispose();

                    Line = null;
                }

                if (CurrentSurface != null)
                {
                    if (!CurrentSurface.Disposed)
                        CurrentSurface.Dispose();

                    CurrentSurface = null;
                }

                if (MainSurface != null)
                {
                    if (!MainSurface.Disposed)
                        MainSurface.Dispose();

                    MainSurface = null;
                }

                if (Device != null)
                {
                    if (!Device.Disposed)
                        Device.Dispose();

                    Device = null;
                }
                if (AttributeTexture != null)
                {
                    if (!AttributeTexture.Disposed)
                        AttributeTexture.Dispose();

                    AttributeTexture = null;
                }

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
                DeviceLost = false;


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

    public sealed class MirLibrary : IDisposable
    {
        public readonly object LoadLocker = new object();

        public string FileName;

        private FileStream _FStream;
        private BinaryReader _BReader;

        public bool Loaded, Loading;

        public MirImage[] Images;

        public readonly DXManager Manager;

        public MirLibrary(string fileName, DXManager manager)
        {
            _FStream = File.OpenRead(fileName);
            _BReader = new BinaryReader(_FStream);

            Manager = manager;
        }
        public void ReadLibrary()
        {
            lock (LoadLocker)
            {
                if (Loading) return;
                Loading = true;
            }

            if (_BReader == null)
            {
                Loaded = true;
                return;
            }

            using (MemoryStream mstream = new MemoryStream(_BReader.ReadBytes(_BReader.ReadInt32())))
            using (BinaryReader reader = new BinaryReader(mstream))
            {
                Images = new MirImage[reader.ReadInt32()];

                for (int i = 0; i < Images.Length; i++)
                {
                    if (!reader.ReadBoolean()) continue;

                    Images[i] = new MirImage(reader, Manager);
                }
            }


            Loaded = true;
        }


        public Size GetSize(int index)
        {
            if (!CheckImage(index)) return Size.Empty;

            return new Size(Images[index].Width, Images[index].Height);
        }
        public Point GetOffSet(int index)
        {
            if (!CheckImage(index)) return Point.Empty;

            return new Point(Images[index].OffSetX, Images[index].OffSetY);
        }
        public MirImage GetImage(int index)
        {
            if (!CheckImage(index)) return null;

            return Images[index];
        }
        public MirImage CreateImage(int index, ImageType type)
        {
            if (!CheckImage(index)) return null;

            MirImage image = Images[index];

            Texture texture;

            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid) image.CreateImage(_BReader);
                    texture = image.Image;
                    break;
                case ImageType.Shadow:
                    if (!image.ShadowValid) image.CreateShadow(_BReader);
                    texture = image.Shadow;
                    break;
                case ImageType.Overlay:
                    if (!image.OverlayValid) image.CreateOverlay(_BReader);
                    texture = image.Overlay;
                    break;
                default:
                    return null;
            }

            if (texture == null) return null;

            return image;
        }

        private bool CheckImage(int index)
        {
            if (!Loaded) ReadLibrary();

            while (!Loaded)
                Thread.Sleep(1);

            return index >= 0 && index < Images.Length && Images[index] != null;
        }

        public bool VisiblePixel(int index, Point location, bool accurate = true, bool offSet = false)
        {
            if (!CheckImage(index)) return false;

            MirImage image = Images[index];

            if (offSet)
                location = new Point(location.X - image.OffSetX, location.Y - image.OffSetY);

            return image.VisiblePixel(location, accurate);
        }

        public void Draw(int index, float x, float y, Color4 colour, Rectangle area, float opacity, ImageType type, byte shadow = 0)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];

            Texture texture;

            float oldOpacity = Manager.Opacity;
            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid) image.CreateImage(_BReader);
                    texture = image.Image;
                    break;
                case ImageType.Shadow:
                    if (!image.ShadowValid) image.CreateShadow(_BReader);
                    texture = image.Shadow;

                    if (texture == null)
                    {
                        if (!image.ImageValid) image.CreateImage(_BReader);
                        texture = image.Image;

                        switch (image.ShadowType)
                        {
                            case 177:
                            case 176:
                            case 49:
                                Matrix m = Matrix.Scaling(1F, 0.5f, 0);

                                m.M21 = -0.50F;
                                Manager.Sprite.Transform = m * Matrix.Translation(x + image.Height / 2, y, 0);

                                Manager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);
                                if (oldOpacity != 0.5F) Manager.SetOpacity(0.5F);

                                Manager.Sprite.Draw(texture, Vector3.Zero, Vector3.Zero, Color.Black);

                                Manager.SetOpacity(oldOpacity);
                                Manager.Sprite.Transform = Matrix.Identity;
                                Manager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point);

                                image.ExpireTime = SEnvir.Now.AddMinutes(10);
                                break;
                            case 50:
                                if (oldOpacity != 0.5F) Manager.SetOpacity(0.5F);

                                Manager.Sprite.Draw(texture, Vector3.Zero, new Vector3(x, y, 0), Color.Black);

                                Manager.SetOpacity(oldOpacity);

                                image.ExpireTime = SEnvir.Now.AddMinutes(10);
                                break;
                        }



                        return;
                    }
                    break;
                case ImageType.Overlay:
                    if (!image.OverlayValid) image.CreateOverlay(_BReader);
                    texture = image.Overlay;
                    break;
                default:
                    return;
            }

            if (texture == null) return;

            Manager.SetOpacity(opacity);

            Manager.Sprite.Draw(texture, area, Vector3.Zero, new Vector3(x, y, 0), colour);

            Manager.SetOpacity(oldOpacity);

            image.ExpireTime = SEnvir.Now.AddMinutes(10);
        }
        public void Draw(int index, float x, float y, Color4 colour, bool useOffSet, float opacity, ImageType type)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];

            Texture texture;

            float oldOpacity = Manager.Opacity;
            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid) image.CreateImage(_BReader);
                    texture = image.Image;
                    if (useOffSet)
                    {
                        x += image.OffSetX;
                        y += image.OffSetY;
                    }
                    break;
                case ImageType.Shadow:
                    if (!image.ShadowValid) image.CreateShadow(_BReader);
                    texture = image.Shadow;

                    if (useOffSet)
                    {
                        x += image.ShadowOffSetX;
                        y += image.ShadowOffSetY;
                    }


                    if (texture == null)
                    {
                        if (!image.ImageValid) image.CreateImage(_BReader);
                        texture = image.Image;

                        switch (image.ShadowType)
                        {
                            case 177:
                            case 176:
                            case 49:
                                Matrix m = Matrix.Scaling(1F, 0.5f, 0);

                                m.M21 = -0.50F;
                                Manager.Sprite.Transform = m * Matrix.Translation(x + image.Height / 2, y, 0);

                                Manager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);
                                if (oldOpacity != 0.5F) Manager.SetOpacity(0.5F);

                                Manager.Sprite.Draw(texture, Vector3.Zero, Vector3.Zero, Color.Black);

                                Manager.SetOpacity(oldOpacity);
                                Manager.Sprite.Transform = Matrix.Identity;
                                Manager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point);

                                image.ExpireTime = SEnvir.Now.AddMinutes(10);
                                break;
                            case 50:
                                if (oldOpacity != 0.5F) Manager.SetOpacity(0.5F);

                                Manager.Sprite.Draw(texture, Vector3.Zero, new Vector3(x, y, 0), Color.Black);

                                Manager.SetOpacity(oldOpacity);

                                image.ExpireTime = SEnvir.Now.AddMinutes(10);
                                break;
                        }



                        return;
                    }

                    break;
                case ImageType.Overlay:
                    if (!image.OverlayValid) image.CreateOverlay(_BReader);
                    texture = image.Overlay;

                    if (useOffSet)
                    {
                        x += image.OffSetX;
                        y += image.OffSetY;
                    }
                    break;
                default:
                    return;
            }

            if (texture == null) return;

            Manager.SetOpacity(opacity);

            Manager.Sprite.Draw(texture, Vector3.Zero, new Vector3(x, y, 0), colour);

            Manager.SetOpacity(oldOpacity);

            image.ExpireTime = SEnvir.Now.AddMinutes(10);
        }
        public void DrawBlend(int index, float x, float y, Color4 colour, bool useOffSet, float rate, ImageType type, byte shadow = 0)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];

            Texture texture;

            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid) image.CreateImage(_BReader);
                    texture = image.Image;
                    if (useOffSet)
                    {
                        x += image.OffSetX;
                        y += image.OffSetY;
                    }
                    break;
                case ImageType.Shadow:
                    return;
                case ImageType.Overlay:
                    if (!image.OverlayValid) image.CreateOverlay(_BReader);
                    texture = image.Overlay;

                    if (useOffSet)
                    {
                        x += image.OffSetX;
                        y += image.OffSetY;
                    }
                    break;
                default:
                    return;
            }
            if (texture == null) return;


            bool oldBlend = Manager.Blending;
            float oldRate = Manager.BlendRate;

            Manager.SetBlend(true, rate);

            Manager.Sprite.Draw(texture, Vector3.Zero, new Vector3(x, y, 0), colour);

            Manager.SetBlend(oldBlend, oldRate);

            image.ExpireTime = SEnvir.Now.AddMinutes(10);
        }


        #region IDisposable Support

        public bool IsDisposed { get; private set; }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsDisposed = true;

                if (Images != null)
                {
                    foreach (MirImage image in Images)
                        image?.Dispose();
                }


                Images = null;


                _FStream?.Dispose();
                _FStream = null;

                _BReader?.Dispose();
                _BReader = null;

                Loading = false;
                Loaded = false;
            }

        }

        ~MirLibrary()
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
    
    public sealed class MirImage : IDisposable
    {
        public int Position;

        public DXManager Manager;

        #region Texture

        public short Width;
        public short Height;
        public short OffSetX;
        public short OffSetY;
        public byte ShadowType;
        public Texture Image;
        public bool ImageValid { get; private set; }
        public unsafe byte* ImageData;
        public int ImageDataSize
        {
            get
            {
                int w = Width + (4 - Width % 4) % 4;
                int h = Height + (4 - Height % 4) % 4;

                return w * h / 2;
            }
        }
        #endregion

        #region Shadow
        public short ShadowWidth;
        public short ShadowHeight;

        public short ShadowOffSetX;
        public short ShadowOffSetY;

        public Texture Shadow;
        public bool ShadowValid { get; private set; }
        public unsafe byte* ShadowData;
        public int ShadowDataSize
        {
            get
            {
                int w = ShadowWidth + (4 - ShadowWidth % 4) % 4;
                int h = ShadowHeight + (4 - ShadowHeight % 4) % 4;

                return w * h / 2;
            }
        }
        #endregion

        #region Overlay
        public short OverlayWidth;
        public short OverlayHeight;

        public Texture Overlay;
        public bool OverlayValid { get; private set; }
        public unsafe byte* OverlayData;
        public int OverlayDataSize
        {
            get
            {
                int w = OverlayWidth + (4 - OverlayWidth % 4) % 4;
                int h = OverlayHeight + (4 - OverlayHeight % 4) % 4;

                return w * h / 2;
            }
        }
        #endregion


        public DateTime ExpireTime;

        public MirImage(BinaryReader reader, DXManager manager)
        {
            Position = reader.ReadInt32();

            Width = reader.ReadInt16();
            Height = reader.ReadInt16();
            OffSetX = reader.ReadInt16();
            OffSetY = reader.ReadInt16();

            ShadowType = reader.ReadByte();
            ShadowWidth = reader.ReadInt16();
            ShadowHeight = reader.ReadInt16();
            ShadowOffSetX = reader.ReadInt16();
            ShadowOffSetY = reader.ReadInt16();

            OverlayWidth = reader.ReadInt16();
            OverlayHeight = reader.ReadInt16();

            Manager = manager;
        }

        public unsafe bool VisiblePixel(Point p, bool acurrate)
        {
            if (p.X < 0 || p.Y < 0 || !ImageValid || ImageData == null) return false;

            int w = Width + (4 - Width % 4) % 4;
            int h = Height + (4 - Height % 4) % 4;

            if (p.X >= w || p.Y >= h)
                return false;

            int x = (p.X - p.X % 4) / 4;
            int y = (p.Y - p.Y % 4) / 4;
            int index = (y * (w / 4) + x) * 8;

            int col0 = ImageData[index + 1] << 8 | ImageData[index], col1 = ImageData[index + 3] << 8 | ImageData[index + 2];

            if (col0 == 0 && col1 == 0) return false;

            if (!acurrate || col1 < col0) return true;

            x = p.X % 4;
            y = p.Y % 4;
            x *= 2;

            return (ImageData[index + 4 + y] & 1 << x) >> x != 1 || (ImageData[index + 4 + y] & 1 << x + 1) >> x + 1 != 1;
        }


        public unsafe void DisposeTexture()
        {
            if (Image != null && !Image.Disposed)
                Image.Dispose();

            if (Shadow != null && !Shadow.Disposed)
                Shadow.Dispose();

            if (Overlay != null && !Overlay.Disposed)
                Overlay.Dispose();

            ImageData = null;
            ShadowData = null;
            OverlayData = null;

            Image = null;
            Shadow = null;
            Overlay = null;

            ImageValid = false;
            ShadowValid = false;
            OverlayValid = false;

            ExpireTime = DateTime.MinValue;

            Manager.TextureList.Remove(this);
        }

        public unsafe void CreateImage(BinaryReader reader)
        {
            if (Position == 0) return;

            int w = Width + (4 - Width % 4) % 4;
            int h = Height + (4 - Height % 4) % 4;

            if (w == 0 || h == 0) return;

            Image = new Texture(Manager.Device, w, h, 1, Usage.None, Format.Dxt1, Pool.Managed);
            DataRectangle rect = Image.LockRectangle(0, LockFlags.Discard);
            ImageData = (byte*)rect.Data.DataPointer;

            lock (reader)
            {
                reader.BaseStream.Seek(Position, SeekOrigin.Begin);
                rect.Data.Write(reader.ReadBytes(ImageDataSize), 0, ImageDataSize);
            }

            Image.UnlockRectangle(0);
            rect.Data.Dispose();

            ImageValid = true;
            ExpireTime = SEnvir.Now.AddMinutes(30);
            Manager.TextureList.Add(this);
        }
        public unsafe void CreateShadow(BinaryReader reader)
        {
            if (Position == 0) return;

            if (!ImageValid)
                CreateImage(reader);

            int w = ShadowWidth + (4 - ShadowWidth % 4) % 4;
            int h = ShadowHeight + (4 - ShadowHeight % 4) % 4;

            if (w == 0 || h == 0) return;

            Shadow = new Texture(Manager.Device, w, h, 1, Usage.None, Format.Dxt1, Pool.Managed);
            DataRectangle rect = Shadow.LockRectangle(0, LockFlags.Discard);
            ShadowData = (byte*)rect.Data.DataPointer;

            lock (reader)
            {
                reader.BaseStream.Seek(Position + ImageDataSize, SeekOrigin.Begin);
                rect.Data.Write(reader.ReadBytes(ShadowDataSize), 0, ShadowDataSize);
            }

            Shadow.UnlockRectangle(0);
            rect.Data.Dispose();

            ShadowValid = true;
        }
        public unsafe void CreateOverlay(BinaryReader reader)
        {
            if (Position == 0) return;

            if (!ImageValid)
                CreateImage(reader);

            int w = OverlayWidth + (4 - OverlayWidth % 4) % 4;
            int h = OverlayHeight + (4 - OverlayHeight % 4) % 4;

            if (w == 0 || h == 0) return;

            Overlay = new Texture(Manager.Device, w, h, 1, Usage.None, Format.Dxt1, Pool.Managed);
            DataRectangle rect = Overlay.LockRectangle(0, LockFlags.Discard);
            OverlayData = (byte*)rect.Data.DataPointer;

            lock (reader)
            {
                reader.BaseStream.Seek(Position + ImageDataSize + ShadowDataSize, SeekOrigin.Begin);
                rect.Data.Write(reader.ReadBytes(OverlayDataSize), 0, OverlayDataSize);
            }

            Overlay.UnlockRectangle(0);
            rect.Data.Dispose();

            OverlayValid = true;
        }


        #region IDisposable Support

        public bool IsDisposed { get; private set; }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsDisposed = true;

                Position = 0;

                Width = 0;
                Height = 0;
                OffSetX = 0;
                OffSetY = 0;

                ShadowWidth = 0;
                ShadowHeight = 0;
                ShadowOffSetX = 0;
                ShadowOffSetY = 0;

                OverlayWidth = 0;
                OverlayHeight = 0;
            }

        }

        public void Dispose()
        {
            Dispose(!IsDisposed);
            GC.SuppressFinalize(this);
        }
        ~MirImage()
        {
            Dispose(false);
        }

        #endregion

    }

    public enum ImageType
    {
        Image,
        Shadow,
        Overlay,
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
        public Texture ControlTexture { get; set; }
        public Size TextureSize { get; set; }
        public Surface ControlSurface { get; set; }
        public DateTime ExpireTime { get; protected set; }

        protected virtual void CreateTexture()
        {
            if (ControlTexture == null || Size != TextureSize)
            {
                DisposeTexture();
                TextureSize = Size;
                ControlTexture = new Texture(Manager.Device, TextureSize.Width, TextureSize.Height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default); ;
                ControlSurface = ControlTexture.GetSurfaceLevel(0);
                Manager.Map = this;
            }

            Surface previous = Manager.CurrentSurface;
            Manager.SetSurface(ControlSurface);

            Manager.Device.Clear(ClearFlags.Target, Color.Black, 0, 0);

            OnClearTexture();

            Manager.SetSurface(previous);
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
            if (ControlTexture != null)
            {
                if (!ControlTexture.Disposed)
                    ControlTexture.Dispose();

                ControlTexture = null;
            }

            if (ControlSurface != null)
            {
                if (!ControlSurface.Disposed)
                    ControlSurface.Dispose();

                ControlSurface = null;
            }

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
            
            DrawControl();
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
            int maxX = Math.Min(Width - 1, StartX + (int) Math.Ceiling(Size.Width/CellWidth));

            int minY = Math.Max(0, StartY - 1);
            int maxY = Math.Min(Height - 1, StartY + (int) Math.Ceiling(Size.Height/CellHeight));

            Matrix scale = Matrix.Scaling(Zoom, Zoom, 1);

            for (int y = minY; y <= maxY; y++)
            {
                if (y%2 != 0) continue;

                float drawY = (y - StartY)*BaseCellHeight;

                for (int x = minX; x <= maxX; x++)
                {
                    if (x%2 != 0) continue;

                    float drawX = (x - StartX)*BaseCellWidth;

                    Cell tile = Cells[x, y];

                    MirLibrary library;
                    LibraryFile file;

                    if (!Libraries.KROrder.TryGetValue(tile.BackFile, out file)) continue;

                    if (!Manager.LibraryList.TryGetValue(file, out library)) continue;

                    Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY, 0), scale);

                    library.Draw(tile.BackImage, 0, 0, Color.White, false, 1F, ImageType.Image);
                }
            }

            for (int y = minY; y <= maxY; y++)
            {
                float drawY = (y - StartY + 1)*BaseCellHeight;

                for (int x = minX; x <= maxX; x++)
                {
                    float drawX = (x - StartX)*BaseCellWidth;

                    Cell cell = Cells[x, y];

                    MirLibrary library;
                    LibraryFile file;

                    if (Libraries.KROrder.TryGetValue(cell.MiddleFile, out file) && file != LibraryFile.Tilesc && Manager.LibraryList.TryGetValue(file, out library))
                    {
                        int index = cell.MiddleImage - 1;

                        if (cell.MiddleAnimationFrame > 1 && cell.MiddleAnimationFrame < 255)
                            continue; //   index += GameScene.Game.MapControl.Animation % cell.MiddleAnimationFrame;

                        Size s = library.GetSize(index);

                        if ((s.Width == CellWidth && s.Height == CellHeight) || (s.Width == CellWidth*2 && s.Height == CellHeight*2))
                        {
                            Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY - BaseCellHeight, 0), scale);
                            
                            library.Draw(index, 0, 0, Color.White, false, 1F, ImageType.Image);
                        }
                    }


                    if (Libraries.KROrder.TryGetValue(cell.FrontFile, out file) && file != LibraryFile.Tilesc && Manager.LibraryList.TryGetValue(file, out library))
                    {
                        int index = cell.FrontImage - 1;

                        if (cell.FrontAnimationFrame > 1 && cell.FrontAnimationFrame < 255)
                            continue; //  index += GameScene.Game.MapControl.Animation % cell.FrontAnimationFrame;

                        Size s = library.GetSize(index);

                        if ((s.Width == CellWidth && s.Height == CellHeight) || (s.Width == CellWidth*2 && s.Height == CellHeight*2))
                        {
                            Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY - BaseCellHeight, 0), scale);
                            
                            library.Draw(index, 0, 0, Color.White, false, 1F, ImageType.Image);
                        }
                    }
                }
            }

            maxY = Math.Min(Height - 1, StartY + 20 + (int)Math.Ceiling(Size.Height / CellHeight) );
            for (int y = minY; y <= maxY; y++)
            {
                float drawY = (y - StartY + 1)*BaseCellHeight;

                for (int x = minX; x <= maxX; x++)
                {
                    float drawX = (x - StartX)*BaseCellWidth;

                    Cell cell = Cells[x, y];

                    MirLibrary library;
                    LibraryFile file;

                    if (Libraries.KROrder.TryGetValue(cell.MiddleFile, out file) && file != LibraryFile.Tilesc && Manager.LibraryList.TryGetValue(file, out library))
                    {
                        int index = cell.MiddleImage - 1;

                        bool blend = false;
                        if (cell.MiddleAnimationFrame > 1 && cell.MiddleAnimationFrame < 255)
                        {
                            index += Animation%(cell.MiddleAnimationFrame & 0x4F);
                            blend = (cell.MiddleAnimationFrame & 0x50) > 0;
                        }

                        Size s = library.GetSize(index);

                        if ((s.Width != CellWidth || s.Height != CellHeight) && (s.Width != CellWidth*2 || s.Height != CellHeight*2))
                        {
                            Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY - s.Height, 0), scale);
                            
                            if (!blend)
                                library.Draw(index, 0, 0, Color.White, false, 1F, ImageType.Image);
                            else
                                library.DrawBlend(index, 0, 0, Color.White, false, 0.5F, ImageType.Image);
                        }
                    }


                    if (Libraries.KROrder.TryGetValue(cell.FrontFile, out file) && file != LibraryFile.Tilesc && Manager.LibraryList.TryGetValue(file, out library))
                    {
                        int index = cell.FrontImage - 1;

                        bool blend = false;
                        if (cell.FrontAnimationFrame > 1 && cell.FrontAnimationFrame < 255)
                        {
                            index += Animation%(cell.FrontAnimationFrame & 0x4F);
                            blend = (cell.MiddleAnimationFrame & 0x50) > 0;
                        }

                        Size s = library.GetSize(index);


                        if ((s.Width != CellWidth || s.Height != CellHeight) && (s.Width != CellWidth*2 || s.Height != CellHeight*2))
                        {
                            Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY - s.Height, 0), scale);
                            
                            if (!blend)
                                library.Draw(index, 0, 0, Color.White, false, 1F, ImageType.Image);
                            else
                                library.DrawBlend(index, 0, 0, Color.White, false, 0.5F, ImageType.Image);
                        }
                    }
                }
            }

            //Invalid Tile = 59
            //Selected Tile = 58
            

            maxY = Math.Min(Height - 1, StartY + (int)Math.Ceiling(Size.Height / CellHeight));


            Manager.SetOpacity(0.35F);
            for (int y = minY; y <= maxY; y++)
            {
                float drawY = (y - StartY )*BaseCellHeight;

                for (int x = minX; x <= maxX; x++)
                {
                    float drawX = (x - StartX)*BaseCellWidth;

                    Cell tile = Cells[x, y];

                    if (tile.Flag != AttributeSelection)
                    {
                        if (!DrawAttributes) continue;

                        Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY, 0), scale);

                        //markLibrary.Draw(59, 0, 0, Color.White, false, 1F, ImageType.Image);
                        Manager.Sprite.Draw(Manager.AttributeTexture, Vector3.Zero, Vector3.Zero, Color.Red);
                    }
                    else
                    {
                        if (!DrawSelection) continue;
                        if (!Selection.Contains(new Point(x, y))) continue;

                        Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY, 0), scale);

                        Manager.Sprite.Draw(Manager.AttributeTexture, Vector3.Zero, Vector3.Zero, Color.Yellow);

                        //markLibrary.Draw(58, 0, 0, Color.Lime, false, 1F, ImageType.Image);
                        //If Selected.
                    }
                }
            }
            Manager.Sprite.Flush();

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





            Manager.Sprite.Transform = Matrix.Identity;
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
                    path = Config.MapPath + fileName + ".map";
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
                            Cells[x, y].FrontAnimationFrame &= 0x8F; //Probably a Blend Flag

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

                if (ControlTexture != null)
                {
                    if (!ControlTexture.Disposed)
                        ControlTexture.Dispose();

                    ControlTexture = null;
                }

                if (ControlSurface != null)
                {
                    if (!ControlSurface.Disposed)
                        ControlSurface.Dispose();

                    ControlSurface = null;
                }

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
            public int BackFile;
            public int BackImage;

            public int MiddleFile;
            public int MiddleImage;

            public int FrontFile;
            public int FrontImage;

            public int FrontAnimationFrame;
            public int FrontAnimationTick;

            public int MiddleAnimationFrame;
            public int MiddleAnimationTick;

            public int Light;

            public bool Flag;
        }

    }


}