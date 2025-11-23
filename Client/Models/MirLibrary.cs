using Client.Rendering;
using Library;
using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Client.Envir
{
    public sealed class MirLibrary : IDisposable
    {
        public readonly object LoadLocker = new object();

        public int Version;

        public string FileName;

        private FileStream _FStream;
        private BinaryReader _BReader;

        public bool Loaded, Loading;

        public MirImage[] Images;

        public MirLibrary(string fileName)
        {
            FileName = fileName;

            _FStream = File.OpenRead(fileName);
            _BReader = new BinaryReader(_FStream);
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
                int value = reader.ReadInt32();

                int count = value & 0x1FFFFFF;
                Version = (value >> 25) & 0x7F;

                if (Version == 0)
                {
                    count = value;
                }
                else
                {

                }

                Images = new MirImage[count];

                for (int i = 0; i < Images.Length; i++)
                {
                    if (!reader.ReadBoolean()) continue;

                    Images[i] = new MirImage(reader, Version);
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
            if (!CheckImage(index))
            {
                return null;
            }

            MirImage image = Images[index];

            RenderTexture texture = GetRenderTexture(image, type);

            if (!texture.IsValid)
            {
                return null;
            }

            return image;
        }
        private RenderTexture GetRenderTexture(MirImage image, ImageType type)
        {
            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid)
                    {
                        image.CreateImage(_BReader);
                    }
                    return image.Image;
                case ImageType.Shadow:
                    if (!image.ShadowValid)
                    {
                        image.CreateShadow(_BReader);
                    }
                    return image.Shadow;
                case ImageType.Overlay:
                    if (!image.OverlayValid)
                    {
                        image.CreateOverlay(_BReader);
                    }
                    return image.Overlay;
                default:
                    return default;
            }
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

        public void Draw(int index, float x, float y, Color colour, Rectangle area, float opacity, ImageType type, byte shadow = 0)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];

            RenderTexture texture;

            float oldOpacity = RenderingPipelineManager.GetOpacity();
            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid) image.CreateImage(_BReader);
                    texture = image.Image;
                    break;
                case ImageType.Shadow:
                    if (!image.ShadowValid) image.CreateShadow(_BReader);
                    texture = image.Shadow;

                    if (!texture.IsValid)
                    {
                        if (!image.ImageValid)
                        {
                            image.CreateImage(_BReader);
                        }

                        texture = image.Image;

                        switch (image.ShadowType)
                        {
                            case 177:
                            case 176:
                            case 49:
                                Matrix3x2 transform = Matrix3x2.CreateScale(1F, 0.5f);

                                transform.M21 = -0.50F;
                                transform = transform * Matrix3x2.CreateTranslation(x + image.Height / 2F, y);
                                RenderTexture renderTexture = texture;

                                RenderingPipelineManager.SetTextureFilter(TextureFilterMode.None);
                                if (oldOpacity != 0.5F)
                                {
                                    RenderingPipelineManager.SetOpacity(0.5F);
                                }

                                RenderingPipelineManager.DrawTexture(renderTexture, null, transform, System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero, Color.Black);
                                CEnvir.DPSCounter++;

                                RenderingPipelineManager.SetOpacity(oldOpacity);
                                RenderingPipelineManager.SetTextureFilter(TextureFilterMode.Point);

                                image.ExpireTime = Time.Now + Config.CacheDuration;
                                break;
                            case 50:
                                if (oldOpacity != 0.5F)
                                {
                                    RenderingPipelineManager.SetOpacity(0.5F);
                                }

                                Rectangle fallbackSource = new Rectangle(0, 0, image.Width, image.Height);
                                RectangleF fallbackDestination = new RectangleF(x, y, fallbackSource.Width, fallbackSource.Height);
                                RenderTexture fallbackTexture = texture;

                                RenderingPipelineManager.DrawTexture(fallbackTexture, fallbackSource, fallbackDestination, Color.Black);
                                CEnvir.DPSCounter++;
                                RenderingPipelineManager.SetOpacity(oldOpacity);

                                image.ExpireTime = Time.Now + Config.CacheDuration;
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

            if (!texture.IsValid)
            {
                return;
            }

            Rectangle sourceRectangle = area;
            RectangleF destinationRectangle = new RectangleF(x, y, sourceRectangle.Width, sourceRectangle.Height);

            RenderingPipelineManager.SetOpacity(opacity);

            RenderingPipelineManager.DrawTexture(texture, sourceRectangle, destinationRectangle, Color.FromArgb(colour.ToArgb()));
            CEnvir.DPSCounter++;
            RenderingPipelineManager.SetOpacity(oldOpacity);

            image.ExpireTime = Time.Now + Config.CacheDuration;
        }

        public void Draw(int index, float x, float y, Color colour, bool useOffSet, float opacity, ImageType type, float scale = 1F)
        {
            if (!CheckImage(index))
            {
                return;
            }

            MirImage image = Images[index];

            RenderTexture texture;

            Matrix3x2 scaling;
            Matrix3x2 translation;

            float oldOpacity = RenderingPipelineManager.GetOpacity();
            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid)
                    {
                        image.CreateImage(_BReader);
                    }

                    texture = image.Image;
                    if (useOffSet)
                    {
                        x += image.OffSetX;
                        y += image.OffSetY;
                    }
                    break;
                case ImageType.Shadow:
                    {
                        if (!image.ShadowValid) image.CreateShadow(_BReader);
                        texture = image.Shadow;

                        if (useOffSet)
                        {
                            x += image.ShadowOffSetX;
                            y += image.ShadowOffSetY;
                        }

                        if (!texture.IsValid)
                        {
                            if (!image.ImageValid) image.CreateImage(_BReader);
                            texture = image.Image;
                            switch (image.ShadowType)
                            {
                                case 177:
                                case 176:
                                case 49:
                                    {
                                        Matrix3x2 transform = Matrix3x2.CreateScale(1F * scale, 0.5f * scale);
                                        transform.M21 = -0.50F;
                                        transform = transform * Matrix3x2.CreateTranslation(x + image.Height / 2F, y);
                                        RenderTexture renderTexture = texture;

                                        RenderingPipelineManager.SetTextureFilter(TextureFilterMode.None);
                                        if (oldOpacity != 0.5F) RenderingPipelineManager.SetOpacity(0.5F);

                                        RenderingPipelineManager.DrawTexture(renderTexture, null, transform, System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero, Color.Black);
                                        CEnvir.DPSCounter++;

                                        RenderingPipelineManager.SetOpacity(oldOpacity);
                                        RenderingPipelineManager.SetTextureFilter(TextureFilterMode.Point);

                                        image.ExpireTime = Time.Now + Config.CacheDuration;
                                        break;
                                    }
                                case 50:
                                    {
                                        if (oldOpacity != 0.5F) RenderingPipelineManager.SetOpacity(0.5F);

                                        scaling = Matrix3x2.CreateScale(scale);
                                        translation = Matrix3x2.CreateTranslation(x + (image.Width / 2), y + (image.Height / 2));

                                        Matrix3x2 transform = scaling * translation;
                                        RenderTexture renderTexture = texture;

                                        RenderingPipelineManager.DrawTexture(renderTexture, null, transform, System.Numerics.Vector3.Zero, new System.Numerics.Vector3((image.Width / 2F) * -1F, (image.Height / 2F) * -1F, 0F), Color.Black);
                                        CEnvir.DPSCounter++;
                                        RenderingPipelineManager.SetOpacity(oldOpacity);

                                        image.ExpireTime = Time.Now + Config.CacheDuration;
                                        break;
                                    }
                            }

                            return;
                        }
                    }
                    break;
                case ImageType.Overlay:
                    if (!image.OverlayValid)
                    {
                        image.CreateOverlay(_BReader);
                    }

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

            if (!texture.IsValid)
            {
                return;
            }

            scaling = Matrix3x2.CreateScale(scale);
            translation = Matrix3x2.CreateTranslation(x + (image.Width / 2), y + (image.Height / 2));

            RenderingPipelineManager.SetOpacity(opacity);

            var vector = new Vector3(-(image.Width / 2), -(image.Height / 2), 0f);

            Matrix3x2 transformMatrix = scaling * translation;
            RenderingPipelineManager.DrawTexture(texture, null, transformMatrix, Vector3.Zero, vector, colour);

            CEnvir.DPSCounter++;

            RenderingPipelineManager.SetOpacity(oldOpacity);

            image.ExpireTime = Time.Now + Config.CacheDuration;
        }

        public void DrawBlendScaled(int index, float scaleX, float scaleY, Color colour, float x, float y, float angle, float opacity, ImageType type, bool useOffSet = false, byte shadow = 0)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];

            if (type == ImageType.Shadow) return;

            RenderTexture texture = GetRenderTexture(image, type);
            if (!texture.IsValid) return;

            if (useOffSet)
            {
                x += image.OffSetX;
                y += image.OffSetY;
            }

            bool oldBlend = RenderingPipelineManager.IsBlending();
            float oldRate = RenderingPipelineManager.GetBlendRate();
            BlendMode oldMode = RenderingPipelineManager.GetBlendMode();
            RenderingPipelineManager.SetBlend(true, opacity);

            float cx = image.Width / 2f;
            float cy = image.Height / 2f;

            // 1) Move pivot to origin
            Matrix3x2 toOrigin = Matrix3x2.CreateTranslation(-cx, -cy);

            // 2) Scale around pivot
            Matrix3x2 scaling = Matrix3x2.CreateScale(scaleX, scaleY);

            // 3) Rotate around pivot
            Matrix3x2 rotation = Matrix3x2.CreateRotation(angle);

            // 4) Move final rotated/scaled image center to (x, y)
            Matrix3x2 translation = Matrix3x2.CreateTranslation(x + cx, y + cy);

            // FINAL ORDER (DX accurate):
            Matrix3x2 final =
                toOrigin     // move pivot
                * scaling    // scale around pivot
                * rotation   // rotate around pivot
                * translation; // place in world

            RenderingPipelineManager.DrawTexture(
                texture,
                null,
                final,
                Vector3.Zero,
                Vector3.Zero, // IMPORTANT: origin handled explicitly in matrix
                colour);

            CEnvir.DPSCounter++;
            RenderingPipelineManager.SetBlend(oldBlend, oldRate, oldMode);
            image.ExpireTime = Time.Now + Config.CacheDuration;
        }

        public void DrawBlend(int index, float size, Color colour, float x, float y, float angle, float opacity, ImageType type, bool useOffSet = false, byte shadow = 0)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];

            if (type == ImageType.Shadow) return;

            RenderTexture texture = GetRenderTexture(image, type);

            if (!texture.IsValid) return;

            if (useOffSet)
            {
                x += image.OffSetX;
                y += image.OffSetY;
            }

            bool oldBlend = RenderingPipelineManager.IsBlending();
            float oldRate = RenderingPipelineManager.GetBlendRate();
            BlendMode previousBlendMode = RenderingPipelineManager.GetBlendMode();

            RenderingPipelineManager.SetBlend(true, opacity);

            Matrix3x2 scaling = Matrix3x2.CreateScale(size);
            Matrix3x2 rotation = Matrix3x2.CreateRotation(angle);
            Matrix3x2 translation = Matrix3x2.CreateTranslation(x + (image.Width / 2), y + (image.Height / 2));

            var vector = new Vector3(-(image.Width / 2), -(image.Height / 2), 0f);

            Matrix3x2 pipelineTransform = scaling * rotation * translation;
            RenderingPipelineManager.DrawTexture(texture, null, pipelineTransform, System.Numerics.Vector3.Zero, vector, colour);

            CEnvir.DPSCounter++;

            RenderingPipelineManager.SetBlend(oldBlend, oldRate, previousBlendMode);

            image.ExpireTime = Time.Now + Config.CacheDuration;
        }

        public void DrawBlendCentered(int index, float size, Color colour, float x, float y, float angle, float opacity, ImageType type, bool useOffSet = false, byte shadow = 0)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];

            if (type == ImageType.Shadow) return;

            RenderTexture texture = GetRenderTexture(image, type);

            if (!texture.IsValid) return;

            if (useOffSet)
            {
                x += image.OffSetX;
                y += image.OffSetY;
            }

            bool oldBlend = RenderingPipelineManager.IsBlending();
            float oldRate = RenderingPipelineManager.GetBlendRate();
            BlendMode previousBlendMode = RenderingPipelineManager.GetBlendMode();

            RenderingPipelineManager.SetBlend(true, opacity);

            Matrix3x2 scaling = Matrix3x2.CreateScale(size);
            Matrix3x2 rotation = Matrix3x2.CreateRotation(angle);
            Matrix3x2 translation = Matrix3x2.CreateTranslation(x, y);

            Matrix3x2 pipelineTransform = scaling * rotation * translation;

            var vector = new Vector3(image.Width / 2F, image.Height / 2F, 0F);

            // Pass Center (w/2, h/2) and Translation (0)
            // DrawTexture will apply Translate(-Center) * Transform.
            // Result: Translate(-Center) * Scale * Rotate * Translate(Position).
            // This rotates around the center of the image and places it at Position.
            RenderingPipelineManager.DrawTexture(texture, null, pipelineTransform, vector, System.Numerics.Vector3.Zero, colour);

            CEnvir.DPSCounter++;

            RenderingPipelineManager.SetBlend(oldBlend, oldRate, previousBlendMode);

            image.ExpireTime = Time.Now + Config.CacheDuration;
        }

        public void DrawBlend(int index, float x, float y, Color colour, bool useOffSet, float rate, ImageType type, byte shadow = 0)
        {
            if (!CheckImage(index))
            {
                return;
            }

            MirImage image = Images[index];

            RenderTexture texture;

            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid)
                    {
                        image.CreateImage(_BReader);
                    }

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
                    if (!image.OverlayValid)
                    {
                        image.CreateOverlay(_BReader);
                    }

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

            if (!texture.IsValid)
            {
                return;
            }

            bool oldBlend = RenderingPipelineManager.IsBlending();
            float oldRate = RenderingPipelineManager.GetBlendRate();
            BlendMode previousBlendMode = RenderingPipelineManager.GetBlendMode();

            RenderingPipelineManager.SetBlend(true, rate);

            RenderingPipelineManager.DrawTexture(texture, null, Matrix3x2.Identity, System.Numerics.Vector3.Zero, new System.Numerics.Vector3(x, y, 0F), colour);
            CEnvir.DPSCounter++;

            RenderingPipelineManager.SetBlend(oldBlend, oldRate, previousBlendMode);

            image.ExpireTime = Time.Now + Config.CacheDuration;
        }

        #region IDisposable Support

        public bool IsDisposed { get; private set; }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsDisposed = true;

                foreach (MirImage image in Images)
                    image.Dispose();


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

    public sealed class MirImage : IDisposable, ITextureCacheItem
    {
        public int Version;
        public int Position;

        #region Texture

        public short Width;
        public short Height;
        public short OffSetX;
        public short OffSetY;
        public byte ShadowType;
        public RenderTexture Image;
        public bool ImageValid { get; private set; }
        public byte[] ImageData;
        public int ImageDataSize
        {
            get
            {
                int w = Width + (4 - Width % 4) % 4;
                int h = Height + (4 - Height % 4) % 4;

                if (Version > 0)
                {
                    return w * h;
                }
                else
                {
                    return w * h / 2;
                }
            }
        }
        #endregion

        #region Shadow
        public short ShadowWidth;
        public short ShadowHeight;

        public short ShadowOffSetX;
        public short ShadowOffSetY;

        public RenderTexture Shadow;
        public bool ShadowValid { get; private set; }
        public byte[] ShadowData;
        public int ShadowDataSize
        {
            get
            {
                int w = ShadowWidth + (4 - ShadowWidth % 4) % 4;
                int h = ShadowHeight + (4 - ShadowHeight % 4) % 4;

                if (Version > 0)
                {
                    return w * h;
                }
                else
                {
                    return w * h / 2;
                }
            }
        }
        #endregion

        #region Overlay
        public short OverlayWidth;
        public short OverlayHeight;

        public RenderTexture Overlay;
        public bool OverlayValid { get; private set; }
        public byte[] OverlayData;
        public int OverlayDataSize
        {
            get
            {
                int w = OverlayWidth + (4 - OverlayWidth % 4) % 4;
                int h = OverlayHeight + (4 - OverlayHeight % 4) % 4;

                if (Version > 0)
                {
                    return w * h;
                }
                else
                {
                    return w * h / 2;
                }
            }
        }
        #endregion

        private RenderTextureFormat DrawFormat
        {
            get
            {
                return Version switch
                {
                    0 => RenderTextureFormat.Dxt1,
                    _ => RenderTextureFormat.Dxt5,
                };
            }
        }

        public DateTime ExpireTime { get; set; }

        public MirImage(BinaryReader reader, int version)
        {
            Version = version;

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

            int col0 = ImageData[index + 1] << 8 | ImageData[index];
            int col1 = ImageData[index + 3] << 8 | ImageData[index + 2];

            if (col0 == 0 && col1 == 0) return false;

            if (!acurrate || col1 < col0) return true;

            x = p.X % 4;
            y = p.Y % 4;
            x *= 2;

            return (ImageData[index + 4 + y] & 1 << x) >> x != 1 || (ImageData[index + 4 + y] & 1 << x + 1) >> x + 1 != 1;
        }

        public void DisposeTexture()
        {
            RenderingPipelineManager.ReleaseTexture(Image);
            RenderingPipelineManager.ReleaseTexture(Shadow);
            RenderingPipelineManager.ReleaseTexture(Overlay);

            ImageData = null;
            ShadowData = null;
            OverlayData = null;

            Image = default;
            Shadow = default;
            Overlay = default;

            ImageValid = false;
            ShadowValid = false;
            OverlayValid = false;

            ExpireTime = DateTime.MinValue;

            RenderingPipelineManager.UnregisterTextureCache(this);
        }

        public void CreateImage(BinaryReader reader)
        {
            if (Position == 0) return;

            int w = Width + (4 - Width % 4) % 4;
            int h = Height + (4 - Height % 4) % 4;

            if (w == 0 || h == 0) return;

            byte[] buffer;

            lock (reader)
            {
                reader.BaseStream.Seek(Position, SeekOrigin.Begin);
                buffer = reader.ReadBytes(ImageDataSize);
            }

            ImageData = buffer;

            Image = RenderingPipelineManager.CreateTexture(new Size(w, h), DrawFormat, RenderTextureUsage.None, RenderTexturePool.Managed);

            using (TextureLock textureLock = RenderingPipelineManager.LockTexture(Image, TextureLockMode.Discard))
            {
                Marshal.Copy(buffer, 0, textureLock.DataPointer, buffer.Length);
            }

            ImageValid = true;
            ExpireTime = CEnvir.Now + Config.CacheDuration;
            RenderingPipelineManager.RegisterTextureCache(this);
        }
        public void CreateShadow(BinaryReader reader)
        {
            if (Position == 0) return;

            if (!ImageValid)
                CreateImage(reader);

            int w = ShadowWidth + (4 - ShadowWidth % 4) % 4;
            int h = ShadowHeight + (4 - ShadowHeight % 4) % 4;

            if (w == 0 || h == 0) return;

            byte[] buffer;

            lock (reader)
            {
                reader.BaseStream.Seek(Position + ImageDataSize, SeekOrigin.Begin);
                buffer = reader.ReadBytes(ShadowDataSize);
            }

            ShadowData = buffer;

            Shadow = RenderingPipelineManager.CreateTexture(new Size(w, h), DrawFormat, RenderTextureUsage.None, RenderTexturePool.Managed);

            using (TextureLock textureLock = RenderingPipelineManager.LockTexture(Shadow, TextureLockMode.Discard))
            {
                Marshal.Copy(buffer, 0, textureLock.DataPointer, buffer.Length);
            }

            ShadowValid = true;
        }
        public void CreateOverlay(BinaryReader reader)
        {
            if (Position == 0) return;

            if (!ImageValid)
                CreateImage(reader);

            int w = OverlayWidth + (4 - OverlayWidth % 4) % 4;
            int h = OverlayHeight + (4 - OverlayHeight % 4) % 4;

            if (w == 0 || h == 0) return;

            byte[] buffer;

            lock (reader)
            {
                reader.BaseStream.Seek(Position + ImageDataSize + ShadowDataSize, SeekOrigin.Begin);
                buffer = reader.ReadBytes(OverlayDataSize);
            }

            OverlayData = buffer;

            Overlay = RenderingPipelineManager.CreateTexture(new Size(w, h), DrawFormat, RenderTextureUsage.None, RenderTexturePool.Managed);

            using (TextureLock textureLock = RenderingPipelineManager.LockTexture(Overlay, TextureLockMode.Discard))
            {
                Marshal.Copy(buffer, 0, textureLock.DataPointer, buffer.Length);
            }

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
}
