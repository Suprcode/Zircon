using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Library;
using Sentry;
using SlimDX;
using SlimDX.Direct3D9;

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

            float oldOpacity = DXManager.Opacity;
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
                                DXManager.Sprite.Transform = m*Matrix.Translation(x + image.Height/2, y, 0);

                                DXManager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);
                                if (oldOpacity != 0.5F) DXManager.SetOpacity(0.5F);

                                DXManager.Sprite.Draw(texture, Vector3.Zero, Vector3.Zero, Color.Black);
                                CEnvir.DPSCounter++;

                                DXManager.SetOpacity(oldOpacity);
                                DXManager.Sprite.Transform = Matrix.Identity;
                                DXManager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point);

                                image.ExpireTime = Time.Now + Config.CacheDuration;
                                break;
                            case 50:
                                if (oldOpacity != 0.5F) DXManager.SetOpacity(0.5F);

                                DXManager.Sprite.Draw(texture, Vector3.Zero, new Vector3(x, y, 0), Color.Black);
                                CEnvir.DPSCounter++;
                                DXManager.SetOpacity(oldOpacity);

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

            if (texture == null) return;

            DXManager.SetOpacity(opacity);

            DXManager.Sprite.Draw(texture, area, Vector3.Zero, new Vector3(x, y, 0), colour);
            CEnvir.DPSCounter++;
            DXManager.SetOpacity(oldOpacity);

            image.ExpireTime = Time.Now + Config.CacheDuration;
        }
        public void Draw(int index, float x, float y, Color4 colour, bool useOffSet, float opacity, ImageType type, float scale = 1F)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];
            
            Texture texture;

            Matrix scaling, rotationZ, translation;

            float oldOpacity = DXManager.Opacity;
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
                    {
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
                                    Matrix m = Matrix.Scaling(1F * scale, 0.5f * scale, 0);

                                    m.M21 = -0.50F;
                                    DXManager.Sprite.Transform = m * Matrix.Translation(x + image.Height / 2, y, 0);

                                    DXManager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);
                                    if (oldOpacity != 0.5F) DXManager.SetOpacity(0.5F);

                                    DXManager.Sprite.Draw(texture, Vector3.Zero, Vector3.Zero, Color.Black);
                                    CEnvir.DPSCounter++;

                                    DXManager.SetOpacity(oldOpacity);
                                    DXManager.Sprite.Transform = Matrix.Identity;
                                    DXManager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point);

                                    image.ExpireTime = Time.Now + Config.CacheDuration;
                                    break;
                                case 50:
                                    if (oldOpacity != 0.5F) DXManager.SetOpacity(0.5F);

                                    scaling = Matrix.Scaling(scale, scale, 0f);
                                    rotationZ = Matrix.RotationZ(0F);
                                    translation = Matrix.Translation(x + (image.Width / 2), y + (image.Height / 2), 0);

                                    DXManager.Sprite.Transform = scaling * rotationZ * translation;

                                    DXManager.Sprite.Draw(texture, Vector3.Zero, new Vector3((image.Width / 2) * -1, (image.Height / 2) * -1, 0), Color.Black);

                                    CEnvir.DPSCounter++;
                                    DXManager.SetOpacity(oldOpacity);

                                    image.ExpireTime = Time.Now + Config.CacheDuration;
                                    break;
                            }

                            return;
                        }
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

            scaling = Matrix.Scaling(scale, scale, 0f);
            rotationZ = Matrix.RotationZ(0F);
            translation = Matrix.Translation(x + (image.Width / 2), y + (image.Height / 2), 0);

            DXManager.SetOpacity(opacity);

            DXManager.Sprite.Transform = scaling * rotationZ * translation;

            DXManager.Sprite.Draw(texture, Vector3.Zero, new Vector3((image.Width / 2) * -1, (image.Height / 2) * -1, 0), colour);

            DXManager.Sprite.Transform = Matrix.Identity;

            CEnvir.DPSCounter++;
            
            DXManager.SetOpacity(oldOpacity);

            image.ExpireTime = Time.Now + Config.CacheDuration;
        }
        public void DrawBlend(int index, float size, Color4 colour, float x, float y, float angle, float opacity, ImageType type, bool useOffSet = false, byte shadow = 0)
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
                /*     if (!image.ShadowValid) image.CreateShadow(_BReader);
                     texture = image.Shadow;

                     if (useOffSet)
                     {
                         x += image.ShadowOffSetX;
                         y += image.ShadowOffSetY;
                     }
                     break;*/
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


            bool oldBlend = DXManager.Blending;
            float oldRate = DXManager.BlendRate;

            DXManager.SetBlend(true, opacity);

            var scaling = Matrix.Scaling(size, size, 0f);
            var rotationZ = Matrix.RotationZ(angle);
            var translation = Matrix.Translation(x + (image.Width / 2), y + (image.Height / 2), 0);

            DXManager.Sprite.Transform = scaling * rotationZ * translation;

            DXManager.Sprite.Draw(texture, Vector3.Zero, new Vector3((image.Width / 2) * -1, (image.Height / 2) * -1, 0), colour);

            DXManager.Sprite.Transform = Matrix.Identity;

            CEnvir.DPSCounter++;

            DXManager.SetBlend(oldBlend, oldRate);

            image.ExpireTime = Time.Now + Config.CacheDuration;
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
               /*     if (!image.ShadowValid) image.CreateShadow(_BReader);
                    texture = image.Shadow;

                    if (useOffSet)
                    {
                        x += image.ShadowOffSetX;
                        y += image.ShadowOffSetY;
                    }
                    break;*/
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


            bool oldBlend = DXManager.Blending;
            float oldRate = DXManager.BlendRate;

            DXManager.SetBlend(true, rate);

            DXManager.Sprite.Draw(texture, Vector3.Zero, new Vector3(x, y, 0), colour);
            CEnvir.DPSCounter++;

            DXManager.SetBlend(oldBlend, oldRate);

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


    public sealed class MirImage : IDisposable
    {
        public int Version;
        public int Position;

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

        public Texture Shadow;
        public bool ShadowValid { get; private set; }
        public unsafe byte* ShadowData;
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

        public Texture Overlay;
        public bool OverlayValid { get; private set; }
        public unsafe byte* OverlayData;
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

        private Format DrawFormat
        {
            get
            {
                return Version switch
                {
                    0 => Format.Dxt1,
                    _ => Format.Dxt5,
                };
            }
        }

        public DateTime ExpireTime;

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

            DXManager.TextureList.Remove(this);
        }

        public unsafe void CreateImage(BinaryReader reader)
        {
            if (Position == 0) return;

            int w = Width + (4 - Width % 4) % 4;
            int h = Height + (4 - Height % 4) % 4;

            if (w == 0 || h == 0) return;

            Image = new Texture(DXManager.Device, w, h, 1, Usage.None, DrawFormat, Pool.Managed);
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
            ExpireTime = CEnvir.Now + Config.CacheDuration;
            DXManager.TextureList.Add(this);
        }
        public unsafe void CreateShadow(BinaryReader reader)
        {
            if (Position == 0) return;

            if (!ImageValid)
                CreateImage(reader);

            int w = ShadowWidth + (4 - ShadowWidth % 4) % 4;
            int h = ShadowHeight + (4 - ShadowHeight % 4) % 4;

            if (w == 0 || h == 0) return;

            Shadow = new Texture(DXManager.Device, w, h, 1, Usage.None, DrawFormat, Pool.Managed);
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

            Overlay = new Texture(DXManager.Device, w, h, 1, Usage.None, DrawFormat, Pool.Managed);
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

}
