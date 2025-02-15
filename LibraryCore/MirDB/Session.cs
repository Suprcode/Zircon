using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Library.MirDB;

namespace MirDB
{
    public sealed class Session
    {
        public const string Extension = @".db";
        public const string TempExtension = @".TMP";
        public const string CompressExtension = @".gz";

        public string Root { get; }
        public SessionMode Mode { get; }

        public bool BackUp { get; set; } = true;
        public int BackUpDelay { get; set; }
        private string BackupRoot { get; }

        public string SystemPath => Root + "System" + Extension;
        public string SystemBackupPath => BackupRoot + @"System\";
        public byte[] SystemHeader;

        public string UsersPath => Root + "Users" + Extension;
        public string UsersBackupPath => BackupRoot + @"Users\";

        public Assembly[] Assemblies { get; private set; }

        public byte[] UsersHeader;

        //internal ConcurrentQueue<DBObject> KeyedObjects = new ConcurrentQueue<DBObject>();
        internal Dictionary<Type, DBRelationship> Relationships = new Dictionary<Type, DBRelationship>();

        private Dictionary<Type, ADBCollection> Collections;

        public Session(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "Database:DBConnStr is empty");

            var args = connectionString.Split(';').Select(x => x.Split(new char[] { '=' }, 2)).ToDictionary(x => x[0].ToUpperInvariant(), x => x[1]);

            if (!args.ContainsKey("MODE") || !args.ContainsKey("ROOT") || !args.ContainsKey("BACKUP"))
                throw new ArgumentException($"Connection string is not valid");

            if (!Enum.TryParse(args["MODE"], out SessionMode mode))
                throw new ArgumentException($"{args["MODE"]} is not valid option for Mode");

            if (args.ContainsKey("BACKUPDELAY"))
            {
                if (!int.TryParse(args["BACKUPDELAY"], out int backupDelay))
                    throw new ArgumentException($"{args["BACKUPDELAY"]} is not valid number");
                BackUpDelay = backupDelay;
            }

            Root = args["ROOT"];
            BackupRoot = args["BACKUP"];
            Mode = mode;
        }

        public Session(SessionMode mode, string root = @".\Database\", string backup = @".\Backup\")
        {
            Root = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, root));
            BackupRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, backup));

            Mode = mode;
        }

        public void Initialize(params Assembly[] assemblies)
        {
            Assemblies = assemblies;

            if (!Directory.Exists(Root))
                Directory.CreateDirectory(Root);

            Collections = new Dictionary<Type, ADBCollection>();

            List<Type> types = assemblies
                .Select(x => x.GetTypes())
                .SelectMany(x => x)
                .ToList();

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

            using (BinaryReader reader = Library.Encryption.GetReader(File.OpenRead(SystemPath)))
            {
                int count = reader.ReadInt32();

                for (int i = 0; i < count; i++)
                    mappings.Add(new DBMapping(Assemblies, reader));

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

            using (BinaryReader reader = Library.Encryption.GetReader(File.OpenRead(UsersPath)))
            {
                int count = reader.ReadInt32();

                for (int i = 0; i < count; i++)
                    mappings.Add(new DBMapping(Assemblies, reader));

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

            using (BinaryWriter writer = Library.Encryption.GetWriter(File.Create(SystemPath + TempExtension)))
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
                    using (FileStream destStream = File.Create(SystemBackupPath + "System " + ToBackUpFileName(DateTime.UtcNow) + Extension + CompressExtension))
                    using (GZipStream compress = new GZipStream(destStream, CompressionMode.Compress))
                        sourceStream.CopyTo(compress);
                }

                File.Delete(SystemPath);
            }

            File.Move(SystemPath + TempExtension, SystemPath);
        }
        private void SaveUsers()
        {
            if ((Mode & SessionMode.Users) != SessionMode.Users) return;

            if (!Directory.Exists(Root))
                Directory.CreateDirectory(Root);

            using (BinaryWriter writer = Library.Encryption.GetWriter(File.Create(UsersPath + TempExtension)))
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
                    using (FileStream destStream = File.Create(UsersBackupPath + "Users " + ToBackUpFileName(DateTime.UtcNow) + Extension + CompressExtension))
                    using (GZipStream compress = new GZipStream(destStream, CompressionMode.Compress))
                        sourceStream.CopyTo(compress);
                }

                File.Delete(UsersPath);
            }

            File.Move(UsersPath + TempExtension, UsersPath);
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

        public string ToBackUpFileName(DateTime time)
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
                AssociationAttribute link = property.GetCustomAttribute<AssociationAttribute>();

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
