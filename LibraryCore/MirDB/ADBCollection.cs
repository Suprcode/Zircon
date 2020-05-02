using System;
using MirDB;

namespace Library.MirDB
{
    public abstract class ADBCollection
    {
        public abstract int Count { get; }

        internal DBMapping Mapping { get; set; }

        internal bool IsSystemData { get; set; }

        internal Session Session { get; set; }

        internal Type Type { get; set; }
        internal bool ReadOnly { get; set; }

        public bool RaisePropertyChanges { get; set; }

        internal abstract void Load(byte[] data, DBMapping mapping);
        internal abstract void SaveObjects();
        internal abstract byte[] GetSaveData();
        internal abstract void Delete(DBObject ob);

        internal abstract DBObject CreateObject();

        internal abstract DBObject GetObjectByIndex(int index);
        protected internal abstract DBObject GetObjectbyFieldName(string fieldName, object value);
        internal abstract void OnLoaded();
    }
}
