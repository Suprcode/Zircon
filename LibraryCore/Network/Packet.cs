using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using LibraryCore;
using MemoryPack;
using MemoryPack.Formatters;

namespace Library.Network
{
    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class Packet
    {
        public static bool IsClient { get; set; }

        [MemoryPackIgnore]
        public int Length;
        [MemoryPackIgnore]
        public bool ObserverPacket = true;

        static Packet()
        {
            MemoryPackFormatterProvider.Register(new StatsFormatter());
            MemoryPackFormatterProvider.Register(new ColorFormatter());

            var array = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(type=>type.IsSubclassOf(typeof(Packet)))
                .OrderBy(type=>type.FullName)
                .Select((type, index) => ((ushort)index, type))
                .ToArray();
        
            var formatter = new DynamicUnionFormatter<Packet>(array);
            MemoryPackFormatterProvider.Register(formatter);
        }

        public static Packet ReceivePacket(byte[] rawBytes, out byte[] extra)
        {
            extra = rawBytes;

            if (rawBytes.Length < 4) return null; //4Bytes: Packet Size |

            int length = rawBytes[3] << 24 | rawBytes[2] << 16 | rawBytes[1] << 8 | rawBytes[0];

            if (length > rawBytes.Length) return null;

            extra = new byte[rawBytes.Length - length];
            Buffer.BlockCopy(rawBytes, length, extra, 0, rawBytes.Length - length);
            
            var p = MemoryPackSerializer.Deserialize<Packet>(rawBytes.AsSpan(4));
            p.Length = length;

            return p;
        }
        public byte[] GetPacketBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                var array = MemoryPackSerializer.Serialize(this);
                writer.Write(array.Length + 4); //| 4Bytes: Packet Size | Data... |
                writer.Write(array);

                Length = (int)stream.Length;
                return stream.ToArray();
            }
        }

    }
}
