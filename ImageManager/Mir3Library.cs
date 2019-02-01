using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManager
{
    public sealed class Mir3Library
    {
        public Mir3Image[] Images;


        public void Save(string path)
        {
            //|Header Size|Count|T|Header|F|F|T|Header|F|T|Header...|Image|Image|Im...

            int headerSize = 4 + Images.Length;
            
            foreach (Mir3Image image in Images)
            {
                if (image?.Data == null) continue;

                headerSize += Mir3Image.HeaderSize;
            }

            int position = headerSize + 4;

            foreach (Mir3Image image in Images)
            {
                if (image?.Data == null) continue;

                image.Position = position;

                position += image.DataSize;
            }


            using (MemoryStream buffer = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(buffer))
            {
                writer.Write(headerSize);
                writer.Write(Images.Length);

                foreach (Mir3Image image in Images)
                {
                    writer.Write(image?.Data != null);

                    if (image?.Data == null) continue;

                    image.SaveHeader(writer);
                }

                foreach (Mir3Image image in Images)
                {
                    if (image?.Data == null) continue;
                    
                    if (image.Data != null)
                        writer.Write(image.Data);
                    
                    if (image.ShadowData != null)
                        writer.Write(image.ShadowData);
                    
                    if (image.OverLayData != null)
                        writer.Write(image.OverLayData);
                }

                File.WriteAllBytes(path, buffer.ToArray());
            }
        }
           
    }

    public sealed class Mir3Image
    {
        public int Position;

        public short Width;
        public short Height;

        public short OffSetX;
        public short OffSetY;

        public byte ShadowType;
        public short ShadowWidth;
        public short ShadowHeight;
        public short ShadowOffSetX;
        public short ShadowOffSetY;

        public short OverlayWidth;
        public short OverlayHeight;

        
        public byte[] Data { get; set; }

        public byte[] ShadowData { get; set; }

        public byte[] OverLayData { get; set; }

        public const int HeaderSize = 25;

        public int DataSize => (Data?.Length ?? 0) + (ShadowData?.Length ?? 0) + (OverLayData?.Length ?? 0);

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
    }
}
