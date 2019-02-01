using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace MirDB
{
    public abstract class DBObject : INotifyPropertyChanged
    {
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
        

        internal Dictionary<PropertyInfo, int> ForeignKeys;
        internal readonly Type ThisType;

        [IgnoreProperty]
        protected internal bool IsLoaded { get; private set; }

        [IgnoreProperty]
        protected internal bool IsDeleted { get; private set; }
        [IgnoreProperty]
        protected internal bool IsModified { get; private set; }
        [IgnoreProperty]
        public bool IsTemporary { get; set; }

        protected internal byte[] RawData;

        protected internal void UseKeys()
        {
            foreach (KeyValuePair<PropertyInfo, int> pair in ForeignKeys)
            {
                if (pair.Key.GetValue(this) != null) continue;

                DBObject ob = Collection.Session.GetObject(pair.Key.PropertyType, pair.Value);

                if (ob == null) continue;

                pair.Key.SetValue(this, ob);
            }

            ForeignKeys = null;
        }

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
                        if (ForeignKeys == null)
                            ForeignKeys = new Dictionary<PropertyInfo, int>();

                        ForeignKeys[dbValue.Property] = (int)value;
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
        internal void Save(DBMapping mapping)
        {
            using (MemoryStream mStream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(mStream))
            {
                foreach (DBValue dbValue in mapping.Properties)
                {
                    if (dbValue.Property.PropertyType.IsSubclassOf(typeof(DBObject)))
                    {
                        DBObject linkOb = (DBObject)dbValue.Property.GetValue(this);
                        dbValue.WriteValue(linkOb?.Index ?? 0, writer);
                    }
                    else
                        dbValue.WriteValue(dbValue.Property.GetValue(this), writer);
                }
                RawData = mStream.ToArray();
            }

            OnSaved();
        }

        public void Delete()
        {
            if (Collection.ReadOnly) return;

            Collection.Session.Delete(this);
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

            Association link = info.GetCustomAttribute<Association>();

            if (link == null) return;

            PropertyInfo[] properties = ob.GetType().GetProperties();
            
            foreach (PropertyInfo p in properties)
            {
                Association obLink = p.GetCustomAttribute<Association>();

                if (obLink == null || obLink.Identity != link.Identity || p == info) continue;

                if (p.PropertyType == info.DeclaringType)
                {
                    p.SetValue(ob, this);
                    return;
                }

                if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DBBindingList<>) &&
                    p.PropertyType.GetGenericArguments()[0] == info.DeclaringType) //Basically this type
                {
                    ((IBindingList)p.GetValue(ob)).Add(this);
                    return;
                }
            }
            
            throw new ArgumentException($"Unable to find Association {ThisType.Name}, Link: {link.Identity ?? "Empty"} -> {info.PropertyType.Name}");
        }
        private void RemoveLink(object ob, PropertyInfo info)
        {
            if (ob == null) return;

            Association link = info.GetCustomAttribute<Association>();

            if (link == null) return;

            PropertyInfo[] properties = ob.GetType().GetProperties();
            
            foreach (PropertyInfo p in properties)
            {
                Association obLink = p.GetCustomAttribute<Association>();

                if (obLink == null || obLink.Identity != link.Identity || p == info) continue;

                if (p.PropertyType == info.DeclaringType)
                {
                    p.SetValue(ob, null);
                    return;
                }

                if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DBBindingList<>) &&
                    p.PropertyType.GetGenericArguments()[0] == info.DeclaringType) //Basically this type
                {
                    ((IBindingList)p.GetValue(ob)).Remove(this);
                    return;
                }
            }
            

            throw new ArgumentException($"Unable to find Association {ThisType.Name}, Link: {link.Identity ?? "Empty"} -> {info.PropertyType.Name}");
        }

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected internal virtual void OnChanged(object oldValue, object newValue, string propertyName)
        {
            //if (!IsLoaded) return;

            if (Collection.Session.KeyedObjects == null) IsModified = true; //Keys have been consumed.

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
