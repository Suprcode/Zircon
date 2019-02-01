using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace MirDB
{
    public sealed class DBCollection<T> : ADBCollection where T : DBObject, new()
    {
        public int Index { get; set; }

        public T this[int index] => Binding[index];
        public override int Count => Binding.Count;

        public BindingList<T> Binding { get; } = new BindingList<T>();
        private Dictionary<int, T> Dictionary = new Dictionary<int, T>(); //For Obtaining Keys.

        private bool VersionValid;
        private List<T> SaveList;

        public DBCollection(Session session)
        {
            Type = typeof(T);
            Mapping = new DBMapping(Type);

            IsSystemData = Type.GetCustomAttribute<UserObject>() == null;

            RaisePropertyChanges = IsSystemData;
            Binding.RaiseListChangedEvents = RaisePropertyChanges;
            Session = session;

            ReadOnly = IsSystemData ? (Session.Mode & SessionMode.System) == 0 : (Session.Mode & SessionMode.Users) == 0;
            
            Binding.AddingNew += (o, e) => e.NewObject = CreateNew();
        }

        private T CreateNew()
        {
            T ob = new T
            {
                Index = ++Index,
                Collection = this,
            };
            ob.OnCreated();

            return ob;
        }
        public T CreateNewObject()
        {
            T ob = CreateNew();

            Binding.Add(ob);

            return ob;
        }


        internal override DBObject GetObjectByIndex(int index)
        {
            T ob;

            Dictionary.TryGetValue(index, out ob);

            return ob;
        }
        protected internal override DBObject GetObjectbyFieldName(string fieldName, object value)
        {
            PropertyInfo info = Type.GetProperty(fieldName);

            if (info == null) return null;

            foreach (T ob in Binding)
            {
                if (info.GetValue(ob).Equals(value))
                    return ob;
            }

            return null;
        }
        internal override void Load(byte[] data, DBMapping mapping)
        {
            using (MemoryStream mStream = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(mStream))
            {
                Index = reader.ReadInt32();

                int count = reader.ReadInt32();

                for (int i = 0; i < count; i++)
                {
                    T ob = new T { Collection = this };
                    Binding.Add(ob);

                    ob.RawData = reader.ReadBytes(reader.ReadInt32());
                    ob.Load(mapping);

                    if (ob.ForeignKeys != null)
                        Session.KeyedObjects.Enqueue(ob);
                    
                    Dictionary[ob.Index] = ob;
                }

            }
            VersionValid = mapping.IsMatch(Mapping);
        }

        internal override void SaveObjects()
        {
            if (ReadOnly || SaveList != null) return;

            SaveList = new List<T>();

            foreach (T ob in Binding)
            {
                if (ob.IsTemporary) continue;

                if (!VersionValid || ob.IsModified)
                    ob.Save(Mapping);

                SaveList.Add(ob);
            }

            VersionValid = true;
        }

        internal override byte[] GetSaveData()
        {
            using (MemoryStream mStream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(mStream))
            {
                writer.Write(Index);
                writer.Write(SaveList.Count);
                
                foreach (T ob in SaveList)
                {
                    writer.Write(ob.RawData.Length);
                    writer.Write(ob.RawData);
                }

                mStream.Seek(4, SeekOrigin.Begin);

                SaveList = null;
                return mStream.ToArray();
            }

        }


        internal override void Delete(DBObject ob)
        {
            Binding.Remove((T)ob);
        }

        internal override DBObject CreateObject()
        {
            return CreateNewObject();
        }

        internal override void OnLoaded()
        {
            Dictionary.Clear();
            Dictionary = null;
            foreach (T ob in Binding)
                ob.OnLoaded();
        }

    }
}
