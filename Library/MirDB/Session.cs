using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Library.MirDB;
using Library.SystemModels;

namespace MirDB
{
    public sealed class Session
    {
        private const string Extention = @".db";
        private const string TempExtention = @".TMP";
        private const string CompressExtention = @".gz";

        private string Root { get; }
        internal SessionMode Mode { get; }

        public bool BackUp { get; set; } = true;
        public int BackUpDelay { get; set; }
        private string BackupRoot { get; }


        private string SystemPath => Root + "System" + Extention;
        private string SystemBackupPath => BackupRoot + @"System\";
        private byte[] SystemHeader;

        private string UsersPath => Root + "Users" + Extention;
        private string UsersBackupPath => BackupRoot + @"Users\";
        private byte[] UsersHeader;

        //internal ConcurrentQueue<DBObject> KeyedObjects = new ConcurrentQueue<DBObject>();
        internal Dictionary<Type, DBRelationship> Relationships = new Dictionary<Type, DBRelationship>();

        private Dictionary<Type, ADBCollection> Collections;

        public Session(SessionMode mode, string root = @".\Database\", string backup = @".\Backup\")
        {
            Root = root;
            BackupRoot = backup;
            Mode = mode;

            Initialize();
        }
        private void Initialize()
        {
            if (!Directory.Exists(Root))
                Directory.CreateDirectory(Root);

            Collections = new Dictionary<Type, ADBCollection>();

            List<Type> types = new List<Type>();
            types.AddRange(Assembly.GetEntryAssembly().GetTypes());
            types.AddRange(Assembly.GetExecutingAssembly().GetTypes());
            types.AddRange(Assembly.GetCallingAssembly().GetTypes());

            Type collectionType = typeof(DBCollection<>);

            foreach (Type type in types)
            {
                if (!type.IsSubclassOf(typeof(DBObject))) continue;

                Collections[type] = (ADBCollection)Activator.CreateInstance(collectionType.MakeGenericType(type), this);
            }

            InitializeSystem();

            if ((Mode & SessionMode.Users) == SessionMode.Users)
                InitializeUsers();

            Parallel.ForEach(Relationships, x => x.Value.ConsumeKeys(this));

            Relationships = null;
            /*
            DBCollection<ItemInfo> itemList = GetCollection<ItemInfo>();

            ItemInfo Female = (ItemInfo)itemList.GetObjectByIndex(1100);
            ItemInfo Male = (ItemInfo)itemList.GetObjectByIndex(1043);

            int maleIndex = itemList.Binding.IndexOf(Male );
            Female.Index = 1044;
            itemList.Binding.Remove(Female);
            itemList.Binding.Insert(maleIndex  + 1, Female);
               */
            

            foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
                pair.Value.OnLoaded();
        }

        private void InitializeSystem()
        {
            List<DBMapping> mappings = new List<DBMapping>();
            if ((Mode & SessionMode.System) == SessionMode.System)
            {
                foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
                {
                    if (!pair.Value.IsSystemData) continue;

                    mappings.Add(pair.Value.Mapping);
                }

                using (MemoryStream stream = new MemoryStream())
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(mappings.Count);
                    foreach (DBMapping mapping in mappings)
                        mapping.Save(writer);

                    SystemHeader = stream.ToArray();
                }

                mappings.Clear();
            }

            if (!File.Exists(SystemPath)) return;

            using (BinaryReader reader = new BinaryReader(File.OpenRead(SystemPath)))
            {
                int count = reader.ReadInt32();

                for (int i = 0; i < count; i++)
                    mappings.Add(new DBMapping(reader));

                List<Task> loadingTasks = new List<Task>();
                foreach (DBMapping mapping in mappings)
                {
                    byte[] data = reader.ReadBytes(reader.ReadInt32());

                    ADBCollection value;
                    if (mapping.Type == null || !Collections.TryGetValue(mapping.Type, out value)) continue;

                    loadingTasks.Add(Task.Run(() => value.Load(data, mapping)));
                }

                if (loadingTasks.Count > 0)
                    Task.WaitAll(loadingTasks.ToArray());
            }
        }
        private void InitializeUsers()
        {
            List<DBMapping> mappings = new List<DBMapping>();

            foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
            {
                if (pair.Value.IsSystemData) continue;

                mappings.Add(pair.Value.Mapping);
            }

            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(mappings.Count);
                foreach (DBMapping mapping in mappings)
                    mapping.Save(writer);

                UsersHeader = stream.ToArray();
            }
            mappings.Clear();

            if (!File.Exists(UsersPath)) return;

            using (BinaryReader reader = new BinaryReader(File.OpenRead(UsersPath)))
            {
                int count = reader.ReadInt32();

                for (int i = 0; i < count; i++)
                    mappings.Add(new DBMapping(reader));

                List<Task> loadingTasks = new List<Task>();
                foreach (DBMapping mapping in mappings)
                {
                    byte[] data = reader.ReadBytes(reader.ReadInt32());

                    ADBCollection value;
                    if (mapping.Type == null || !Collections.TryGetValue(mapping.Type, out value)) continue;

                    loadingTasks.Add(Task.Run(() => value.Load(data, mapping)));
                }

                if (loadingTasks.Count > 0)
                    Task.WaitAll(loadingTasks.ToArray());
            }
        }

