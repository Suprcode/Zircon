using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace MirDB
{
    public sealed class DBValue
    {
        internal static readonly Dictionary<string, Type> TypeList;
        private static readonly Dictionary<Type, Func<BinaryReader, object>> TypeRead;
        private static readonly Dictionary<Type, Action<object, BinaryWriter>> TypeWrite;

        static DBValue()
        {
            #region Types
            TypeList = new Dictionary<string, Type>
            {
                [typeof(Boolean).FullName] = typeof(Boolean),
                [typeof(Byte).FullName] = typeof(Byte),
                [typeof(Byte[]).FullName] = typeof(Byte[]),
                [typeof(Char).FullName] = typeof(Char),
                [typeof(Color).FullName] = typeof(Color),
                [typeof(DateTime).FullName] = typeof(DateTime),
                [typeof(Decimal).FullName] = typeof(Decimal),
                [typeof(Double).FullName] = typeof(Double),
                [typeof(Int16).FullName] = typeof(Int16),
                [typeof(Int32).FullName] = typeof(Int32),
                [typeof(Int32[]).FullName] = typeof(Int32[]),
                [typeof(Int64).FullName] = typeof(Int64),
                [typeof(Point).FullName] = typeof(Point),
                [typeof(SByte).FullName] = typeof(SByte),
                [typeof(Single).FullName] = typeof(Single),
                [typeof(Size).FullName] = typeof(Size),
                [typeof(String).FullName] = typeof(String),
                [typeof(TimeSpan).FullName] = typeof(TimeSpan),
                [typeof(UInt16).FullName] = typeof(UInt16),
                [typeof(UInt32).FullName] = typeof(UInt32),
                [typeof(UInt64).FullName] = typeof(UInt64),
                [typeof(Point[]).FullName] = typeof(Point[]),
                [typeof(Stats).FullName] = typeof(Stats)
            };
            #endregion

            #region Reads

            TypeRead = new Dictionary<Type, Func<BinaryReader, object>>
            {
                [typeof(Boolean)] = r => r.ReadBoolean(),
                [typeof(Byte)] = r => r.ReadByte(),
                [typeof(Byte[])] = r => r.ReadBytes(r.ReadInt32()),
                [typeof(Char)] = r => r.ReadChar(),
                [typeof(Color)] = r => Color.FromArgb(r.ReadInt32()),
                [typeof(DateTime)] = r => DateTime.FromBinary(r.ReadInt64()),
                [typeof(Decimal)] = r => r.ReadDecimal(),
                [typeof(Double)] = r => r.ReadDouble(),
                [typeof(Enum)] = r => r.ReadInt32(),
                [typeof(Int16)] = r => r.ReadInt16(),
                [typeof(Int32)] = r => r.ReadInt32(),
                [typeof(Int32[])] = r =>
                {
                    if (!r.ReadBoolean()) return null;

                    int length = r.ReadInt32();

                    Int32[] values = new Int32[length];
                    for (int i = 0; i < length; i++)
                        values[i] = r.ReadInt32();

                    return values;

                },
                [typeof(Int64)] = r => r.ReadInt64(),
                [typeof(Point)] = r => new Point(r.ReadInt32(), r.ReadInt32()),
                [typeof(SByte)] = r => r.ReadSByte(),
                [typeof(Single)] = r => r.ReadSingle(),
                [typeof(Size)] = r => new Size(r.ReadInt32(), r.ReadInt32()),
                [typeof(String)] = r => r.ReadString(),
                [typeof(TimeSpan)] = r => TimeSpan.FromTicks(r.ReadInt64()),
                [typeof(UInt16)] = r => r.ReadUInt16(),
                [typeof(UInt32)] = r => r.ReadUInt32(),
                [typeof(UInt64)] = r => r.ReadUInt64(),
                [typeof(Point[])] = r =>
                {
                    if (!r.ReadBoolean()) return null;

                    int length = r.ReadInt32();

                    Point[] points = new Point[length];
                    for (int i = 0; i < length; i++)
                        points[i] = new Point(r.ReadInt32(), r.ReadInt32());

                    return points;

                },
                [typeof(Stats)] = r => r.ReadBoolean() ? new Stats(r) : null
            };

            #endregion

            #region Writes

            TypeWrite = new Dictionary<Type, Action<object, BinaryWriter>>
            {
                [typeof(Boolean)] = (v, w) => w.Write((bool) v),
                [typeof(Byte)] = (v, w) => w.Write((Byte) v),
                [typeof(Byte[])] = (v, w) =>
                {
                    w.Write(((Byte[]) v).Length);
                    w.Write((Byte[]) v);
                },
                [typeof(Char)] = (v, w) => w.Write((Char) v),
                [typeof(Color)] = (v, w) => w.Write(((Color) v).ToArgb()),
                [typeof(DateTime)] = (v, w) => w.Write(((DateTime) v).ToBinary()),
                [typeof(Decimal)] = (v, w) => w.Write((Decimal) v),
                [typeof(Double)] = (v, w) => w.Write((Double) v),
                [typeof(Int16)] = (v, w) => w.Write((Int16) v),
                [typeof(Int32)] = (v, w) => w.Write((Int32) v),
                [typeof(Int32[])] = (v, w) =>
                {
                    w.Write(v != null);
                    if (v == null) return;
                    Int32[] values = (Int32[]) v;

                    w.Write(values.Length);

                    foreach (Int32 value in values)
                        w.Write(value);
                },
                [typeof(Int64)] = (v, w) => w.Write((Int64) v),
                [typeof(Point)] = (v, w) =>
                {
                    w.Write(((Point) v).X);
                    w.Write(((Point) v).Y);
                },
                [typeof(SByte)] = (v, w) => w.Write((SByte) v),
                [typeof(Single)] = (v, w) => w.Write((Single) v),
                [typeof(Size)] = (v, w) =>
                {
                    w.Write(((Size) v).Width);
                    w.Write(((Size) v).Height);
                },
                [typeof(String)] = (v, w) => w.Write((String) v ?? string.Empty),
                [typeof(TimeSpan)] = (v, w) => w.Write(((TimeSpan) v).Ticks),
                [typeof(UInt16)] = (v, w) => w.Write((UInt16) v),
                [typeof(UInt32)] = (v, w) => w.Write((UInt32) v),
                [typeof(UInt64)] = (v, w) => w.Write((UInt64) v),
                [typeof(Point[])] = (v, w) =>
                {
                    w.Write(v != null);
                    if (v == null) return;
                    Point[] points = (Point[]) v;

                    w.Write(points.Length);

                    foreach (Point point in points)
                    {
                        w.Write(point.X);
                        w.Write(point.Y);
                    }
                },
                [typeof(Stats)] = (v, w) =>
                {
                    w.Write(v != null);
                    if (v == null) return;

                    ((Stats) v).Write(w);
                },
            };

            #endregion
        }

        public string PropertyName { get; }
        public Type PropertyType { get; }
        public PropertyInfo Property { get; }

        public DBValue(BinaryReader reader, Type type)
        {
            PropertyName = reader.ReadString();
            PropertyType = TypeList[reader.ReadString()];


            PropertyInfo property = type?.GetProperty(PropertyName);

            if (property != null)
                if (property.GetCustomAttribute<IgnoreProperty>() != null) return;

            Property = property;
        }
        public DBValue(PropertyInfo property)
        {
            Property = property;

            PropertyName = property.Name;

            if (property.PropertyType.IsEnum)
            {
                PropertyType = property.PropertyType.GetEnumUnderlyingType();
                return;
            }

            if (property.PropertyType.IsSubclassOf(typeof(DBObject)))
            {
                PropertyType = typeof(int);
                return;
            }

            PropertyType = property.PropertyType;
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(PropertyName);
            writer.Write(PropertyType.FullName);
        }

        public object ReadValue(BinaryReader reader)
        {
            return TypeRead[PropertyType](reader);
        }
        public void WriteValue(object value, BinaryWriter writer)
        {
            TypeWrite[PropertyType](value, writer);
        }


        public bool IsMatch(DBValue value)
        {
            return string.Compare(PropertyName, value.PropertyName, StringComparison.Ordinal) == 0 && PropertyType == value.PropertyType;
        }
    }
}
