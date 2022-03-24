using System.IO;
using System.Security.Cryptography;

namespace PatchManager
{
    public sealed class PatchInformation
    {
        public string FileName { get; set; }

        public string UploadFileName { get; set; }
        public string PatchFileName { get; set; }
        public long CompressedLength { get; set; }
        public byte[] CheckSum { get; set; }

        public PatchInformation()//FileInfo
        {
        }

        public PatchInformation(string fileName)
        {
            FileName = fileName.Remove(0, Config.CleanClient.Length);

            using (MD5 md5 = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(fileName))
                    CheckSum = md5.ComputeHash(stream);
            }
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
