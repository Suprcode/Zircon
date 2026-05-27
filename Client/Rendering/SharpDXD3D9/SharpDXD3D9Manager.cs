using Client.Controls;
using Client.Envir;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Blend = SharpDX.Direct3D9.Blend;
using DxColor = SharpDX.Color;
using GdiColor = System.Drawing.Color;
using GdiPoint = System.Drawing.Point;
using GdiRectangle = System.Drawing.Rectangle;

namespace Client.Rendering.SharpDXD3D9
{
    public static class SharpDXD3D9Manager
    {
        public static Graphics Graphics { get; private set; }

        public static List<Size> ValidResolutions = new List<Size>();
        private static Size MinimumResolution = new Size(1024, 768);

        public static List<Size> ValidDisplays = new List<Size>();
        private static PresentParameters _parameters;
        public static PresentParameters Parameters => _parameters;
        private static Direct3D _direct3D;
        private static int _adapterIndex;
        private static readonly DisplayModeManager _displayMode = new DisplayModeManager();
        private static System.Windows.Forms.Timer _postTogglePlacementTimer;
        public static Device Device { get; private set; }
        public static Sprite Sprite { get; private set; }
        public static Line Line { get; private set; }
        public static SharpDXD3D9SpriteRenderer SpriteRenderer { get; private set; }

        public static Surface CurrentSurface { get; private set; }
        public static Surface MainSurface { get; private set; }

        public static float Opacity { get; private set; } = 1F;

        public static bool Blending { get; private set; }
        public static float BlendRate { get; private set; } = 1F;
        public static BlendMode BlendMode { get; private set; } = BlendMode.NORMAL;

        public static bool DeviceLost { get; set; }

        public static List<DXControl> ControlList { get; } = new List<DXControl>();
        public static List<MirImage> TextureList { get; } = new List<MirImage>();
        public static List<DXSound> SoundList { get; } = new List<DXSound>();

        public static Texture ScratchTexture;
        public static Surface ScratchSurface;