        public void Save(bool commit)
        {
            Parallel.ForEach(Collections, x => x.Value.SaveObjects());

            if (commit)
                Commit();
        }
        public void Commit()
        {
            SaveSystem();
            SaveUsers();
        }

        private void SaveSystem()
        {
            if ((Mode & SessionMode.System) != SessionMode.System) return;

            if (!Directory.Exists(Root))
                Directory.CreateDirectory(Root);

            using (BinaryWriter writer = new BinaryWriter(File.Create(SystemPath + TempExtention)))
            {
                writer.Write(SystemHeader);

                foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
                {
                    if (!pair.Value.IsSystemData) continue;
                    byte[] data = pair.Value.GetSaveData();

                    writer.Write(data.Length);
                    writer.Write(data);
                }
            }

            if (BackUp && !Directory.Exists(SystemBackupPath))
                Directory.CreateDirectory(SystemBackupPath);

            if (File.Exists(SystemPath))
            {
                if (BackUp)
                {
                    using (FileStream sourceStream = File.OpenRead(SystemPath))
                    using (FileStream destStream = File.Create(SystemBackupPath + "System " + ToBackUpFileName(DateTime.UtcNow) + Extention + CompressExtention))
                    using (GZipStream compress = new GZipStream(destStream, CompressionMode.Compress))
                        sourceStream.CopyTo(compress);
                }

                File.Delete(SystemPath);
            }

            File.Move(SystemPath + TempExtention, SystemPath);
        }
        private void SaveUsers()
        {
            if ((Mode & SessionMode.Users) != SessionMode.Users) return;

            if (!Directory.Exists(Root))
                Directory.CreateDirectory(Root);

            using (BinaryWriter writer = new BinaryWriter(File.Create(UsersPath + TempExtention)))
            {
                writer.Write(UsersHeader);

                foreach (KeyValuePair<Type, ADBCollection> pair in Collections)
                {
                    if (pair.Value.IsSystemData) continue;

                    byte[] data = pair.Value.GetSaveData();

                    writer.Write(data.Length);
                    writer.Write(data);
                }
            }
            if (BackUp && !Directory.Exists(UsersBackupPath))
                Directory.CreateDirectory(UsersBackupPath);

            if (File.Exists(UsersPath))
            {
                if (BackUp)
                {
                    using (FileStream sourceStream = File.OpenRead(UsersPath))
                    using (FileStream destStream = File.Create(UsersBackupPath + "Users " + ToBackUpFileName(DateTime.UtcNow) + Extention + CompressExtention))
                    using (GZipStream compress = new GZipStream(destStream, CompressionMode.Compress))
                        sourceStream.CopyTo(compress);
                }

                File.Delete(UsersPath);
            }

            File.Move(UsersPath + TempExtention, UsersPath);
        }

        public DBCollection<T> GetCollection<T>() where T : DBObject, new()
        {
            return (DBCollection<T>)Collections[typeof(T)];
        }
        public ADBCollection GetCollection(Type type)
        {
            return Collections[type];
        }
        internal DBObject GetObject(Type type, int index)
        {
            return Collections[type].GetObjectByIndex(index);
        }
        public DBObject GetObject(Type type, string fieldName, object value)
        {
            return Collections[type].GetObjectbyFieldName(fieldName, value);
        }

        internal T CreateObject<T>() where T : DBObject, new()
        {
            return (T)Collections[typeof(T)].CreateObject();
        }

        private static string ToFileName(DateTime time)
        {
            return $"{time.Year:0000}-{time.Month:00}-{time.Day:00} {time.Hour:00}-{time.Minute:00}";
        }
        private string ToBackUpFileName(DateTime time)
        {
            if (BackUpDelay == 0)
                return ToFileName(time);

            time = new DateTime(time.Ticks - (time.Ticks % (BackUpDelay * TimeSpan.TicksPerMinute)));

            return $"{time.Year:0000}-{time.Month:00}-{time.Day:00} {time.Hour:00}-{time.Minute:00}";
        }
        internal void Delete(DBObject ob)
        {
            if (ob.IsDeleted) return;

            Collections[ob.ThisType].Delete(ob);

            ob.OnDeleted();

            PropertyInfo[] properties = ob.ThisType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);

            //Remove Internal Reference
            foreach (PropertyInfo property in properties)
            {
                Association link = property.GetCustomAttribute<Association>();

                if (property.PropertyType.IsSubclassOf(typeof(DBObject)))
                {
                    if (link != null && link.Aggregate)
                    {
                        DBObject tempOb = (DBObject)property.GetValue(ob);

                        tempOb?.Delete();
                        continue;
                    }

                    property.SetValue(ob, null);
                    continue;
                }

                if (!property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(DBBindingList<>)) continue;

                IBindingList list = (IBindingList)property.GetValue(ob);

                if (link != null && link.Aggregate)
                {
                    for (int i = list.Count - 1; i >= 0; i--)
                        ((DBObject)list[i]).Delete();
                    continue;
                }

                list.Clear();
            }

        }
        internal void FastDelete(DBObject ob)
        {
            if (ob.IsDeleted) return;

            ob.IsTemporary = true;

            ob.OnDeleted();
        }
    }
}
