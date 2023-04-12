using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using Library.MirDB;

namespace MirDB
{
    public abstract class DBObject : INotifyPropertyChanged
    {
        [JsonIgnore]
        public int Index { get; internal set; }

        [IgnoreProperty]
        internal ADBCollection Collection
        {
            get { return _Collection; }
            set
            {
                if (_Collection != null) return;

                _Collection = value;

                CreateBindings();
            }
        }
        private ADBCollection _Collection;

        [IgnoreProperty]
        protected Session Session { get { return Collection.Session; } }

        internal readonly Type ThisType;

        [IgnoreProperty]
        protected internal bool IsLoaded { get; private set; }

        [IgnoreProperty]
        protected internal bool IsDeleted { get; private set; }

        [JsonIgnore]
        [IgnoreProperty]
        public bool IsTemporary { get; set; }

        [IgnoreProperty]
        protected internal bool IsModified { get; private set; }

        private MemoryStream SaveMemoryStream;
        private BinaryWriter SaveBinaryWriter;
        protected internal byte[] RawData;

        protected DBObject()
        {
            ThisType = GetType();
        }

        internal void Load(DBMapping mapping)
        {
            using (MemoryStream mStream = new MemoryStream(RawData))
            using (BinaryReader reader = new BinaryReader(mStream))
            {
                foreach (DBValue dbValue in mapping.Properties)
                {
                    object value = dbValue.ReadValue(reader);

                    if (dbValue.Property == null) continue;

                    if (dbValue.Property.PropertyType.IsSubclassOf(typeof(DBObject)))
                    {
                        if (!Collection.Session.Relationships.TryGetValue(dbValue.Property.PropertyType, out DBRelationship relationship))
                        {
                            lock (Collection.Session.Relationships)
                                if (!Collection.Session.Relationships.TryGetValue(dbValue.Property.PropertyType, out relationship))
                                    Collection.Session.Relationships[dbValue.Property.PropertyType] = relationship = new DBRelationship(dbValue.Property.PropertyType);
                        }

                        if (!relationship.LinkTargets.TryGetValue((int)value, out DBRelationshipTargets targets))
                        {
                            lock (relationship)
                                if (!relationship.LinkTargets.TryGetValue((int)value, out targets))
                                    relationship.LinkTargets[(int)value] = targets = new DBRelationshipTargets();
                        }

                        if (!targets.PropertyTargets.TryGetValue(dbValue.Property, out ConcurrentQueue<DBObject> list))
                        {
                            lock (targets)
                                if (!targets.PropertyTargets.TryGetValue(dbValue.Property, out list))
                                    targets.PropertyTargets[dbValue.Property] = list = new ConcurrentQueue<DBObject>();
                        }

                        list.Enqueue(this);


                        continue;
                    }

                    if (dbValue.PropertyType.IsEnum)
                    {
                        if (dbValue.PropertyType.GetEnumUnderlyingType() == dbValue.Property.PropertyType)
                        {
                            dbValue.Property.SetValue(this, value);
                            continue;
                        }
                    }
                    else if (dbValue.PropertyType == dbValue.Property.PropertyType)
                    {
                        dbValue.Property.SetValue(this, value);
                        continue;
                    }

                    try
                    {
                        dbValue.Property.SetValue(this, Convert.ChangeType(value, dbValue.PropertyType));
                    }
                    catch { }
                }
            }
        }
        internal void Save()
        {
            if (IsTemporary) return;

            //Disposing of Streams might be causing GC Collect on thread.

            if (SaveMemoryStream == null)
                SaveMemoryStream = new MemoryStream();

            if (SaveBinaryWriter == null)
                SaveBinaryWriter = new BinaryWriter(SaveMemoryStream);

            SaveMemoryStream.SetLength(0);

            foreach (DBValue dbValue in Collection.Mapping.Properties)
            {
                if (dbValue.Property.PropertyType.IsSubclassOf(typeof(DBObject)))
                {
                    DBObject linkOb = (DBObject)dbValue.Property.GetValue(this);
                    dbValue.WriteValue(linkOb?.Index ?? 0, SaveBinaryWriter);
                }
                else
                    dbValue.WriteValue(dbValue.Property.GetValue(this), SaveBinaryWriter);
            }

            RawData = SaveMemoryStream.ToArray();

            OnSaved();
        }