        public static byte[] PalleteData;
        private static Texture _ColourPallete;
        public static Texture ColourPallete
        {
            get
            {
                if (_ColourPallete == null || _ColourPallete.IsDisposed)
                {
                    _ColourPallete = null;

                    if (PalleteData != null)
                    {
                        _ColourPallete = new Texture(Device, 200, 149, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
                        DataRectangle rect = _ColourPallete.LockRectangle(0, LockFlags.Discard);
                        Marshal.Copy(PalleteData, 0, rect.DataPointer, PalleteData.Length);
                        _ColourPallete.UnlockRectangle(0);
                    }
                }

                return _ColourPallete;
            }
        }


        public const int LightWidth = 1024;
        public const int LightHeight = 768;

        public static byte[] LightData;
        private static Texture _LightTexture;
        public static Texture LightTexture
        {
            get
            {
                if (_LightTexture == null || _LightTexture.IsDisposed)
                {
                    CreateLight();
                }

                return _LightTexture;
            }
        }

        private static Surface _LightSurface;
        public static Surface LightSurface
        {
            get
            {
                if (_LightSurface == null || _LightSurface.IsDisposed)
                {
                    _LightSurface = LightTexture.GetSurfaceLevel(0);
                }

                return _LightSurface;
            }
        }

        public static Texture PoisonTexture;

        static SharpDXD3D9Manager()
        {
            Graphics = Graphics.FromHwnd(IntPtr.Zero);
            ConfigureGraphics(Graphics);
        }


        public static void Create()
        {
            if (Device != null || _direct3D != null)
                Unload();

            ApplyWindowStyle();

            if (CEnvir.Target != null && CEnvir.Target.ClientSize != Config.GameSize)
                CEnvir.Target.ClientSize = Config.GameSize;

            ApplyWindowBounds();

            _direct3D = new Direct3D();

            int adapterIndex = GetSelectedAdapterIndex();
            _adapterIndex = adapterIndex;

            try
            {
                _parameters = new PresentParameters
                {
                    Windowed = ShouldUseWindowedPresentation(),
                    SwapEffect = SwapEffect.Discard,
                    BackBufferFormat = Format.X8R8G8B8,
                    PresentationInterval = Config.VSync ? PresentInterval.Default : PresentInterval.Immediate,
                    PresentFlags = PresentFlags.LockableBackBuffer,
                    DeviceWindowHandle = CEnvir.Target.Handle,
                    FullScreenRefreshRateInHz = 0,
                };

                Size backBufferSize = GetBackBufferSize();
                _parameters.BackBufferWidth = backBufferSize.Width;
                _parameters.BackBufferHeight = backBufferSize.Height;

                // Use the determined adapter index when creating the Device.
                //Debug.WriteLine($"Attempting to create device on Adapter Index: {adapterIndex}");
                Device = new Device(_direct3D, adapterIndex, DeviceType.Hardware, CEnvir.Target.Handle, CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded | CreateFlags.FpuPreserve, _parameters);

                if (Config.FullScreen)
                    ApplyWindowBounds();

                // The rest of your code remains unchanged
                AdapterInformation adapterInfo = _direct3D.Adapters[adapterIndex];
                var modes = adapterInfo.GetDisplayModes(Format.X8R8G8B8);

                foreach (DisplayMode mode in modes)
                {
                    Size s = new Size(mode.Width, mode.Height);
                    if (s.Width < MinimumResolution.Width || s.Height < MinimumResolution.Height) continue;

                    if (!ValidResolutions.Contains(s))
                        ValidResolutions.Add(s);
                }
                ValidResolutions.Sort((s1, s2) => (s1.Width * s1.Height).CompareTo(s2.Width * s2.Height));

                LoadTextures();

                const string path = @".\Data\Pallete.png";

                if (File.Exists(path))
                {
                    using (Bitmap pallete = new Bitmap(path))
                    {
                        BitmapData data = pallete.LockBits(new GdiRectangle(GdiPoint.Empty, pallete.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                        PalleteData = new byte[pallete.Width * pallete.Height * 4];
                        Marshal.Copy(data.Scan0, PalleteData, 0, PalleteData.Length);

                        pallete.UnlockBits(data);
                    }
                }
            }
            catch (SharpDXException ex)
            {
                CEnvir.SaveException(ex);
                throw;
            }

            if (!Config.FullScreen)
                CenterOnSelectedMonitor();
        }

        private static unsafe void LoadTextures()
        {
            Sprite = new Sprite(Device);
            Line = new Line(Device) { Width = 1F };
            SpriteRenderer = new SharpDXD3D9SpriteRenderer(Device);

            MainSurface = Device.GetBackBuffer(0, 0);
            CurrentSurface = MainSurface;
            Device.SetRenderTarget(0, MainSurface);


            PoisonTexture = new Texture(Device, 6, 6, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);

            DataRectangle rect = PoisonTexture.LockRectangle(0, LockFlags.Discard);

            int* data = (int*)rect.DataPointer;

            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 6; x++)
                {
                    data[y * 6 + x] = x == 0 || y == 0 || x == 5 || y == 5 ? -16777216 : -1;
                }
            }

            ScratchTexture = new Texture(Device, _parameters.BackBufferWidth, _parameters.BackBufferHeight, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            ScratchSurface = ScratchTexture.GetSurfaceLevel(0);
        }

        private static void CreateLight()
        {
            Texture light = new Texture(Device, LightWidth, LightHeight, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);

            LightData = LightGenerator.CreateLightData(LightWidth, LightHeight);

            DataRectangle rect = light.LockRectangle(0, LockFlags.Discard);
            Marshal.Copy(LightData, 0, rect.DataPointer, LightData.Length);
            light.UnlockRectangle(0);

            _LightTexture = light;
        }
        private static void CleanUp()
        {
            if (Sprite != null)
            {
                if (!Sprite.IsDisposed)
                {
                    Sprite.Dispose();
                }

                Sprite = null;
            }

            if (Line != null)
            {
                if (!Line.IsDisposed)
                {
                    Line.Dispose();
                }

                Line = null;
            }

            if (SpriteRenderer != null)
            {
                SpriteRenderer.Dispose();
                SpriteRenderer = null;
            }

            if (CurrentSurface != null)
            {
                if (!CurrentSurface.IsDisposed)
                {
                    CurrentSurface.Dispose();
                }

                CurrentSurface = null;
            }

            if (_ColourPallete != null)
            {
                if (!_ColourPallete.IsDisposed)
                {
                    _ColourPallete.Dispose();
                }

                _ColourPallete = null;
            }

            if (ScratchTexture != null)
            {
                if (!ScratchTexture.IsDisposed)
                {
                    ScratchTexture.Dispose();
                }

                ScratchTexture = null;
            }

            if (ScratchSurface != null)
            {
                if (!ScratchSurface.IsDisposed)
                {
                    ScratchSurface.Dispose();
                }

                ScratchSurface = null;
            }

            if (PoisonTexture != null)
            {
                if (!PoisonTexture.IsDisposed)
                {
                    PoisonTexture.Dispose();
                }

                PoisonTexture = null;
            }


            if (_LightTexture != null)
            {
                if (!_LightTexture.IsDisposed)
                {
                    _LightTexture.Dispose();
                }

                _LightTexture = null;
            }


            if (_LightSurface != null)
            {
                if (!_LightSurface.IsDisposed)
                {
                    _LightSurface.Dispose();
                }

                _LightSurface = null;
            }

            for (int i = ControlList.Count - 1; i >= 0; i--)
                ControlList[i].DisposeTexture();

            ControlList.Clear();

            for (int i = TextureList.Count - 1; i >= 0; i--)
                TextureList[i].DisposeTexture();

            TextureList.Clear();
        }

        public static void MemoryClear()
        {
            for (int i = ControlList.Count - 1; i >= 0; i--)
            {
                if (CEnvir.Now < ControlList[i].ExpireTime)
                {
                    continue;
                }

                ControlList[i].DisposeTexture();
            }

            for (int i = TextureList.Count - 1; i >= 0; i--)
            {
                if (CEnvir.Now < TextureList[i].ExpireTime)
                {
                    continue;
                }

                TextureList[i].DisposeTexture();
            }

            for (int i = SoundList.Count - 1; i >= 0; i--)
            {
                if (CEnvir.Now < SoundList[i].ExpireTime)
                {
                    continue;
                }

                SoundList[i].DisposeSoundBuffer();
            }
        }

        public static void Unload()
        {
            CleanUp();

            if (Device != null)
            {
                if (!Device.IsDisposed)
                {
                    Device.Dispose();
                }

                Device = null;
            }

            if (_direct3D != null && !_direct3D.IsDisposed)
                _direct3D.Dispose();

            _direct3D = null;

            _displayMode.Restore();
        }

        public static void SetSurface(Surface surface)
        {
            if (CurrentSurface == surface)
            {
                return;
            }

            Sprite.Flush();
            CurrentSurface = surface;
            Device.SetRenderTarget(0, surface);
        }
        public static void SetOpacity(float opacity)
        {
            if (Opacity == opacity)
            {
                return;
            }

            Sprite.Flush();
            Device.SetRenderState(RenderState.AlphaBlendEnable, true);

            if (opacity >= 1 || opacity < 0)
            {
                Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
                Device.SetRenderState(RenderState.SourceBlendAlpha, Blend.One);
                Device.SetRenderState(RenderState.BlendFactor, GdiColor.FromArgb(255, 255, 255, 255).ToArgb());
            }
            else
            {
                Device.SetRenderState(RenderState.SourceBlend, Blend.BlendFactor);
                Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseBlendFactor);
                Device.SetRenderState(RenderState.SourceBlendAlpha, Blend.SourceAlpha);
                Device.SetRenderState(RenderState.BlendFactor, GdiColor.FromArgb((byte)(255 * opacity), (byte)(255 * opacity),
                    (byte)(255 * opacity), (byte)(255 * opacity)).ToArgb());
            }

            Opacity = opacity;
            Sprite.Flush();
        }
        public static void SetBlend(bool value, float rate = 1F, BlendMode mode = BlendMode.NORMAL)
        {
            if (Blending == value && BlendRate == rate && BlendMode == mode) return;

            Blending = value;
            BlendRate = 1F;
            BlendRate = rate;
            BlendMode = mode;

            Sprite.Flush();
            Sprite.End();

            if (Blending)
            {
                Sprite.Begin(SpriteFlags.DoNotSaveState);
                Device.SetRenderState(RenderState.AlphaBlendEnable, true);
                Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
                Device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Modulate);

                switch (BlendMode)
                {
                    case BlendMode.INVLIGHT:
                        Device.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);
                        Device.SetRenderState(RenderState.SourceBlend, Blend.BlendFactor);
                        Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceColor);
                        break;
                    case BlendMode.COLORFY:
                        Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                        Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
                        break;
                    case BlendMode.MASK:
                        Device.SetRenderState(RenderState.SourceBlend, Blend.Zero);
                        Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
                        break;
                    case BlendMode.EFFECTMASK:
                        Device.SetRenderState(RenderState.SourceBlend, Blend.DestinationAlpha);
                        Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
                        break;
                    case BlendMode.HIGHLIGHT:
                        Device.SetRenderState(RenderState.SourceBlend, Blend.BlendFactor);
                        Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
                        break;
                    case BlendMode.LIGHTMAP:
                        Device.SetRenderState(RenderState.SourceBlend, Blend.Zero);
                        Device.SetRenderState(RenderState.DestinationBlend, Blend.SourceColor);
                        break;
                    default:
                        Device.SetRenderState(RenderState.SourceBlend, Blend.InverseDestinationColor);
                        Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
                        break;
                }

