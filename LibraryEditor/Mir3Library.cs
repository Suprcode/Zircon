using ManagedSquish;
using BCnEncoder.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.IO.Compression;
using BcDecoder = BCnEncoder.Decoder.BcDecoder;
using BcEncoder = BCnEncoder.Encoder.BcEncoder;
using BcCompressionQuality = BCnEncoder.Encoder.CompressionQuality;
using BcPixelFormat = BCnEncoder.Encoder.PixelFormat;

namespace LibraryEditor
{

    public enum ImageType
    {
        Image,
        Shadow,
        Overlay,
    }

    public enum ZlImageCodec : byte
    {
        Dxt1,
        Dxt5,
        Bgra32,
        Bc7,
        Png,
    }

    public enum ZlRuntimeTexturePreference : byte
    {
        None,
        Bgra32,
        Bc7Dxt5,
        Bc7,
        Dxt1,
        Dxt5,
        SourceType,
    }

    public enum ZlContainerCompression : byte
    {
        None,
        DeflateFast,
        DeflateBest,
    }

    public enum ZlEntryType : byte
    {
        ImagePayload = 1,
        AtlasPagePayload = 4,
    }

    public enum ZlAtlasLayer : byte
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
        /// V2 - Compressed random-access container with atlas/codec metadata and entry-indexed payloads.
        /// </summary>
        public const int LIBRARY_VERSION = 1;
        public const int COMPRESSED_LIBRARY_VERSION = 2;
        private static readonly byte[] CompressedContainerSignature = Encoding.ASCII.GetBytes("ZL2");
        private static readonly int CompressedContainerHeaderByteCount = CompressedContainerSignature.Length + sizeof(int) * 5 + sizeof(byte) * 2 + sizeof(short) + sizeof(long) * 2;
        private const int CompressedContainerHasAtlasFlag = 1;

        public int Version;

        public string FileName;
        public string _fileName;

        private FileStream _fStream;
        private BinaryReader _bReader;
        private readonly Dictionary<int, Zl2Entry> _zl2Entries = new Dictionary<int, Zl2Entry>();

        public List<Mir3Image> Images;
        public List<Mir3AtlasPage> AtlasPages = new List<Mir3AtlasPage>();
        public int AtlasGroupImageCount;
        public int AtlasPageSize;
        public ZlContainerCompression ContainerCompression = ZlContainerCompression.DeflateBest;

        public bool UseBlackKeyTransparency { get; }
        public string LastCompressionReport { get; private set; }

        public static string GetConvertedLibraryPath(string sourceFileName)
        {
            string directory = Path.GetDirectoryName(sourceFileName) ?? string.Empty;
            string convertedDirectory = Path.Combine(directory, "Converted");
            Directory.CreateDirectory(convertedDirectory);
            return Path.Combine(convertedDirectory, Path.ChangeExtension(Path.GetFileName(sourceFileName), ".Zl"));
        }

        public Mir3Library(string fileName, bool useBlackKeyTransparency = false)
        {
            FileName = fileName;
            _fileName = Path.ChangeExtension(fileName, null);
            UseBlackKeyTransparency = useBlackKeyTransparency;
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

            _bReader.BaseStream.Seek(0, SeekOrigin.Begin);
            if (TryReadCompressedContainer())
                return;

            _bReader.BaseStream.Seek(0, SeekOrigin.Begin);

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

        private bool TryReadCompressedContainer()
        {
            if (_bReader.BaseStream.Length < CompressedContainerHeaderByteCount)
                return false;

            if (!ReadCompressedContainerSignature(_bReader))
                return false;

            Version = _bReader.ReadInt32();
            int imageCount = _bReader.ReadInt32();
            int atlasCount = _bReader.ReadInt32();
            ZlContainerCompression defaultCompression = (ZlContainerCompression)_bReader.ReadByte();
            int flags = _bReader.ReadByte();
            _bReader.ReadInt16();
            long metadataOffset = _bReader.ReadInt64();
            int metadataSize = _bReader.ReadInt32();
            long indexOffset = _bReader.ReadInt64();
            int indexSize = _bReader.ReadInt32();

            _zl2Entries.Clear();
            _bReader.BaseStream.Seek(indexOffset, SeekOrigin.Begin);
            using (MemoryStream indexStream = new MemoryStream(_bReader.ReadBytes(indexSize)))
            using (BinaryReader indexReader = new BinaryReader(indexStream))
            {
                int entryCount = indexReader.ReadInt32();
                for (int i = 0; i < entryCount; i++)
                {
                    Zl2Entry entry = Zl2Entry.Read(indexReader);
                    _zl2Entries[entry.Id] = entry;
                }
            }

            _bReader.BaseStream.Seek(metadataOffset, SeekOrigin.Begin);
            using (MemoryStream metadataStream = new MemoryStream(_bReader.ReadBytes(metadataSize)))
            using (BinaryReader reader = new BinaryReader(metadataStream))
            {
                int metadataVersion = reader.ReadInt32();
                int count = reader.ReadInt32();
                AtlasGroupImageCount = reader.ReadInt32();
                AtlasPageSize = reader.ReadInt32();
                Version = metadataVersion;
                Images.Clear();

                for (int i = 0; i < count; i++)
                    Images.Add(null);

                for (int i = 0; i < Images.Count; i++)
                {
                    if (!reader.ReadBoolean()) continue;

                    Images[i] = new Mir3Image(reader, Version);
                }

                AtlasPages.Clear();
                if ((flags & CompressedContainerHasAtlasFlag) != 0 && reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    int metadataAtlasCount = reader.ReadInt32();
                    int expectedAtlasCount = atlasCount > 0 ? atlasCount : metadataAtlasCount;
                    for (int i = 0; i < expectedAtlasCount; i++)
                        AtlasPages.Add(null);

                    for (int i = 0; i < expectedAtlasCount; i++)
                    {
                        Mir3AtlasPage page = new Mir3AtlasPage(reader);
                        if (page.Id >= 0 && page.Id < AtlasPages.Count)
                            AtlasPages[page.Id] = page;
                    }
                }

                ReadAtlasLayerMappings(reader);
            }

            for (int i = 0; i < Images.Count; i++)
            {
                if (Images[i] == null) continue;

                CreateImage(i, ImageType.Image);
                CreateImage(i, ImageType.Shadow);
                CreateImage(i, ImageType.Overlay);
            }

            return true;
        }

        private static bool ReadCompressedContainerSignature(BinaryReader reader)
        {
            byte[] signature = reader.ReadBytes(CompressedContainerSignature.Length);
            if (signature.Length != CompressedContainerSignature.Length)
                return false;

            for (int i = 0; i < CompressedContainerSignature.Length; i++)
            {
                if (signature[i] != CompressedContainerSignature[i])
                    return false;
            }

            return true;
        }

        public void Close()
        {
            if (_bReader != null)
                _bReader.Dispose();
            if (_fStream != null)
                _fStream.Dispose();

            _bReader = null;
            _fStream = null;
        }
        public Mir3Image CreateImage(int index, ImageType type)
        {
            if (!CheckImage(index)) return null;

            Mir3Image image = Images[index];
            Bitmap bmp;

            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid) image.CreateImage(_bReader, ReadCompressedPayload);
                    bmp = image.Image;
                    break;
                case ImageType.Shadow:
                    if (!image.ShadowValid) image.CreateShadow(_bReader, ReadCompressedPayload);
                    bmp = image.ShadowImage;
                    break;
                case ImageType.Overlay:
                    if (!image.OverlayValid) image.CreateOverlay(_bReader, ReadCompressedPayload);
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
            Mir3Image mImage = new Mir3Image(image, COMPRESSED_LIBRARY_VERSION, UseBlackKeyTransparency) { OffSetX = x, OffSetY = y };

            Images.Add(mImage);
        }

        public void ReplaceImage(int Index, Bitmap image, short x, short y)
        {
            Mir3Image mImage = new Mir3Image(image, COMPRESSED_LIBRARY_VERSION, UseBlackKeyTransparency) { OffSetX = x, OffSetY = y };

            Images[Index] = mImage;
        }

        public void InsertImage(int index, Bitmap image, short x, short y)
        {
            Mir3Image mImage = new Mir3Image(image, COMPRESSED_LIBRARY_VERSION, UseBlackKeyTransparency) { OffSetX = x, OffSetY = y };

            Images.Insert(index, mImage);
        }

