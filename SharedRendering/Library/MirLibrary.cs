using Shared.Envir;
using SharedRendering.LibraryFormat;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Shared.Rendering
{
    public sealed class MirLibrary : IDisposable
    {
        public static Func<DateTime> GetNow { get; set; } = () => DateTime.Now;
        public static Func<TimeSpan> GetCacheDuration { get; set; } = () => TimeSpan.FromMinutes(30);
        public static Func<bool> GetUseZlAtlasPages { get; set; } = () => false;
        public static Action DrawCounted { get; set; }

        public readonly object LoadLocker = new object();

        public int Version;

        public string FileName;

        private FileStream _FStream;
        private BinaryReader _BReader;

        public bool Loaded, Loading;

        public MirImage[] Images;
        public MirAtlasPage[] AtlasPages;
        public int AtlasGroupImageCount;
        public int AtlasPageSize;
        private readonly Dictionary<int, Zl2Entry> _zl2Entries = new Dictionary<int, Zl2Entry>();

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

            _BReader.BaseStream.Seek(0, SeekOrigin.Begin);
            if (TryReadCompressedContainer())
            {
                Loaded = true;
                return;
            }

            _BReader.BaseStream.Seek(0, SeekOrigin.Begin);

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

                    Images[i] = new MirImage(reader, Version)
                    {
                        OwnerName = Path.GetFileNameWithoutExtension(FileName)
                    };
                }

            }

            Loaded = true;
        }

        private bool TryReadCompressedContainer()
        {
            if (_BReader.BaseStream.Length < ZlContainerHeader.ByteCount)
                return false;

            if (!ZlContainerHeader.ReadSignature(_BReader))
                return false;

            Version = _BReader.ReadInt32();
            int imageCount = _BReader.ReadInt32();
            int atlasCount = _BReader.ReadInt32();
            _BReader.ReadByte();
            int flags = _BReader.ReadByte();
            _BReader.ReadInt16();
            long metadataOffset = _BReader.ReadInt64();
            int metadataSize = _BReader.ReadInt32();
            long indexOffset = _BReader.ReadInt64();
            int indexSize = _BReader.ReadInt32();

            _zl2Entries.Clear();
            _BReader.BaseStream.Seek(indexOffset, SeekOrigin.Begin);
            using (MemoryStream indexStream = new MemoryStream(_BReader.ReadBytes(indexSize)))
            using (BinaryReader indexReader = new BinaryReader(indexStream))
            {
                int entryCount = indexReader.ReadInt32();
                for (int i = 0; i < entryCount; i++)
                {
                    Zl2Entry entry = Zl2Entry.Read(indexReader);
                    _zl2Entries[entry.Id] = entry;
                }
            }

            _BReader.BaseStream.Seek(metadataOffset, SeekOrigin.Begin);
            using (MemoryStream metadataStream = new MemoryStream(_BReader.ReadBytes(metadataSize)))
            using (BinaryReader reader = new BinaryReader(metadataStream))
            {
                Version = reader.ReadInt32();
                int count = reader.ReadInt32();
                AtlasGroupImageCount = reader.ReadInt32();
                AtlasPageSize = reader.ReadInt32();
                Images = new MirImage[count];

                for (int i = 0; i < Images.Length; i++)
                {
                    if (!reader.ReadBoolean()) continue;

                    Images[i] = new MirImage(reader, Version)
                    {
                        OwnerName = Path.GetFileNameWithoutExtension(FileName)
                    };
                }

                if ((flags & ZlContainerHeader.HasAtlasFlag) != 0 && reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    int metadataAtlasCount = reader.ReadInt32();
                    int expectedAtlasCount = atlasCount > 0 ? atlasCount : metadataAtlasCount;
                    AtlasPages = new MirAtlasPage[expectedAtlasCount];

                    for (int i = 0; i < expectedAtlasCount; i++)
                    {
                        MirAtlasPage page = new MirAtlasPage(reader);
                        page.OwnerName = Path.GetFileNameWithoutExtension(FileName);
                        if (page.Id >= 0 && page.Id < AtlasPages.Length)
                            AtlasPages[page.Id] = page;
                    }
                }

                ReadAtlasLayerMappings(reader);
            }

            return true;
        }

        private void ReadAtlasLayerMappings(BinaryReader reader)
        {
            if (reader.BaseStream.Position + sizeof(int) > reader.BaseStream.Length)
                return;

            int imageLayerCount = reader.ReadInt32();
            for (int i = 0; i < imageLayerCount; i++)
            {
                int imageIndex = reader.ReadInt32();
                int shadowPage = reader.ReadInt32();
                Rectangle shadowSource = ReadRectangle(reader);
                int overlayPage = reader.ReadInt32();
                Rectangle overlaySource = ReadRectangle(reader);

                if (imageIndex < 0 || imageIndex >= Images.Length || Images[imageIndex] == null)
                    continue;

                Images[imageIndex].ShadowAtlasPage = shadowPage;
                Images[imageIndex].ShadowSourceRectangle = shadowSource;
                Images[imageIndex].OverlayAtlasPage = overlayPage;
                Images[imageIndex].OverlaySourceRectangle = overlaySource;
            }
        }

        private static Rectangle ReadRectangle(BinaryReader reader)
        {
            return new Rectangle(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16());
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

            if (type == ImageType.Image && !image.ImageValid)
            {
                image.CreateImage(_BReader, ReadCompressedPayload);

                if (!image.Image.IsValid)
                {
                    return null;
                }
            }

            return image;
        }
        public bool TryGetTexture(int index, ImageType type, out MirImage image, out RenderTexture texture, out Rectangle? sourceRectangle)
        {
            image = null;
            texture = default;
            sourceRectangle = null;

            if (!CheckImage(index))
            {
                return false;
            }

            image = Images[index];
            texture = GetRenderTexture(image, type, out sourceRectangle);

            return texture.IsValid;
        }
        private RenderTexture GetRenderTexture(MirImage image, ImageType type)
        {
            return GetRenderTexture(image, type, out _);
        }

        private RenderTexture GetRenderTexture(MirImage image, ImageType type, out Rectangle? sourceRectangle)
        {
            sourceRectangle = null;

            switch (type)
            {
                case ImageType.Image:
                    if (TryGetAtlasTexture(image, type, out RenderTexture atlasTexture, out Rectangle atlasSource))
                    {
                        sourceRectangle = atlasSource;
                        return atlasTexture;
                    }

                    if (!image.ImageValid)
                    {
                        image.CreateImage(_BReader, ReadCompressedPayload);
                    }
                    return image.Image;
                case ImageType.Shadow:
                    if (TryGetAtlasTexture(image, type, out RenderTexture shadowAtlasTexture, out Rectangle shadowAtlasSource))
                    {
                        sourceRectangle = shadowAtlasSource;
                        return shadowAtlasTexture;
                    }

                    if (!image.ShadowValid)
                    {
                        image.CreateShadow(_BReader, ReadCompressedPayload);
                    }
                    return image.Shadow;
                case ImageType.Overlay:
                    if (TryGetAtlasTexture(image, type, out RenderTexture overlayAtlasTexture, out Rectangle overlayAtlasSource))
                    {
                        sourceRectangle = overlayAtlasSource;
                        return overlayAtlasTexture;
                    }

                    if (!image.OverlayValid)
                    {
                        image.CreateOverlay(_BReader, ReadCompressedPayload);
                    }
                    return image.Overlay;
                default:
                    return default;
            }
        }

        private bool TryGetAtlasTexture(MirImage image, ImageType type, out RenderTexture texture, out Rectangle sourceRectangle)
        {
            texture = default;
            sourceRectangle = default;

            if (!GetUseZlAtlasPages())
            {
                return false;
            }

            if (!RenderingPipelineManager.SupportsAtlasTextures)
            {
                return false;
            }

            int atlasPage = image.GetAtlasPage(type);
            if (image.Version < 2 || AtlasPages == null || atlasPage < 0 || atlasPage >= AtlasPages.Length)
            {
                return false;
            }

            MirAtlasPage page = AtlasPages[atlasPage];
            if (page == null)
            {
                return false;
            }

            if (page.Layer != GetAtlasLayer(type))
            {
                return false;
            }

            if (!page.TextureValid)
                page.CreateTexture(_BReader, ReadCompressedPayload);

            if (!page.Texture.IsValid)
            {
                return false;
            }

            sourceRectangle = image.GetAtlasSourceRectangle(type);
            if (!IsValidAtlasSourceRectangle(sourceRectangle, image, type, page))
            {
                return false;
            }

            texture = page.Texture;
            return true;
        }

        private static bool IsValidAtlasSourceRectangle(Rectangle sourceRectangle, MirImage image, ImageType type, MirAtlasPage page)
        {
            if (sourceRectangle.X < 0 || sourceRectangle.Y < 0 || sourceRectangle.Width <= 0 || sourceRectangle.Height <= 0)
                return false;

            if (sourceRectangle.Right > page.Width || sourceRectangle.Bottom > page.Height)
                return false;

            Size expectedSize = image.GetLayerSize(type);
            return sourceRectangle.Width == expectedSize.Width && sourceRectangle.Height == expectedSize.Height;
        }

        private static ZlAtlasLayer GetAtlasLayer(ImageType type)
        {
            return type switch
            {
                ImageType.Shadow => ZlAtlasLayer.Shadow,
                ImageType.Overlay => ZlAtlasLayer.Overlay,
                _ => ZlAtlasLayer.Image,
            };
        }

        private bool CheckImage(int index)
        {
            if (!Loaded) ReadLibrary();

            while (!Loaded)
                Thread.Sleep(1);

            return index >= 0 && index < Images.Length && Images[index] != null;
        }

        private byte[] ReadCompressedPayload(int entryId)
        {
            if (!_zl2Entries.TryGetValue(entryId, out Zl2Entry entry))
                return null;

            lock (_BReader)
            {
                _BReader.BaseStream.Seek(entry.Offset, SeekOrigin.Begin);
                byte[] payload = _BReader.ReadBytes(entry.CompressedSize);
                return Decompress(payload, entry.UncompressedSize, entry.Compression);
            }
        }

        private static byte[] Decompress(byte[] payload, int uncompressedSize, ZlContainerCompression compression)
        {
            if (payload == null || payload.Length == 0)
                return Array.Empty<byte>();

            if (compression == ZlContainerCompression.None)
                return payload;

            if (compression != ZlContainerCompression.DeflateFast && compression != ZlContainerCompression.DeflateBest)
                throw new InvalidDataException($"Unsupported ZL v2 compression method: {(byte)compression}.");

            using (MemoryStream input = new MemoryStream(payload))
            using (DeflateStream deflate = new DeflateStream(input, CompressionMode.Decompress))
            using (MemoryStream output = new MemoryStream(uncompressedSize))
            {
                deflate.CopyTo(output);
                return output.ToArray();
            }
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
                    if (!image.ImageValid) image.CreateImage(_BReader, ReadCompressedPayload);
                    texture = image.Image;
                    break;
                case ImageType.Shadow:
                    if (!image.ShadowValid) image.CreateShadow(_BReader, ReadCompressedPayload);
                    texture = image.Shadow;

                    if (!texture.IsValid)
                    {
                        if (!image.ImageValid)
                        {
                            image.CreateImage(_BReader, ReadCompressedPayload);
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
                                TextureFilterMode oldTextureFilter = RenderingPipelineManager.GetTextureFilter();

                                RenderingPipelineManager.SetTextureFilter(TextureFilterMode.None);
                                if (oldOpacity != 0.5F)
                                {
                                    RenderingPipelineManager.SetOpacity(0.5F);
                                }

                                RenderingPipelineManager.DrawTexture(renderTexture, null, transform, System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero, Color.Black);
                                DrawCounted?.Invoke();

                                RenderingPipelineManager.SetOpacity(oldOpacity);
                                RenderingPipelineManager.SetTextureFilter(oldTextureFilter);

                                image.ExpireTime = GetNow() + GetCacheDuration();
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
                                DrawCounted?.Invoke();
                                RenderingPipelineManager.SetOpacity(oldOpacity);

                                image.ExpireTime = GetNow() + GetCacheDuration();
                                break;
                        }
                        return;
                    }
                    break;
                case ImageType.Overlay:
                    if (!image.OverlayValid) image.CreateOverlay(_BReader, ReadCompressedPayload);
                    texture = image.Overlay;
                    break;
                default:
                    return;
            }

            if (!texture.IsValid)
            {
                return;
            }

            Rectangle sourceRectangle = ClampSourceRectangle(area, image, type);
            if (sourceRectangle.Width <= 0 || sourceRectangle.Height <= 0)
            {
                RenderingPipelineManager.SetOpacity(oldOpacity);
                return;
            }

            RectangleF destinationRectangle = new RectangleF(x, y, sourceRectangle.Width, sourceRectangle.Height);

            RenderingPipelineManager.SetOpacity(opacity);

            RenderingPipelineManager.DrawTexture(texture, sourceRectangle, destinationRectangle, Color.FromArgb(colour.ToArgb()));
            DrawCounted?.Invoke();
            RenderingPipelineManager.SetOpacity(oldOpacity);

            image.ExpireTime = GetNow() + GetCacheDuration();
        }

        private static Rectangle ClampSourceRectangle(Rectangle area, MirImage image, ImageType type)
        {
            int width;
            int height;

            switch (type)
            {
                case ImageType.Shadow:
                    width = image.ShadowWidth;
                    height = image.ShadowHeight;
                    break;
                case ImageType.Overlay:
                    width = image.OverlayWidth;
                    height = image.OverlayHeight;
                    break;
                default:
                    width = image.Width;
                    height = image.Height;
                    break;
            }

            if (area.X >= width || area.Y >= height)
                return Rectangle.Empty;

            return new Rectangle(
                area.X,
                area.Y,
                Math.Min(area.Width, width - area.X),
                Math.Min(area.Height, height - area.Y));
        }

        public void Draw(int index, float x, float y, Color colour, bool useOffSet, float opacity, ImageType type, float scale = 1F)
        {
            if (!CheckImage(index))
            {
                return;
            }

            MirImage image = Images[index];

            RenderTexture texture;
            Rectangle? sourceRectangle = null;

            Matrix3x2 scaling;
            Matrix3x2 translation;

            float oldOpacity = RenderingPipelineManager.GetOpacity();
            switch (type)
            {
                case ImageType.Image:
                    texture = GetRenderTexture(image, type, out sourceRectangle);
                    if (useOffSet)
                    {
                        x += image.OffSetX;
                        y += image.OffSetY;
                    }
                    break;
                case ImageType.Shadow:
                    {
                        texture = GetRenderTexture(image, type, out sourceRectangle);

                        if (useOffSet)
                        {
                            x += image.ShadowOffSetX;
                            y += image.ShadowOffSetY;
                        }

                        if (!texture.IsValid)
                        {
                            if (!image.ImageValid) image.CreateImage(_BReader, ReadCompressedPayload);
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
                                        TextureFilterMode oldTextureFilter = RenderingPipelineManager.GetTextureFilter();

                                        RenderingPipelineManager.SetTextureFilter(TextureFilterMode.None);
                                        if (oldOpacity != 0.5F) RenderingPipelineManager.SetOpacity(0.5F);

                                        RenderingPipelineManager.DrawTexture(renderTexture, null, transform, System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero, Color.Black);
                                        DrawCounted?.Invoke();

                                        RenderingPipelineManager.SetOpacity(oldOpacity);
                                        RenderingPipelineManager.SetTextureFilter(oldTextureFilter);

                                        image.ExpireTime = GetNow() + GetCacheDuration();
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
                                        DrawCounted?.Invoke();
                                        RenderingPipelineManager.SetOpacity(oldOpacity);

                                        image.ExpireTime = GetNow() + GetCacheDuration();
                                        break;
                                    }
                            }

                            return;
                        }
                    }
                    break;
                case ImageType.Overlay:
                    texture = GetRenderTexture(image, type, out sourceRectangle);

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
            RenderingPipelineManager.DrawTexture(texture, sourceRectangle, transformMatrix, Vector3.Zero, vector, colour);

            DrawCounted?.Invoke();

            RenderingPipelineManager.SetOpacity(oldOpacity);

            image.ExpireTime = GetNow() + GetCacheDuration();
        }

        public void DrawBlendScaled(int index, float scaleX, float scaleY, Color colour, float x, float y, float angle, float opacity, ImageType type, bool useOffSet = false, byte shadow = 0)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];

            if (type == ImageType.Shadow) return;

            RenderTexture texture = GetRenderTexture(image, type, out Rectangle? sourceRectangle);
            if (!texture.IsValid) return;

            if (useOffSet)
            {
                x += image.OffSetX;
                y += image.OffSetY;
            }

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

            RenderingPipelineManager.DrawTextureBlend(texture, sourceRectangle, final, System.Numerics.Vector3.Zero, Vector3.Zero, colour, opacity);

            DrawCounted?.Invoke();

            image.ExpireTime = GetNow() + GetCacheDuration();
        }

        public void DrawBlend(int index, float size, Color colour, float x, float y, float angle, float opacity, ImageType type, bool useOffSet = false, byte shadow = 0)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];

            if (type == ImageType.Shadow) return;

            RenderTexture texture = GetRenderTexture(image, type, out Rectangle? sourceRectangle);

            if (!texture.IsValid) return;

            if (useOffSet)
            {
                x += image.OffSetX;
                y += image.OffSetY;
            }

            Matrix3x2 scaling = Matrix3x2.CreateScale(size);
            Matrix3x2 rotation = Matrix3x2.CreateRotation(angle);
            Matrix3x2 translation = Matrix3x2.CreateTranslation(x + (image.Width / 2), y + (image.Height / 2));

            var vector = new Vector3((image.Width / 2F) * -1F, (image.Height / 2F) * -1F, 0F);

            Matrix3x2 pipelineTransform = scaling * rotation * translation;
            RenderingPipelineManager.DrawTextureBlend(texture, sourceRectangle, pipelineTransform, System.Numerics.Vector3.Zero, vector, colour, opacity);

            DrawCounted?.Invoke();

            image.ExpireTime = GetNow() + GetCacheDuration();
        }

        public void DrawBlendCentered(int index, float size, Color colour, float x, float y, float angle, float opacity, ImageType type, bool useOffSet = false, byte shadow = 0)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];

            if (type == ImageType.Shadow) return;

            RenderTexture texture = GetRenderTexture(image, type, out Rectangle? sourceRectangle);

            if (!texture.IsValid) return;

            if (useOffSet)
            {
                x += image.OffSetX;
                y += image.OffSetY;
            }

            Matrix3x2 scaling = Matrix3x2.CreateScale(size);
            Matrix3x2 rotation = Matrix3x2.CreateRotation(angle);
            Matrix3x2 translation = Matrix3x2.CreateTranslation(x, y);

            Matrix3x2 pipelineTransform = scaling * rotation * translation;

            var vector = new Vector3(image.Width / 2F, image.Height / 2F, 0F);

            // Pass Center (w/2, h/2) and Translation (0)
            // DrawTexture will apply Translate(-Center) * Transform.
            // Result: Translate(-Center) * Scale * Rotate * Translate(Position).
            // This rotates around the center of the image and places it at Position.
            RenderingPipelineManager.DrawTextureBlend(texture, sourceRectangle, pipelineTransform, vector, System.Numerics.Vector3.Zero, colour, opacity);

            DrawCounted?.Invoke();

            image.ExpireTime = GetNow() + GetCacheDuration();
        }

        public void DrawBlend(int index, float x, float y, Color colour, bool useOffSet, float rate, ImageType type, byte shadow = 0)
        {
            if (!CheckImage(index))
            {
                return;
            }

            MirImage image = Images[index];

            RenderTexture texture;
            Rectangle? sourceRectangle = null;

            switch (type)
            {
                case ImageType.Image:
                    texture = GetRenderTexture(image, type, out sourceRectangle);
                    if (useOffSet)
                    {
                        x += image.OffSetX;
                        y += image.OffSetY;
                    }
                    break;
                case ImageType.Shadow:
                    return;
                case ImageType.Overlay:
                    texture = GetRenderTexture(image, type, out sourceRectangle);

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

            var vector = new System.Numerics.Vector3(x, y, 0F);

            RenderingPipelineManager.DrawTextureBlend(texture, sourceRectangle, Matrix3x2.Identity, System.Numerics.Vector3.Zero, vector, colour, rate);

            DrawCounted?.Invoke();

            image.ExpireTime = GetNow() + GetCacheDuration();
        }

        #region IDisposable Support

        public bool IsDisposed { get; private set; }

        public void DisposeTextures()
        {
            if (Images != null)
            {
                foreach (MirImage image in Images)
                    image?.DisposeTexture();
            }

            if (AtlasPages != null)
            {
                foreach (MirAtlasPage page in AtlasPages)
                    page?.Dispose();
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsDisposed = true;

                DisposeTextures();

                Images = null;
                AtlasPages = null;


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

    public sealed class MirAtlasPage : IDisposable
    {
        public int Id;
        public int Position;
        public short Width;
        public short Height;
        public ZlAtlasLayer Layer;
        public ZlImageCodec Codec;
        public ZlRuntimeTexturePreference RuntimePreference;
        public int DataSize;
        public int Bc7DataSize;
        public int FallbackDataSize;
        public string OwnerName;

        public RenderTexture Texture;
        public bool TextureValid { get; private set; }

        public MirAtlasPage(BinaryReader reader)
        {
            ZlAtlasPageMetadata metadata = ZlAtlasPageMetadata.Read(reader);
            Id = metadata.Id;
            Position = metadata.Position;
            Width = metadata.Width;
            Height = metadata.Height;
            Layer = metadata.Layer;
            Codec = metadata.Codec;
            DataSize = metadata.DataSize;
            RuntimePreference = metadata.RuntimePreference;
            Bc7DataSize = metadata.Bc7DataSize;
            FallbackDataSize = metadata.FallbackDataSize;
        }

        public void CreateTexture(BinaryReader reader, Func<int, byte[]> payloadReader = null)
        {
            if ((Position <= 0 && payloadReader == null) || Width <= 0 || Height <= 0 || DataSize + Bc7DataSize + FallbackDataSize <= 0)
                return;

            RenderTextureFormat textureFormat = ResolveRuntimeFormat(RuntimePreference, Bc7DataSize > 0, FallbackDataSize > 0);
            ZlPayloadSegment segment = ZlPayloadSegment.Select(Codec, textureFormat, DataSize, Bc7DataSize, FallbackDataSize);
            int offset = segment.Offset;
            int readSize = segment.Size;

            if (readSize <= 0)
                return;

            byte[] payload = payloadReader?.Invoke(Position);
            byte[] buffer;
            if (payload != null)
            {
                buffer = ReadPayloadSegment(payload, offset, readSize);
            }
            else
            {
                lock (reader)
                {
                    reader.BaseStream.Seek(Position + offset, SeekOrigin.Begin);
                    buffer = reader.ReadBytes(readSize);
                }
            }

            Size textureSize = GetTextureSize(Width, Height, textureFormat);
            if (textureFormat == RenderTextureFormat.Bgra32)
                buffer = DecodeBgra(buffer, Codec, Width, Height, out textureSize);

            Texture = RenderingPipelineManager.CreateTexture(textureSize, textureFormat, RenderTextureUsage.None, RenderTexturePool.Managed);

            using (TextureLock textureLock = RenderingPipelineManager.LockTexture(Texture, TextureLockMode.Discard))
            {
                Marshal.Copy(buffer, 0, textureLock.DataPointer, buffer.Length);
            }

            TextureValid = true;
        }

        private static byte[] ReadPayloadSegment(byte[] payload, int offset, int count)
        {
            if (payload == null || count <= 0 || offset < 0 || offset >= payload.Length)
                return Array.Empty<byte>();

            byte[] segment = new byte[Math.Min(count, payload.Length - offset)];
            Buffer.BlockCopy(payload, offset, segment, 0, segment.Length);
            return segment;
        }

        private static RenderTextureFormat ConvertFormat(ZlImageCodec codec)
        {
            return codec switch
            {
                ZlImageCodec.Dxt1 => RenderTextureFormat.Dxt1,
                ZlImageCodec.Dxt5 => RenderTextureFormat.Dxt5,
                ZlImageCodec.Bgra32 => RenderTextureFormat.Bgra32,
                ZlImageCodec.Bc7 => RenderTextureFormat.Bc7,
                ZlImageCodec.Png => RenderTextureFormat.Bgra32,
                _ => RenderTextureFormat.Dxt5,
            };
        }

        private static RenderTextureFormat ResolveRuntimeFormat(ZlRuntimeTexturePreference preference, bool hasBc7, bool hasFallback)
        {
            if (preference == ZlRuntimeTexturePreference.Dxt1 && hasBc7)
                return RenderTextureFormat.Dxt1;

            if (preference == ZlRuntimeTexturePreference.Dxt5 && hasFallback)
                return RenderTextureFormat.Dxt5;

            if ((preference == ZlRuntimeTexturePreference.Bc7 || preference == ZlRuntimeTexturePreference.Bc7Dxt5) && hasBc7 && RenderingPipelineManager.SupportsBc7Textures)
                return RenderTextureFormat.Bc7;

            if (preference == ZlRuntimeTexturePreference.Bc7Dxt5 && hasFallback)
                return RenderTextureFormat.Dxt5;

            return RenderTextureFormat.Bgra32;
        }

        private static Size GetTextureSize(short width, short height, RenderTextureFormat format)
        {
            if (format == RenderTextureFormat.Bgra32)
                return new Size(width, height);

            return new Size(
                width + (4 - width % 4) % 4,
                height + (4 - height % 4) % 4);
        }

        private static byte[] DecodeBgra(byte[] buffer, ZlImageCodec codec, short width, short height, out Size size)
        {
            if (codec == ZlImageCodec.Png)
                return DecodePngBgra(buffer, out size);

            if (codec == ZlImageCodec.Bc7)
                return DecodeBc7Bgra(buffer, width, height, out size);

            size = new Size(width, height);
            return buffer;
        }

        private static byte[] DecodePngBgra(byte[] buffer, out Size size)
        {
            using (MemoryStream stream = new MemoryStream(buffer))
            using (Bitmap bitmap = new Bitmap(stream))
            {
                size = bitmap.Size;
                BitmapData data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                try
                {
                    byte[] pixels = new byte[bitmap.Width * bitmap.Height * 4];
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        Marshal.Copy(IntPtr.Add(data.Scan0, y * data.Stride), pixels, y * bitmap.Width * 4, bitmap.Width * 4);
                    }

                    return pixels;
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }
            }
        }

        private static byte[] DecodeBc7Bgra(byte[] buffer, short width, short height, out Size size)
        {
            int safeWidth = Math.Max(0, (int)width);
            int safeHeight = Math.Max(0, (int)height);
            size = new Size(safeWidth, safeHeight);

            if (safeWidth == 0 || safeHeight == 0 || buffer == null || buffer.Length == 0)
                return Array.Empty<byte>();

            BCnEncoder.Decoder.BcDecoder decoder = new BCnEncoder.Decoder.BcDecoder();
            BCnEncoder.Shared.ColorRgba32[] pixels = decoder.DecodeRaw(buffer, safeWidth, safeHeight, BCnEncoder.Shared.CompressionFormat.Bc7);
            byte[] result = new byte[safeWidth * safeHeight * 4];

            for (int i = 0; i < pixels.Length && i * 4 + 3 < result.Length; i++)
            {
                int offset = i * 4;
                result[offset] = pixels[i].b;
                result[offset + 1] = pixels[i].g;
                result[offset + 2] = pixels[i].r;
                result[offset + 3] = pixels[i].a;
            }

            return result;
        }

        public void Dispose()
        {
            RenderingPipelineManager.ReleaseTexture(Texture);
            Texture = default;
            TextureValid = false;
        }
    }

    public sealed class MirImage : IDisposable, ITextureCacheItem
    {
        public int Version;
        public int Position;
        public string OwnerName;
        public int AtlasPage;
        public int ShadowAtlasPage = -1;
        public int OverlayAtlasPage = -1;
        public Rectangle SourceRectangle;
        public Rectangle ShadowSourceRectangle;
        public Rectangle OverlaySourceRectangle;
        public Rectangle VisibleBounds;
        public ZlImageCodec ImageCodec;
        public ZlImageCodec ShadowCodec;
        public ZlImageCodec OverlayCodec;
        public ZlRuntimeTexturePreference ImageRuntimePreference;
        public ZlRuntimeTexturePreference ShadowRuntimePreference;
        public ZlRuntimeTexturePreference OverlayRuntimePreference;
        private ZlImageCodec? _imageDataCodec;
        private RenderTextureFormat _imageTextureFormat;

        #region Texture

        public short Width;
        public short Height;
        public short OffSetX;
        public short OffSetY;
        public byte ShadowType;
        public RenderTexture Image;
        public bool ImageValid { get; private set; }
        public byte[] ImageData;
        public int StoredImageDataSize;
        public int ImageBc7DataSize;
        public int ImageFallbackDataSize;
        public int ImageDataSize
        {
            get { return Version >= 2 && StoredImageDataSize > 0 ? StoredImageDataSize : GetDataSize(Width, Height, ImageCodec); }
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
        public int StoredShadowDataSize;
        public int ShadowBc7DataSize;
        public int ShadowFallbackDataSize;
        public int ShadowDataSize
        {
            get { return Version >= 2 && StoredShadowDataSize > 0 ? StoredShadowDataSize : GetDataSize(ShadowWidth, ShadowHeight, ShadowCodec); }
        }
        #endregion

        #region Overlay
        public short OverlayWidth;
        public short OverlayHeight;

        public RenderTexture Overlay;
        public bool OverlayValid { get; private set; }
        public byte[] OverlayData;
        public int StoredOverlayDataSize;
        public int OverlayBc7DataSize;
        public int OverlayFallbackDataSize;
        public int OverlayDataSize
        {
            get { return Version >= 2 && StoredOverlayDataSize > 0 ? StoredOverlayDataSize : GetDataSize(OverlayWidth, OverlayHeight, OverlayCodec); }
        }
        #endregion

        public DateTime ExpireTime { get; set; }

        private Rectangle _visibleBounds = default;

        public MirImage(BinaryReader reader, int version)
        {
            ZlImageMetadata metadata = ZlImageMetadata.Read(reader, version);
            Version = metadata.Version;
            Position = metadata.Position;
            Width = metadata.Width;
            Height = metadata.Height;
            OffSetX = metadata.OffSetX;
            OffSetY = metadata.OffSetY;
            ShadowType = metadata.ShadowType;
            ShadowWidth = metadata.ShadowWidth;
            ShadowHeight = metadata.ShadowHeight;
            ShadowOffSetX = metadata.ShadowOffSetX;
            ShadowOffSetY = metadata.ShadowOffSetY;
            OverlayWidth = metadata.OverlayWidth;
            OverlayHeight = metadata.OverlayHeight;
            AtlasPage = metadata.AtlasPage;
            SourceRectangle = metadata.SourceRectangle;
            VisibleBounds = metadata.VisibleBounds;
            ImageCodec = metadata.ImageCodec;
            ShadowCodec = metadata.ShadowCodec;
            OverlayCodec = metadata.OverlayCodec;
            ImageRuntimePreference = metadata.ImageRuntimePreference;
            ShadowRuntimePreference = metadata.ShadowRuntimePreference;
            OverlayRuntimePreference = metadata.OverlayRuntimePreference;
            StoredImageDataSize = metadata.StoredImageDataSize;
            ImageBc7DataSize = metadata.ImageBc7DataSize;
            ImageFallbackDataSize = metadata.ImageFallbackDataSize;
            StoredShadowDataSize = metadata.StoredShadowDataSize;
            ShadowBc7DataSize = metadata.ShadowBc7DataSize;
            ShadowFallbackDataSize = metadata.ShadowFallbackDataSize;
            StoredOverlayDataSize = metadata.StoredOverlayDataSize;
            OverlayBc7DataSize = metadata.OverlayBc7DataSize;
            OverlayFallbackDataSize = metadata.OverlayFallbackDataSize;
        }

        public Rectangle GetVisibleBounds()
        {
            if (_visibleBounds != default)
                return _visibleBounds;

            _visibleBounds = VisibleBounds != default ? VisibleBounds : CalculateVisibleBounds();

            return _visibleBounds;
        }

        public int GetAtlasPage(ImageType type)
        {
            return type switch
            {
                ImageType.Shadow => ShadowAtlasPage,
                ImageType.Overlay => OverlayAtlasPage,
                _ => AtlasPage,
            };
        }

        public Rectangle GetAtlasSourceRectangle(ImageType type)
        {
            return type switch
            {
                ImageType.Shadow => ShadowSourceRectangle,
                ImageType.Overlay => OverlaySourceRectangle,
                _ => SourceRectangle,
            };
        }

        public Size GetLayerSize(ImageType type)
        {
            return type switch
            {
                ImageType.Shadow => new Size(ShadowWidth, ShadowHeight),
                ImageType.Overlay => new Size(OverlayWidth, OverlayHeight),
                _ => new Size(Width, Height),
            };
        }

        private Rectangle CalculateVisibleBounds()
        {
            if (!ImageValid || ImageData == null || Width <= 0 || Height <= 0)
                return new Rectangle(0, 0, Width, Height);

            int minX = Width;
            int minY = Height;
            int maxX = -1;
            int maxY = -1;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (!VisiblePixel(new Point(x, y), true)) continue;

                    if (x < minX) minX = x;
                    if (y < minY) minY = y;
                    if (x > maxX) maxX = x;
                    if (y > maxY) maxY = y;
                }
            }

            if (maxX < minX || maxY < minY)
                return new Rectangle(0, 0, Width, Height);

            return Rectangle.FromLTRB(minX, minY, maxX + 1, maxY + 1);
        }

        public bool VisiblePixel(Point p, bool acurrate)
        {
            if (p.X < 0 || p.Y < 0 || !ImageValid || ImageData == null) return false;

            int w = Width + (4 - Width % 4) % 4;
            int h = Height + (4 - Height % 4) % 4;

            ZlImageCodec dataCodec = _imageDataCodec ?? ImageCodec;

            if (dataCodec == ZlImageCodec.Bgra32)
            {
                w = Width;
                h = Height;
            }

            if (dataCodec == ZlImageCodec.Png)
            {
                w = Width;
                h = Height;
            }

            if (p.X >= w || p.Y >= h)
                return false;

            if (dataCodec == ZlImageCodec.Bgra32 || dataCodec == ZlImageCodec.Png)
            {
                int rawIndex = (p.Y * w + p.X) * 4 + 3;
                return rawIndex >= 0 && rawIndex < ImageData.Length && ImageData[rawIndex] != 0;
            }

            if (_imageTextureFormat == RenderTextureFormat.Bc7)
                return true;

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

            _visibleBounds = Rectangle.Empty;

            RenderingPipelineManager.UnregisterTextureCache(this);
        }

        public void CreateImage(BinaryReader reader, Func<int, byte[]> payloadReader = null)
        {
            if (Position == 0 && payloadReader == null) return;

            RenderTextureFormat textureFormat = ResolveRuntimeFormat(ImageCodec, ImageRuntimePreference, ImageBc7DataSize > 0, ImageFallbackDataSize > 0);
            Size textureSize = GetTextureSize(Width, Height, textureFormat);
            int w = textureSize.Width;
            int h = textureSize.Height;

            if (w == 0 || h == 0) return;

            byte[] buffer;

            lock (reader)
            {
                ZlPayloadSegment segment = ZlPayloadSegment.Select(ImageCodec, textureFormat, ImageDataSize, ImageBc7DataSize, ImageFallbackDataSize);
                int payloadOffset = segment.Offset;
                int payloadSize = segment.Size;

                byte[] payload = payloadReader?.Invoke(Position);
                if (payload != null)
                {
                    buffer = ReadPayloadSegment(payload, payloadOffset, payloadSize);
                }
                else
                {
                    reader.BaseStream.Seek(Position + payloadOffset, SeekOrigin.Begin);
                    buffer = reader.ReadBytes(payloadSize);
                }
            }

            if (textureFormat == RenderTextureFormat.Bgra32)
            {
                buffer = DecodeBgra(buffer, ImageCodec, Width, Height, out textureSize);
                _imageDataCodec = ZlImageCodec.Bgra32;
            }
            else
            {
                _imageDataCodec = textureFormat switch
                {
                    RenderTextureFormat.Dxt1 => ZlImageCodec.Dxt1,
                    RenderTextureFormat.Dxt5 => ZlImageCodec.Dxt5,
                    RenderTextureFormat.Bc7 => ZlImageCodec.Bc7,
                    _ => ImageCodec,
                };
            }
            _imageTextureFormat = textureFormat;

            ImageData = buffer;

            Image = RenderingPipelineManager.CreateTexture(textureSize, textureFormat, RenderTextureUsage.None, RenderTexturePool.Managed);

            using (TextureLock textureLock = RenderingPipelineManager.LockTexture(Image, TextureLockMode.Discard))
            {
                Marshal.Copy(buffer, 0, textureLock.DataPointer, buffer.Length);
            }

            ImageValid = true;
            ExpireTime = MirLibrary.GetNow() + MirLibrary.GetCacheDuration();
            RenderingPipelineManager.RegisterTextureCache(this);
        }
        public void CreateShadow(BinaryReader reader, Func<int, byte[]> payloadReader = null)
        {
            if (Position == 0 && payloadReader == null) return;

            if (!ImageValid)
                CreateImage(reader, payloadReader);

            RenderTextureFormat textureFormat = ResolveRuntimeFormat(ShadowCodec, ShadowRuntimePreference, ShadowBc7DataSize > 0, ShadowFallbackDataSize > 0);
            Size textureSize = GetTextureSize(ShadowWidth, ShadowHeight, textureFormat);
            int w = textureSize.Width;
            int h = textureSize.Height;

            if (w == 0 || h == 0) return;

            byte[] buffer;

            lock (reader)
            {
                int imagePayloadSize = ImageDataSize + ImageBc7DataSize + ImageFallbackDataSize;
                ZlPayloadSegment segment = ZlPayloadSegment.Select(ShadowCodec, textureFormat, ShadowDataSize, ShadowBc7DataSize, ShadowFallbackDataSize);
                int payloadOffset = segment.Offset;
                int payloadSize = segment.Size;

                byte[] payload = payloadReader?.Invoke(Position);
                if (payload != null)
                {
                    buffer = ReadPayloadSegment(payload, imagePayloadSize + payloadOffset, payloadSize);
                }
                else
                {
                    reader.BaseStream.Seek(Position + imagePayloadSize + payloadOffset, SeekOrigin.Begin);
                    buffer = reader.ReadBytes(payloadSize);
                }
            }

            if (textureFormat == RenderTextureFormat.Bgra32)
                buffer = DecodeBgra(buffer, ShadowCodec, ShadowWidth, ShadowHeight, out textureSize);

            ShadowData = buffer;

            Shadow = RenderingPipelineManager.CreateTexture(textureSize, textureFormat, RenderTextureUsage.None, RenderTexturePool.Managed);

            using (TextureLock textureLock = RenderingPipelineManager.LockTexture(Shadow, TextureLockMode.Discard))
            {
                Marshal.Copy(buffer, 0, textureLock.DataPointer, buffer.Length);
            }

            ShadowValid = true;
        }
        public void CreateOverlay(BinaryReader reader, Func<int, byte[]> payloadReader = null)
        {
            if (Position == 0 && payloadReader == null) return;

            if (!ImageValid)
                CreateImage(reader, payloadReader);

            RenderTextureFormat textureFormat = ResolveRuntimeFormat(OverlayCodec, OverlayRuntimePreference, OverlayBc7DataSize > 0, OverlayFallbackDataSize > 0);
            Size textureSize = GetTextureSize(OverlayWidth, OverlayHeight, textureFormat);
            int w = textureSize.Width;
            int h = textureSize.Height;

            if (w == 0 || h == 0) return;

            byte[] buffer;

            lock (reader)
            {
                int imagePayloadSize = ImageDataSize + ImageBc7DataSize + ImageFallbackDataSize;
                int shadowPayloadSize = ShadowDataSize + ShadowBc7DataSize + ShadowFallbackDataSize;
                ZlPayloadSegment segment = ZlPayloadSegment.Select(OverlayCodec, textureFormat, OverlayDataSize, OverlayBc7DataSize, OverlayFallbackDataSize);
                int payloadOffset = segment.Offset;
                int payloadSize = segment.Size;

                byte[] payload = payloadReader?.Invoke(Position);
                if (payload != null)
                {
                    buffer = ReadPayloadSegment(payload, imagePayloadSize + shadowPayloadSize + payloadOffset, payloadSize);
                }
                else
                {
                    reader.BaseStream.Seek(Position + imagePayloadSize + shadowPayloadSize + payloadOffset, SeekOrigin.Begin);
                    buffer = reader.ReadBytes(payloadSize);
                }
            }

            if (textureFormat == RenderTextureFormat.Bgra32)
                buffer = DecodeBgra(buffer, OverlayCodec, OverlayWidth, OverlayHeight, out textureSize);

            OverlayData = buffer;

            Overlay = RenderingPipelineManager.CreateTexture(textureSize, textureFormat, RenderTextureUsage.None, RenderTexturePool.Managed);

            using (TextureLock textureLock = RenderingPipelineManager.LockTexture(Overlay, TextureLockMode.Discard))
            {
                Marshal.Copy(buffer, 0, textureLock.DataPointer, buffer.Length);
            }

            OverlayValid = true;
        }

        private static int GetDataSize(short width, short height, ZlImageCodec codec)
        {
            return codec switch
            {
                ZlImageCodec.Dxt1 => GetBlockCount(width, height) * 8,
                ZlImageCodec.Dxt5 => GetBlockCount(width, height) * 16,
                ZlImageCodec.Bgra32 => Math.Max(0, (int)width) * Math.Max(0, (int)height) * 4,
                ZlImageCodec.Bc7 => GetBlockCount(width, height) * 16,
                ZlImageCodec.Png => 0,
                _ => GetBlockCount(width, height) * 16,
            };
        }

        private static Size GetTextureSize(short width, short height, ZlImageCodec codec)
        {
            if (codec == ZlImageCodec.Bgra32 || codec == ZlImageCodec.Png)
                return new Size(width, height);

            return new Size(
                width + (4 - width % 4) % 4,
                height + (4 - height % 4) % 4);
        }

        private static int GetBlockCount(short width, short height)
        {
            if (width <= 0 || height <= 0)
                return 0;

            int blocksX = Math.Max(1, (width + 3) / 4);
            int blocksY = Math.Max(1, (height + 3) / 4);
            return blocksX * blocksY;
        }

        private static RenderTextureFormat ConvertFormat(ZlImageCodec codec)
        {
            return codec switch
            {
                ZlImageCodec.Dxt1 => RenderTextureFormat.Dxt1,
                ZlImageCodec.Dxt5 => RenderTextureFormat.Dxt5,
                ZlImageCodec.Bgra32 => RenderTextureFormat.Bgra32,
                ZlImageCodec.Bc7 => RenderTextureFormat.Bc7,
                ZlImageCodec.Png => RenderTextureFormat.Bgra32,
                _ => RenderTextureFormat.Dxt5,
            };
        }

        private static RenderTextureFormat ResolveRuntimeFormat(ZlImageCodec codec, ZlRuntimeTexturePreference preference, bool hasBc7, bool hasFallback)
        {
            if (codec == ZlImageCodec.Bc7 && !RenderingPipelineManager.SupportsBc7Textures)
                return hasFallback ? RenderTextureFormat.Dxt5 : RenderTextureFormat.Bgra32;

            if (codec != ZlImageCodec.Png)
                return ConvertFormat(codec);

            if (preference == ZlRuntimeTexturePreference.Dxt1 && hasBc7)
                return RenderTextureFormat.Dxt1;

            if (preference == ZlRuntimeTexturePreference.Dxt5 && hasFallback)
                return RenderTextureFormat.Dxt5;

            if ((preference == ZlRuntimeTexturePreference.Bc7 || preference == ZlRuntimeTexturePreference.Bc7Dxt5) && hasBc7 && RenderingPipelineManager.SupportsBc7Textures)
                return RenderTextureFormat.Bc7;

            if (preference == ZlRuntimeTexturePreference.Bc7Dxt5 && hasFallback)
                return RenderTextureFormat.Dxt5;

            return RenderTextureFormat.Bgra32;
        }

        private static Size GetTextureSize(short width, short height, RenderTextureFormat format)
        {
            if (format == RenderTextureFormat.Bgra32)
                return new Size(width, height);

            return new Size(
                width + (4 - width % 4) % 4,
                height + (4 - height % 4) % 4);
        }

        private static byte[] DecodeBgra(byte[] buffer, ZlImageCodec codec, short width, short height, out Size size)
        {
            if (codec == ZlImageCodec.Png)
                return DecodePngBgra(buffer, out size);

            if (codec == ZlImageCodec.Bc7)
                return DecodeBc7Bgra(buffer, width, height, out size);

            size = new Size(width, height);
            return buffer;
        }

        private static byte[] DecodePngBgra(byte[] buffer, out Size size)
        {
            using (MemoryStream stream = new MemoryStream(buffer))
            using (Bitmap bitmap = new Bitmap(stream))
            {
                size = bitmap.Size;
                BitmapData data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                try
                {
                    byte[] pixels = new byte[bitmap.Width * bitmap.Height * 4];
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        Marshal.Copy(IntPtr.Add(data.Scan0, y * data.Stride), pixels, y * bitmap.Width * 4, bitmap.Width * 4);
                    }

                    return pixels;
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }
            }
        }

        private static byte[] DecodeBc7Bgra(byte[] buffer, short width, short height, out Size size)
        {
            int safeWidth = Math.Max(0, (int)width);
            int safeHeight = Math.Max(0, (int)height);
            size = new Size(safeWidth, safeHeight);

            if (safeWidth == 0 || safeHeight == 0 || buffer == null || buffer.Length == 0)
                return Array.Empty<byte>();

            BCnEncoder.Decoder.BcDecoder decoder = new BCnEncoder.Decoder.BcDecoder();
            BCnEncoder.Shared.ColorRgba32[] pixels = decoder.DecodeRaw(buffer, safeWidth, safeHeight, BCnEncoder.Shared.CompressionFormat.Bc7);
            byte[] result = new byte[safeWidth * safeHeight * 4];

            for (int i = 0; i < pixels.Length && i * 4 + 3 < result.Length; i++)
            {
                int offset = i * 4;
                result[offset] = pixels[i].b;
                result[offset + 1] = pixels[i].g;
                result[offset + 2] = pixels[i].r;
                result[offset + 3] = pixels[i].a;
            }

            return result;
        }

        private static byte[] ReadPayloadSegment(byte[] payload, int offset, int count)
        {
            if (payload == null || count <= 0 || offset < 0 || offset >= payload.Length)
                return Array.Empty<byte>();

            byte[] segment = new byte[Math.Min(count, payload.Length - offset)];
            Buffer.BlockCopy(payload, offset, segment, 0, segment.Length);
            return segment;
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
}
