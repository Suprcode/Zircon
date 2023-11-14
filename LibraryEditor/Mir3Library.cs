using ManagedSquish;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace LibraryEditor
{

    public enum ImageType
    {
        Image,
        Shadow,
        Overlay,
    }

    public sealed class Mir3Library
    {
        /// <summary>
        /// V0 - Default version - Dxt1 Images
        /// V1 - First version - Dxt5 Images
        /// </summary>
        public const int LIBRARY_VERSION = 1;

        public int Version;

        public string FileName;
        public string _fileName;

        private FileStream _fStream;
        private BinaryReader _bReader;

        public List<Mir3Image> Images;

        public Mir3Library(string fileName)
        {
            FileName = fileName;
            _fileName = Path.ChangeExtension(fileName, null);
            Images = new List<Mir3Image>();
            if (!File.Exists(fileName))
                return;

            _fStream = File.OpenRead(fileName);
            _bReader = new BinaryReader(_fStream);

            ReadLibrary();
            Close();
        }
        public void ReadLibrary()
        {
            if (_bReader == null)
                return;

            using (MemoryStream mstream = new MemoryStream(_bReader.ReadBytes(_bReader.ReadInt32())))
            using (BinaryReader reader = new BinaryReader(mstream))
            {
                int value = reader.ReadInt32();

                int count = value & 0x1FFFFFF;
                Version = (value >> 25) & 0x7F;

                if (Version == 0)
                {
                    count = value;
                }

                for (int i = 0; i < count; i++)
                    Images.Add(null);

                for (int i = 0; i < Images.Count; i++)
                {
                    if (!reader.ReadBoolean()) continue;

                    Images[i] = new Mir3Image(reader, Version);
                }
            }

            for (int i = 0; i < Images.Count; i++)
            {
                if (Images[i] == null) continue;

                CreateImage(i, ImageType.Image);
                CreateImage(i, ImageType.Shadow);
                CreateImage(i, ImageType.Overlay);
            }
        }
        public void Close()
        {
            if (_fStream != null)
                _fStream.Dispose();
            if (_bReader != null)
                _bReader.Dispose();
        }
        public Mir3Image CreateImage(int index, ImageType type)
        {
            if (!CheckImage(index)) return null;

            Mir3Image image = Images[index];
            Bitmap bmp;

            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid) image.CreateImage(_bReader);
                    bmp = image.Image;
                    break;
                case ImageType.Shadow:
                    if (!image.ShadowValid) image.CreateShadow(_bReader);
                    bmp = image.ShadowImage;
                    break;
                case ImageType.Overlay:
                    if (!image.OverlayValid) image.CreateOverlay(_bReader);
                    bmp = image.OverlayImage;
                    break;
                default:
                    return null;
            }

            if (bmp == null) return null;

            return image;
        }

        private bool CheckImage(int index)
        {
            return index >= 0 && index < Images.Count && Images[index] != null;
        }

        public Mir3Image GetImage(int index)
        {
            if (index < 0 || index >= Images.Count)
                return null;

            return Images[index];
        }

        public Bitmap GetPreview(int index, ImageType type)
        {
            if (index < 0 || index >= Images.Count)
                return new Bitmap(1, 1);

            Mir3Image image = Images[index];

            switch (type)
            {
                case ImageType.Image:
                    if (image == null || image.Image == null)
                        return new Bitmap(1, 1);

                    if (image.Preview == null)
                        image.CreatePreview();

                    return image.Preview;
                case ImageType.Shadow:
                    if (image == null || image.ShadowImage == null)
                        return new Bitmap(1, 1);

                    if (image.ShadowPreview == null)
                        image.CreateShadowPreview();

                    return image.ShadowPreview;
                case ImageType.Overlay:
                    if (image == null || image.OverlayImage == null)
                        return new Bitmap(1, 1);

                    if (image.OverlayPreview == null)
                        image.CreateOverlayPreview();

                    return image.OverlayPreview;
            }

            return new Bitmap(1, 1);
        }

        public void RemoveBlanks(bool safe = false)
        {
            for (int i = Images.Count - 1; i >= 0; i--)
            {
                if (Images[i] == null)
                {
                    if (!safe)
                        RemoveImage(i);
                    else if (Images[i].OffSetX == 0 && Images[i].OffSetY == 0)
                        RemoveImage(i);
                }
            }
        }

        public void RemoveImage(int index)
        {
            if (Images == null || Images.Count <= 1)
            {
                Images = new List<Mir3Image>();
                return;
            }

            Images.RemoveAt(index);
        }
        public void AddBlanks(int newImages)
        {
            if (newImages == 0)
                return;

            int count = Images.Count;

            int cap = newImages - (count % newImages);
            if (cap != newImages)
            {
                Bitmap image;
                for (int i = cap - 1; i >= 0; i--)
                {
                    try
                    {
                        image = new Bitmap(1, 1);
                    }
                    catch
                    {
                        return;
                    }

                    short x = 0;
                    short y = 0;


                    AddImage(image, x, y);
                }
            }
        }
        public void AddImage(Bitmap image, short x, short y)
        {
            Mir3Image mImage = new Mir3Image(image, Version) { OffSetX = x, OffSetY = y };

            Images.Add(mImage);
        }

        public void ReplaceImage(int Index, Bitmap image, short x, short y)
        {
            Mir3Image mImage = new Mir3Image(image, Version) { OffSetX = x, OffSetY = y };

            Images[Index] = mImage;
        }

        public void InsertImage(int index, Bitmap image, short x, short y)
        {
            Mir3Image mImage = new Mir3Image(image, Version) { OffSetX = x, OffSetY = y };

            Images.Insert(index, mImage);
        }

        public void Save(string path)
        {
            //|Header Size|Count|T|Header|F|F|T|Header|F|T|Header...|Image|Image|Im...

            int headerSize = 4 + Images.Count;

            foreach (Mir3Image image in Images)
            {
                if (image == null || image.FBytes.Length == 0) continue;

                headerSize += Mir3Image.HeaderSize;
            }

            int position = headerSize + 4;

            foreach (Mir3Image image in Images)
            {
                if (image == null || image.FBytes.Length == 0) continue;

                image.Position = position;

                position += image.DataSize;
            }


            using (MemoryStream buffer = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(buffer))
            {
                writer.Write(headerSize);

                writer.Write((Version & 0x7F) << 25 | (Images.Count & 0x1FFFFFF));

                foreach (Mir3Image image in Images)
                {
                    writer.Write(image != null && image.FBytes.Length > 0);

                    if (image == null || image.FBytes.Length == 0) continue;

                    image.SaveHeader(writer);
                }

                foreach (Mir3Image image in Images)
                {
                    if (image == null || image.FBytes.Length == 0) continue;

                    if (image.FBytes != null)
                        writer.Write(image.FBytes);

                    if (image.ShadowFBytes != null)
                        writer.Write(image.ShadowFBytes);

                    if (image.OverlayFBytes != null)
                        writer.Write(image.OverlayFBytes);
                }

                File.WriteAllBytes(path, buffer.ToArray());
            }
        }

        public sealed class Mir3Image : IDisposable
        {
            public const int HeaderSize = 25;

            public int Version;

            public int DataSize => (FBytes?.Length ?? 0) + (ShadowFBytes?.Length ?? 0) + (OverlayFBytes?.Length ?? 0);

            public int Position;

            #region Texture

            public short Width;
            public short Height;
            public short OffSetX;
            public short OffSetY;
            public byte ShadowType;
            public Bitmap Image, Preview;
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
            public byte[] FBytes;
            #endregion

            #region Shadow
            public short ShadowWidth;
            public short ShadowHeight;

            public short ShadowOffSetX;
            public short ShadowOffSetY;

            public Bitmap ShadowImage, ShadowPreview;
            public bool ShadowValid { get; private set; }
            public unsafe byte* ShadowData;
            public byte[] ShadowFBytes;
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

            public Bitmap OverlayImage, OverlayPreview;
            public bool OverlayValid { get; private set; }
            public unsafe byte* OverlayData;
            public byte[] OverlayFBytes;
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


            public DateTime ExpireTime;

            public Mir3Image(int version)
            {
                Version = version;
            }

            public Mir3Image(BinaryReader reader, int version)
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

            public unsafe Mir3Image(Bitmap image, int version)
            {
                if (image == null)
                {
                    FBytes = new byte[0];
                    return;
                }

                Version = version;

                Position = -1;
                Width = (short)image.Width;
                Height = (short)image.Height;

                int w = image.Width + (4 - image.Width % 4) % 4;
                int h = image.Height + (4 - image.Height % 4) % 4;

                if (image.Width != w || image.Height != h)
                {
                    Bitmap temp = new Bitmap(w, h);
                    using (Graphics g = Graphics.FromImage(temp))
                    {
                        g.Clear(Color.Transparent);
                        g.DrawImage(image, 0, 0);
                        g.Save();
                    }
                    image.Dispose();
                    image = temp;
                }

                Image = image;

                BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly,
                                                 PixelFormat.Format32bppArgb);

                byte[] pixels = new byte[image.Width * image.Height * 4];

                Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);
                image.UnlockBits(data);

                for (int i = 0; i < pixels.Length; i += 4)
                {
                    //Reverse Red/Blue
                    byte b = pixels[i];
                    pixels[i] = pixels[i + 2];
                    pixels[i + 2] = b;

                    if (pixels[i] == 0 && pixels[i + 1] == 0 && pixels[i + 2] == 0)
                        pixels[i + 3] = 0; //Make Transparent
                }

                int count = Squish.GetStorageRequirements(image.Width, image.Height, DxtFlags);

                FBytes = new byte[count];
                fixed (byte* dest = FBytes)
                fixed (byte* source = pixels)
                {
                    Squish.CompressImage((IntPtr)source, image.Width, image.Height, (IntPtr)dest, DxtFlags);
                }
            }

            public unsafe Mir3Image(Bitmap image, Bitmap shadow, Bitmap overlay, int version)
            {
                if (image == null)
                {
                    FBytes = new byte[0];
                    return;
                }

                Version = version;

                Position = -1;
                Width = (short)image.Width;
                Height = (short)image.Height;

                int w = image.Width + (4 - image.Width % 4) % 4;
                int h = image.Height + (4 - image.Height % 4) % 4;

                if (image.Width != w || image.Height != h)
                {
                    Bitmap temp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
                    using (Graphics g = Graphics.FromImage(temp))
                    {
                        g.Clear(Color.Transparent);
                        g.DrawImage(image, 0, 0);
                        g.Save();
                    }
                    image.Dispose();
                    image = temp;
                }

                Image = image;

                BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly,
                                                 PixelFormat.Format32bppArgb);

                byte[] pixels = new byte[image.Width * image.Height * 4];

                Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);
                image.UnlockBits(data);

                for (int i = 0; i < pixels.Length; i += 4)
                {
                    //Reverse Red/Blue
                    byte b = pixels[i];
                    pixels[i] = pixels[i + 2];
                    pixels[i + 2] = b;

                    if (pixels[i] == 0 && pixels[i + 1] == 0 && pixels[i + 2] == 0)
                        pixels[i + 3] = 0; //Make Transparent
                }

                int count = Squish.GetStorageRequirements(image.Width, image.Height, DxtFlags);

                FBytes = new byte[count];
                fixed (byte* dest = FBytes)
                fixed (byte* source = pixels)
                {
                    Squish.CompressImage((IntPtr)source, image.Width, image.Height, (IntPtr)dest, DxtFlags);
                }

                //Shadow
                if (shadow != null)
                {
                    ShadowWidth = (short)shadow.Width;
                    ShadowHeight = (short)shadow.Height;

                    w = shadow.Width + (4 - shadow.Width % 4) % 4;
                    h = shadow.Height + (4 - shadow.Height % 4) % 4;

                    if (shadow.Width != w || shadow.Height != h)
                    {
                        Bitmap temp = new Bitmap(w, h);
                        using (Graphics g = Graphics.FromImage(temp))
                        {
                            g.Clear(Color.Transparent);
                            g.DrawImage(shadow, 0, 0);
                            g.Save();
                        }
                        shadow.Dispose();
                        shadow = temp;
                    }

                    ShadowImage = shadow;

                    data = shadow.LockBits(new Rectangle(0, 0, shadow.Width, shadow.Height), ImageLockMode.ReadOnly,
                                                     PixelFormat.Format32bppArgb);

                    pixels = new byte[shadow.Width * shadow.Height * 4];

                    Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);
                    shadow.UnlockBits(data);

                    for (int i = 0; i < pixels.Length; i += 4)
                    {
                        //Reverse Red/Blue
                        byte b = pixels[i];
                        pixels[i] = pixels[i + 2];
                        pixels[i + 2] = b;

                        if (pixels[i] == 0 && pixels[i + 1] == 0 && pixels[i + 2] == 0)
                            pixels[i + 3] = 0; //Make Transparent
                    }

                    count = Squish.GetStorageRequirements(shadow.Width, shadow.Height, DxtFlags);

                    ShadowFBytes = new byte[count];
                    fixed (byte* dest = ShadowFBytes)
                    fixed (byte* source = pixels)
                    {
                        Squish.CompressImage((IntPtr)source, shadow.Width, shadow.Height, (IntPtr)dest, DxtFlags);
                    }
                }

                //Overlay
                if (overlay != null)
                {
                    OverlayWidth = (short)overlay.Width;
                    OverlayHeight = (short)overlay.Height;

                    w = overlay.Width + (4 - overlay.Width % 4) % 4;
                    h = overlay.Height + (4 - overlay.Height % 4) % 4;

                    if (overlay.Width != w || overlay.Height != h)
                    {
                        Bitmap temp = new Bitmap(w, h);
                        using (Graphics g = Graphics.FromImage(temp))
                        {
                            g.Clear(Color.Transparent);
                            g.DrawImage(overlay, 0, 0);
                            g.Save();
                        }
                        overlay.Dispose();
                        overlay = temp;
                    }

                    OverlayImage = overlay;

                    data = overlay.LockBits(new Rectangle(0, 0, overlay.Width, overlay.Height), ImageLockMode.ReadOnly,
                                                     PixelFormat.Format32bppArgb);

                    pixels = new byte[overlay.Width * overlay.Height * 4];

                    Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);
                    overlay.UnlockBits(data);

                    for (int i = 0; i < pixels.Length; i += 4)
                    {
                        //Reverse Red/Blue
                        byte b = pixels[i];
                        pixels[i] = pixels[i + 2];
                        pixels[i + 2] = b;

                        if (pixels[i] == 0 && pixels[i + 1] == 0 && pixels[i + 2] == 0)
                            pixels[i + 3] = 0; //Make Transparent
                    }

                    count = Squish.GetStorageRequirements(overlay.Width, overlay.Height, DxtFlags);

                    OverlayFBytes = new byte[count];
                    fixed (byte* dest = OverlayFBytes)
                    fixed (byte* source = pixels)
                    {
                        Squish.CompressImage((IntPtr)source, overlay.Width, overlay.Height, (IntPtr)dest, DxtFlags);
                    }
                }
            }

            public unsafe void CreateImage(BinaryReader reader)
            {
                if (Position == 0) return;

                int w = Width + (4 - Width % 4) % 4;
                int h = Height + (4 - Height % 4) % 4;

                if (w == 0 || h == 0) return;

                Image = new Bitmap(w, h);
                BitmapData data = Image.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                reader.BaseStream.Seek(Position, SeekOrigin.Begin);
                FBytes = reader.ReadBytes(ImageDataSize);

                fixed (byte* source = FBytes)
                    Squish.DecompressImage(data.Scan0, w, h, (IntPtr)source, DxtFlags);

                byte* dest = (byte*)data.Scan0;

                for (int i = 0; i < h * w * 4; i += 4)
                {
                    //Reverse Red/Blue
                    byte b = dest[i];
                    dest[i] = dest[i + 2];
                    dest[i + 2] = b;
                }

                Image.UnlockBits(data);
                ImageValid = true;
            }
            public unsafe void CreateShadow(BinaryReader reader)
            {
                if (Position == 0) return;

                if (!ImageValid)
                    CreateImage(reader);

                int w = ShadowWidth + (4 - ShadowWidth % 4) % 4;
                int h = ShadowHeight + (4 - ShadowHeight % 4) % 4;

                if (w == 0 || h == 0) return;

                ShadowImage = new Bitmap(w, h);
                BitmapData data = ShadowImage.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                reader.BaseStream.Seek(Position + ImageDataSize, SeekOrigin.Begin);
                ShadowFBytes = reader.ReadBytes(ShadowDataSize);

                fixed (byte* source = ShadowFBytes)
                    Squish.DecompressImage(data.Scan0, w, h, (IntPtr)source, DxtFlags);

                byte* dest = (byte*)data.Scan0;

                for (int i = 0; i < h * w * 4; i += 4)
                {
                    //Reverse Red/Blue
                    byte b = dest[i];
                    dest[i] = dest[i + 2];
                    dest[i + 2] = b;
                }

                ShadowImage.UnlockBits(data);
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

                OverlayImage = new Bitmap(w, h);
                BitmapData data = OverlayImage.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                reader.BaseStream.Seek(Position + ImageDataSize + ShadowDataSize, SeekOrigin.Begin);
                OverlayFBytes = reader.ReadBytes(OverlayDataSize);

                fixed (byte* source = OverlayFBytes)
                    Squish.DecompressImage(data.Scan0, w, h, (IntPtr)source, DxtFlags);

                byte* dest = (byte*)data.Scan0;

                for (int i = 0; i < h * w * 4; i += 4)
                {
                    //Reverse Red/Blue
                    byte b = dest[i];
                    dest[i] = dest[i + 2];
                    dest[i + 2] = b;
                }

                OverlayImage.UnlockBits(data);
                OverlayValid = true;
            }

            public void CreatePreview()
            {
                if (Image == null)
                {
                    Preview = new Bitmap(1, 1);
                    return;
                }

                Preview = new Bitmap(64, 64);

                using (Graphics g = Graphics.FromImage(Preview))
                {
                    g.InterpolationMode = InterpolationMode.Low;//HighQualityBicubic
                    g.Clear(Color.Transparent);
                    int w = Math.Min((int)Width, 64);
                    int h = Math.Min((int)Height, 64);
                    g.DrawImage(Image, new Rectangle((64 - w) / 2, (64 - h) / 2, w, h), new Rectangle(0, 0, Width, Height), GraphicsUnit.Pixel);

                    g.Save();
                }
            }

            public void CreateShadowPreview()
            {
                if (ShadowImage == null)
                {
                    ShadowPreview = new Bitmap(1, 1);
                    return;
                }

                ShadowPreview = new Bitmap(64, 64);

                using (Graphics g = Graphics.FromImage(ShadowPreview))
                {
                    g.InterpolationMode = InterpolationMode.Low;//HighQualityBicubic
                    g.Clear(Color.Transparent);
                    int w = Math.Min((int)Width, 64);
                    int h = Math.Min((int)Height, 64);
                    g.DrawImage(ShadowImage, new Rectangle((64 - w) / 2, (64 - h) / 2, w, h), new Rectangle(0, 0, Width, Height), GraphicsUnit.Pixel);

                    g.Save();
                }
            }

            public void CreateOverlayPreview()
            {
                if (OverlayImage == null)
                {
                    OverlayPreview = new Bitmap(1, 1);
                    return;
                }

                OverlayPreview = new Bitmap(64, 64);

                using (Graphics g = Graphics.FromImage(OverlayPreview))
                {
                    g.InterpolationMode = InterpolationMode.Low;//HighQualityBicubic
                    g.Clear(Color.Transparent);
                    int w = Math.Min((int)Width, 64);
                    int h = Math.Min((int)Height, 64);
                    g.DrawImage(OverlayImage, new Rectangle((64 - w) / 2, (64 - h) / 2, w, h), new Rectangle(0, 0, Width, Height), GraphicsUnit.Pixel);

                    g.Save();
                }
            }

            public void SaveHeader(BinaryWriter writer)
            {
                writer.Write(Position);

                writer.Write(Width);
                writer.Write(Height);
                writer.Write(OffSetX);
                writer.Write(OffSetY);

                writer.Write(ShadowType);
                writer.Write(ShadowWidth);
                writer.Write(ShadowHeight);
                writer.Write(ShadowOffSetX);
                writer.Write(ShadowOffSetY);

                writer.Write(OverlayWidth);
                writer.Write(OverlayHeight);
            }

            private SquishFlags DxtFlags
            {
                get
                {
                    return Version switch
                    {
                        0 => SquishFlags.Dxt1,
                        _ => SquishFlags.Dxt5,
                    };
                }
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
            ~Mir3Image()
            {
                Dispose(false);
            }

            #endregion
        }
    }
}