        public void Save(string path)
        {
            if (Version >= COMPRESSED_LIBRARY_VERSION)
            {
                SaveCompressedContainer(path, ContainerCompression);
                return;
            }

            //|Header Size|Count|T|Header|F|F|T|Header|F|T|Header...|Image|Image|Im...

            int headerSize = 4 + Images.Count;

            foreach (Mir3Image image in Images)
            {
                if (image == null || image.DataSize == 0) continue;

                headerSize += image.HeaderSize;
            }

            int position = headerSize + 4;

            foreach (Mir3Image image in Images)
            {
                if (image == null || image.DataSize == 0) continue;

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
                    writer.Write(image != null && image.DataSize > 0);

                    if (image == null || image.DataSize == 0) continue;

                    image.SaveHeader(writer);
                }

                foreach (Mir3Image image in Images)
                {
                    if (image == null || image.DataSize == 0) continue;

                    image.WritePayload(writer);
                }

                File.WriteAllBytes(path, buffer.ToArray());
            }
        }

        private void SaveCompressedContainer(string path, ZlContainerCompression compression)
        {
            int oldVersion = Version;
            Version = COMPRESSED_LIBRARY_VERSION;
            LastCompressionReport = null;

            List<Zl2EntryWrite> entries = new List<Zl2EntryWrite>();
            List<Zl2EntryCompressionStats> stats = new List<Zl2EntryCompressionStats>();

            using (MemoryStream metadata = new MemoryStream())
            using (BinaryWriter metadataWriter = new BinaryWriter(metadata))
            {
                metadataWriter.Write(Version);
                metadataWriter.Write(Images.Count);
                metadataWriter.Write(AtlasGroupImageCount);
                metadataWriter.Write(AtlasPageSize);

                int entryId = 0;
                foreach (Mir3Image image in Images)
                {
                    bool hasImage = image != null;
                    bool hasPayload = hasImage && image.DataSize > 0;
                    metadataWriter.Write(hasImage);

                    if (!hasImage)
                        continue;

                    image.Position = hasPayload ? entryId : -1;
                    image.SaveHeader(metadataWriter);

                    if (!hasPayload)
                        continue;

                    using (MemoryStream payload = new MemoryStream())
                    using (BinaryWriter payloadWriter = new BinaryWriter(payload))
                    {
                        image.WritePayload(payloadWriter);
                        payloadWriter.Flush();
                        entries.Add(Zl2EntryWrite.Create(ZlEntryType.ImagePayload, entryId, image.ImageCodec, payload.ToArray(), compression, stats));
                    }

                    entryId++;
                }

                int atlasPageHeaderCount = CountAtlasPageHeaders();
                if (atlasPageHeaderCount > 0)
                {
                    metadataWriter.Write(atlasPageHeaderCount);
                    foreach (Mir3AtlasPage page in AtlasPages)
                    {
                        if (page == null)
                            continue;

                        page.Position = entryId;
                        page.SaveHeader(metadataWriter);

                        using (MemoryStream payload = new MemoryStream())
                        using (BinaryWriter payloadWriter = new BinaryWriter(payload))
                        {
                            page.WritePayload(payloadWriter);
                            payloadWriter.Flush();
                            entries.Add(Zl2EntryWrite.Create(ZlEntryType.AtlasPagePayload, entryId, page.Codec, payload.ToArray(), compression, stats));
                        }

                        entryId++;
                    }

                    WriteAtlasLayerMappings(metadataWriter);
                }

                metadataWriter.Flush();

                using (FileStream fileStream = File.Create(path))
                using (BinaryWriter writer = new BinaryWriter(fileStream))
                {
                    writer.Write(CompressedContainerSignature);
                    writer.Write(Version);
                    writer.Write(Images.Count);
                    writer.Write(atlasPageHeaderCount);
                    writer.Write((byte)compression);
                    writer.Write((byte)(atlasPageHeaderCount > 0 ? CompressedContainerHasAtlasFlag : 0));
                    writer.Write((short)0);

                    long headerPatchOffset = fileStream.Position;
                    writer.Write((long)0);
                    writer.Write(0);
                    writer.Write((long)0);
                    writer.Write(0);

                    long metadataOffset = fileStream.Position;
                    byte[] metadataBytes = metadata.ToArray();
                    writer.Write(metadataBytes);

                    foreach (Zl2EntryWrite entry in entries)
                    {
                        entry.Offset = fileStream.Position;
                        writer.Write(entry.Payload);
                    }

                    long indexOffset = fileStream.Position;
                    using (MemoryStream index = new MemoryStream())
                    using (BinaryWriter indexWriter = new BinaryWriter(index))
                    {
                        indexWriter.Write(entries.Count);
                        foreach (Zl2EntryWrite entry in entries)
                            entry.WriteIndex(indexWriter);

                        indexWriter.Flush();
                        byte[] indexBytes = index.ToArray();
                        writer.Write(indexBytes);

                        writer.Seek((int)headerPatchOffset, SeekOrigin.Begin);
                        writer.Write(metadataOffset);
                        writer.Write(metadataBytes.Length);
                        writer.Write(indexOffset);
                        writer.Write(indexBytes.Length);
                    }
                }
            }

            LastCompressionReport = BuildCompressionReport(path, compression, stats);
            WriteCompressionReport(path);
            Version = oldVersion;
        }

        private int CountAtlasPageHeaders()
        {
            int count = 0;
            foreach (Mir3AtlasPage page in AtlasPages)
            {
                if (page != null)
                    count++;
            }

            return count;
        }

        private string BuildCompressionReport(string path, ZlContainerCompression compression, List<Zl2EntryCompressionStats> stats)
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine($"ZL v2 conversion report: {Path.GetFileName(path)}");
            report.AppendLine($"Compression selected: {compression}");
            report.AppendLine($"Images: {Images.Count:N0}");
            report.AppendLine($"Atlas pages: {AtlasPages.Count:N0}");
            report.AppendLine($"Atlas group split: {AtlasGroupImageCount:N0}");
            report.AppendLine($"Atlas page size: {AtlasPageSize:N0}");
            report.AppendLine();
            report.AppendLine(BuildStoredPayloadReport());
            report.AppendLine();
            report.AppendLine(BuildCompressionSummary("All entries", stats));
            report.AppendLine(BuildCompressionSummary("Image entries", stats.FindAll(x => x.Type == ZlEntryType.ImagePayload)));
            report.AppendLine(BuildCompressionSummary("Atlas page entries", stats.FindAll(x => x.Type == ZlEntryType.AtlasPagePayload)));
            report.AppendLine();
            report.AppendLine("Atlas page ranges:");
            AppendAtlasPageRanges(report);
            report.AppendLine();
            report.AppendLine(ValidateAtlasMetadata());
            return report.ToString();
        }

        private string BuildStoredPayloadReport()
        {
            PayloadLayerStats imageStats = BuildImagePayloadStats();
            PayloadLayerStats shadowStats = BuildShadowPayloadStats();
            PayloadLayerStats overlayStats = BuildOverlayPayloadStats();
            StringBuilder report = new StringBuilder();

            report.AppendLine("Stored payloads:");
            report.AppendLine(BuildLayerPayloadLine("Image", imageStats));
            report.AppendLine(BuildLayerPayloadLine("Shadow", shadowStats));
            report.AppendLine(BuildLayerPayloadLine("Overlay", overlayStats));
            report.AppendLine(BuildAtlasPayloadLine(ZlAtlasLayer.Image));
            report.AppendLine(BuildAtlasPayloadLine(ZlAtlasLayer.Shadow));
            report.AppendLine(BuildAtlasPayloadLine(ZlAtlasLayer.Overlay));

            return report.ToString().TrimEnd();
        }

        private PayloadLayerStats BuildImagePayloadStats()
        {
            PayloadLayerStats stats = new PayloadLayerStats();
            foreach (Mir3Image image in Images)
            {
                if (image == null || image.Width <= 0 || image.Height <= 0)
                    continue;

                stats.Count++;
                stats.SourceBytes += image.FBytes?.Length ?? image.StoredImageDataSize;
                stats.Bc7Bytes += image.ImageBc7Bytes?.Length ?? image.ImageBc7DataSize;
                stats.FallbackBytes += image.ImageFallbackBytes?.Length ?? image.ImageFallbackDataSize;
            }

            return stats;
        }

        private PayloadLayerStats BuildShadowPayloadStats()
        {
            PayloadLayerStats stats = new PayloadLayerStats();
            foreach (Mir3Image image in Images)
            {
                if (image == null || image.ShadowWidth <= 0 || image.ShadowHeight <= 0 || image.ShadowImage == null)
                    continue;

                stats.Count++;
                stats.SourceBytes += image.ShadowFBytes?.Length ?? image.StoredShadowDataSize;
                stats.Bc7Bytes += image.ShadowBc7Bytes?.Length ?? image.ShadowBc7DataSize;
                stats.FallbackBytes += image.ShadowFallbackBytes?.Length ?? image.ShadowFallbackDataSize;
            }

            return stats;
        }

        private PayloadLayerStats BuildOverlayPayloadStats()
        {
            PayloadLayerStats stats = new PayloadLayerStats();
            foreach (Mir3Image image in Images)
            {
                if (image == null || image.OverlayWidth <= 0 || image.OverlayHeight <= 0 || image.OverlayImage == null)
                    continue;

                stats.Count++;
                stats.SourceBytes += image.OverlayFBytes?.Length ?? image.StoredOverlayDataSize;
                stats.Bc7Bytes += image.OverlayBc7Bytes?.Length ?? image.OverlayBc7DataSize;
                stats.FallbackBytes += image.OverlayFallbackBytes?.Length ?? image.OverlayFallbackDataSize;
            }

            return stats;
        }

        private string BuildLayerPayloadLine(string name, PayloadLayerStats stats)
        {
            return $"{name}: entries {stats.Count:N0}, primary/source {FormatBytes(stats.SourceBytes)}, individual runtime {FormatBytes(stats.Bc7Bytes)}, fallback {FormatBytes(stats.FallbackBytes)}, total {FormatBytes(stats.TotalBytes)}.";
        }

        private string BuildAtlasPayloadLine(ZlAtlasLayer layer)
        {
            int count = 0;
            long sourceBytes = 0;
            long bc7Bytes = 0;
            long fallbackBytes = 0;

            foreach (Mir3AtlasPage page in AtlasPages)
            {
                if (page == null || page.Layer != layer)
                    continue;

                count++;
                sourceBytes += page.FBytes?.Length ?? page.StoredDataSize;
                bc7Bytes += page.Bc7Bytes?.Length ?? page.Bc7DataSize;
                fallbackBytes += page.FallbackBytes?.Length ?? page.FallbackDataSize;
            }

            return $"{GetAtlasLayerName(layer)} atlas: pages {count:N0}, PNG/source {FormatBytes(sourceBytes)}, runtime {FormatBytes(bc7Bytes)}, fallback {FormatBytes(fallbackBytes)}, total {FormatBytes(sourceBytes + bc7Bytes + fallbackBytes)}.";
        }

        private void ReadAtlasLayerMappings(BinaryReader reader)
        {
            if (reader.BaseStream.Position + sizeof(int) > reader.BaseStream.Length)
                return;

            int imageLayerCount = reader.ReadInt32();
            for (int i = 0; i < imageLayerCount; i++)
            {
                if (reader.BaseStream.Position + sizeof(int) * 3 + sizeof(short) * 8 > reader.BaseStream.Length)
                    return;

                int imageIndex = reader.ReadInt32();
                int shadowPage = reader.ReadInt32();
                Rectangle shadowSource = ReadRectangle(reader);
                int overlayPage = reader.ReadInt32();
                Rectangle overlaySource = ReadRectangle(reader);

                if (imageIndex < 0 || imageIndex >= Images.Count || Images[imageIndex] == null)
                    continue;

                Images[imageIndex].ShadowAtlasPage = shadowPage;
                Images[imageIndex].ShadowSourceRectangle = shadowSource;
                Images[imageIndex].OverlayAtlasPage = overlayPage;
                Images[imageIndex].OverlaySourceRectangle = overlaySource;
            }
        }

        private void WriteAtlasLayerMappings(BinaryWriter writer)
        {
            writer.Write(Images.Count);
            for (int i = 0; i < Images.Count; i++)
            {
                Mir3Image image = Images[i];
                writer.Write(i);
                writer.Write(image?.ShadowAtlasPage ?? -1);
                WriteRectangle(writer, image?.ShadowSourceRectangle ?? Rectangle.Empty);
                writer.Write(image?.OverlayAtlasPage ?? -1);
                WriteRectangle(writer, image?.OverlaySourceRectangle ?? Rectangle.Empty);
            }
        }

        private static Rectangle ReadRectangle(BinaryReader reader)
        {
            return new Rectangle(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16());
        }

        private static void WriteRectangle(BinaryWriter writer, Rectangle rectangle)
        {
            writer.Write((short)rectangle.X);
            writer.Write((short)rectangle.Y);
            writer.Write((short)rectangle.Width);
            writer.Write((short)rectangle.Height);
        }

        private static string BuildCompressionSummary(string title, List<Zl2EntryCompressionStats> stats)
        {
            long uncompressed = 0;
            long stored = 0;
            long deflateBest = 0;
            long none = 0;

            foreach (Zl2EntryCompressionStats stat in stats)
            {
                uncompressed += stat.UncompressedSize;
                stored += stat.StoredSize;
                deflateBest += stat.DeflateBestSize;
                none += stat.NoneSize;
            }

            if (uncompressed <= 0)
                return $"{title}: no payloads.";

            return $"{title}: entries {stats.Count:N0}, raw {FormatBytes(uncompressed)}, stored {FormatBytes(stored)} ({FormatRatio(stored, uncompressed)}), DeflateBest {FormatBytes(deflateBest)} ({FormatRatio(deflateBest, uncompressed)}), None {FormatBytes(none)}.";
        }

        private void WriteCompressionReport(string path)
        {
            if (string.IsNullOrEmpty(LastCompressionReport))
                return;

            string reportPath = path + ".report.txt";
            File.WriteAllText(reportPath, LastCompressionReport);
        }

        private static string FormatRatio(long compressed, long uncompressed)
        {
            if (uncompressed <= 0)
                return "0.00%";

            return $"{compressed * 100D / uncompressed:N2}%";
        }

        public void BuildAtlasMetadata(int pageSize = 2048, int padding = 2, int groupImageCount = 0, ZlRuntimeTexturePreference? runtimePreference = null, IProgress<LibraryProgress> progress = null, string progressName = null, bool buildImageAtlas = true, bool buildShadowAtlas = false, bool buildOverlayAtlas = false)
        {
            AtlasPages.Clear();
            AtlasGroupImageCount = groupImageCount;
            AtlasPageSize = pageSize;
            ZlRuntimeTexturePreference atlasRuntimePreference = runtimePreference ?? GetAtlasRuntimePreference();
            progressName ??= "library";
            int groupCount = groupImageCount > 0
                ? Math.Max(1, (Images.Count + groupImageCount - 1) / groupImageCount)
                : 1;

            foreach (Mir3Image image in Images)
            {
                if (image == null) continue;

                image.AtlasPage = -1;
                image.ShadowAtlasPage = -1;
                image.OverlayAtlasPage = -1;
                image.SourceRectangle = Rectangle.Empty;
                image.ShadowSourceRectangle = Rectangle.Empty;
                image.OverlaySourceRectangle = Rectangle.Empty;
                image.VisibleBounds = Rectangle.Empty;
            }

            if (buildImageAtlas)
                BuildAtlasLayer(ZlAtlasLayer.Image, pageSize, padding, groupImageCount, atlasRuntimePreference, progress, progressName, groupCount);

            if (buildShadowAtlas)
                BuildAtlasLayer(ZlAtlasLayer.Shadow, pageSize, padding, groupImageCount, atlasRuntimePreference, progress, progressName, groupCount);

            if (buildOverlayAtlas)
                BuildAtlasLayer(ZlAtlasLayer.Overlay, pageSize, padding, groupImageCount, atlasRuntimePreference, progress, progressName, groupCount);
        }

        private void BuildAtlasLayer(ZlAtlasLayer layer, int pageSize, int padding, int groupImageCount, ZlRuntimeTexturePreference atlasRuntimePreference, IProgress<LibraryProgress> progress, string progressName, int groupCount)
        {
            int x = padding;
            int y = padding;
            int rowHeight = 0;
            int page = AtlasPages.Count;
            int group = -1;
            bool pageHasImages = false;
            Bitmap atlas = CreateAtlasBitmap(pageSize);
            string layerName = GetAtlasLayerName(layer);

            int GetGroupStart(int groupNumber)
            {
                return groupImageCount > 0 ? groupNumber * groupImageCount : 0;
            }

            int GetGroupEnd(int groupNumber)
            {
                return groupImageCount > 0 ? Math.Min(Images.Count, (groupNumber + 1) * groupImageCount) : Images.Count;
            }

            int GetGroupMaximum(int groupNumber)
            {
                return Math.Max(1, GetGroupEnd(groupNumber) - GetGroupStart(groupNumber));
            }

            int GetGroupValue(int imageIndex, int groupNumber)
            {
                return Math.Min(GetGroupMaximum(groupNumber), Math.Max(0, imageIndex - GetGroupStart(groupNumber) + 1));
            }

            bool HasAtlasLayerEntry(int imageIndex)
            {
                Mir3Image image = Images[imageIndex];
                Bitmap source = GetAtlasLayerBitmap(image, layer);
                Size sourceSize = GetAtlasLayerSize(image, layer);
                return source != null && sourceSize.Width > 0 && sourceSize.Height > 0;
            }

            int GetRunEnd(int runStart)
            {
                int runEnd = runStart;
                while (runEnd < Images.Count && HasAtlasLayerEntry(runEnd))
                    runEnd++;

                return runEnd;
            }

            bool CanPlaceRunOnCurrentPage(int runStart, int runEnd)
            {
                int runX = x;
                int runY = y;
                int runRowHeight = rowHeight;

                for (int runIndex = runStart; runIndex < runEnd; runIndex++)
                {
                    Size sourceSize = GetAtlasLayerSize(Images[runIndex], layer);
                    int paddedWidth = sourceSize.Width + padding * 2;
                    int paddedHeight = sourceSize.Height + padding * 2;

                    if (runX + paddedWidth > pageSize)
                    {
                        runX = padding;
                        runY += runRowHeight + padding;
                        runRowHeight = 0;
                    }

                    if (runY + paddedHeight > pageSize)
                        return false;

                    runX += paddedWidth;
                    runRowHeight = Math.Max(runRowHeight, paddedHeight);
                }

                return true;
            }

            void FinishPage()
            {
                if (!pageHasImages)
                    return;

                int currentGroup = Math.Max(group, 0);
                progress?.Report(new LibraryProgress($"Encoding atlas page {page + 1} for {progressName}", 0, 0, true)
                {
                    CountText = $"Encoding atlas page {page + 1:N0}",
                    GroupValue = currentGroup + 1,
                    GroupMaximum = groupCount,
                    GroupText = $"{layerName} atlas group {currentGroup + 1:N0} of {groupCount:N0}: page {page + 1:N0}"
                });

                AtlasPages.Add(Mir3AtlasPage.FromBitmap(page, atlas, atlasRuntimePreference, layer));
                atlas.Dispose();
                page++;
                atlas = CreateAtlasBitmap(pageSize);
                x = padding;
                y = padding;
                rowHeight = 0;
                pageHasImages = false;
            }

            for (int imageIndex = 0; imageIndex < Images.Count; imageIndex++)
            {
                bool isGapSplitRunStart = groupImageCount == 0 && HasAtlasLayerEntry(imageIndex) && (imageIndex == 0 || !HasAtlasLayerEntry(imageIndex - 1));
                if (isGapSplitRunStart && pageHasImages)
                {
                    int runEnd = GetRunEnd(imageIndex);
                    if (!CanPlaceRunOnCurrentPage(imageIndex, runEnd))
                        FinishPage();
                }

                if (groupImageCount > 0)
                {
                    int nextGroup = imageIndex / groupImageCount;
                    if (group >= 0 && nextGroup != group)
                        FinishPage();

                    group = nextGroup;
                }

                Mir3Image image = Images[imageIndex];
                Bitmap source = GetAtlasLayerBitmap(image, layer);
                Size sourceSize = GetAtlasLayerSize(image, layer);
                if (image == null || source == null || sourceSize.Width <= 0 || sourceSize.Height <= 0)
                    continue;

                int currentGroup = Math.Max(group, 0);
                int groupValue = GetGroupValue(imageIndex, currentGroup);
                int groupMaximum = GetGroupMaximum(currentGroup);

                progress?.Report(new LibraryProgress($"Building {layerName.ToLowerInvariant()} atlas for {progressName}", imageIndex + 1, Images.Count)
                {
                    CountText = $"{layerName} atlas image {imageIndex + 1:N0} of {Images.Count:N0}",
                    GroupValue = currentGroup + 1,
                    GroupMaximum = groupCount,
                    GroupText = $"{layerName} atlas group {currentGroup + 1:N0} of {groupCount:N0}: image {groupValue:N0} of {groupMaximum:N0}, page {page + 1:N0}"
                });

                int paddedWidth = sourceSize.Width + padding * 2;
                int paddedHeight = sourceSize.Height + padding * 2;

                if (x + paddedWidth > pageSize)
                {
                    x = padding;
                    y += rowHeight + padding;
                    rowHeight = 0;
                }

                if (y + paddedHeight > pageSize)
                    FinishPage();

                Rectangle sourceRectangle = new Rectangle(x, y, sourceSize.Width, sourceSize.Height);
                SetAtlasLayerPlacement(image, layer, page, sourceRectangle);

                if (layer == ZlAtlasLayer.Image)
                    image.VisibleBounds = image.CalculateVisibleBoundsFromBitmap();

                DrawAtlasImageWithPadding(atlas, source, sourceRectangle, padding);

                x += paddedWidth;
                rowHeight = Math.Max(rowHeight, paddedHeight);
                pageHasImages = true;
            }

            FinishPage();
            atlas.Dispose();
        }

        private static string GetAtlasLayerName(ZlAtlasLayer layer)
        {
            return layer switch
            {
                ZlAtlasLayer.Shadow => "Shadow",
                ZlAtlasLayer.Overlay => "Overlay",
                _ => "Image",
            };
        }

        private static Bitmap GetAtlasLayerBitmap(Mir3Image image, ZlAtlasLayer layer)
        {
            if (image == null)
                return null;

            return layer switch
            {
                ZlAtlasLayer.Shadow => image.ShadowImage,
                ZlAtlasLayer.Overlay => image.OverlayImage,
                _ => image.Image,
            };
        }

        private static Size GetAtlasLayerSize(Mir3Image image, ZlAtlasLayer layer)
        {
            if (image == null)
                return Size.Empty;

            return layer switch
            {
                ZlAtlasLayer.Shadow => new Size(image.ShadowWidth, image.ShadowHeight),
                ZlAtlasLayer.Overlay => new Size(image.OverlayWidth, image.OverlayHeight),
                _ => new Size(image.Width, image.Height),
            };
        }

        public int CountAtlasLayerEntries(ZlAtlasLayer layer)
        {
            int count = 0;

            foreach (Mir3Image image in Images)
            {
                Bitmap source = GetAtlasLayerBitmap(image, layer);
                Size size = GetAtlasLayerSize(image, layer);
                if (source != null && size.Width > 0 && size.Height > 0)
                    count++;
            }

            return count;
        }

        public void SetRuntimePreferenceForAllImages(ZlRuntimeTexturePreference preference, bool storePngSourceImages, IProgress<LibraryProgress> progress = null, string progressName = null, CancellationToken cancellationToken = default)
        {
            progressName ??= "library";
            int maximum = Images?.Count ?? 0;

            for (int i = 0; i < maximum; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                progress?.Report(new LibraryProgress($"Building runtime textures for {progressName}", i + 1, maximum)
                {
                    CountText = $"Runtime image {i + 1:N0} of {maximum:N0}",
                    GroupText = GetRuntimeProgressText(preference, storePngSourceImages)
                });

                Images[i]?.SetStorageOptions(storePngSourceImages, preference);
            }
        }

        private static string GetRuntimeProgressText(ZlRuntimeTexturePreference preference, bool storePngSourceImages)
        {
            string sourceText = storePngSourceImages ? "existing PNG source where present" : "runtime-only";
            return preference switch
            {
                ZlRuntimeTexturePreference.Dxt1 => $"Encoding {sourceText} + DXT1 textures",
                ZlRuntimeTexturePreference.Dxt5 => $"Encoding {sourceText} + DXT5 textures",
                ZlRuntimeTexturePreference.Bc7 => $"Encoding {sourceText} + BC7 textures",
                ZlRuntimeTexturePreference.SourceType => "Preserving source texture type where possible",
                _ => storePngSourceImages ? "Preserving existing PNG source textures" : "Encoding runtime textures",
            };
        }

        private static void SetAtlasLayerPlacement(Mir3Image image, ZlAtlasLayer layer, int page, Rectangle sourceRectangle)
        {
            switch (layer)
            {
                case ZlAtlasLayer.Shadow:
                    image.ShadowAtlasPage = page;
                    image.ShadowSourceRectangle = sourceRectangle;
                    break;
                case ZlAtlasLayer.Overlay:
                    image.OverlayAtlasPage = page;
                    image.OverlaySourceRectangle = sourceRectangle;
                    break;
                default:
                    image.AtlasPage = page;
                    image.SourceRectangle = sourceRectangle;
                    break;
            }
        }

        private ZlRuntimeTexturePreference GetAtlasRuntimePreference()
        {
            foreach (Mir3Image image in Images)
            {
                if (image == null)
                    continue;

                return image.ImageRuntimePreference;
            }

            return ZlRuntimeTexturePreference.Bc7;
        }

        public string ValidateAtlasMetadata(int groupImageCount = 0)
        {
            if (AtlasPages.Count == 0)
                return $"Atlas metadata not present. Images: {Images.Count}, Group split: {AtlasGroupImageCount:N0}, Page size: {AtlasPageSize:N0}.";

            StringBuilder report = new StringBuilder();
            int issues = 0;
            int validationGroupImageCount = groupImageCount > 0 ? groupImageCount : AtlasGroupImageCount;

            for (int i = 0; i < AtlasPages.Count; i++)
            {
                Mir3AtlasPage page = AtlasPages[i];
                if (page == null)
                {
                    AppendAtlasIssue(report, ref issues, $"Atlas page slot {i} is empty.");
                    continue;
                }

                if (page.Id != i)
                    AppendAtlasIssue(report, ref issues, $"Atlas page slot {i} has id {page.Id}.");

                if (page.Width <= 0 || page.Height <= 0 || page.DataSize == 0)
                    AppendAtlasIssue(report, ref issues, $"Atlas page {page.Id} has no payload.");
            }

            for (int i = 0; i < Images.Count; i++)
            {
                Mir3Image image = Images[i];
                if (image == null || image.Image == null || image.Width <= 0 || image.Height <= 0)
                    continue;

                if (image.AtlasPage < 0 || image.AtlasPage >= AtlasPages.Count)
                {
                    AppendAtlasIssue(report, ref issues, $"Image {i} points at missing atlas page {image.AtlasPage}.");
                    continue;
                }

                Mir3AtlasPage page = AtlasPages[image.AtlasPage];
                Rectangle source = image.SourceRectangle;

                if (source.Width != image.Width || source.Height != image.Height)
                    AppendAtlasIssue(report, ref issues, $"Image {i} source size {source.Width}x{source.Height} does not match image size {image.Width}x{image.Height}.");

                if (source.X < 0 || source.Y < 0 || source.Right > page.Width || source.Bottom > page.Height)
                    AppendAtlasIssue(report, ref issues, $"Image {i} source rectangle {source} is outside atlas page {page.Id} ({page.Width}x{page.Height}).");

                Rectangle visible = image.VisibleBounds;
                if (visible != Rectangle.Empty && (visible.X < 0 || visible.Y < 0 || visible.Right > image.Width || visible.Bottom > image.Height))
                    AppendAtlasIssue(report, ref issues, $"Image {i} visible bounds {visible} are outside image size {image.Width}x{image.Height}.");
            }

            ValidateAtlasLayerMetadata(report, ref issues, ZlAtlasLayer.Shadow);
            ValidateAtlasLayerMetadata(report, ref issues, ZlAtlasLayer.Overlay);

            if (validationGroupImageCount > 0)
                ValidateAtlasGroupBoundaries(report, ref issues, validationGroupImageCount);

            if (issues == 0)
                report.Insert(0, $"Atlas metadata valid. Pages: {AtlasPages.Count}, Images: {Images.Count}, Group split: {validationGroupImageCount:N0}, Page size: {AtlasPageSize:N0}.{Environment.NewLine}");
            else
                report.Insert(0, $"Atlas metadata has {issues} issue(s).{Environment.NewLine}");

            return report.ToString();
        }

        private void ValidateAtlasLayerMetadata(StringBuilder report, ref int issues, ZlAtlasLayer layer)
        {
            for (int i = 0; i < Images.Count; i++)
            {
                Mir3Image image = Images[i];
                Bitmap bitmap = GetAtlasLayerBitmap(image, layer);
                Size size = GetAtlasLayerSize(image, layer);
                int atlasPage = GetAtlasLayerPage(image, layer);
                Rectangle source = GetAtlasLayerSourceRectangle(image, layer);

                if (image == null || bitmap == null || size.Width <= 0 || size.Height <= 0 || atlasPage < 0)
                    continue;

                string layerName = GetAtlasLayerName(layer);

                if (atlasPage >= AtlasPages.Count)
                {
                    AppendAtlasIssue(report, ref issues, $"{layerName} image {i} points at missing atlas page {atlasPage}.");
                    continue;
                }

                Mir3AtlasPage page = AtlasPages[atlasPage];
                if (page == null)
                {
                    AppendAtlasIssue(report, ref issues, $"{layerName} image {i} points at empty atlas page slot {atlasPage}.");
                    continue;
                }

                if (page.Layer != layer)
                    AppendAtlasIssue(report, ref issues, $"{layerName} image {i} points at {GetAtlasLayerName(page.Layer).ToLowerInvariant()} atlas page {atlasPage}.");

                if (source.Width != size.Width || source.Height != size.Height)
                    AppendAtlasIssue(report, ref issues, $"{layerName} image {i} source size {source.Width}x{source.Height} does not match layer size {size.Width}x{size.Height}.");

                if (source.X < 0 || source.Y < 0 || source.Right > page.Width || source.Bottom > page.Height)
                    AppendAtlasIssue(report, ref issues, $"{layerName} image {i} source rectangle {source} is outside atlas page {page.Id} ({page.Width}x{page.Height}).");
            }
        }

        private static int GetAtlasLayerPage(Mir3Image image, ZlAtlasLayer layer)
        {
            if (image == null)
                return -1;

            return layer switch
            {
                ZlAtlasLayer.Shadow => image.ShadowAtlasPage,
                ZlAtlasLayer.Overlay => image.OverlayAtlasPage,
                _ => image.AtlasPage,
            };
        }

        private static Rectangle GetAtlasLayerSourceRectangle(Mir3Image image, ZlAtlasLayer layer)
        {
            if (image == null)
                return Rectangle.Empty;

            return layer switch
            {
                ZlAtlasLayer.Shadow => image.ShadowSourceRectangle,
                ZlAtlasLayer.Overlay => image.OverlaySourceRectangle,
                _ => image.SourceRectangle,
            };
        }

        private void ValidateAtlasGroupBoundaries(StringBuilder report, ref int issues, int groupImageCount)
        {
            foreach (Mir3AtlasPage page in AtlasPages)
            {
                if (page == null)
                    continue;

                int minIndex = -1;
                int maxIndex = -1;

                for (int imageIndex = 0; imageIndex < Images.Count; imageIndex++)
                {
                    Mir3Image image = Images[imageIndex];
                    if (image == null || GetAtlasLayerPage(image, page.Layer) != page.Id || GetAtlasLayerSourceRectangle(image, page.Layer) == Rectangle.Empty)
                        continue;

                    if (minIndex < 0)
                        minIndex = imageIndex;

                    maxIndex = imageIndex;
                }

                if (minIndex < 0)
                    continue;

                int minGroup = minIndex / groupImageCount;
                int maxGroup = maxIndex / groupImageCount;
                if (minGroup != maxGroup)
                    AppendAtlasIssue(report, ref issues, $"{GetAtlasLayerName(page.Layer)} atlas page {page.Id} crosses group boundary {groupImageCount:N0}: image {minIndex:N0} to {maxIndex:N0}.");
            }
        }

        public string ExportAtlasDebugPages(string directory)
        {
            Directory.CreateDirectory(directory);
            if (AtlasPages.Count == 0)
                BuildAtlasMetadata();

            for (int i = 0; i < AtlasPages.Count; i++)
            {
                Mir3AtlasPage page = AtlasPages[i];
                string basePath = Path.Combine(directory, $"atlas_{page.Id:000}");

                using (Bitmap bitmap = CreateAtlasDebugBitmap(page))
                    bitmap.Save(basePath + ".png", ImageFormat.Png);

                using (Bitmap overlay = CreateAtlasDebugBitmap(page))
                using (Graphics g = Graphics.FromImage(overlay))
                using (Font font = new Font(FontFamily.GenericSansSerif, 8F))
                using (Pen pen = new Pen(Color.Red, 1F))
                using (Brush brush = new SolidBrush(Color.Yellow))
                {
                    g.CompositingMode = CompositingMode.SourceOver;
                    g.SmoothingMode = SmoothingMode.None;

                    for (int imageIndex = 0; imageIndex < Images.Count; imageIndex++)
                    {
                        Mir3Image image = Images[imageIndex];
                        Rectangle source = GetAtlasLayerSourceRectangle(image, page.Layer);
                        if (image == null || GetAtlasLayerPage(image, page.Layer) != page.Id || source == Rectangle.Empty)
                            continue;

                        g.DrawRectangle(pen, source);

                        if (source.Width >= 20 && source.Height >= 12)
                            g.DrawString(imageIndex.ToString(), font, brush, source.Location);
                    }

                    overlay.Save(basePath + "_rects.png", ImageFormat.Png);
                }
            }

            string reportPath = Path.Combine(directory, "atlas_report.txt");
            File.WriteAllText(reportPath, BuildAtlasDebugReport());
            return reportPath;
        }

        private void AppendAtlasPageRanges(StringBuilder report)
        {
            foreach (Mir3AtlasPage page in AtlasPages)
            {
                if (page == null)
                    continue;

                int minIndex = -1;
                int maxIndex = -1;
                int count = 0;

                for (int imageIndex = 0; imageIndex < Images.Count; imageIndex++)
                {
                    Mir3Image image = Images[imageIndex];
                    if (image == null || GetAtlasLayerPage(image, page.Layer) != page.Id || GetAtlasLayerSourceRectangle(image, page.Layer) == Rectangle.Empty)
                        continue;

                    if (minIndex < 0)
                        minIndex = imageIndex;

                    maxIndex = imageIndex;
                    count++;
                }

                if (count == 0)
                    report.AppendLine($"{GetAtlasLayerName(page.Layer)} page {page.Id:N0}: no images");
                else
                    report.AppendLine($"{GetAtlasLayerName(page.Layer)} page {page.Id:N0}: {count:N0} images, index {minIndex:N0} to {maxIndex:N0}");
            }
        }

        private string BuildAtlasDebugReport()
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine(ValidateAtlasMetadata());
            report.AppendLine(BuildCompressionDebugReport());
            report.AppendLine("Atlas page image ranges:");
            report.AppendLine($"Atlas group split: {AtlasGroupImageCount:N0}; atlas page size: {AtlasPageSize:N0}");

            AppendAtlasPageRanges(report);

            return report.ToString();
        }

        public string BuildCompressionDebugReport()
        {
            if (_zl2Entries.Count > 0)
            {
                long compressed = 0;
                long uncompressed = 0;
                long atlasCompressed = 0;
                long atlasUncompressed = 0;
                long imageCompressed = 0;
                long imageUncompressed = 0;

                foreach (Zl2Entry entry in _zl2Entries.Values)
                {
                    compressed += entry.CompressedSize;
                    uncompressed += entry.UncompressedSize;

                    if (entry.Type == ZlEntryType.AtlasPagePayload)
                    {
                        atlasCompressed += entry.CompressedSize;
                        atlasUncompressed += entry.UncompressedSize;
                    }
                    else if (entry.Type == ZlEntryType.ImagePayload)
                    {
                        imageCompressed += entry.CompressedSize;
                        imageUncompressed += entry.UncompressedSize;
                    }
                }

                return $"Compressed payload totals: image entries {FormatBytes(imageCompressed)} / {FormatBytes(imageUncompressed)}, atlas entries {FormatBytes(atlasCompressed)} / {FormatBytes(atlasUncompressed)}, combined {FormatBytes(compressed)} / {FormatBytes(uncompressed)}.";
            }

            long imageSourceBytes = 0;
            long imageRuntimeBytes = 0;
            long atlasBytes = 0;

            foreach (Mir3Image image in Images)
            {
                if (image == null)
                    continue;

                imageSourceBytes += image.FBytes?.Length ?? image.StoredImageDataSize;
                imageSourceBytes += image.ShadowFBytes?.Length ?? image.StoredShadowDataSize;
                imageSourceBytes += image.OverlayFBytes?.Length ?? image.StoredOverlayDataSize;
                imageRuntimeBytes += image.ImageBc7Bytes?.Length ?? image.ImageBc7DataSize;
                imageRuntimeBytes += image.ImageFallbackBytes?.Length ?? image.ImageFallbackDataSize;
                imageRuntimeBytes += image.ShadowBc7Bytes?.Length ?? image.ShadowBc7DataSize;
                imageRuntimeBytes += image.ShadowFallbackBytes?.Length ?? image.ShadowFallbackDataSize;
                imageRuntimeBytes += image.OverlayBc7Bytes?.Length ?? image.OverlayBc7DataSize;
                imageRuntimeBytes += image.OverlayFallbackBytes?.Length ?? image.OverlayFallbackDataSize;
            }

            foreach (Mir3AtlasPage page in AtlasPages)
            {
                if (page == null)
                    continue;

                atlasBytes += page.DataSize;
            }

            return $"Payload totals: image source {FormatBytes(imageSourceBytes)}, per-image runtime {FormatBytes(imageRuntimeBytes)}, atlas pages {FormatBytes(atlasBytes)}, combined {FormatBytes(imageSourceBytes + imageRuntimeBytes + atlasBytes)}.";
        }

        private static string FormatBytes(long bytes)
        {
            return bytes >= 1024 * 1024
                ? $"{bytes / 1024D / 1024D:N2} MB"
                : $"{bytes / 1024D:N2} KB";
        }

        private byte[] ReadCompressedPayload(int entryId)
        {
            if (!_zl2Entries.TryGetValue(entryId, out Zl2Entry entry))
                return null;

            lock (_bReader)
            {
                _bReader.BaseStream.Seek(entry.Offset, SeekOrigin.Begin);
                byte[] payload = _bReader.ReadBytes(entry.CompressedSize);
                return Decompress(payload, entry.UncompressedSize, entry.Compression);
            }
        }

        private Bitmap CreateAtlasDebugBitmap(Mir3AtlasPage page)
        {
            if (page.FBytes != null && page.FBytes.Length > 0)
                return page.ToBitmap();

            Bitmap bitmap = CreateAtlasBitmap(Math.Max(page.Width, page.Height));

            foreach (Mir3Image image in Images)
            {
                Bitmap source = GetAtlasLayerBitmap(image, page.Layer);
                Rectangle sourceRectangle = GetAtlasLayerSourceRectangle(image, page.Layer);
                if (image == null || source == null || GetAtlasLayerPage(image, page.Layer) != page.Id || sourceRectangle == Rectangle.Empty)
                    continue;

                DrawAtlasImageWithPadding(bitmap, source, sourceRectangle, 2);
            }

            return bitmap;
        }

        private static void AppendAtlasIssue(StringBuilder report, ref int issues, string issue)
        {
            issues++;
            report.AppendLine(issue);
        }

        private static Bitmap CreateAtlasBitmap(int pageSize)
        {
            Bitmap bitmap = new Bitmap(pageSize, pageSize, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bitmap))
                g.Clear(Color.Transparent);

            return bitmap;
        }

        private static void DrawAtlasImageWithPadding(Bitmap atlas, Bitmap source, Rectangle destination, int padding)
        {
            for (int y = 0; y < destination.Height; y++)
            {
                for (int x = 0; x < destination.Width; x++)
                {
                    atlas.SetPixel(destination.X + x, destination.Y + y, source.GetPixel(x, y));
                }
            }

            if (padding <= 0 || destination.Width <= 0 || destination.Height <= 0)
                return;

            for (int i = 1; i <= padding; i++)
            {
                for (int y = 0; y < destination.Height; y++)
                {
                    atlas.SetPixel(destination.Left - i, destination.Top + y, source.GetPixel(0, y));
                    atlas.SetPixel(destination.Right + i - 1, destination.Top + y, source.GetPixel(destination.Width - 1, y));
                }

                for (int x = 0; x < destination.Width; x++)
                {
                    atlas.SetPixel(destination.Left + x, destination.Top - i, source.GetPixel(x, 0));
                    atlas.SetPixel(destination.Left + x, destination.Bottom + i - 1, source.GetPixel(x, destination.Height - 1));
                }

                atlas.SetPixel(destination.Left - i, destination.Top - i, source.GetPixel(0, 0));
                atlas.SetPixel(destination.Right + i - 1, destination.Top - i, source.GetPixel(destination.Width - 1, 0));
                atlas.SetPixel(destination.Left - i, destination.Bottom + i - 1, source.GetPixel(0, destination.Height - 1));
                atlas.SetPixel(destination.Right + i - 1, destination.Bottom + i - 1, source.GetPixel(destination.Width - 1, destination.Height - 1));
            }
        }

        public sealed class Mir3AtlasPage
        {
            public int Id;
            public int Position;
            public short Width;
            public short Height;
            public ZlAtlasLayer Layer;
            public ZlImageCodec Codec;
            public ZlRuntimeTexturePreference RuntimePreference;
            public byte[] FBytes;
            public byte[] Bc7Bytes;
            public byte[] FallbackBytes;
            public int StoredDataSize;
            public int Bc7DataSize;
            public int FallbackDataSize;
            public int DataSize => GetPayloadSize(FBytes, StoredDataSize) + GetPayloadSize(Bc7Bytes, Bc7DataSize) + GetPayloadSize(FallbackBytes, FallbackDataSize);

            public Mir3AtlasPage()
            {
            }

            public Mir3AtlasPage(BinaryReader reader)
            {
                Id = reader.ReadInt32();
                Position = reader.ReadInt32();
                Width = reader.ReadInt16();
                Height = reader.ReadInt16();
                Layer = (ZlAtlasLayer)reader.ReadByte();
                Codec = (ZlImageCodec)reader.ReadByte();
                StoredDataSize = reader.ReadInt32();
                RuntimePreference = (ZlRuntimeTexturePreference)reader.ReadByte();
                Bc7DataSize = reader.ReadInt32();
                FallbackDataSize = reader.ReadInt32();
            }

            public void SaveHeader(BinaryWriter writer)
            {
                writer.Write(Id);
                writer.Write(Position);
                writer.Write(Width);
                writer.Write(Height);
                writer.Write((byte)Layer);
                writer.Write((byte)Codec);
                writer.Write(GetPayloadSize(FBytes, StoredDataSize));
                writer.Write((byte)RuntimePreference);
                writer.Write(GetPayloadSize(Bc7Bytes, Bc7DataSize));
                writer.Write(GetPayloadSize(FallbackBytes, FallbackDataSize));
            }

            public void WritePayload(BinaryWriter writer)
            {
                if (FBytes != null)
                    writer.Write(FBytes);
                if (Bc7Bytes != null)
                    writer.Write(Bc7Bytes);
                if (FallbackBytes != null)
                    writer.Write(FallbackBytes);
            }

            public static Mir3AtlasPage FromBitmap(int id, Bitmap bitmap, ZlRuntimeTexturePreference runtimePreference, ZlAtlasLayer layer = ZlAtlasLayer.Image)
            {
                bool writeBc7Payload = runtimePreference == ZlRuntimeTexturePreference.Bc7 || runtimePreference == ZlRuntimeTexturePreference.Bc7Dxt5;
                bool writeDxtFallbackPayload = runtimePreference == ZlRuntimeTexturePreference.Bc7Dxt5;
                return new Mir3AtlasPage
                {
                    Id = id,
                    Layer = layer,
                    Width = (short)bitmap.Width,
                    Height = (short)bitmap.Height,
                    Codec = ZlImageCodec.Png,
                    RuntimePreference = runtimePreference,
                    FBytes = writeBc7Payload ? null : Mir3Image.EncodeBitmap(bitmap, (short)bitmap.Width, (short)bitmap.Height, ZlImageCodec.Png, false),
                    Bc7Bytes = writeBc7Payload ? Mir3Image.EncodeBitmap(bitmap, (short)bitmap.Width, (short)bitmap.Height, ZlImageCodec.Bc7, false) : null,
                    FallbackBytes = writeDxtFallbackPayload ? Mir3Image.EncodeBitmap(bitmap, (short)bitmap.Width, (short)bitmap.Height, ZlImageCodec.Dxt5, false) : null
                };
            }

            private static int GetPayloadSize(byte[] bytes, int storedSize)
            {
                return bytes?.Length ?? storedSize;
            }

            public Bitmap ToBitmap()
            {
                Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                try
                {
                    byte[] payload = FBytes;
                    ZlImageCodec codec = Codec;

                    if ((payload == null || payload.Length == 0) && Bc7Bytes != null && Bc7Bytes.Length > 0)
                    {
                        payload = Bc7Bytes;
                        codec = RuntimePreference == ZlRuntimeTexturePreference.Dxt1 ? ZlImageCodec.Dxt1 : ZlImageCodec.Bc7;
                    }

                    if ((payload == null || payload.Length == 0) && FallbackBytes != null && FallbackBytes.Length > 0)
                    {
                        payload = FallbackBytes;
                        codec = ZlImageCodec.Dxt5;
                    }

                    if (payload == null || payload.Length == 0)
                        return bitmap;

                    byte[] pixels = Mir3Image.DecodeBitmapToBgra(payload, Width, Height, codec);
                    int rowBytes = Width * 4;
                    for (int y = 0; y < Height; y++)
                        Marshal.Copy(pixels, y * rowBytes, IntPtr.Add(data.Scan0, y * data.Stride), rowBytes);
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }

                return bitmap;
            }
        }

        private sealed class Zl2Entry
        {
            public ZlEntryType Type;
            public int Id;
            public int UncompressedSize;
            public int CompressedSize;
            public long Offset;
            public ZlContainerCompression Compression;
            public ZlImageCodec Codec;

            public static Zl2Entry Read(BinaryReader reader)
            {
                return new Zl2Entry
                {
                    Type = (ZlEntryType)reader.ReadByte(),
                    Id = reader.ReadInt32(),
                    UncompressedSize = reader.ReadInt32(),
                    CompressedSize = reader.ReadInt32(),
                    Offset = reader.ReadInt64(),
                    Compression = (ZlContainerCompression)reader.ReadByte(),
                    Codec = (ZlImageCodec)reader.ReadByte()
                };
            }
        }

        private sealed class Zl2EntryWrite
        {
            public ZlEntryType Type;
            public int Id;
            public int UncompressedSize;
            public byte[] Payload;
            public long Offset;
            public ZlContainerCompression Compression;
            public ZlImageCodec Codec;

            public static Zl2EntryWrite Create(ZlEntryType type, int id, ZlImageCodec codec, byte[] payload, ZlContainerCompression compression, List<Zl2EntryCompressionStats> stats)
            {
                byte[] compressed = Compress(payload, compression, out ZlContainerCompression storedCompression);
                stats?.Add(Zl2EntryCompressionStats.Create(type, id, codec, payload, compressed.Length));
                return new Zl2EntryWrite
                {
                    Type = type,
                    Id = id,
                    Codec = codec,
                    Compression = storedCompression,
                    UncompressedSize = payload.Length,
                    Payload = compressed
                };
            }

            public void WriteIndex(BinaryWriter writer)
            {
                writer.Write((byte)Type);
                writer.Write(Id);
                writer.Write(UncompressedSize);
                writer.Write(Payload.Length);
                writer.Write(Offset);
                writer.Write((byte)Compression);
                writer.Write((byte)Codec);
            }
        }

        private sealed class Zl2EntryCompressionStats
        {
            public ZlEntryType Type;
            public int Id;
            public ZlImageCodec Codec;
            public int UncompressedSize;
            public int StoredSize;
            public int NoneSize;
            public int DeflateBestSize;

            public static Zl2EntryCompressionStats Create(ZlEntryType type, int id, ZlImageCodec codec, byte[] payload, int storedSize)
            {
                return new Zl2EntryCompressionStats
                {
                    Type = type,
                    Id = id,
                    Codec = codec,
                    UncompressedSize = payload?.Length ?? 0,
                    StoredSize = storedSize,
                    NoneSize = payload?.Length ?? 0,
                    DeflateBestSize = CompressForReport(payload, ZlContainerCompression.DeflateBest)
                };
            }
        }

        private sealed class PayloadLayerStats
        {
            public int Count;
            public long SourceBytes;
            public long Bc7Bytes;
            public long FallbackBytes;
            public long TotalBytes => SourceBytes + Bc7Bytes + FallbackBytes;
        }

        private static byte[] Compress(byte[] payload, ZlContainerCompression compression, out ZlContainerCompression storedCompression)
        {
            storedCompression = compression;
            if (payload == null || payload.Length == 0 || compression == ZlContainerCompression.None)
            {
                storedCompression = ZlContainerCompression.None;
                return payload ?? Array.Empty<byte>();
            }

            using (MemoryStream output = new MemoryStream())
            {
                CompressionLevel level = compression == ZlContainerCompression.DeflateFast
                    ? CompressionLevel.Fastest
                    : CompressionLevel.SmallestSize;

                using (DeflateStream deflate = new DeflateStream(output, level, true))
                    deflate.Write(payload, 0, payload.Length);

                byte[] compressed = output.ToArray();
                if (compressed.Length < payload.Length)
                    return compressed;

                storedCompression = ZlContainerCompression.None;
                return payload;
            }
        }

        private static int CompressForReport(byte[] payload, ZlContainerCompression compression)
        {
            byte[] compressed = Compress(payload, compression, out _);
            return compressed.Length;
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

        public sealed class Mir3Image : IDisposable
        {
            //TODO - Code smell
            public const int LegacyHeaderSize = 25;
            public const int AtlasHeaderExtraSize = 62;
            private static readonly object PngEncoderLock = new object();

            public int Version;
            public int HeaderSize => Version >= 2 ? LegacyHeaderSize + AtlasHeaderExtraSize : LegacyHeaderSize;

            public int DataSize =>
                (FBytes?.Length ?? 0) + (ImageBc7Bytes?.Length ?? 0) + (ImageFallbackBytes?.Length ?? 0) +
                (ShadowFBytes?.Length ?? 0) + (ShadowBc7Bytes?.Length ?? 0) + (ShadowFallbackBytes?.Length ?? 0) +
                (OverlayFBytes?.Length ?? 0) + (OverlayBc7Bytes?.Length ?? 0) + (OverlayFallbackBytes?.Length ?? 0);

            public int Position;
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

            #region Texture

            public short Width;
            public short Height;
            public short OffSetX;
            public short OffSetY;
            public byte ShadowType;
            public Bitmap Image, Preview;
            public bool ImageValid { get; private set; }
            public unsafe byte* ImageData;
            public int StoredImageDataSize;
            public int ImageBc7DataSize;
            public int ImageFallbackDataSize;
            public int ImageDataSize
            {
                get { return Version >= 2 && StoredImageDataSize > 0 ? StoredImageDataSize : GetDataSize(Width, Height, ImageCodec); }
            }
            public byte[] FBytes;
            public byte[] ImageBc7Bytes;
            public byte[] ImageFallbackBytes;
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
            public byte[] ShadowBc7Bytes;
            public byte[] ShadowFallbackBytes;
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

            public Bitmap OverlayImage, OverlayPreview;
            public bool OverlayValid { get; private set; }
            public unsafe byte* OverlayData;
            public byte[] OverlayFBytes;
            public byte[] OverlayBc7Bytes;
            public byte[] OverlayFallbackBytes;
            public int StoredOverlayDataSize;
            public int OverlayBc7DataSize;
            public int OverlayFallbackDataSize;
            public int OverlayDataSize
            {
                get { return Version >= 2 && StoredOverlayDataSize > 0 ? StoredOverlayDataSize : GetDataSize(OverlayWidth, OverlayHeight, OverlayCodec); }
            }
            #endregion


            public DateTime ExpireTime;

            public Mir3Image(int version)
            {
                Version = version;
                ImageCodec = GetDefaultCodec(version);
                ShadowCodec = ImageCodec;
                OverlayCodec = ImageCodec;
                ImageRuntimePreference = GetDefaultRuntimePreference(ImageCodec);
                ShadowRuntimePreference = ImageRuntimePreference;
                OverlayRuntimePreference = ImageRuntimePreference;
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

                ImageCodec = GetDefaultCodec(Version);
                ShadowCodec = ImageCodec;
                OverlayCodec = ImageCodec;
                ImageRuntimePreference = GetDefaultRuntimePreference(ImageCodec);
                ShadowRuntimePreference = ImageRuntimePreference;
                OverlayRuntimePreference = ImageRuntimePreference;
                ImageRuntimePreference = GetDefaultRuntimePreference(ImageCodec);
                ShadowRuntimePreference = ImageRuntimePreference;
                OverlayRuntimePreference = ImageRuntimePreference;
                SourceRectangle = new Rectangle(0, 0, Width, Height);

                if (Version < 2)
                    return;

                AtlasPage = reader.ReadInt32();
                SourceRectangle = new Rectangle(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16());
                VisibleBounds = new Rectangle(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16());
                ImageCodec = (ZlImageCodec)reader.ReadByte();
                ShadowCodec = (ZlImageCodec)reader.ReadByte();
                OverlayCodec = (ZlImageCodec)reader.ReadByte();

                ImageRuntimePreference = (ZlRuntimeTexturePreference)reader.ReadByte();
                ShadowRuntimePreference = (ZlRuntimeTexturePreference)reader.ReadByte();
                OverlayRuntimePreference = (ZlRuntimeTexturePreference)reader.ReadByte();

                StoredImageDataSize = reader.ReadInt32();
                ImageBc7DataSize = reader.ReadInt32();
                ImageFallbackDataSize = reader.ReadInt32();
                StoredShadowDataSize = reader.ReadInt32();
                ShadowBc7DataSize = reader.ReadInt32();
                ShadowFallbackDataSize = reader.ReadInt32();
                StoredOverlayDataSize = reader.ReadInt32();
                OverlayBc7DataSize = reader.ReadInt32();
                OverlayFallbackDataSize = reader.ReadInt32();
            }

            public void SetVersion(int version, ZlImageCodec? codecOverride = null)
            {
                Version = version;

                if (Version >= 2)
                {
                    ZlImageCodec requestedCodec = codecOverride ?? ImageCodec;
                    if (requestedCodec == default)
                        requestedCodec = ZlImageCodec.Png;

                    ZlRuntimeTexturePreference preference = GetDefaultRuntimePreference(requestedCodec);

                    ImageCodec = requestedCodec;
                    ShadowCodec = ShadowCodec == default ? requestedCodec : ShadowCodec;
                    OverlayCodec = OverlayCodec == default ? requestedCodec : OverlayCodec;
                    ImageRuntimePreference = preference;
                    ShadowRuntimePreference = GetDefaultRuntimePreference(ShadowCodec);
                    OverlayRuntimePreference = GetDefaultRuntimePreference(OverlayCodec);
                }
                else if (codecOverride.HasValue)
                {
                    ImageCodec = codecOverride.Value;
                    ShadowCodec = codecOverride.Value;
                    OverlayCodec = codecOverride.Value;
                    ImageRuntimePreference = GetDefaultRuntimePreference(ImageCodec);
                    ShadowRuntimePreference = ImageRuntimePreference;
                    OverlayRuntimePreference = ImageRuntimePreference;
                }
                else if (ImageCodec == default || Version < 2)
                {
                    ImageCodec = GetDefaultCodec(version);
                    ImageRuntimePreference = GetDefaultRuntimePreference(ImageCodec);
                }

                if (ShadowCodec == default || Version < 2)
                {
                    ShadowCodec = ImageCodec;
                    ShadowRuntimePreference = ImageRuntimePreference;
                }

                if (OverlayCodec == default || Version < 2)
                {
                    OverlayCodec = ImageCodec;
                    OverlayRuntimePreference = ImageRuntimePreference;
                }

                RebuildBytes();
            }

            public void SetRuntimePreference(ZlRuntimeTexturePreference preference)
            {
                ImageRuntimePreference = preference;
                ShadowRuntimePreference = preference;
                OverlayRuntimePreference = preference;
                RebuildBytes();
            }

            public void SetStorageOptions(bool storePngSourceImages, ZlRuntimeTexturePreference runtimePreference)
            {
                if (runtimePreference == ZlRuntimeTexturePreference.SourceType)
                    runtimePreference = ZlRuntimeTexturePreference.Bc7;

                ZlImageCodec runtimeCodec = GetPrimaryRuntimeCodec(runtimePreference);

                ImageCodec = ShouldKeepExistingPngSource(storePngSourceImages, ImageCodec, FBytes)
                    ? ZlImageCodec.Png
                    : runtimeCodec;
                ShadowCodec = ShouldKeepExistingPngSource(storePngSourceImages, ShadowCodec, ShadowFBytes)
                    ? ZlImageCodec.Png
                    : runtimeCodec;
                OverlayCodec = ShouldKeepExistingPngSource(storePngSourceImages, OverlayCodec, OverlayFBytes)
                    ? ZlImageCodec.Png
                    : runtimeCodec;
                ImageRuntimePreference = runtimePreference;
                ShadowRuntimePreference = runtimePreference;
                OverlayRuntimePreference = runtimePreference;
                RebuildBytes();
            }

            private static bool ShouldKeepExistingPngSource(bool storePngSourceImages, ZlImageCodec codec, byte[] payload)
            {
                return storePngSourceImages && codec == ZlImageCodec.Png && payload != null && payload.Length > 0;
            }

            public void SetSourceTypeStorage(
                bool storePngSourceImages,
                ZlImageCodec imageCodec,
                byte[] imagePayload,
                ZlImageCodec? shadowCodec = null,
                byte[] shadowPayload = null,
                ZlImageCodec? overlayCodec = null,
                byte[] overlayPayload = null)
            {
                imagePayload = GetUsableSourcePayload(imagePayload, Width, Height, imageCodec);

                ImageCodec = storePngSourceImages ? ZlImageCodec.Png : imageCodec;
                ImageRuntimePreference = GetRuntimePreferenceForCodec(imageCodec);
                FBytes = storePngSourceImages ? EncodeBitmap(Image, Width, Height, ZlImageCodec.Png, false) : (imagePayload ?? EncodeBitmap(Image, Width, Height, imageCodec, false));
                ImageBc7Bytes = storePngSourceImages ? (imagePayload ?? EncodeBitmap(Image, Width, Height, imageCodec, false)) : null;
                ImageFallbackBytes = null;
                StoredImageDataSize = FBytes?.Length ?? 0;
                ImageBc7DataSize = ImageBc7Bytes?.Length ?? 0;
                ImageFallbackDataSize = 0;

                if (ShadowImage != null)
                {
                    ZlImageCodec codec = shadowCodec ?? imageCodec;
                    shadowPayload = GetUsableSourcePayload(shadowPayload, ShadowWidth, ShadowHeight, codec);
                    ShadowCodec = storePngSourceImages ? ZlImageCodec.Png : codec;
                    ShadowRuntimePreference = GetRuntimePreferenceForCodec(codec);
                    ShadowFBytes = storePngSourceImages ? EncodeBitmap(ShadowImage, ShadowWidth, ShadowHeight, ZlImageCodec.Png, false) : (shadowPayload ?? EncodeBitmap(ShadowImage, ShadowWidth, ShadowHeight, codec, false));
                    ShadowBc7Bytes = storePngSourceImages ? (shadowPayload ?? EncodeBitmap(ShadowImage, ShadowWidth, ShadowHeight, codec, false)) : null;
                    ShadowFallbackBytes = null;
                    StoredShadowDataSize = ShadowFBytes?.Length ?? 0;
                    ShadowBc7DataSize = ShadowBc7Bytes?.Length ?? 0;
                    ShadowFallbackDataSize = 0;
                }

                if (OverlayImage != null)
                {
                    ZlImageCodec codec = overlayCodec ?? imageCodec;
                    overlayPayload = GetUsableSourcePayload(overlayPayload, OverlayWidth, OverlayHeight, codec);
                    OverlayCodec = storePngSourceImages ? ZlImageCodec.Png : codec;
                    OverlayRuntimePreference = GetRuntimePreferenceForCodec(codec);
                    OverlayFBytes = storePngSourceImages ? EncodeBitmap(OverlayImage, OverlayWidth, OverlayHeight, ZlImageCodec.Png, false) : (overlayPayload ?? EncodeBitmap(OverlayImage, OverlayWidth, OverlayHeight, codec, false));
                    OverlayBc7Bytes = storePngSourceImages ? (overlayPayload ?? EncodeBitmap(OverlayImage, OverlayWidth, OverlayHeight, codec, false)) : null;
                    OverlayFallbackBytes = null;
                    StoredOverlayDataSize = OverlayFBytes?.Length ?? 0;
                    OverlayBc7DataSize = OverlayBc7Bytes?.Length ?? 0;
                    OverlayFallbackDataSize = 0;
                }
            }

            private static byte[] GetUsableSourcePayload(byte[] payload, short width, short height, ZlImageCodec codec)
            {
                if (payload == null)
                    return null;

                return payload.Length == GetDataSize(width, height, codec) ? payload : null;
            }

            private static byte[] PreparePixels(Bitmap bitmap, bool useBlackKeyTransparency, bool reverseRedBlue)
            {
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                                                 PixelFormat.Format32bppArgb);

                byte[] pixels = new byte[bitmap.Width * bitmap.Height * 4];

                Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);
                bitmap.UnlockBits(data);

                for (int i = 0; i < pixels.Length; i += 4)
                {
                    if (reverseRedBlue)
                    {
                        // Squish expects RGBA input; GDI bitmaps expose BGRA bytes.
                        byte b = pixels[i];
                        pixels[i] = pixels[i + 2];
                        pixels[i + 2] = b;
                    }

                    if (useBlackKeyTransparency && pixels[i] == 0 && pixels[i + 1] == 0 && pixels[i + 2] == 0)
                        pixels[i + 3] = 0; //Make Transparent
                }

                return pixels;
            }

            public unsafe Mir3Image(Bitmap image, int version, bool useBlackKeyTransparency = false)
            {
                Version = version;
                ImageCodec = GetDefaultCodec(Version);
                ShadowCodec = ImageCodec;
                OverlayCodec = ImageCodec;
                ImageRuntimePreference = GetDefaultRuntimePreference(ImageCodec);
                ShadowRuntimePreference = ImageRuntimePreference;
                OverlayRuntimePreference = ImageRuntimePreference;
                Position = -1;

                if (image == null)
                {
                    FBytes = new byte[0];
                    return;
                }

                Width = (short)image.Width;
                Height = (short)image.Height;
                SourceRectangle = new Rectangle(0, 0, Width, Height);

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

                FBytes = EncodeBitmap(image, Width, Height, ImageCodec, useBlackKeyTransparency);
            }

            public unsafe Mir3Image(Bitmap image, Bitmap shadow, Bitmap overlay, int version, bool useBlackKeyTransparency = false)
            {
                Version = version;
                ImageCodec = GetDefaultCodec(Version);
                ShadowCodec = ImageCodec;
                OverlayCodec = ImageCodec;
                ImageRuntimePreference = GetDefaultRuntimePreference(ImageCodec);
                ShadowRuntimePreference = ImageRuntimePreference;
                OverlayRuntimePreference = ImageRuntimePreference;
                Position = -1;

                if (image == null)
                {
                    FBytes = new byte[0];
                    return;
                }

                Width = (short)image.Width;
                Height = (short)image.Height;
                SourceRectangle = new Rectangle(0, 0, Width, Height);

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

                FBytes = EncodeBitmap(image, Width, Height, ImageCodec, useBlackKeyTransparency);

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

                    ShadowFBytes = EncodeBitmap(shadow, ShadowWidth, ShadowHeight, ShadowCodec, useBlackKeyTransparency);
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

                    OverlayFBytes = EncodeBitmap(overlay, OverlayWidth, OverlayHeight, OverlayCodec, useBlackKeyTransparency);
                }
            }

            public unsafe void CreateImage(BinaryReader reader, Func<int, byte[]> payloadReader = null)
            {
                if (Position < 0) return;
                if (Position == 0 && payloadReader == null) return;

                Size textureSize = GetTextureSize(Width, Height, ImageCodec);
                int w = textureSize.Width;
                int h = textureSize.Height;

                if (w == 0 || h == 0) return;

                byte[] payload = payloadReader?.Invoke(Position);
                if (payload != null)
                    FBytes = ReadPayloadSegment(payload, 0, ImageDataSize);
                else
                {
                    reader.BaseStream.Seek(Position, SeekOrigin.Begin);
                    FBytes = reader.ReadBytes(ImageDataSize);
                }

                Image = DecodeBitmap(FBytes, w, h, ImageCodec);
                ImageValid = true;
            }
            public unsafe void CreateShadow(BinaryReader reader, Func<int, byte[]> payloadReader = null)
            {
                if (Position < 0) return;
                if (Position == 0 && payloadReader == null) return;

                if (!ImageValid)
                    CreateImage(reader, payloadReader);

                Size textureSize = GetTextureSize(ShadowWidth, ShadowHeight, ShadowCodec);
                int w = textureSize.Width;
                int h = textureSize.Height;

                if (w == 0 || h == 0) return;

                int offset = ImageDataSize + ImageBc7DataSize + ImageFallbackDataSize;
                byte[] payload = payloadReader?.Invoke(Position);
                if (payload != null)
                    ShadowFBytes = ReadPayloadSegment(payload, offset, ShadowDataSize);
                else
                {
                    reader.BaseStream.Seek(Position + offset, SeekOrigin.Begin);
                    ShadowFBytes = reader.ReadBytes(ShadowDataSize);
                }

                ShadowImage = DecodeBitmap(ShadowFBytes, w, h, ShadowCodec);
                ShadowValid = true;
            }
            public unsafe void CreateOverlay(BinaryReader reader, Func<int, byte[]> payloadReader = null)
            {
                if (Position < 0) return;
                if (Position == 0 && payloadReader == null) return;

                if (!ImageValid)
                    CreateImage(reader, payloadReader);

                Size textureSize = GetTextureSize(OverlayWidth, OverlayHeight, OverlayCodec);
                int w = textureSize.Width;
                int h = textureSize.Height;

                if (w == 0 || h == 0) return;

                int offset = ImageDataSize + ImageBc7DataSize + ImageFallbackDataSize + ShadowDataSize + ShadowBc7DataSize + ShadowFallbackDataSize;
                byte[] payload = payloadReader?.Invoke(Position);
                if (payload != null)
                    OverlayFBytes = ReadPayloadSegment(payload, offset, OverlayDataSize);
                else
                {
                    reader.BaseStream.Seek(Position + offset, SeekOrigin.Begin);
                    OverlayFBytes = reader.ReadBytes(OverlayDataSize);
                }

                OverlayImage = DecodeBitmap(OverlayFBytes, w, h, OverlayCodec);
                OverlayValid = true;
            }

            private static byte[] ReadPayloadSegment(byte[] payload, int offset, int count)
            {
                if (payload == null || count <= 0 || offset < 0 || offset >= payload.Length)
                    return Array.Empty<byte>();

                byte[] segment = new byte[Math.Min(count, payload.Length - offset)];
                Buffer.BlockCopy(payload, offset, segment, 0, segment.Length);
                return segment;
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

                if (Version < 2)
                    return;

                Rectangle source = SourceRectangle == default ? new Rectangle(0, 0, Width, Height) : SourceRectangle;
                Rectangle visible = VisibleBounds == default ? new Rectangle(0, 0, Width, Height) : VisibleBounds;

                writer.Write(AtlasPage);
                writer.Write((short)source.X);
                writer.Write((short)source.Y);
                writer.Write((short)source.Width);
                writer.Write((short)source.Height);
                writer.Write((short)visible.X);
                writer.Write((short)visible.Y);
                writer.Write((short)visible.Width);
                writer.Write((short)visible.Height);
                writer.Write((byte)ImageCodec);
                writer.Write((byte)ShadowCodec);
                writer.Write((byte)OverlayCodec);
                writer.Write((byte)ImageRuntimePreference);
                writer.Write((byte)ShadowRuntimePreference);
                writer.Write((byte)OverlayRuntimePreference);
                writer.Write(FBytes?.Length ?? 0);
                writer.Write(ImageBc7Bytes?.Length ?? 0);
                writer.Write(ImageFallbackBytes?.Length ?? 0);
                writer.Write(ShadowFBytes?.Length ?? 0);
                writer.Write(ShadowBc7Bytes?.Length ?? 0);
                writer.Write(ShadowFallbackBytes?.Length ?? 0);
                writer.Write(OverlayFBytes?.Length ?? 0);
                writer.Write(OverlayBc7Bytes?.Length ?? 0);
                writer.Write(OverlayFallbackBytes?.Length ?? 0);
            }

            private SquishFlags DxtFlags
            {
                get
                {
                    return ImageCodec switch
                    {
                        ZlImageCodec.Dxt1 => SquishFlags.Dxt1,
                        _ => SquishFlags.Dxt5,
                    };
                }
            }

            public Rectangle CalculateVisibleBoundsFromBitmap()
            {
                if (Image == null || Width <= 0 || Height <= 0)
                    return new Rectangle(0, 0, Width, Height);

                int minX = Width;
                int minY = Height;
                int maxX = -1;
                int maxY = -1;

                BitmapData data = Image.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                try
                {
                    int stride = data.Stride;
                    byte[] pixels = new byte[stride * Height];
                    Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);

                    for (int y = 0; y < Height; y++)
                    {
                        int row = y * stride;
                        for (int x = 0; x < Width; x++)
                        {
                            if (pixels[row + x * 4 + 3] == 0)
                                continue;

                            if (x < minX) minX = x;
                            if (y < minY) minY = y;
                            if (x > maxX) maxX = x;
                            if (y > maxY) maxY = y;
                        }
                    }
                }
                finally
                {
                    Image.UnlockBits(data);
                }

                if (maxX < minX || maxY < minY)
                    return new Rectangle(0, 0, Width, Height);

                return Rectangle.FromLTRB(minX, minY, maxX + 1, maxY + 1);
            }

            private void RebuildBytes()
            {
                if (Image != null)
                {
                    FBytes = ShouldWritePrimaryPayload(ImageCodec, ImageRuntimePreference) ? EncodeBitmap(Image, Width, Height, ImageCodec, false) : null;
                    ImageBc7Bytes = ShouldWriteRuntimeTexture(ImageCodec, ImageRuntimePreference) ? EncodeBitmap(Image, Width, Height, GetRuntimeCodec(ImageRuntimePreference), false) : null;
                    ImageFallbackBytes = ShouldWriteDxtFallback(ImageRuntimePreference) ? EncodeBitmap(Image, Width, Height, ZlImageCodec.Dxt5, false) : null;
                    StoredImageDataSize = FBytes?.Length ?? 0;
                    ImageBc7DataSize = ImageBc7Bytes?.Length ?? 0;
                    ImageFallbackDataSize = ImageFallbackBytes?.Length ?? 0;
                }

                if (ShadowImage != null)
                {
                    ShadowFBytes = ShouldWritePrimaryPayload(ShadowCodec, ShadowRuntimePreference) ? EncodeBitmap(ShadowImage, ShadowWidth, ShadowHeight, ShadowCodec, false) : null;
                    ShadowBc7Bytes = ShouldWriteRuntimeTexture(ShadowCodec, ShadowRuntimePreference) ? EncodeBitmap(ShadowImage, ShadowWidth, ShadowHeight, GetRuntimeCodec(ShadowRuntimePreference), false) : null;
                    ShadowFallbackBytes = ShouldWriteDxtFallback(ShadowRuntimePreference) ? EncodeBitmap(ShadowImage, ShadowWidth, ShadowHeight, ZlImageCodec.Dxt5, false) : null;
                    StoredShadowDataSize = ShadowFBytes?.Length ?? 0;
                    ShadowBc7DataSize = ShadowBc7Bytes?.Length ?? 0;
                    ShadowFallbackDataSize = ShadowFallbackBytes?.Length ?? 0;
                }

                if (OverlayImage != null)
                {
                    OverlayFBytes = ShouldWritePrimaryPayload(OverlayCodec, OverlayRuntimePreference) ? EncodeBitmap(OverlayImage, OverlayWidth, OverlayHeight, OverlayCodec, false) : null;
                    OverlayBc7Bytes = ShouldWriteRuntimeTexture(OverlayCodec, OverlayRuntimePreference) ? EncodeBitmap(OverlayImage, OverlayWidth, OverlayHeight, GetRuntimeCodec(OverlayRuntimePreference), false) : null;
                    OverlayFallbackBytes = ShouldWriteDxtFallback(OverlayRuntimePreference) ? EncodeBitmap(OverlayImage, OverlayWidth, OverlayHeight, ZlImageCodec.Dxt5, false) : null;
                    StoredOverlayDataSize = OverlayFBytes?.Length ?? 0;
                    OverlayBc7DataSize = OverlayBc7Bytes?.Length ?? 0;
                    OverlayFallbackDataSize = OverlayFallbackBytes?.Length ?? 0;
                }
            }

            public void WritePayload(BinaryWriter writer)
            {
                if (FBytes != null)
                    writer.Write(FBytes);
                if (ImageBc7Bytes != null)
                    writer.Write(ImageBc7Bytes);
                if (ImageFallbackBytes != null)
                    writer.Write(ImageFallbackBytes);
                if (ShadowFBytes != null)
                    writer.Write(ShadowFBytes);
                if (ShadowBc7Bytes != null)
                    writer.Write(ShadowBc7Bytes);
                if (ShadowFallbackBytes != null)
                    writer.Write(ShadowFallbackBytes);
                if (OverlayFBytes != null)
                    writer.Write(OverlayFBytes);
                if (OverlayBc7Bytes != null)
                    writer.Write(OverlayBc7Bytes);
                if (OverlayFallbackBytes != null)
                    writer.Write(OverlayFallbackBytes);
            }

            public static unsafe byte[] EncodeBitmap(Bitmap bitmap, short width, short height, ZlImageCodec codec, bool useBlackKeyTransparency)
            {
                Bitmap source = bitmap;
                if (width > 0 && height > 0 && (bitmap.Width != width || bitmap.Height != height))
                {
                    source = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                    using (Graphics g = Graphics.FromImage(source))
                    {
                        g.Clear(Color.Transparent);
                        g.DrawImage(bitmap, new Rectangle(0, 0, width, height), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
                    }
                }

                try
                {
                if (codec == ZlImageCodec.Png)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        lock (PngEncoderLock)
                            source.Save(stream, ImageFormat.Png);

                        return stream.ToArray();
                    }
                }

                if (codec == ZlImageCodec.Bgra32)
                    return PreparePixels(source, useBlackKeyTransparency, false);

                if (codec == ZlImageCodec.Bc7)
                    return EncodeBc7(source, useBlackKeyTransparency);

                SquishFlags flags = codec == ZlImageCodec.Dxt1 ? SquishFlags.Dxt1 : SquishFlags.Dxt5;
                byte[] pixels = PreparePixels(source, useBlackKeyTransparency, true);
                int count = Squish.GetStorageRequirements(source.Width, source.Height, flags);
                byte[] bytes = new byte[count];

                fixed (byte* dest = bytes)
                fixed (byte* pixelSource = pixels)
                {
                    Squish.CompressImage((IntPtr)pixelSource, source.Width, source.Height, (IntPtr)dest, flags);
                }

                return bytes;
                }
                finally
                {
                    if (!ReferenceEquals(source, bitmap))
                        source.Dispose();
                }
            }

            private static unsafe Bitmap DecodeBitmap(byte[] bytes, int width, int height, ZlImageCodec codec)
            {
                bytes ??= Array.Empty<byte>();

                if (codec == ZlImageCodec.Png)
                {
                    using (MemoryStream stream = new MemoryStream(bytes))
                    using (Bitmap source = new Bitmap(stream))
                    {
                        return new Bitmap(source);
                    }
                }

                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                if (codec == ZlImageCodec.Bgra32)
                {
                    Marshal.Copy(bytes, 0, data.Scan0, Math.Min(bytes.Length, Math.Abs(data.Stride) * height));
                    bitmap.UnlockBits(data);
                    return bitmap;
                }

                if (codec == ZlImageCodec.Bc7)
                {
                    bitmap.UnlockBits(data);
                    bitmap.Dispose();
                    return DecodeBc7(bytes, width, height);
                }

                SquishFlags flags = codec == ZlImageCodec.Dxt1 ? SquishFlags.Dxt1 : SquishFlags.Dxt5;
                int expectedSize = Squish.GetStorageRequirements(width, height, flags);
                if (bytes.Length < expectedSize)
                {
                    byte[] padded = new byte[expectedSize];
                    Buffer.BlockCopy(bytes, 0, padded, 0, bytes.Length);
                    bytes = padded;
                }

                fixed (byte* source = bytes)
                    Squish.DecompressImage(data.Scan0, width, height, (IntPtr)source, flags);

                byte* dest = (byte*)data.Scan0;

                for (int i = 0; i < height * width * 4; i += 4)
                {
                    byte b = dest[i];
                    dest[i] = dest[i + 2];
                    dest[i + 2] = b;
                }

                bitmap.UnlockBits(data);
                return bitmap;
            }

            private static ZlImageCodec GetDefaultCodec(int version)
            {
                if (version == 0)
                    return ZlImageCodec.Dxt1;

                return version >= 2 ? ZlImageCodec.Png : ZlImageCodec.Dxt5;
            }

            private static ZlRuntimeTexturePreference GetDefaultRuntimePreference(ZlImageCodec codec)
            {
                return (codec == ZlImageCodec.Bgra32 || codec == ZlImageCodec.Png)
                    ? ZlRuntimeTexturePreference.Bgra32
                    : ZlRuntimeTexturePreference.Bc7;
            }

            private static bool ShouldWriteBc7(ZlRuntimeTexturePreference preference)
            {
                return preference == ZlRuntimeTexturePreference.Bc7 || preference == ZlRuntimeTexturePreference.Bc7Dxt5;
            }

            private static bool ShouldWriteRuntimeTexture(ZlImageCodec codec, ZlRuntimeTexturePreference preference)
            {
                return codec == ZlImageCodec.Png && (preference == ZlRuntimeTexturePreference.Bc7 || preference == ZlRuntimeTexturePreference.Dxt1 || preference == ZlRuntimeTexturePreference.Dxt5);
            }

            private static bool ShouldWritePrimaryPayload(ZlImageCodec codec, ZlRuntimeTexturePreference preference)
            {
                return codec == ZlImageCodec.Png || preference != ZlRuntimeTexturePreference.None;
            }

            private static ZlImageCodec GetRuntimeCodec(ZlRuntimeTexturePreference preference)
            {
                return preference switch
                {
                    ZlRuntimeTexturePreference.Dxt1 => ZlImageCodec.Dxt1,
                    ZlRuntimeTexturePreference.Dxt5 => ZlImageCodec.Dxt5,
                    ZlRuntimeTexturePreference.Bc7 => ZlImageCodec.Bc7,
                    _ => ZlImageCodec.Bgra32,
                };
            }

            private static ZlImageCodec GetPrimaryRuntimeCodec(ZlRuntimeTexturePreference preference)
            {
                return preference == ZlRuntimeTexturePreference.Bgra32 || preference == ZlRuntimeTexturePreference.None
                    ? ZlImageCodec.Bgra32
                    : GetRuntimeCodec(preference);
            }

            private static bool ShouldWriteDxtFallback(ZlRuntimeTexturePreference preference)
            {
                return preference == ZlRuntimeTexturePreference.Bc7Dxt5;
            }

            private static ZlRuntimeTexturePreference GetRuntimePreferenceForCodec(ZlImageCodec codec)
            {
                return codec switch
                {
                    ZlImageCodec.Dxt1 => ZlRuntimeTexturePreference.Dxt1,
                    ZlImageCodec.Dxt5 => ZlRuntimeTexturePreference.Dxt5,
                    ZlImageCodec.Bc7 => ZlRuntimeTexturePreference.Bc7,
                    _ => ZlRuntimeTexturePreference.Bgra32,
                };
            }

            private static byte[] EncodeBc7(Bitmap bitmap, bool useBlackKeyTransparency)
            {
                BcEncoder encoder = new BcEncoder(CompressionFormat.Bc7);
                encoder.OutputOptions.GenerateMipMaps = false;
                encoder.OutputOptions.Quality = BcCompressionQuality.Balanced;

                byte[] pixels = PreparePixels(bitmap, useBlackKeyTransparency, false);
                return encoder.EncodeToRawBytes(pixels, bitmap.Width, bitmap.Height, BcPixelFormat.Bgra32)[0];
            }

            private static Bitmap DecodeBc7(byte[] bytes, int width, int height)
            {
                BcDecoder decoder = new BcDecoder();
                ColorRgba32[] rgbaPixels = decoder.DecodeRaw(bytes, width, height, CompressionFormat.Bc7);
                byte[] bgraPixels = new byte[width * height * 4];

                for (int i = 0; i < rgbaPixels.Length; i++)
                {
                    int offset = i * 4;
                    ColorRgba32 pixel = rgbaPixels[i];
                    bgraPixels[offset] = pixel.b;
                    bgraPixels[offset + 1] = pixel.g;
                    bgraPixels[offset + 2] = pixel.r;
                    bgraPixels[offset + 3] = pixel.a;
                }

                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                try
                {
                    int rowBytes = width * 4;
                    for (int y = 0; y < height; y++)
                        Marshal.Copy(bgraPixels, y * rowBytes, IntPtr.Add(data.Scan0, y * data.Stride), rowBytes);
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }

                return bitmap;
            }

            public static byte[] DecodeBitmapToBgra(byte[] bytes, int width, int height, ZlImageCodec codec)
            {
                using (Bitmap bitmap = DecodeBitmap(bytes, width, height, codec))
                {
                    BitmapData data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                    try
                    {
                        byte[] pixels = new byte[bitmap.Width * bitmap.Height * 4];
                        for (int y = 0; y < bitmap.Height; y++)
                            Marshal.Copy(IntPtr.Add(data.Scan0, y * data.Stride), pixels, y * bitmap.Width * 4, bitmap.Width * 4);

                        return pixels;
                    }
                    finally
                    {
                        bitmap.UnlockBits(data);
                    }
                }
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

            private static int GetBlockCount(short width, short height)
            {
                if (width <= 0 || height <= 0)
                    return 0;

                int blocksX = Math.Max(1, (width + 3) / 4);
                int blocksY = Math.Max(1, (height + 3) / 4);
                return blocksX * blocksY;
            }

            private static Size GetTextureSize(short width, short height, ZlImageCodec codec)
            {
                if (codec == ZlImageCodec.Bgra32 || codec == ZlImageCodec.Png)
                    return new Size(width, height);

                return new Size(
                    width + (4 - width % 4) % 4,
                    height + (4 - height % 4) % 4);
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
