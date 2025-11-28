using Client.Controls;
using Client.Envir;
using Client.Extensions;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using DrawingFont = System.Drawing.Font;
using DxColor = SharpDX.ColorBGRA;
using DxMatrix = SharpDX.Matrix;
using DxVector2 = SharpDX.Vector2;
using GdiColor = System.Drawing.Color;
using GdiPoint = System.Drawing.Point;
using GdiRectangle = System.Drawing.Rectangle;
using GdiRectangleF = System.Drawing.RectangleF;
using NumericsMatrix3x2 = System.Numerics.Matrix3x2;
using NumericsVector3 = System.Numerics.Vector3;
using RawColorBGRA = SharpDX.Mathematics.Interop.RawColorBGRA;
using RawRectangle = SharpDX.Mathematics.Interop.RawRectangle;
using RawVector3 = SharpDX.Mathematics.Interop.RawVector3;

namespace Client.Rendering.SharpDXD3D9
{
    public sealed class SharpDXD3D9RenderingPipeline : IRenderingPipeline
    {
        private RenderingPipelineContext _context;
        private TextureFilterMode _currentFilter = TextureFilterMode.Point;

        public string Id => RenderingPipelineIds.SharpDXD3D9;

        public void Initialize(RenderingPipelineContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            SharpDXD3D9Manager.Create();
        }

        public void RunMessageLoop(Form form, Action loop)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            if (loop == null)
                throw new ArgumentNullException(nameof(loop));

            void Tick(object sender, EventArgs args)
            {
                while (AppStillIdle)
                {
                    loop();
                }
            }

            Application.Idle += Tick;
            try
            {
                Application.Run(form);
            }
            finally
            {
                Application.Idle -= Tick;
            }
        }

