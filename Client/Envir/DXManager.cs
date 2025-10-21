using Client.Controls;
using Client.Extensions;
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
using DataRectangle = SharpDX.DataRectangle;
using Result = SharpDX.Result;
using D3DResultCode = SharpDX.Direct3D9.ResultCode;
using System.Numerics;

namespace Client.Envir
{
    public static class DXManager
    {
        public static Graphics Graphics { get; private set; }

        public static List<Size> ValidResolutions = new List<Size>();
        private static Size MinimumResolution = new Size(1024, 768);

        public static PresentParameters Parameters { get; private set; }
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
                        SharpDX.Utilities.Write(rect.DataPointer, PalleteData, 0, PalleteData.Length);
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
                    CreateLight();

                return _LightTexture;
            }
        }

        private static Surface _LightSurface;
        public static Surface LightSurface
        {
            get
            {
                if (_LightSurface == null || _LightSurface.IsDisposed)
                    _LightSurface = LightTexture.GetSurfaceLevel(0);

                return _LightSurface;
            }
        }

        public static Texture PoisonTexture;

        static DXManager()
        {
            Graphics = Graphics.FromHwnd(IntPtr.Zero);
            ConfigureGraphics(Graphics);
        }


        public static void Create()
        {
            try
            {
                Parameters = new PresentParameters
                {
                    Windowed = !Config.FullScreen,
                    SwapEffect = SwapEffect.Discard,
                    BackBufferFormat = Format.X8R8G8B8,
                    PresentationInterval = Config.VSync ? PresentInterval.Default : PresentInterval.Immediate,
                    BackBufferWidth = CEnvir.Target.ClientSize.Width,
                    BackBufferHeight = CEnvir.Target.ClientSize.Height,
                    PresentFlags = PresentFlags.LockableBackBuffer,
                };

                Direct3D direct3D = new Direct3D();

                Device = new Device(direct3D, 0, DeviceType.Hardware, CEnvir.Target.Handle, CreateFlags.HardwareVertexProcessing, Parameters);

                AdapterInformation adapterInfo = direct3D.Adapters[0];
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

                direct3D.SetDialogBoxMode(true);

                const string path = @".\Data\Pallete.png";

                if (File.Exists(path))
                {
                    using (Bitmap pallete = new Bitmap(path))
                    {
                        BitmapData data = pallete.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, pallete.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

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
                for (int x = 0; x < 6; x++)
                    data[y * 6 + x] = x == 0 || y == 0 || x == 5 || y == 5 ? -16777216 : -1;

            ScratchTexture = new Texture(Device, Parameters.BackBufferWidth, Parameters.BackBufferHeight, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            ScratchSurface = ScratchTexture.GetSurfaceLevel(0);
        }

        private static void CreateLight()
        {
            Texture light = new Texture(Device, LightWidth, LightHeight, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);

            DataRectangle rect = light.LockRectangle(0, LockFlags.Discard);

            using (Bitmap image = new Bitmap(LightWidth, LightHeight, LightWidth * 4, PixelFormat.Format32bppArgb, rect.DataPointer))
            using (Graphics graphics = Graphics.FromImage(image))
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(new System.Drawing.Rectangle(0, 0, LightWidth, LightHeight));
                using (PathGradientBrush brush = new PathGradientBrush(path))
                {
                    graphics.Clear(System.Drawing.Color.FromArgb(0, 0, 0, 0));
                    brush.SurroundColors = new[] { System.Drawing.Color.FromArgb(0, 0, 0, 0) };
                    brush.CenterColor = System.Drawing.Color.FromArgb(255, 200, 200, 200);
                    graphics.FillPath(brush, path);
                    graphics.Save();
                }
            }

            light.UnlockRectangle(0);

            _LightTexture = light;
        }

        private static void CleanUp()
        {
            if (Sprite != null)
            {
                if (!Sprite.IsDisposed)
                    Sprite.Dispose();

                Sprite = null;
            }

            if (Line != null)
            {
                if (!Line.IsDisposed)
                    Line.Dispose();

                Line = null;
            }

            if (CurrentSurface != null)
            {
                if (!CurrentSurface.IsDisposed)
                    CurrentSurface.Dispose();

                CurrentSurface = null;
            }

            if (_ColourPallete != null)
            {
                if (!_ColourPallete.IsDisposed)
                    _ColourPallete.Dispose();

                _ColourPallete = null;
            }

            if (ScratchTexture != null)
            {
                if (!ScratchTexture.IsDisposed)
                    ScratchTexture.Dispose();

                ScratchTexture = null;
            }

            if (ScratchSurface != null)
            {
                if (!ScratchSurface.IsDisposed)
                    ScratchSurface.Dispose();

                ScratchSurface = null;
            }

            if (PoisonTexture != null)
            {
                if (!PoisonTexture.IsDisposed)
                    PoisonTexture.Dispose();

                PoisonTexture = null;
            }

            if (_LightTexture != null)
            {
                if (!_LightTexture.IsDisposed)
                    _LightTexture.Dispose();

                _LightTexture = null;
            }

            if (_LightSurface != null)
            {
                if (!_LightSurface.IsDisposed)
                    _LightSurface.Dispose();

                _LightSurface = null;
            }

            for (int i = ControlList.Count - 1; i >= 0; i--)
                ControlList[i].DisposeTexture();

            for (int i = TextureList.Count - 1; i >= 0; i--)
                TextureList[i].DisposeTexture();
        }
        public static void MemoryClear()
        {
            for (int i = ControlList.Count - 1; i >= 0; i--)
            {
                if (CEnvir.Now < ControlList[i].ExpireTime) continue;

                ControlList[i].DisposeTexture();
            }

            for (int i = TextureList.Count - 1; i >= 0; i--)
            {
                if (CEnvir.Now < TextureList[i].ExpireTime) continue;

                TextureList[i].DisposeTexture();
            }

            for (int i = SoundList.Count - 1; i >= 0; i--)
            {
                if (CEnvir.Now < SoundList[i].ExpireTime) continue;

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
                        Device.Direct3D.Dispose();
                }

                if (!Device.IsDisposed)
                    Device.Dispose();

                Device = null;
            }
        }

        public static void SetSurface(Surface surface)
        {
            if (CurrentSurface == surface) return;

            Sprite.Flush();
            CurrentSurface = surface;
            Device.SetRenderTarget(0, surface);
        }
        public static void SetOpacity(float opacity)
        {
            if (Opacity == opacity)
                return;

            Sprite.Flush();
            Device.SetRenderState(RenderState.AlphaBlendEnable, true);

            if (opacity >= 1 || opacity < 0)
            {
                Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
                Device.SetRenderState(RenderState.SourceBlendAlpha, Blend.One);
                Device.SetRenderState(RenderState.BlendFactor, System.Drawing.Color.FromArgb(255, 255, 255, 255).ToArgb());
            }
            else
            {
                Device.SetRenderState(RenderState.SourceBlend, Blend.BlendFactor);
                Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseBlendFactor);
                Device.SetRenderState(RenderState.SourceBlendAlpha, Blend.SourceAlpha);
                Device.SetRenderState(RenderState.BlendFactor, System.Drawing.Color.FromArgb((byte)(255 * opacity), (byte)(255 * opacity),
                    (byte)(255 * opacity), (byte)(255 * opacity)).ToArgb());
            }

            Opacity = opacity;
            Sprite.Flush();
        }
        public static void SetBlend(bool value, float rate = 1F, BlendMode mode = BlendMode.NORMAL)
        {
            if (Blending == value && BlendRate == rate && BlendMode == mode) return;

            Blending = value;
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
                    default:
                        Device.SetRenderState(RenderState.SourceBlend, Blend.InverseDestinationColor);
                        Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
                        break;
                }

                Device.SetRenderState(RenderState.BlendFactor, System.Drawing.Color.FromArgb((byte)(255 * rate), (byte)(255 * rate), (byte)(255 * rate), (byte)(255 * rate)).ToArgb());
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

            if (CEnvir.Target.ClientSize.Width == 0 || CEnvir.Target.ClientSize.Height == 0) return;

            PresentParameters parameters = Parameters;

            if (parameters.BackBufferWidth == 0 || parameters.BackBufferHeight == 0)
                return;

            parameters.Windowed = !Config.FullScreen;
            parameters.BackBufferWidth = CEnvir.Target.ClientSize.Width;
            parameters.BackBufferHeight = CEnvir.Target.ClientSize.Height;
            parameters.PresentationInterval = Config.VSync ? PresentInterval.Default : PresentInterval.Immediate;

            Device.Reset(parameters);
            Parameters = parameters;
            LoadTextures();
        }
        public static void AttemptReset()
        {
            try
            {
                Result result = Device.TestCooperativeLevel();

                if (result.Code == D3DResultCode.DeviceLost.Code) return;

                if (result.Code == D3DResultCode.DeviceNotReset.Code)
                {
                    ResetDevice();
                    return;
                }

                if (result.Code != D3DResultCode.Success.Code) return;

                DeviceLost = false;
            }
            catch (Exception ex)
            {
                CEnvir.SaveException(ex);
            }
        }
        public static void AttemptRecovery()
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
            if (CEnvir.Target.ClientSize == size) return;

            Device.Clear(ClearFlags.Target, SharpDX.ColorBGRA.Black, 0f, 0);
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
    }

    public enum BlendMode : sbyte
    {
        NONE = -1,
        NORMAL = 0,
        LIGHT = 1,
        LIGHTINV = 2,
        INVNORMAL = 3,
        INVLIGHT = 4,
        INVLIGHTINV = 5,
        INVCOLOR = 6,
        INVBACKGROUND = 7,
        COLORFY = 8,
        MASK = 9,
        HIGHLIGHT = 10,
        EFFECTMASK = 11
    }
}
