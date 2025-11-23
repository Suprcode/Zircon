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
        public static Device Device { get; private set; }
        public static Sprite Sprite { get; private set; }
        public static Line Line { get; private set; }

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
            Direct3D direct3D = new Direct3D();

            // Determine the correct adapter index based on the selected monitor index.
            int adapterIndex = 0;// Config.SelectedMonitorIndex;
            if (adapterIndex < 0 || adapterIndex >= direct3D.Adapters.Count)
                adapterIndex = 0;
            //Config.SelectedMonitorIndex = adapterIndex;

            try
            {
                _parameters = new PresentParameters
                {
                    Windowed = !Config.FullScreen,
                    SwapEffect = SwapEffect.Discard,
                    BackBufferFormat = Format.X8R8G8B8,
                    PresentationInterval = Config.VSync ? PresentInterval.Default : PresentInterval.Immediate,
                    PresentFlags = PresentFlags.LockableBackBuffer,
                    DeviceWindowHandle = CEnvir.Target.Handle,
                };
                // Set BackBuffer dimensions based on mode
                if (Config.FullScreen)
                {
                    _parameters.BackBufferWidth = CEnvir.Target.ClientSize.Width;
                    _parameters.BackBufferHeight = CEnvir.Target.ClientSize.Height;
                }
                else
                {
                    _parameters.BackBufferWidth = CEnvir.Target.ClientSize.Width;
                    _parameters.BackBufferHeight = CEnvir.Target.ClientSize.Height;
                }

                // Use the determined adapter index when creating the Device.
                //Debug.WriteLine($"Attempting to create device on Adapter Index: {adapterIndex}");
                Device = new Device(direct3D, adapterIndex, DeviceType.Hardware, CEnvir.Target.Handle, CreateFlags.HardwareVertexProcessing, _parameters);


                // The rest of your code remains unchanged
                AdapterInformation adapterInfo = direct3D.Adapters[adapterIndex];
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

            CenterOnSelectedMonitor();
        }

        private static unsafe void LoadTextures()
        {
            Sprite = new Sprite(Device);
            Line = new Line(Device) { Width = 1F };

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
            {
                ControlList[i].DisposeTexture();
            }

            for (int i = TextureList.Count - 1; i >= 0; i--)
            {
                TextureList[i].DisposeTexture();
            }
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

                if (Device.Direct3D != null)
                {
                    if (!Device.Direct3D.IsDisposed)
                    {
                        Device.Direct3D.Dispose();
                    }
                }

                if (!Device.IsDisposed)
                {
                    Device.Dispose();
                }

                Device = null;
            }
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

            parameters.Windowed = !Config.FullScreen;
            parameters.BackBufferWidth = CEnvir.Target.ClientSize.Width;
            parameters.BackBufferHeight = CEnvir.Target.ClientSize.Height;
            parameters.PresentationInterval = Config.VSync ? PresentInterval.Default : PresentInterval.Immediate;

            Device.Reset(parameters);
            _parameters = parameters;

            LoadTextures();

            DeviceLost = false;
        }

        public static void RequestReset()
        {
            ResetDevice();
        }

        public static void CenterOnSelectedMonitor()
        {
            int index = 0;// Config.SelectedMonitorIndex;
            if (index < 0 || index >= Screen.AllScreens.Length)
                index = 0;

            Screen selectedScreen = Screen.AllScreens[index];

            GdiRectangle bounds = selectedScreen.Bounds;
            int x = bounds.X + (bounds.Width - CEnvir.Target.Width) / 2;
            int y = bounds.Y + (bounds.Height - CEnvir.Target.Height) / 2;
            CEnvir.Target.Location = new GdiPoint(x, y);
        }

        public static void SetGameWindowToMonitor(int monitorIndex)
        {
            if (monitorIndex < 0 || monitorIndex >= Screen.AllScreens.Length)
                monitorIndex = 0;

            //Config.SelectedMonitorIndex = monitorIndex;
            Screen targetScreen = Screen.AllScreens[monitorIndex];

            if (Config.FullScreen)
            {
                CEnvir.Target.Bounds = targetScreen.Bounds;
                _parameters.BackBufferWidth = targetScreen.Bounds.Width;
                _parameters.BackBufferHeight = targetScreen.Bounds.Height;
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

            Config.FullScreen = !Config.FullScreen;
            DXConfigWindow.ActiveConfig.FullScreenCheckBox.Checked = Config.FullScreen;

            CEnvir.Target.FormBorderStyle = (Config.FullScreen || Config.Borderless) ? FormBorderStyle.None : FormBorderStyle.FixedDialog;
            CEnvir.Target.MaximizeBox = false;

            CEnvir.Target.ClientSize = DXControl.ActiveScene.Size;
            ResetDevice();
            CEnvir.Target.ClientSize = DXControl.ActiveScene.Size;
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
    }

}