        private static bool AppStillIdle
        {
            get
            {
                return !PeekMessage(out PeekMsg msg, IntPtr.Zero, 0, 0, 0);
            }
        }

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern bool PeekMessage(out PeekMsg msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

        [StructLayout(LayoutKind.Sequential)]
        private struct PeekMsg
        {
            private readonly IntPtr hWnd;
            private readonly Message msg;
            private readonly IntPtr wParam;
            private readonly IntPtr lParam;
            private readonly uint time;
            private readonly GdiPoint p;
        }

        public bool RenderFrame(Action drawScene)
        {
            if (drawScene == null)
                throw new ArgumentNullException(nameof(drawScene));

            if (_context == null)
                throw new InvalidOperationException("Pipeline has not been initialized.");

            try
            {
                SharpDXD3D9Manager.AttemptReset();

                if (SharpDXD3D9Manager.Device == null || SharpDXD3D9Manager.Device.IsDisposed)
                {
                    return false;
                }

                if (SharpDXD3D9Manager.DeviceLost)
                {
                    Thread.Sleep(1);
                    return false;
                }

                SharpDXD3D9Manager.Device.Clear(ClearFlags.Target, ToDxColor(GdiColor.Black), 1, 0);
                SharpDXD3D9Manager.Device.BeginScene();
                SharpDXD3D9Manager.Sprite.Begin(SpriteFlags.AlphaBlend);

                drawScene();

                SharpDXD3D9Manager.Sprite.End();
                SharpDXD3D9Manager.Device.EndScene();
                SharpDXD3D9Manager.Device.Present();

                return true;
            }
            catch (SharpDXException ex)
            {
                SharpDXD3D9Manager.DeviceLost = true;
                CEnvir.SaveException(ex);
            }
            catch (Exception ex)
            {
                CEnvir.SaveException(ex);
                SharpDXD3D9Manager.AttemptRecovery();
            }

            return false;
        }

        public void ToggleFullScreen()
        {
            SharpDXD3D9Manager.ToggleFullScreen();
        }

        public void SetResolution(Size size)
        {
            SharpDXD3D9Manager.SetResolution(size);
        }

        public void SetTargetMonitor(int monitorIndex)
        {
            SharpDXD3D9Manager.SetGameWindowToMonitor(monitorIndex);
        }

        public void CenterOnSelectedMonitor()
        {
            SharpDXD3D9Manager.CenterOnSelectedMonitor();
        }

        public void ResetDevice()
        {
            SharpDXD3D9Manager.RequestReset();
        }

        public void OnSceneChanged(bool isGameScene)
        {
            if (!isGameScene)
                return;

            if (!Config.FullScreen)
                SharpDXD3D9Manager.RequestReset();
            else
                SharpDXD3D9Manager.CenterOnSelectedMonitor();
        }

        public IReadOnlyList<Size> GetSupportedResolutions()
        {
            return SharpDXD3D9Manager.ValidResolutions;
        }

        public Size MeasureText(string text, DrawingFont font)
        {
            return TextRenderer.MeasureText(SharpDXD3D9Manager.Graphics, text, font);
        }

        public Size MeasureText(string text, DrawingFont font, Size proposedSize, TextFormatFlags format)
        {
            return TextRenderer.MeasureText(SharpDXD3D9Manager.Graphics, text, font, proposedSize, format);
        }

        public float GetHorizontalDpi()
        {
            return SharpDXD3D9Manager.Graphics.DpiX;
        }

        public void ConfigureGraphics(Graphics graphics)
        {
            SharpDXD3D9Manager.ConfigureGraphics(graphics);
        }

        public GdiColor ConvertHslToRgb(float h, float s, float l)
        {
            return SharpDXD3D9Manager.HSLToRGB(h, s, l);
        }

        public void SetOpacity(float opacity)
        {
            SharpDXD3D9Manager.SetOpacity(opacity);
        }

        public float GetOpacity()
        {
            return SharpDXD3D9Manager.Opacity;
        }

        public void ColorFill(RenderSurface surface, GdiRectangle rectangle, System.Drawing.Color color)
        {
            if (!surface.IsValid)
                throw new ArgumentException("A valid surface handle is required.", nameof(surface));

            if (surface.NativeHandle is not Surface dxSurface)
                throw new ArgumentException("Surface handle must wrap a SharpDX surface instance.", nameof(surface));

            SharpDXD3D9Manager.Device.ColorFill(dxSurface, ToRawRectangle(rectangle), color.ToColorBGRA());
        }

        public void SetBlend(bool enabled, float rate, BlendMode mode)
        {
            SharpDXD3D9Manager.SetBlend(enabled, rate, mode);
        }

        public bool IsBlending()
        {
            return SharpDXD3D9Manager.Blending;
        }

        public float GetBlendRate()
        {
            return SharpDXD3D9Manager.BlendRate;
        }

        public BlendMode GetBlendMode()
        {
            return SharpDXD3D9Manager.BlendMode;
        }

        public float GetLineWidth()
        {
            return SharpDXD3D9Manager.Line != null ? SharpDXD3D9Manager.Line.Width : 0F;
        }

        public void SetLineWidth(float width)
        {
            if (SharpDXD3D9Manager.Line != null)
                SharpDXD3D9Manager.Line.Width = width;
        }

        public void DrawLine(IReadOnlyList<LinePoint> points, GdiColor colour)
        {
            if (points == null || points.Count < 2)
                return;

            if (SharpDXD3D9Manager.Line == null || SharpDXD3D9Manager.Line.IsDisposed)
                return;

            DxVector2[] converted = new DxVector2[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                LinePoint point = points[i];
                converted[i] = new DxVector2(point.X, point.Y);
            }

            SharpDXD3D9Manager.Line.Draw(converted, ToDxColor(colour));
        }

        public void DrawTexture(RenderTexture texture, GdiRectangle sourceRectangle, GdiRectangleF destinationRectangle, GdiColor colour)
        {
            if (!texture.IsValid)
                return;

            if (texture.NativeHandle is not Texture dxTexture)
                throw new ArgumentException("Texture handle must wrap a SharpDX texture instance.", nameof(texture));

            if (dxTexture.IsDisposed)
                return;

            if (sourceRectangle.Width <= 0 || sourceRectangle.Height <= 0)
                return;

            if (destinationRectangle.Width <= 0 || destinationRectangle.Height <= 0)
                return;

            if (RenderingPipelineManager.TryConsumeSpriteEffect(out SpriteEffectRequest effect) &&
                effect.Type == SpriteEffectType.Outline &&
                SharpDXD3D9Manager.TryDrawOutline(dxTexture, sourceRectangle, destinationRectangle, effect.Colour, effect.Thickness))
            {
                return;
            }

            DxMatrix original = SharpDXD3D9Manager.Sprite.Transform;

            float scaleX = destinationRectangle.Width / sourceRectangle.Width;
            float scaleY = destinationRectangle.Height / sourceRectangle.Height;

            if (scaleX == 0 || scaleY == 0)
            {
                SharpDXD3D9Manager.Sprite.Transform = original;
                return;
            }

            try
            {
                SharpDXD3D9Manager.Sprite.Transform = DxMatrix.Scaling(scaleX, scaleY, 1F);

                float translateX = destinationRectangle.X / scaleX;
                float translateY = destinationRectangle.Y / scaleY;

                RawVector3 center = new RawVector3(0F, 0F, 0F);
                RawVector3 translation = new RawVector3(translateX, translateY, 0F);

                SharpDXD3D9Manager.Sprite.Draw(dxTexture, ToRawColor(colour), ToRawRectangle(sourceRectangle), center, translation);
            }
            finally
            {
                SharpDXD3D9Manager.Sprite.Transform = original;
            }
        }

        public void DrawTexture(RenderTexture texture, GdiRectangle? sourceRectangle, NumericsMatrix3x2 transform, NumericsVector3 center, NumericsVector3 translation, GdiColor colour)
        {
            if (!texture.IsValid)
                return;

            if (texture.NativeHandle is not Texture dxTexture)
                throw new ArgumentException("Texture handle must wrap a SharpDX texture instance.", nameof(texture));

            if (dxTexture.IsDisposed)
                return;

            if (RenderingPipelineManager.TryConsumeSpriteEffect(out SpriteEffectRequest effect) &&
                effect.Type == SpriteEffectType.Outline &&
                sourceRectangle.HasValue &&
                SharpDXD3D9Manager.TryDrawOutline(dxTexture, sourceRectangle.Value, new GdiRectangleF(translation.X, translation.Y, sourceRectangle.Value.Width, sourceRectangle.Value.Height), effect.Colour, effect.Thickness))
            {
                return;
            }

            DxMatrix original = SharpDXD3D9Manager.Sprite.Transform;
            DxMatrix converted = new DxMatrix
            {
                M11 = transform.M11,
                M12 = transform.M12,
                M13 = 0F,
                M14 = 0F,
                M21 = transform.M21,
                M22 = transform.M22,
                M23 = 0F,
                M24 = 0F,
                M31 = 0F,
                M32 = 0F,
                M33 = 1F,
                M34 = 0F,
                M41 = transform.M31,
                M42 = transform.M32,
                M43 = 0F,
                M44 = 1F
            };

            SharpDXD3D9Manager.Sprite.Transform = converted;

            var dxCenter = new RawVector3(center.X, center.Y, center.Z);
            var dxTranslation = new RawVector3(translation.X, translation.Y, translation.Z);

            if (sourceRectangle.HasValue)
            {
                SharpDXD3D9Manager.Sprite.Draw(dxTexture, ToRawColor(colour), ToRawRectangle(sourceRectangle.Value), dxCenter, dxTranslation);
            }
            else
            {
                SharpDXD3D9Manager.Sprite.Draw(dxTexture, ToRawColor(colour), null, dxCenter, dxTranslation);
            }

            SharpDXD3D9Manager.Sprite.Transform = original;
        }

        public RenderSurface GetCurrentSurface()
        {
            Surface current = SharpDXD3D9Manager.CurrentSurface;
            if (current == null)
                throw new InvalidOperationException("Current surface is not available.");

            return RenderSurface.From(current);
        }

        public void SetSurface(RenderSurface surface)
        {
            if (!surface.IsValid)
                throw new ArgumentException("A valid surface handle is required.", nameof(surface));

            if (surface.NativeHandle is not Surface dxSurface)
                throw new ArgumentException("Surface handle must wrap a SharpDX surface instance.", nameof(surface));

            SharpDXD3D9Manager.SetSurface(dxSurface);
        }

        public RenderSurface GetScratchSurface()
        {
            Surface scratchSurface = SharpDXD3D9Manager.ScratchSurface;
            if (scratchSurface == null)
                throw new InvalidOperationException("Scratch surface is not available.");

            return RenderSurface.From(scratchSurface);
        }

        public RenderTexture GetScratchTexture()
        {
            Texture scratchTexture = SharpDXD3D9Manager.ScratchTexture;
            if (scratchTexture == null)
                throw new InvalidOperationException("Scratch texture is not available.");

            return RenderTexture.From(scratchTexture);
        }

        public RenderTargetResource CreateRenderTarget(Size size)
        {
            Texture texture = new Texture(SharpDXD3D9Manager.Device, size.Width, size.Height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            Surface surface = texture.GetSurfaceLevel(0);

            return RenderTargetResource.From(RenderTexture.From(texture), RenderSurface.From(surface));
        }

        public void ReleaseRenderTarget(RenderTargetResource renderTarget)
        {
            if (!renderTarget.IsValid)
                return;

            if (renderTarget.Surface.NativeHandle is Surface surface && !surface.IsDisposed)
                surface.Dispose();

            if (renderTarget.Texture.NativeHandle is Texture texture && !texture.IsDisposed)
                texture.Dispose();
        }

        public Size GetBackBufferSize()
        {
            return new Size(SharpDXD3D9Manager.Parameters.BackBufferWidth, SharpDXD3D9Manager.Parameters.BackBufferHeight);
        }

        public void Clear(RenderClearFlags flags, GdiColor colour, float z, int stencil, params GdiRectangle[] regions)
        {
            ClearFlags dxFlags = ConvertClearFlags(flags);

            if (regions != null && regions.Length > 0)
            {
                RawRectangle[] dxRegions = new RawRectangle[regions.Length];
                for (int i = 0; i < regions.Length; i++)
                    dxRegions[i] = ToRawRectangle(regions[i]);

                SharpDXD3D9Manager.Device.Clear(dxFlags, ToDxColor(colour), z, stencil, dxRegions);
            }
            else
            {
                SharpDXD3D9Manager.Device.Clear(dxFlags, ToDxColor(colour), z, stencil);
            }
        }

        public void FlushSprite()
        {
            SharpDXD3D9Manager.Sprite.Flush();
        }

        public RenderTexture CreateTexture(Size size, RenderTextureFormat format, RenderTextureUsage usage, RenderTexturePool pool)
        {
            Texture texture = new Texture(SharpDXD3D9Manager.Device, size.Width, size.Height, 1, ConvertUsage(usage), ConvertFormat(format), ConvertPool(pool));
            return RenderTexture.From(texture);
        }

        public void ReleaseTexture(RenderTexture texture)
        {
            if (texture.NativeHandle is Texture dxTexture && !dxTexture.IsDisposed)
                dxTexture.Dispose();
        }

        public TextureLock LockTexture(RenderTexture texture, TextureLockMode mode)
        {
            if (texture.NativeHandle is not Texture dxTexture)
                throw new InvalidOperationException("SharpDX texture handle expected.");

            LockFlags flags = ConvertLockFlags(mode);
            DataRectangle rect = dxTexture.LockRectangle(0, flags);

            return TextureLock.From(rect.DataPointer, rect.Pitch, () =>
            {
                dxTexture.UnlockRectangle(0);
            });
        }

        public void RegisterControlCache(ITextureCacheItem control)
        {
            if (control is DXControl dxControl && !SharpDXD3D9Manager.ControlList.Contains(dxControl))
                SharpDXD3D9Manager.ControlList.Add(dxControl);
        }

        public void UnregisterControlCache(ITextureCacheItem control)
        {
            if (control is DXControl dxControl)
                SharpDXD3D9Manager.ControlList.Remove(dxControl);
        }

        public void RegisterTextureCache(ITextureCacheItem texture)
        {
            if (texture is MirImage image && !SharpDXD3D9Manager.TextureList.Contains(image))
                SharpDXD3D9Manager.TextureList.Add(image);
        }

        public void UnregisterTextureCache(ITextureCacheItem texture)
        {
            if (texture is MirImage image)
                SharpDXD3D9Manager.TextureList.Remove(image);
        }

        public void RegisterSoundCache(ISoundCacheItem sound)
        {
            if (sound is DXSound dxSound && !SharpDXD3D9Manager.SoundList.Contains(dxSound))
                SharpDXD3D9Manager.SoundList.Add(dxSound);
        }

        public void UnregisterSoundCache(ISoundCacheItem sound)
        {
            if (sound is DXSound dxSound)
                SharpDXD3D9Manager.SoundList.Remove(dxSound);
        }

        public IReadOnlyList<ISoundCacheItem> GetRegisteredSoundCaches()
        {
            if (SharpDXD3D9Manager.SoundList.Count == 0)
                return Array.Empty<ISoundCacheItem>();

            return SharpDXD3D9Manager.SoundList.ConvertAll<ISoundCacheItem>(sound => sound);
        }

        public RenderTexture GetLightTexture()
        {
            Texture lightTexture = SharpDXD3D9Manager.LightTexture;
            return RenderTexture.From(lightTexture);
        }

        public Size GetLightTextureSize()
        {
            return new Size(SharpDXD3D9Manager.LightWidth, SharpDXD3D9Manager.LightHeight);
        }

        public RenderTexture GetPoisonTexture()
        {
            Texture poisonTexture = SharpDXD3D9Manager.PoisonTexture;
            if (poisonTexture == null || poisonTexture.IsDisposed)
                throw new InvalidOperationException("Poison texture has not been initialized.");

            return RenderTexture.From(poisonTexture);
        }

        public Size GetPoisonTextureSize()
        {
            Texture poisonTexture = SharpDXD3D9Manager.PoisonTexture;
            if (poisonTexture == null || poisonTexture.IsDisposed)
                throw new InvalidOperationException("Poison texture has not been initialized.");

            SurfaceDescription description = poisonTexture.GetLevelDescription(0);
            return new Size(description.Width, description.Height);
        }

        public RenderTexture GetColourPaletteTexture()
        {
            Texture palette = SharpDXD3D9Manager.ColourPallete;
            return RenderTexture.From(palette);
        }

        public byte[] GetColourPaletteData()
        {
            if (SharpDXD3D9Manager.PalleteData == null)
                throw new InvalidOperationException("Colour palette data has not been initialized.");

            return SharpDXD3D9Manager.PalleteData;
        }

        public void MemoryClear()
        {
            SharpDXD3D9Manager.MemoryClear();
        }

        public TextureFilterMode GetTextureFilter()
        {
            return _currentFilter;
        }

        public void SetTextureFilter(TextureFilterMode mode)
        {
            TextureFilter filter = mode switch
            {
                TextureFilterMode.Point => TextureFilter.Point,
                TextureFilterMode.Linear => TextureFilter.Linear,
                TextureFilterMode.None => TextureFilter.None,
                _ => TextureFilter.Point
            };

            SharpDXD3D9Manager.Device.SetSamplerState(0, SamplerState.MinFilter, filter);
            _currentFilter = mode;
        }

        public void Shutdown()
        {
            SharpDXD3D9Manager.Unload();
        }

        private static DxColor ToDxColor(GdiColor colour)
        {
            return new DxColor
            {
                R = colour.R,
                G = colour.G,
                B = colour.B,
                A = colour.A
            };
        }

        private static RawColorBGRA ToRawColor(GdiColor colour)
        {
            return new RawColorBGRA
            {
                R = colour.R,
                G = colour.G,
                B = colour.B,
                A = colour.A
            };
        }

        private static RawRectangle ToRawRectangle(GdiRectangle rectangle)
        {
            return new RawRectangle(rectangle.X, rectangle.Y, rectangle.Right, rectangle.Bottom);
        }

        private static ClearFlags ConvertClearFlags(RenderClearFlags flags)
        {
            ClearFlags result = 0;

            if ((flags & RenderClearFlags.Target) != 0)
                result |= ClearFlags.Target;

            if ((flags & RenderClearFlags.ZBuffer) != 0)
                result |= ClearFlags.ZBuffer;

            if ((flags & RenderClearFlags.Stencil) != 0)
                result |= ClearFlags.Stencil;

            return result;
        }

        private static Format ConvertFormat(RenderTextureFormat format)
        {
            return format switch
            {
                RenderTextureFormat.A8R8G8B8 => Format.A8R8G8B8,
                RenderTextureFormat.Dxt1 => Format.Dxt1,
                RenderTextureFormat.Dxt5 => Format.Dxt5,
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }

        private static Usage ConvertUsage(RenderTextureUsage usage)
        {
            return usage switch
            {
                RenderTextureUsage.None => Usage.None,
                RenderTextureUsage.RenderTarget => Usage.RenderTarget,
                RenderTextureUsage.Dynamic => Usage.Dynamic,
                _ => Usage.None
            };
        }

        private static Pool ConvertPool(RenderTexturePool pool)
        {
            return pool switch
            {
                RenderTexturePool.Managed => Pool.Managed,
                RenderTexturePool.Default => Pool.Default,
                _ => Pool.Managed
            };
        }

        private static LockFlags ConvertLockFlags(TextureLockMode mode)
        {
            return mode switch
            {
                TextureLockMode.None => LockFlags.None,
                TextureLockMode.Discard => LockFlags.Discard,
                TextureLockMode.ReadOnly => LockFlags.ReadOnly,
                _ => LockFlags.None
            };
        }
    }
}
