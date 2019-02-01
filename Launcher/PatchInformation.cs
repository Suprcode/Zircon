using System.IO;

namespace Launcher
{
    public sealed class PatchInformation
    {
        public string FileName { get; set; }
        public long CompressedLength { get; set; }
        public byte[] CheckSum { get; set; }

        public PatchInformation()//FileInfo
        {
        }

        public PatchInformation(BinaryReader reader)
        {
            FileName = reader.ReadString();
            CompressedLength = reader.ReadInt64();

            CheckSum = reader.ReadBytes(reader.ReadInt32());
        }
        public void Save(BinaryWriter writer)
        {
            writer.Write(FileName);
            writer.Write(CompressedLength);
            writer.Write(CheckSum.Length);
            writer.Write(CheckSum);
        }
    }
}