        public void Delete()
        {
            if (Collection.ReadOnly) return;

            PropertyChanged = null;

            Collection.Session.Delete(this);
        }

        public void FastDelete()
        {
            if (Collection.ReadOnly) return;

            Collection.Session.FastDelete(this);
        }


        protected internal virtual void OnCreated()
        {
            IsModified = true;
            IsLoaded = true;
        }
        protected internal virtual void OnLoaded()
        {
            IsLoaded = true;
        }
        protected virtual void OnSaved()
        {
            IsModified = false;
        }
        protected internal virtual void OnDeleted()
        {
            IsDeleted = true;
        }

        public void CreateBindings()
        {
            PropertyInfo[] properties = ThisType.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (!property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(DBBindingList<>)) continue;

                property.SetValue(this, Activator.CreateInstance(property.PropertyType, this, property));
            }
        }

        private void CreateLink(object ob, PropertyInfo info)
        {
            if (ob == null) return;

            AssociationAttribute link = info.GetCustomAttribute<AssociationAttribute>();

            if (link == null) return;

            PropertyInfo[] properties = ob.GetType().GetProperties();

            PropertyInfo best = null;

            foreach (PropertyInfo p in properties)
            {
                AssociationAttribute obLink = p.GetCustomAttribute<AssociationAttribute>();

                if (obLink == null || obLink.Identity != link.Identity) continue;

                if (p.PropertyType == info.DeclaringType)
                {
                    best = p;

                    if (p == info) continue;
                    break;
                }

                if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DBBindingList<>) &&
                    p.PropertyType.GetGenericArguments()[0] == info.DeclaringType) //Basically this type
                {
                    best = p;

                    if (p == info) continue;
                    break;
                }
            }

            if (best != null)
            {
                if (best.PropertyType == info.DeclaringType)
                {
                    best.SetValue(ob, this);
                    return;
                }

                if (best.PropertyType.IsGenericType && best.PropertyType.GetGenericTypeDefinition() == typeof(DBBindingList<>) &&
                    best.PropertyType.GetGenericArguments()[0] == info.DeclaringType) //Basically this type
                {
                    ((IBindingList)best.GetValue(ob)).Add(this);
                    return;
                }
            }


            throw new ArgumentException($"Unable to find Association {ThisType.Name}, Link: {link.Identity ?? "Empty"} -> {info.PropertyType.Name}");
        }
        private void RemoveLink(object ob, PropertyInfo info)
        {
            if (ob == null) return;

            AssociationAttribute link = info.GetCustomAttribute<AssociationAttribute>();

            if (link == null) return;

            PropertyInfo[] properties = ob.GetType().GetProperties();

            PropertyInfo best = null;

            foreach (PropertyInfo p in properties)
            {
                AssociationAttribute obLink = p.GetCustomAttribute<AssociationAttribute>();

                if (obLink == null || obLink.Identity != link.Identity) continue;

                if (p.PropertyType == info.DeclaringType)
                {
                    best = p;

                    if (p == info) continue;
                    break;
                }

                if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DBBindingList<>) &&
                    p.PropertyType.GetGenericArguments()[0] == info.DeclaringType) //Basically this type
                {
                    best = p;

                    if (p == info) continue;
                    break;
                }
            }

            if (best != null)
            {
                if (best.PropertyType == info.DeclaringType)
                {
                    best.SetValue(ob, null);
                    return;
                }

                if (best.PropertyType.IsGenericType && best.PropertyType.GetGenericTypeDefinition() == typeof(DBBindingList<>) &&
                    best.PropertyType.GetGenericArguments()[0] == info.DeclaringType) //Basically this type
                {
                    ((IBindingList)best.GetValue(ob)).Remove(this);
                    return;
                }
            }


            throw new ArgumentException($"Unable to find Association {ThisType.Name}, Link: {link.Identity ?? "Empty"} -> {info.PropertyType.Name}");
        }

        protected Session GetCurrencySession()
        {
            return Collection.Session;
        }

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected internal virtual void OnChanged(object oldValue, object newValue, string propertyName)
        {
            if (Collection.Session.Relationships == null)
                IsModified = true;

            if (oldValue is DBObject || newValue is DBObject)
            {
                PropertyInfo info = ThisType.GetProperty(propertyName);

                RemoveLink(oldValue, info);
                CreateLink(newValue, info);
            }

            if (IsLoaded && Collection.RaisePropertyChanges)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