                Device.SetRenderState(RenderState.BlendFactor, GdiColor.FromArgb((byte)(255 * rate), (byte)(255 * rate), (byte)(255 * rate), (byte)(255 * rate)).ToArgb());
            }
            else
            {
                Sprite.Begin(SpriteFlags.AlphaBlend);
            }

            Device.SetRenderTarget(0, CurrentSurface);
        }
        public static void SetColour(int colour)
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

        public static void ResetDevice()
        {
            ApplyWindowStyle();
            ApplyWindowBounds();

            CleanUp();

            DeviceLost = true;

            if (CEnvir.Target.ClientSize.Width == 0 || CEnvir.Target.ClientSize.Height == 0)
            {
                return;
            }

            PresentParameters parameters = _parameters;

            if (parameters.BackBufferWidth == 0 || parameters.BackBufferHeight == 0)
            {
                return;
            }

            parameters.Windowed = ShouldUseWindowedPresentation();
            Size backBufferSize = GetBackBufferSize();
            parameters.BackBufferWidth = backBufferSize.Width;
            parameters.BackBufferHeight = backBufferSize.Height;
            parameters.PresentationInterval = Config.VSync ? PresentInterval.Default : PresentInterval.Immediate;
            parameters.PresentFlags = PresentFlags.LockableBackBuffer;
            parameters.FullScreenRefreshRateInHz = 0;

            Device.Reset(parameters);
            _parameters = parameters;

            if (Config.FullScreen)
                ApplyWindowBounds();

            LoadTextures();

            DeviceLost = false;
        }

        public static void RequestReset()
        {
            ResetDevice();
        }

        public static void CenterOnSelectedMonitor()
        {
            CenterWindowOnScreen(RenderingPipelineManager.GetSelectedScreen());
        }

        public static void SetGameWindowToMonitor(int monitorIndex)
        {
            if (monitorIndex < 0 || monitorIndex >= Screen.AllScreens.Length)
                monitorIndex = RenderingPipelineManager.GetSelectedMonitorIndex();

            Screen targetScreen = Screen.AllScreens[monitorIndex];

            if (Config.FullScreen)
            {
                CEnvir.Target.Bounds = RenderingPipelineManager.GetMonitorDisplayBounds(targetScreen);

                if (Device != null && _adapterIndex != GetSelectedAdapterIndex())
                {
                    Create();
                    return;
                }
            }
            else
            {
                CenterOnSelectedMonitor();
            }

            RequestReset();
        }

        public static void AttemptReset()
        {
            try
            {
                Result result = Device.TestCooperativeLevel();

                if (result.Code == ResultCode.DeviceLost.Code)
                {
                    return;
                }

                if (result.Code == ResultCode.DeviceNotReset.Code)
                {
                    ResetDevice();
                    return;
                }

                if (result.Code != ResultCode.Success.Code)
                {
                    return;
                }

                DeviceLost = false;
            }
            catch (Exception ex)
            {
                CEnvir.SaveException(ex);
            }
        }
        public static void AttemptRecovery()
        {
            if (Device == null || Device.IsDisposed)
            {
                return;
            }

            try
            {
                if (Sprite != null && !Sprite.IsDisposed)
                {
                    Sprite.End();
                }
            }
            catch
            {
            }

            try
            {
                if (Device != null && !Device.IsDisposed)
                {
                    Device.EndScene();
                }
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

        public static void ToggleFullScreen()
        {
            if (CEnvir.Target == null) return;

            bool enteringFullScreen = !Config.FullScreen;
            Screen selectedScreen = RenderingPipelineManager.GetSelectedScreen();
            GdiPoint? cursorOffset = CaptureCursorOffset(selectedScreen);

            Config.FullScreen = enteringFullScreen;
            DXConfigWindow.ActiveConfig.FullScreenCheckBox.Checked = Config.FullScreen;

            ApplyWindowStyle();
            ApplyWindowBounds();

            if (Config.FullScreen && _adapterIndex != GetSelectedAdapterIndex())
            {
                Create();
                ApplyPostTogglePlacement(Config.FullScreen, selectedScreen, cursorOffset);
                return;
            }

            ResetDevice();
            ApplyPostTogglePlacement(Config.FullScreen, selectedScreen, cursorOffset);
        }

        public static void SetResolution(Size size)
        {
            if (CEnvir.Target.ClientSize == size)
            {
                return;
            }

            Device.Clear(ClearFlags.Target, DxColor.Black, 0, 0);
            Device.Present();

            CEnvir.Target.ClientSize = size;
            CEnvir.Target.MaximizeBox = false;
            if (!Config.FullScreen)
                CenterOnSelectedMonitor();

            ResetDevice();
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

        private static void ApplyWindowStyle()
        {
            if (CEnvir.Target == null)
                return;

            CEnvir.Target.FormBorderStyle = (Config.FullScreen || Config.Borderless) ? FormBorderStyle.None : FormBorderStyle.FixedDialog;
            CEnvir.Target.MaximizeBox = false;
        }

        private static void ApplyWindowBounds()
        {
            if (CEnvir.Target == null)
                return;

            if (Config.FullScreen)
            {
                Screen selectedScreen = RenderingPipelineManager.GetSelectedScreen();

                if (Config.FullScreen && _displayMode.Apply(selectedScreen, Config.GameSize))
                {
                    string deviceName = selectedScreen.DeviceName;
                    selectedScreen = DisplayModeManager.GetScreenByDeviceName(deviceName, RenderingPipelineManager.GetSelectedScreen());
                    CEnvir.Target.Bounds = RenderingPipelineManager.GetMonitorDisplayBounds(deviceName, selectedScreen.Bounds);
                    return;
                }

                _displayMode.Restore();
                CEnvir.Target.Bounds = RenderingPipelineManager.GetMonitorDisplayBounds(selectedScreen);
                return;
            }

            _displayMode.Restore();

            Size size = DXControl.ActiveScene?.Size ?? Config.GameSize;
            if (CEnvir.Target.ClientSize != size)
                CEnvir.Target.ClientSize = size;

            if (!Config.FullScreen)
                CenterOnSelectedMonitor();
        }

        private static Size GetBackBufferSize()
        {
            if (Config.FullScreen)
                return Config.GameSize;

            Size size = CEnvir.Target.ClientSize;
            return new Size(Math.Max(size.Width, 1), Math.Max(size.Height, 1));
        }

        private static bool ShouldUseWindowedPresentation()
        {
            if (!Config.FullScreen)
                return true;

            return _displayMode.IsActive(RenderingPipelineManager.GetSelectedScreen(), Config.GameSize);
        }

        private static void CenterWindowOnScreen(Screen screen)
        {
            if (CEnvir.Target == null)
                return;

            screen = string.IsNullOrEmpty(screen?.DeviceName)
                ? RenderingPipelineManager.GetSelectedScreen()
                : DisplayModeManager.GetScreenByDeviceName(screen.DeviceName, RenderingPipelineManager.GetSelectedScreen());

            Size size = DXControl.ActiveScene?.Size ?? Config.GameSize;
            if (!Config.FullScreen && CEnvir.Target.ClientSize != size)
                CEnvir.Target.ClientSize = size;

            GdiRectangle bounds = RenderingPipelineManager.GetMonitorDisplayBounds(screen.DeviceName, screen.Bounds);
            int x = bounds.X + (bounds.Width - CEnvir.Target.Width) / 2;
            int y = bounds.Y + (bounds.Height - CEnvir.Target.Height) / 2;
            CEnvir.Target.Location = new GdiPoint(x, y);
        }

        private static GdiPoint? CaptureCursorOffset(Screen screen)
        {
            if (screen == null || screen.Primary)
                return null;

            GdiRectangle bounds = RenderingPipelineManager.GetMonitorDisplayBounds(screen);
            GdiPoint position = Cursor.Position;

            if (!bounds.Contains(position))
                return new GdiPoint(bounds.Width / 2, bounds.Height / 2);

            return new GdiPoint(position.X - bounds.X, position.Y - bounds.Y);
        }

        private static void RestoreCursorToScreen(Screen screen, GdiPoint? cursorOffset)
        {
            if (screen == null || !cursorOffset.HasValue)
                return;

            screen = DisplayModeManager.GetScreenByDeviceName(screen.DeviceName, RenderingPipelineManager.GetSelectedScreen());
            GdiRectangle bounds = RenderingPipelineManager.GetMonitorDisplayBounds(screen.DeviceName, screen.Bounds);

            int x = bounds.X + Math.Max(0, Math.Min(cursorOffset.Value.X, bounds.Width - 1));
            int y = bounds.Y + Math.Max(0, Math.Min(cursorOffset.Value.Y, bounds.Height - 1));
            SetCursorPos(x, y);
        }

        private static void ApplyPostTogglePlacement(bool fullScreen, Screen screen, GdiPoint? cursorOffset)
        {
            ApplyTogglePlacement(fullScreen, screen, cursorOffset);

            if (CEnvir.Target == null || CEnvir.Target.IsDisposed || !CEnvir.Target.IsHandleCreated)
                return;

            try
            {
                CEnvir.Target.BeginInvoke((MethodInvoker)(() =>
                {
                    ApplyTogglePlacement(fullScreen, screen, cursorOffset);
                    QueuePostTogglePlacement(fullScreen, screen, cursorOffset);
                }));
            }
            catch
            {
                QueuePostTogglePlacement(fullScreen, screen, cursorOffset);
            }
        }

        private static void QueuePostTogglePlacement(bool fullScreen, Screen screen, GdiPoint? cursorOffset)
        {
            if (CEnvir.Target == null || CEnvir.Target.IsDisposed)
                return;

            _postTogglePlacementTimer?.Stop();
            _postTogglePlacementTimer?.Dispose();

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = 150 };
            timer.Tick += (o, e) =>
            {
                timer.Stop();
                timer.Dispose();
                if (_postTogglePlacementTimer == timer)
                    _postTogglePlacementTimer = null;

                ApplyTogglePlacement(fullScreen, screen, cursorOffset);
            };

            _postTogglePlacementTimer = timer;
            timer.Start();
        }

        private static void ApplyTogglePlacement(bool fullScreen, Screen screen, GdiPoint? cursorOffset)
        {
            if (fullScreen)
                RestoreCursorToScreen(screen, cursorOffset);
            else
            {
                CenterWindowOnScreen(screen);
                RestoreCursorToScreen(screen, cursorOffset);
            }
        }

        private static int GetSelectedAdapterIndex()
        {
            if (_direct3D == null)
                return 0;

            Screen selectedScreen = RenderingPipelineManager.GetSelectedScreen();

            for (int i = 0; i < _direct3D.Adapters.Count; i++)
            {
                AdapterInformation adapter = _direct3D.Adapters[i];
                if (string.Equals(adapter.Details.DeviceName, selectedScreen.DeviceName, StringComparison.OrdinalIgnoreCase))
                    return i;

                Screen adapterScreen = Screen.FromHandle(adapter.Monitor);

                if (string.Equals(adapterScreen.DeviceName, selectedScreen.DeviceName, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            int fallbackIndex = RenderingPipelineManager.GetSelectedMonitorIndex();
            if (fallbackIndex < 0 || fallbackIndex >= _direct3D.Adapters.Count)
                fallbackIndex = 0;

            return fallbackIndex;
        }

        public static GdiColor HSLToRGB(float h, float s, float l)
        {
            float r, g, b;

            if (s == 0)
            {
                r = g = b = l; // achromatic
            }
            else
            {
                float q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                float p = 2 * l - q;
                r = HueToRGB(p, q, h + 1f / 3f);
                g = HueToRGB(p, q, h);
                b = HueToRGB(p, q, h - 1f / 3f);
            }

            return GdiColor.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }

        private static float HueToRGB(float p, float q, float t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1f / 6f) return p + (q - p) * 6f * t;
            if (t < 1f / 2f) return q;
            if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
            return p;
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int x, int y);
    }

}
