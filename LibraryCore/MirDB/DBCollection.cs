using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Library.MirDB;

namespace MirDB
{
    public sealed class DBCollection<T> : ADBCollection where T : DBObject, new()
    {
        public int Index { get; set; }

        public T this[int index] => Binding[index];
        public override int Count => Binding.Count;

        public IList<T> Binding;
        //private SortedDictionary<int, T> Dictionary = new SortedDictionary<int, T>(); //For Obtaining Keys.

        private bool VersionValid;
        private List<T> SaveList;


        public DBCollection(Session session)
        {
            Type = typeof(T);
            Mapping = new DBMapping(session.Assemblies, Type);

            IsSystemData = Type.GetCustomAttribute<UserObjectAttribute>() == null;

            RaisePropertyChanges = IsSystemData;
            Session = session;

            ReadOnly = IsSystemData ? (Session.Mode & SessionMode.System) != SessionMode.System : (Session.Mode & SessionMode.Users) != SessionMode.Users;

            if (IsSystemData)
            {
                BindingList<T> binding = new BindingList<T>
                {
                    RaiseListChangedEvents = RaisePropertyChanges
                };

                binding.AddingNew += (o, e) => e.NewObject = CreateNew();
                Binding = binding;
            }
            else
                Binding = new List<T>();

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
            index = FastFind(index);

            if (index >= 0) return Binding[index];

            return null;
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
            VersionValid = mapping.IsMatch(Mapping);
            
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
                }

            }
        }

        internal override void SaveObjects()
        {
            if (ReadOnly || SaveList != null) return;

            SaveList = new List<T>(Binding.Count);

            foreach (T ob in Binding)
            {
                if (ob.IsTemporary) continue;

                if (!VersionValid || ob.IsModified)
                    ob.Save();

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
            /* for (int i = Binding.Count - 1; i >= 0; i--)
             {
                 if (Binding[i] != ob) continue;

                 Binding.RemoveAt(i);
                 break;
             }*/


            int index = FastFind(ob.Index);

            if (index >= 0) Binding.RemoveAt(index);
        }
        private int FastFind(int index)
        {
            int pos = 0;

            if (Binding.Count == 0) return -1;

            int shift = Binding.Count / 2;

            bool? dir = null;

            while (true)
            {
                shift = Math.Max(1, shift);

                int cur = Binding[pos].Index;
                if (cur == index) return pos;


                if (cur > index)
                {
                    pos -= shift;
                    shift /= 2;

                    if (pos <= -1) break;

                    if (shift == 0)
                    {
                        if (dir.HasValue && dir.Value) break;

                        dir = false;
                    }
                }
                else
                {
                    //Increase pos

                    pos += shift;
                    shift /= 2;

                    if (pos >= Binding.Count) break;

                    if (shift == 0)
                    {
                        if (dir.HasValue && !dir.Value) break;

                        dir = true;
                    }
                }

            }

            return -1;
        }

        internal override DBObject CreateObject()
        {
            return CreateNewObject();
        }

        internal override void OnLoaded()
        {
            foreach (T ob in Binding)
                ob.OnLoaded();
        }

    }
}
