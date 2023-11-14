using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using ManagedSquish;

namespace ImageManager
{
    public class WTLLibrary : IDisposable
    {
        private readonly string _fileName;

        public MImage[] Images;

        private BinaryReader _bReader;
        private int _count;
        private FileStream _fStream;
        private int[] _indexList;
        private bool _initialized;
        
        public WTLLibrary(string filename)
        {
            _fileName = filename;
            Initialize();
        }

        public void Initialize()
        {
            _initialized = true;
            _fStream = new FileStream(_fileName, FileMode.Open, FileAccess.ReadWrite);
            _bReader = new BinaryReader(_fStream);

            _fStream.Seek(28, SeekOrigin.Begin);

            _count = _bReader.ReadInt32();
            Images = new MImage[_count];
            _indexList = new int[_count];

            for (int i = 0; i < _count; i++)
                _indexList[i] = _bReader.ReadInt32();

             for (int i = 0; i < Images.Length; i++)
               CheckImage(i);
        }
        public void Close()
        {
            _fStream?.Dispose();
            _bReader?.Dispose();
        }

        public void CheckImage(int index)
        {
            if (!_initialized) Initialize();
            if (Images == null || index < 0 || index >= Images.Length || _indexList[index] <= 0) return;

            if (Images[index] == null)
            {
                _fStream.Position = _indexList[index];
                Images[index] = new MImage(_bReader);
            }

            if (Images[index].Texture == null)
            {
                _fStream.Position = _indexList[index] + 16;
                Images[index].CreateTexture(_bReader);
            }

            long max = _fStream.Length;
            for (int i = index + 1; i < Images.Length; i++)
            {
                if (_indexList[i] == 0) continue;

                max = _indexList[i];
                break;
            }

            if (_indexList[index] + 16 + Images[index].TLength < max)
            {
                _fStream.Position = _indexList[index] + 16 + Images[index].TLength;
                Images[index].CreateOverlayTexture(_bReader);
            }
        }

        public bool HasImage(int index)
        {
            return Images != null && index >= 0 && index < Images.Length && Images[index]?.Texture != null;
        }

        public void Dispose()
        {

            _indexList = null;
            _bReader?.Dispose();
            _bReader = null;
            _fStream?.Dispose();
            _fStream = null;

            for (int i = 0; i < Images.Length; i++)
            {
                if (Images[i] != null)
                    Images[i].Dispose();
            }

            Images = null;
        }


        public Mir3Library Convert()
        {
            string shadowPath = _fileName.Replace(".wtl", "_S.wtl");

            WTLLibrary shadowLibrary = null;
            if (File.Exists(shadowPath))
                shadowLibrary = new WTLLibrary(shadowPath);
            else
            {
                shadowPath = _fileName.Replace("Mon-", "MonS-");
                if (File.Exists(shadowPath))
                    shadowLibrary = new WTLLibrary(shadowPath);
            }

            Mir3Library lib = new Mir3Library
            {
                Images = new Mir3Image[Images.Length]
            };

            for (int i = 0; i < Images.Length; i++)
            {
                MImage image = Images[i];
                if (image?.Texture == null) continue;

                lib.Images[i] = image.Convert(shadowLibrary, i);

            }

            shadowLibrary?.Dispose();

            return lib;
        }
    }

    public class MImage : IDisposable
    {
        //Own Palette 
        public readonly short THeight;
        public readonly int TLength;
        public readonly short TOffSetX;
        public readonly short TOffSetY;
        public readonly byte TShadow;
        public readonly short TShadowX;
        public readonly short TShadowY;
        public readonly short TWidth;
        
        public Bitmap Texture;

        public short OHeight;
        public int OLength;
        public short OOffSetX;
        public short OOffSetY;
        public byte OShadow;
        public short OShadowX;
        public short OShadowY;
        public short OWidth;

        public Bitmap Overlay;
        public MImage(BinaryReader bReader)
        {
            TWidth = bReader.ReadInt16();
            THeight = bReader.ReadInt16();
            TOffSetX = bReader.ReadInt16();
            TOffSetY = bReader.ReadInt16();
            TShadowX = bReader.ReadInt16();
            TShadowY = bReader.ReadInt16();
            TLength = bReader.ReadByte() | bReader.ReadByte() << 8 | bReader.ReadByte() << 16;
            TShadow = bReader.ReadByte();
        }

