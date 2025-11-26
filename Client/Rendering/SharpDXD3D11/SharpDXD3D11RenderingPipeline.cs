using Client.Controls;
using Client.Envir;
using SharpDX.Direct2D1;
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Client.Rendering.SharpDXD3D11
{
    public sealed class SharpDXD3D11RenderingPipeline : IRenderingPipeline
    {
        private float _lineWidth = 1F;

        public string Id => RenderingPipelineIds.SharpDXD3D11;

        public void Initialize(RenderingPipelineContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            SharpDXD3D11Manager.Create();
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
                    loop();
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
            private readonly System.Windows.Forms.Message msg;
            private readonly IntPtr wParam;
            private readonly IntPtr lParam;
            private readonly uint time;
            private readonly System.Drawing.Point p;
        }

        public bool RenderFrame(Action drawScene)
        {
            if (drawScene == null)
                throw new ArgumentNullException(nameof(drawScene));

            try
            {
                SharpDXD3D11Manager.AttemptReset();

                SharpDXD3D11Manager.SetRenderTarget(SharpDXD3D11Manager.CurrentTarget ?? SharpDXD3D11Manager.ScratchTarget);
                SharpDXD3D11Manager.BeginDraw(Color.Black);

                drawScene();

                SharpDXD3D11Manager.EndDraw();
                return true;
            }
            catch (SharpDX.SharpDXException ex)
            {
                CEnvir.SaveException(ex);
            }
            catch (Exception ex)
            {
                CEnvir.SaveException(ex);
            }

            return false;
        }

        public void ToggleFullScreen()
        {
            SharpDXD3D11Manager.ToggleFullScreen();
        }

        public void SetResolution(Size size)
        {
            SharpDXD3D11Manager.SetResolution(size);
        }

        public void SetTargetMonitor(int monitorIndex)
        {
            SharpDXD3D11Manager.SetGameWindowToMonitor(monitorIndex);
        }

        public void CenterOnSelectedMonitor()
        {
            SharpDXD3D11Manager.CenterOnSelectedMonitor();
        }

        public void ResetDevice()
        {
            SharpDXD3D11Manager.RequestReset();
        }

        public void OnSceneChanged(bool isGameScene)
        {
            if (!isGameScene)
                return;

            if (!Config.FullScreen)
                SharpDXD3D11Manager.RequestReset();
            else
                SharpDXD3D11Manager.CenterOnSelectedMonitor();
        }

        public IReadOnlyList<Size> GetSupportedResolutions()
        {
            return SharpDXD3D11Manager.ValidResolutions;
        }

        public Size MeasureText(string text, Font font)
        {
            return TextRenderer.MeasureText(SharpDXD3D11Manager.Graphics, text, font);
        }

        public Size MeasureText(string text, Font font, Size proposedSize, TextFormatFlags format)
        {
            return TextRenderer.MeasureText(SharpDXD3D11Manager.Graphics, text, font, proposedSize, format);
        }

        public float GetHorizontalDpi()
        {
            return SharpDXD3D11Manager.Graphics.DpiX;
        }

        public void ConfigureGraphics(Graphics graphics)
        {
            SharpDXD3D11Manager.ConfigureGraphics(graphics);
        }

        public Color ConvertHslToRgb(float h, float s, float l)
        {
            return SharpDXD3D11Manager.HSLToRGB(h, s, l);
        }

        public void SetOpacity(float opacity)
        {
            SharpDXD3D11Manager.SetOpacity(opacity);
        }

        public float GetOpacity()
        {
            return SharpDXD3D11Manager.Opacity;
        }

        public void ColorFill(RenderSurface surface, Rectangle rectangle, System.Drawing.Color color)
        {
            SharpDXD3D11Manager.ColorFill(surface, rectangle, color);
        }

        public void SetBlend(bool enabled, float rate, BlendMode mode)
        {
            SharpDXD3D11Manager.SetBlend(enabled, rate, mode);
        }

        public bool IsBlending()
        {
            return SharpDXD3D11Manager.Blending;
        }

        public float GetBlendRate()
        {
            return SharpDXD3D11Manager.BlendRate;
        }

        public BlendMode GetBlendMode()
        {
            return SharpDXD3D11Manager.BlendMode;
        }

        public float GetLineWidth()
        {
            return _lineWidth;
        }

        public void SetLineWidth(float width)
        {
            if (width <= 0)
                return;

            _lineWidth = width;
        }

        public void DrawLine(IReadOnlyList<LinePoint> points, Color colour)
        {
            if (points == null || points.Count < 2)
                return;

            if (SharpDXD3D11Manager.D2DContext == null)
                return;

            using SolidColorBrush brush = new SolidColorBrush(SharpDXD3D11Manager.D2DContext, ToRawColor(colour, SharpDXD3D11Manager.Opacity));

            for (int i = 0; i < points.Count - 1; i++)
            {
                LinePoint start = points[i];
                LinePoint end = points[i + 1];

                var p0 = new RawVector2(Snap(start.X), Snap(start.Y));
                var p1 = new RawVector2(Snap(end.X), Snap(end.Y));

                SharpDXD3D11Manager.D2DContext.DrawLine(p0, p1, brush, _lineWidth);
            }
        }
        private static float Snap(float v)
        {
            return (float)Math.Floor(v) + 0.5f;
        }

        public void DrawTexture(RenderTexture texture, Rectangle sourceRectangle, RectangleF destinationRectangle, Color colour)
        {
            if (!texture.IsValid)
                return;

            if (SharpDXD3D11Manager.Blending && SharpDXD3D11Manager.BlendMode != BlendMode.NONE)
            {
                // Use Custom D3D11 Renderer for specific blend modes
                Texture2D d3dTex = null;

                if (texture.NativeHandle is SharpD3D11TextureResource texRes) d3dTex = texRes.Texture;
                else if (texture.NativeHandle is SharpD3D11RenderTarget renderTarget) d3dTex = renderTarget.Texture;

                if (d3dTex != null)
                {
                    SharpDXD3D11Manager.FlushSprite(); // Ensure D2D is flushed

                    SharpDXD3D11Manager.SpriteRenderer.Draw(
                        d3dTex,
                        destinationRectangle,
                        sourceRectangle,
                        colour,
                        Matrix3x2.Identity,
                        SharpDXD3D11Manager.BlendMode,
                        SharpDXD3D11Manager.Opacity,
                        SharpDXD3D11Manager.BlendRate);

                    return;
                }
            }

            Bitmap1 bitmap = GetBitmap(texture);

            if (SharpDXD3D11Manager.D2DContext == null)
                return;

            var destination = new RawRectangleF(destinationRectangle.Left, destinationRectangle.Top, destinationRectangle.Right, destinationRectangle.Bottom);
            var source = new RawRectangleF(sourceRectangle.Left, sourceRectangle.Top, sourceRectangle.Right, sourceRectangle.Bottom);

            DrawBitmap(bitmap, destination, source, colour);
        }

        public void DrawTexture(RenderTexture texture, Rectangle? sourceRectangle, Matrix3x2 transform, Vector3 center, Vector3 translation, Color colour)
        {
            if (!texture.IsValid)
                return;

            // Combine Center/Translation into the transform matrix if needed,
            // but DrawBitmap below uses D2D Transform.
            // We need to replicate the transform logic for D3D11.

            // The RenderPipeline signature separates them, but usually Center is for rotation origin
            // and Translation is offset. Matrix3x2 `transform` usually contains scale/rotation.
            // D2D applies them by setting Context.Transform.

            Matrix3x2 finalTransform = transform;

            // Apply Center: Translate(-Center) * Transform
            if (center.X != 0 || center.Y != 0)
            {
                finalTransform = Matrix3x2.CreateTranslation(-center.X, -center.Y) * finalTransform;
            }

            finalTransform.M31 += translation.X;
            finalTransform.M32 += translation.Y;

            if (SharpDXD3D11Manager.Blending && SharpDXD3D11Manager.BlendMode != BlendMode.NONE)
            {
                Texture2D d3dTex = null;

                if (texture.NativeHandle is SharpD3D11TextureResource texRes) d3dTex = texRes.Texture;
                else if (texture.NativeHandle is SharpD3D11RenderTarget renderTarget) d3dTex = renderTarget.Texture;

                if (d3dTex != null)
                {
                    SharpDXD3D11Manager.FlushSprite();
                    // When drawing with a matrix, we usually assume the source texture is drawn at (0,0) with its size,
                    // then transformed.
                    // But `DrawBitmap` logic below uses `null` for destination rect when transform is set.
                    // So we pass a rectangle of (0,0,Width,Height) as the base geometry to be transformed.

                    float w = d3dTex.Description.Width;
                    float h = d3dTex.Description.Height;

                    // If source rectangle is clipped, the geometry size should match the source clip?
                    // In DrawBitmap (D2D), if `destination` is null, it draws the image at its natural size (or source rect size).
                    // If sourceRect is present, it draws that portion.

                    float drawW = sourceRectangle.HasValue ? sourceRectangle.Value.Width : w;
                    float drawH = sourceRectangle.HasValue ? sourceRectangle.Value.Height : h;

                    RectangleF geom = new RectangleF(0, 0, drawW, drawH);

                    SharpDXD3D11Manager.SpriteRenderer.Draw(
                        d3dTex,
                        geom,
                        sourceRectangle,
                        colour,
                        finalTransform,
                        SharpDXD3D11Manager.BlendMode,
                        SharpDXD3D11Manager.Opacity,
                        SharpDXD3D11Manager.BlendRate);

                    return;
                }
            }

            Bitmap1 bitmap = GetBitmap(texture);

            if (SharpDXD3D11Manager.D2DContext == null)
                return;

            RawMatrix3x2 rawTransform = new RawMatrix3x2
            {
                M11 = finalTransform.M11,
                M12 = finalTransform.M12,
                M21 = finalTransform.M21,
                M22 = finalTransform.M22,
                M31 = finalTransform.M31,
                M32 = finalTransform.M32
            };

            SharpDXD3D11Manager.D2DContext.Transform = rawTransform;

            RawRectangleF? source = sourceRectangle.HasValue
                ? new RawRectangleF(sourceRectangle.Value.Left, sourceRectangle.Value.Top, sourceRectangle.Value.Right, sourceRectangle.Value.Bottom)
                : (RawRectangleF?)null;

            DrawBitmap(bitmap, null, source, colour);

            SharpDXD3D11Manager.D2DContext.Transform = new RawMatrix3x2
            {
                M11 = 1,
                M22 = 1
            };
        }

        public RenderSurface GetCurrentSurface()
        {
            if (SharpDXD3D11Manager.CurrentTarget == null)
                throw new InvalidOperationException("Current surface is not available.");

            return RenderSurface.From(SharpDXD3D11Manager.CurrentTarget);
        }

        public void SetSurface(RenderSurface surface)
        {
            if (!surface.IsValid)
                throw new ArgumentException("A valid surface handle is required.", nameof(surface));

            if (surface.NativeHandle is not SharpD3D11RenderTarget target)
                throw new ArgumentException("Surface handle must wrap a Direct3D11 render target.", nameof(surface));

            SharpDXD3D11Manager.SetRenderTarget(target);
        }

        public RenderSurface GetScratchSurface()
        {
            if (SharpDXD3D11Manager.ScratchTarget == null)
                throw new InvalidOperationException("Scratch surface is not available.");

            return RenderSurface.From(SharpDXD3D11Manager.ScratchTarget);
        }

        public RenderTexture GetScratchTexture()
        {
            if (SharpDXD3D11Manager.ScratchTarget == null)
                throw new InvalidOperationException("Scratch texture is not available.");

            return RenderTexture.From(SharpDXD3D11Manager.ScratchTarget);
        }

        public RenderTargetResource CreateRenderTarget(Size size)
        {
            SharpD3D11RenderTarget target = SharpDXD3D11Manager.CreateRenderTarget(size);
            return RenderTargetResource.From(RenderTexture.From(target), RenderSurface.From(target));
        }

        public void ReleaseRenderTarget(RenderTargetResource renderTarget)
        {
            if (!renderTarget.IsValid)
                return;

            if (renderTarget.Surface.NativeHandle is SharpD3D11RenderTarget target)
                SharpDXD3D11Manager.ReleaseRenderTarget(target);
        }

        public Size GetBackBufferSize()
        {
            return SharpDXD3D11Manager.GetBackBufferSize();
        }

        public void Clear(RenderClearFlags flags, Color colour, float z, int stencil, params Rectangle[] regions)
        {
            if (SharpDXD3D11Manager.Context == null || SharpDXD3D11Manager.CurrentTarget == null)
                return;

            if ((flags & RenderClearFlags.Target) == 0)
                return;

            RawColor4 rawColor = ToRawColor(colour, SharpDXD3D11Manager.Opacity);

            if (regions != null && regions.Length > 0)
            {
                if (SharpDXD3D11Manager.D2DContext == null)
                    return;

                PrimitiveBlend originalPrimitiveBlend = SharpDXD3D11Manager.D2DContext.PrimitiveBlend;
                SharpDXD3D11Manager.D2DContext.PrimitiveBlend = PrimitiveBlend.SourceOver;

                using SolidColorBrush brush = new SolidColorBrush(SharpDXD3D11Manager.D2DContext, rawColor);

                foreach (Rectangle region in regions)
                {
                    var rect = new RawRectangleF(region.Left, region.Top, region.Right, region.Bottom);
                    SharpDXD3D11Manager.D2DContext.FillRectangle(rect, brush);
                }

                // Restore the original blend mode for subsequent draw calls.
                SharpDXD3D11Manager.D2DContext.PrimitiveBlend = originalPrimitiveBlend;
            }
            else
            {
                SharpDXD3D11Manager.Context.ClearRenderTargetView(SharpDXD3D11Manager.CurrentTarget.RenderTargetView, rawColor);

                // Clear the D2D target as well so any previous blended content is removed for the next draw pass.
                SharpDXD3D11Manager.D2DContext?.Clear(rawColor);
            }
        }

        public void FlushSprite()
        {
            SharpDXD3D11Manager.FlushSprite();
        }

        public void RegisterControlCache(ITextureCacheItem control)
        {
            if (control is DXControl dxControl && !SharpDXD3D11Manager.ControlList.Contains(dxControl))
                SharpDXD3D11Manager.ControlList.Add(dxControl);
        }

        public void UnregisterControlCache(ITextureCacheItem control)
        {
            if (control is DXControl dxControl)
                SharpDXD3D11Manager.ControlList.Remove(dxControl);
        }

        public RenderTexture CreateTexture(Size size, RenderTextureFormat format, RenderTextureUsage usage, RenderTexturePool pool)
        {
            SharpD3D11TextureResource texture = SharpDXD3D11Manager.CreateTexture(size, format, usage, pool);
            return RenderTexture.From(texture);
        }

        public void ReleaseTexture(RenderTexture texture)
        {
            if (texture.NativeHandle is SharpD3D11TextureResource dxTexture)
                SharpDXD3D11Manager.ReleaseTexture(dxTexture);
        }

        public TextureLock LockTexture(RenderTexture texture, TextureLockMode mode)
        {
            if (texture.NativeHandle is not SharpD3D11TextureResource dxTexture)
                throw new InvalidOperationException("SharpDX Direct3D11 texture handle expected.");

            return SharpDXD3D11Manager.LockTexture(dxTexture, mode);
        }

        public void RegisterTextureCache(ITextureCacheItem texture)
        {
            if (texture is MirImage image && !SharpDXD3D11Manager.TextureList.Contains(image))
                SharpDXD3D11Manager.TextureList.Add(image);
        }

        public void UnregisterTextureCache(ITextureCacheItem texture)
        {
            if (texture is MirImage image)
                SharpDXD3D11Manager.TextureList.Remove(image);
        }

        public void RegisterSoundCache(ISoundCacheItem sound)
        {
            if (sound is DXSound dxSound && !SharpDXD3D11Manager.SoundList.Contains(dxSound))
                SharpDXD3D11Manager.SoundList.Add(dxSound);
        }

        public void UnregisterSoundCache(ISoundCacheItem sound)
        {
            if (sound is DXSound dxSound)
                SharpDXD3D11Manager.SoundList.Remove(dxSound);
        }

        public void MemoryClear()
        {
            SharpDXD3D11Manager.MemoryClear();
        }

        public IReadOnlyList<ISoundCacheItem> GetRegisteredSoundCaches()
        {
            if (SharpDXD3D11Manager.SoundList.Count == 0)
                return Array.Empty<ISoundCacheItem>();

            return SharpDXD3D11Manager.SoundList.ConvertAll<ISoundCacheItem>(sound => sound);
        }

        public RenderTexture GetColourPaletteTexture()
        {
            if (SharpDXD3D11Manager.ColourPallete == null)
                throw new InvalidOperationException("Colour palette texture has not been initialized.");

            return RenderTexture.From(SharpDXD3D11Manager.ColourPallete);
        }

        public byte[] GetColourPaletteData()
        {
            if (SharpDXD3D11Manager.PalleteData == null)
                throw new InvalidOperationException("Colour palette data has not been initialized.");

            return SharpDXD3D11Manager.PalleteData;
        }

        public RenderTexture GetLightTexture()
        {
            if (SharpDXD3D11Manager.LightTexture == null)
                throw new InvalidOperationException("Light texture has not been initialized.");

            return RenderTexture.From(SharpDXD3D11Manager.LightTexture);
        }

        public Size GetLightTextureSize()
        {
            return new Size(SharpDXD3D11Manager.LightWidth, SharpDXD3D11Manager.LightHeight);
        }

        public RenderTexture GetPoisonTexture()
        {
            if (SharpDXD3D11Manager.PoisonTexture == null)
                throw new InvalidOperationException("Poison texture has not been initialized.");

            return RenderTexture.From(SharpDXD3D11Manager.PoisonTexture);
        }

        public Size GetPoisonTextureSize()
        {
            return new Size(48, 48);
        }

        public TextureFilterMode GetTextureFilter()
        {
            return SharpDXD3D11Manager.InterpolationMode == SharpDX.Direct2D1.BitmapInterpolationMode.Linear ? TextureFilterMode.Linear : TextureFilterMode.Point;
        }

        public void SetTextureFilter(TextureFilterMode mode)
        {
            SharpDXD3D11Manager.SetTextureFilter(mode);
        }

        public void Shutdown()
        {
            SharpDXD3D11Manager.Unload();
        }

        private static Bitmap1 GetBitmap(RenderTexture texture)
        {
            return texture.NativeHandle switch
            {
                SharpD3D11TextureResource dxTexture => dxTexture.Bitmap,
                SharpD3D11RenderTarget renderTarget => renderTarget.TargetBitmap,
                _ => throw new ArgumentException("Texture handle must wrap a SharpDX Direct3D11 texture instance.", nameof(texture))
            };
        }

        private static void DrawBitmap(Bitmap1 bitmap, RawRectangleF? destination, RawRectangleF? source, Color colour)
        {
            var ctx = SharpDXD3D11Manager.D2DContext;
            if (ctx == null)
                return;

            float opacity = SharpDXD3D11Manager.Opacity * (colour.A / 255f);
            float colorScale = 1f;

            if (SharpDXD3D11Manager.Blending)
            {
                opacity *= SharpDXD3D11Manager.BlendRate;

                if (RequiresBlendFactorTint(SharpDXD3D11Manager.BlendMode))
                    colorScale = SharpDXD3D11Manager.BlendRate;
            }

            bool isLightBlend = SharpDXD3D11Manager.Blending &&
                (SharpDXD3D11Manager.BlendMode == BlendMode.LIGHT ||
                 SharpDXD3D11Manager.BlendMode == BlendMode.LIGHTINV ||
                 SharpDXD3D11Manager.BlendMode == BlendMode.HIGHLIGHT);

            if (isLightBlend)
            {
                DrawLightBlend(bitmap, destination, source, colour, opacity, colorScale);
                return;
            }

            bool applyTint = colour.R != 255 || colour.G != 255 || colour.B != 255 || Math.Abs(opacity - 1f) > float.Epsilon || Math.Abs(colorScale - 1f) > float.Epsilon;

            InterpolationMode interpolation =
                SharpDXD3D11Manager.InterpolationMode == BitmapInterpolationMode.NearestNeighbor
                    ? InterpolationMode.NearestNeighbor
                    : InterpolationMode.Linear;
            CompositeMode compositeMode = SharpDXD3D11Manager.Blending
                ? SharpDXD3D11Manager.CurrentCompositeMode
                : CompositeMode.SourceOver;

            // Map stored BitmapInterpolationMode -> D2D InterpolationMode (for DrawImage)
            if (applyTint)
            {
                using (var effect = new ColorMatrix(ctx))
                {
                    var matrix = new RawMatrix5x4
                    {
                        M11 = (colour.R / 255f) * colorScale * opacity,
                        M22 = (colour.G / 255f) * colorScale * opacity,
                        M33 = (colour.B / 255f) * colorScale * opacity,
                        M44 = opacity,   // multiply existing alpha by this
                        M54 = 0f
                    };

                    effect.SetValue((int)ColorMatrixProperties.ColorMatrix, matrix);
                    effect.SetEnumValue((int)ColorMatrixProperties.AlphaMode, ColorMatrixAlphaMode.Straight);
                    effect.SetInput(0, bitmap, true);

                    RawMatrix3x2 originalTransform = ctx.Transform;

                    if (destination.HasValue)
                    {
                        RawRectangleF destRect = destination.Value;
                        RawMatrix3x2 local = CreateImageTransform(destRect, source, bitmap.PixelSize);
                        ctx.Transform = Multiply(originalTransform, local);
                    }

                    SharpDX.Direct2D1.Image effectOutput = effect.Output;

                    ctx.DrawImage(effectOutput, null, source, interpolation, compositeMode);

                    // restore whatever the pipeline had before
                    ctx.Transform = originalTransform;
                }
            }
            else
            {
                RawMatrix3x2 originalTransform = ctx.Transform;

                if (destination.HasValue)
                {
                    RawRectangleF destRect = destination.Value;
                    RawMatrix3x2 local = CreateImageTransform(destRect, source, bitmap.PixelSize);
                    ctx.Transform = Multiply(originalTransform, local);
                }

                ctx.DrawImage(bitmap, null, source, interpolation, compositeMode);
                ctx.Transform = originalTransform;
            }

        }

        private static void DrawLightBlend(Bitmap1 bitmap, RawRectangleF? destination, RawRectangleF? source, Color colour, float opacity, float colorScale)
        {
            var ctx = SharpDXD3D11Manager.D2DContext;
            if (ctx == null)
                return;

            // LIGHT / LIGHTINV sprites in DX9 used straight alpha with additive RGB.
            // We emulate this in D2D by:
            //  1. Un-premultiplying the source;
            //  2. Applying tint + opacity in straight alpha space;
            //  3. Premultiplying again;
            //  4. Drawing with PrimitiveBlend.Add so RGB adds but alpha does not punch holes.

            InterpolationMode interpolation =
                SharpDXD3D11Manager.InterpolationMode == BitmapInterpolationMode.NearestNeighbor
                    ? InterpolationMode.NearestNeighbor
                    : InterpolationMode.Linear;

            RawMatrix3x2 originalTransform = ctx.Transform;
            PrimitiveBlend originalPrimitiveBlend = ctx.PrimitiveBlend;

            if (destination.HasValue)
            {
                RawRectangleF destRect = destination.Value;
                RawMatrix3x2 local = CreateImageTransform(destRect, source, bitmap.PixelSize);
                ctx.Transform = Multiply(originalTransform, local);
            }

            try
            {
                using (var unpremultiply = new UnPremultiply(ctx))
                using (var tint = new ColorMatrix(ctx))
                using (var premultiply = new Premultiply(ctx))
                {
                    unpremultiply.SetInput(0, bitmap, true);

                    tint.SetInputEffect(0, unpremultiply, true);
                    var matrix = new RawMatrix5x4
                    {
                        M11 = (colour.R / 255f) * colorScale,
                        M22 = (colour.G / 255f) * colorScale,
                        M33 = (colour.B / 255f) * colorScale,
                        M44 = opacity,
                        M54 = 0f
                    };

                    tint.SetValue((int)ColorMatrixProperties.ColorMatrix, matrix);
                    tint.SetEnumValue((int)ColorMatrixProperties.AlphaMode, ColorMatrixAlphaMode.Straight);

                    premultiply.SetInputEffect(0, tint, true);
                    SharpDX.Direct2D1.Image output = premultiply.Output;

                    ctx.PrimitiveBlend = PrimitiveBlend.Add;
                    ctx.DrawImage(output, null, source, interpolation, CompositeMode.SourceOver);
                }
            }
            finally
            {
                ctx.PrimitiveBlend = originalPrimitiveBlend;
                ctx.Transform = originalTransform;
            }
        }

        private static SharpDX.Mathematics.Interop.RawColor4 ToRawColor(Color colour, float opacity)
        {
            float alpha = colour.A / 255f * opacity;
            return new SharpDX.Mathematics.Interop.RawColor4(colour.R / 255f, colour.G / 255f, colour.B / 255f, alpha);
        }

        private static RawMatrix3x2 Multiply(RawMatrix3x2 a, RawMatrix3x2 b)
        {
            // Result = a * b
            return new RawMatrix3x2
            {
                M11 = a.M11 * b.M11 + a.M12 * b.M21,
                M12 = a.M11 * b.M12 + a.M12 * b.M22,

                M21 = a.M21 * b.M11 + a.M22 * b.M21,
                M22 = a.M21 * b.M12 + a.M22 * b.M22,

                M31 = a.M31 * b.M11 + a.M32 * b.M21 + b.M31,
                M32 = a.M31 * b.M12 + a.M32 * b.M22 + b.M32,
            };
        }

        private static RawMatrix3x2 CreateImageTransform(RawRectangleF destRect, RawRectangleF? sourceRect, SharpDX.Size2 bitmapSize)
        {
            float sourceLeft = 0f;
            float sourceTop = 0f;
            float sourceWidth = bitmapSize.Width;
            float sourceHeight = bitmapSize.Height;

            if (sourceRect.HasValue)
            {
                RawRectangleF src = sourceRect.Value;
                sourceLeft = src.Left;
                sourceTop = src.Top;
                sourceWidth = src.Right - src.Left;
                sourceHeight = src.Bottom - src.Top;
            }

            if (Math.Abs(sourceWidth) < float.Epsilon || Math.Abs(sourceHeight) < float.Epsilon)
            {
                return new RawMatrix3x2
                {
                    M11 = 1,
                    M22 = 1
                };
            }

            float scaleX = (destRect.Right - destRect.Left) / sourceWidth;
            float scaleY = (destRect.Bottom - destRect.Top) / sourceHeight;

            return new RawMatrix3x2
            {
                M11 = scaleX,
                M12 = 0f,
                M21 = 0f,
                M22 = scaleY,
                M31 = destRect.Left,
                M32 = destRect.Top
            };
        }

        private static bool RequiresBlendFactorTint(BlendMode mode)
        {
            return mode == BlendMode.INVLIGHT || mode == BlendMode.HIGHLIGHT;
        }

    }
}
