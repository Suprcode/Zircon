using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MirDB
{
    public sealed class DBMapping
    {
        public Type Type { get; }

        public List<DBValue> Properties { get; } = new List<DBValue>();

        public DBMapping(Type type)
        {
            Type = type;

            PropertyInfo[] properties = Type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);

            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttribute<IgnoreProperty>() != null) continue;
                if (!DBValue.TypeList.ContainsValue(property.PropertyType) && !property.PropertyType.IsEnum && !property.PropertyType.IsSubclassOf(typeof(DBObject))) continue;

                Properties.Add(new DBValue(property));
            }
        }
        public DBMapping(BinaryReader reader)
        {
            string typeName = reader.ReadString();
            Type = Assembly.GetEntryAssembly().GetType(typeName) ?? Assembly.GetCallingAssembly().GetType(typeName);

            if (Type == null)
            {
                typeName = typeName.Replace("Server.DBModels", "Library.SystemModels");
                Type = Assembly.GetEntryAssembly().GetType(typeName) ?? Assembly.GetCallingAssembly().GetType(typeName);
            }

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
                Properties.Add(new DBValue(reader, Type));
        }
        public void Save(BinaryWriter writer)
        {
            writer.Write(Type.FullName);

            writer.Write(Properties.Count);

            foreach (DBValue value in Properties)
                value.Save(writer);
        }

        public bool IsMatch(DBMapping mapping)
        {
            if (Properties.Count != mapping.Properties.Count) return false;

            for (int i = 0; i < Properties.Count; i++)
                if (!Properties[i].IsMatch(mapping.Properties[i])) return false;

            return true;
        }
    }
}