        public unsafe void CreateTexture(BinaryReader bReader)
        {
            const int size = 8;
            var countList = new List<byte>();
            int tWidth = 2;

            while (tWidth < TWidth)
                tWidth *= 2;

            byte[] fBytes = bReader.ReadBytes(TLength);

            Texture = new Bitmap(TWidth, THeight);
            BitmapData textureData = Texture.LockBits(new Rectangle(0, 0, TWidth, THeight), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            var pixels = (byte*)textureData.Scan0;
            int cap = TWidth * THeight * 4;

            int offset = 0, blockOffSet = 0;

            while (blockOffSet < TLength)
            {
                countList.Clear();
                for (int i = 0; i < 8; i++)
                    countList.Add(fBytes[blockOffSet++]);

                for (int i = 0; i < countList.Count; i++)
                {
                    int count = countList[i];

                    if (i % 2 == 0)
                    {
                        offset += count;
                        continue;
                    }

                    for (int c = 0; c < count; c++)
                    {
                        if (blockOffSet >= fBytes.Length)
                            break;

                        var newPixels = new byte[64];
                        var block = new byte[size];

                        Array.Copy(fBytes, blockOffSet, block, 0, size);
                        blockOffSet += size;
                        DecompressBlock(newPixels, block);

                        int pixelOffSet = 0;
                        var sourcePixel = new byte[4];

                        for (int py = 0; py < 4; py++)
                        {
                            for (int px = 0; px < 4; px++)
                            {
                                int blockx = offset % (tWidth / 4);
                                int blocky = offset / (tWidth / 4);

                                int x = blockx * 4;
                                int y = blocky * 4;

                                int destPixel = ((y + py) * TWidth) * 4 + (x + px) * 4;

                                Array.Copy(newPixels, pixelOffSet, sourcePixel, 0, 4);
                                pixelOffSet += 4;

                                if (destPixel + 4 > cap)
                                    break;
                                for (int pc = 0; pc < 4; pc++)
                                    pixels[destPixel + pc] = sourcePixel[pc];
                            }
                        }
                        offset++;
                    }
                }
            }

            Texture.UnlockBits(textureData);
        }
        public unsafe void CreateOverlayTexture(BinaryReader bReader)
        {
            const int size = 8;
            var countList = new List<byte>();
            int tWidth = 2;

            OWidth = bReader.ReadInt16();
            OHeight = bReader.ReadInt16();
            OOffSetX = bReader.ReadInt16();
            OOffSetY = bReader.ReadInt16();
            OShadowX = bReader.ReadInt16();
            OShadowY = bReader.ReadInt16();
            OLength = bReader.ReadByte() | bReader.ReadByte() << 8 | bReader.ReadByte() << 16;
            OShadow = bReader.ReadByte();

            while (tWidth < OWidth)
                tWidth *= 2;

            byte[] fBytes = bReader.ReadBytes(OLength);

            Overlay = new Bitmap(OWidth, OHeight);
            BitmapData textureData = Overlay.LockBits(new Rectangle(0, 0, OWidth, OHeight), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            var pixels = (byte*)textureData.Scan0;
            int cap = OWidth * OHeight * 4;
            
            int offset = 0, blockOffSet = 0;

            while (blockOffSet < fBytes.Length)
            {
                countList.Clear();
                for (int i = 0; i < 8; i++)
                    countList.Add(fBytes[blockOffSet++]);
                for (int i = 0; i < countList.Count; i++)
                {
                    int count = countList[i];

                    if (i % 2 == 0)
                    {
                        offset += count;
                        continue;
                    }

                    for (int c = 0; c < count; c++)
                    {
                        if (blockOffSet >= fBytes.Length)
                            break;

                        var newPixels = new byte[64];
                        var block = new byte[size];

                        Array.Copy(fBytes, blockOffSet, block, 0, size);
                        blockOffSet += size;
                        DecompressBlock(newPixels, block);

                        int pixelOffSet = 0;
                        var sourcePixel = new byte[4];

                        for (int py = 0; py < 4; py++)
                        {
                            for (int px = 0; px < 4; px++)
                            {
                                int blockx = offset % (tWidth / 4);
                                int blocky = offset / (tWidth / 4);

                                int x = blockx * 4;
                                int y = blocky * 4;

                                int destPixel = ((y + py) * OWidth) * 4 + (x + px) * 4;

                                Array.Copy(newPixels, pixelOffSet, sourcePixel, 0, 4);
                                pixelOffSet += 4;

                                if (destPixel + 4 > cap)
                                    break;
                                for (int pc = 0; pc < 4; pc++)
                                    pixels[destPixel + pc] = sourcePixel[pc];
                            }
                        }

                        offset++;
                    }
                }
            }
            /*while (blockOffSet < fBytes.Length)
             {
                 countList.Clear();
                 for (int i = 0; i < 8; i++)
                     countList.Add(fBytes[blockOffSet++]);

                 for (int i = 0; i < countList.Count; i++)
                 {
                     int count = countList[i];

                     if (i % 2 == 0)
                     {
                         if (currentx >= tWidth)
                             currentx -= tWidth;

                         for (int off = 0; off < count; off++)
                         {
                             if (currentx < OWidth)
                                 offset++;

                             currentx += 4;

                             if (currentx >= tWidth)
                                 currentx -= tWidth;
                         }
                         continue;
                     }

                     for (int c = 0; c < count; c++)
                     {
                         if (blockOffSet >= fBytes.Length)
                             break;

                         var newPixels = new byte[64];
                         var block = new byte[size];

                         Array.Copy(fBytes, blockOffSet, block, 0, size);
                         blockOffSet += size;
                         DecompressBlock(newPixels, block);

                         int pixelOffSet = 0;
                         var sourcePixel = new byte[4];

                         for (int py = 0; py < 4; py++)
                         {
                             for (int px = 0; px < 4; px++)
                             {
                                 int blockx = offset % (OWidth / 4);
                                 int blocky = offset / (OWidth / 4);

                                 int x = blockx * 4;
                                 int y = blocky * 4;

                                 int destPixel = ((y + py) * OWidth) * 4 + (x + px) * 4;

                                 Array.Copy(newPixels, pixelOffSet, sourcePixel, 0, 4);
                                 pixelOffSet += 4;

                                 if (destPixel + 4 > cap)
                                     break;
                                 for (int pc = 0; pc < 4; pc++)
                                     pixels[destPixel + pc] = sourcePixel[pc];
                             }
                         }
                         offset++;
                         if (currentx >= OWidth)
                             currentx -= OWidth;
                         currentx += 4;
                     }
                 }
             }
             */
            Overlay.UnlockBits(textureData);
        }

        private static void DecompressBlock(IList<byte> newPixels, byte[] block)
        {
            var colours = new byte[8];
            Array.Copy(block, 0, colours, 0, 8);

            var codes = new byte[16];

            int a = Unpack(block, 0, codes, 0);
            int b = Unpack(block, 2, codes, 4);

            for (int i = 0; i < 3; i++)
            {
                int c = codes[i];
                int d = codes[4 + i];

                if (a <= b)
                {
                    codes[8 + i] = (byte)((c + d) / 2);
                    codes[12 + i] = 0;
                }
                else
                {
                    codes[8 + i] = (byte)((2 * c + d) / 3);
                    codes[12 + i] = (byte)((c + 2 * d) / 3);
                }
            }

            codes[8 + 3] = 255;
            codes[12 + 3] = (a <= b) ? (byte)0 : (byte)255;
            for (int i = 0; i < 4; i++)
            {
                if ((codes[i * 4] == 0) && (codes[(i * 4) + 1] == 0) && (codes[(i * 4) + 2] == 0) && (codes[(i * 4) + 3] == 255))
                { //dont ever use pure black cause that gives transparency issues
                    codes[i * 4] = 1;
                    codes[(i * 4) + 1] = 1;
                    codes[(i * 4) + 2] = 1;
                }
            }

            var indices = new byte[16];
            for (int i = 0; i < 4; i++)
            {
                byte packed = block[4 + i];

                indices[0 + i * 4] = (byte)(packed & 0x3);
                indices[1 + i * 4] = (byte)((packed >> 2) & 0x3);
                indices[2 + i * 4] = (byte)((packed >> 4) & 0x3);
                indices[3 + i * 4] = (byte)((packed >> 6) & 0x3);
            }

            for (int i = 0; i < 16; i++)
            {
                var offset = (byte)(4 * indices[i]);
                for (int j = 0; j < 4; j++)
                    newPixels[4 * i + j] = codes[offset + j];
            }
        }

        private static int Unpack(IList<byte> packed, int srcOffset, IList<byte> colour, int dstOffSet)
        {
            int value = packed[0 + srcOffset] | (packed[1 + srcOffset] << 8);

            // get components in the stored range
            var blue = (byte)((value >> 11) & 0x1F);
            var green = (byte)((value >> 5) & 0x3F);
            var red = (byte)(value & 0x1F);

            // Scale up to 8 Bit
            colour[0 + dstOffSet] = (byte)((red << 3) | (red >> 2));
            colour[1 + dstOffSet] = (byte)((green << 2) | (green >> 4));
            colour[2 + dstOffSet] = (byte)((blue << 3) | (blue >> 2));
            colour[3 + dstOffSet] = 255;

            return value;
        }

        public void Dispose()
        {
            Texture?.Dispose();
            Texture = null;

            Overlay?.Dispose();
            Overlay = null;
        }

        public Mir3Image Convert(WTLLibrary shadowLibrary, int index)
        {
            Mir3Image image = new Mir3Image
            {
                Width = TWidth,
                Height = THeight,
                OffSetX = TOffSetX,
                OffSetY = TOffSetY,
                ShadowType = TShadow,
                Data = GetBytes(Texture),
                ShadowOffSetX = TShadowX,
                ShadowOffSetY = TShadowY
            };
            
            if (shadowLibrary != null && shadowLibrary.HasImage(index))
            {
                MImage sImage = shadowLibrary.Images[index];

                image.ShadowWidth = sImage.TWidth;
                image.ShadowHeight = sImage.THeight;
                image.ShadowOffSetX = sImage.TOffSetX;
                image.ShadowOffSetY = sImage.TOffSetY;
                image.ShadowData = GetBytes(sImage.Texture);
            }

            if (Overlay != null)
            {
                image.OverlayWidth = OWidth;
                image.OverlayHeight = OHeight;


                image.OverLayData = GetBytes(Overlay);
            }

            return image;
        }


        public static unsafe byte[] GetBytes(Bitmap image)
        {
            if (image == null) return null;

            Size Size = new Size(image.Width, image.Height);

            int w = Size.Width + (4 - Size.Width % 4) % 4;
            int h = Size.Height + (4 - Size.Height % 4) % 4;

            //const int pureBlack = 255 << 24, almostBlack = 255 << 24 | 4 << 8;

            Bitmap temp = new Bitmap(w, h);
            BitmapData tempdata = temp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            BitmapData imagedata = image.LockBits(new Rectangle(0, 0, Size.Width, Size.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int* tempPixels = (int*)tempdata.Scan0;
            int* imagePixels = (int*)imagedata.Scan0;

            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                {
                    //if (imagePixels[y * Size.Width + x] == pureBlack)
                    //    tempPixels[y * w + x] = almostBlack;
                    //else
                    tempPixels[y * w + x] = imagePixels[y * Size.Width + x];
                }

            temp.UnlockBits(tempdata);
            image.UnlockBits(imagedata);
            
            BitmapData data = temp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] pixels = new byte[temp.Width * temp.Height * 4];


            Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);
            temp.UnlockBits(data);

            for (int i = 0; i < pixels.Length; i += 4)
            {
                //Reverse Red/Blue
                byte b = pixels[i];
                pixels[i] = pixels[i + 2];
                pixels[i + 2] = b;

                if (pixels[i] == 0 && pixels[i + 1] == 0 && pixels[i + 2] == 0)
                    pixels[i + 3] = 0; //Make Transparent

            }

            int count = Squish.GetStorageRequirements(w, h, SquishFlags.Dxt1);

            byte[] bytes = new byte[count];
            fixed (byte* dest = bytes)
            fixed (byte* source = pixels)
            {
                Squish.CompressImage((IntPtr)source, w, h, (IntPtr)dest, SquishFlags.Dxt1);
            }

            temp.Dispose();

            return bytes;
        }
    }
}