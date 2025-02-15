using MirDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server
{
    public class DBObjectArrayConverter<T> : DBObjectBaseConverter<T[]> where T : DBObject, new()
    {
        public DBObjectArrayConverter(Session session) : base(session) { }

        public override T[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            List<T> collection = new();

            var converterOptions = CreateDBObjectOptions(typeof(T), null, null);

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                    {
                        break;
                    }

                    if (reader.TokenType == JsonTokenType.StartObject)
                    {
                        T obj = JsonSerializer.Deserialize<T>(ref reader, converterOptions);

                        collection.Add(obj);
                    }
                }
            }

            return collection.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, T[] value, JsonSerializerOptions options)
        {
            var converterOptions = CreateDBObjectOptions(typeof(T), null, null);

            writer.WriteStartArray();

            foreach (var obj in value)
            {
                JsonSerializer.Serialize<T>(writer, obj, converterOptions);
            }

            writer.WriteEndArray();
        }
    }

    public class DBObjectConverter<T> : DBObjectBaseConverter<T> where T : DBObject, new()
    {
        private readonly DBBindingList<T> _bindingList;
        private readonly PropertyInfo _parentProperty;

        public DBObjectConverter(Session session, PropertyInfo parentProperty = null, DBBindingList<T> bindingList = null) : base(session)
        {
            Session = session;
            _bindingList = bindingList;
            _parentProperty = parentProperty;
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            T obj = FindObjectOrCreate(ref reader);

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("Expected PropertyName token.");
                }

                string propertyName = reader.GetString();

                var property = Array.Find(properties, p => p.Name == propertyName);

                if (property == null)
                {
                    reader.Skip();
                    continue;
                }

                // read value
                reader.Read();

                ReadPropertyValue(ref reader, obj, property);
            }

            return obj;
        }

        private void ReadPropertyValue(ref Utf8JsonReader reader, T obj, PropertyInfo property)
        {
            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(DBBindingList<>))
            {
                if (reader.TokenType != JsonTokenType.StartArray)
                {
                    throw new JsonException($"Expected StartArray token for DBBindingList at '{property.Name}'.");
                }

                Type bindingListType = property.PropertyType.GetGenericArguments().First();

                List<object> collection = new();

                var converterOptions = CreateDBObjectOptions(bindingListType, property, property.GetValue(obj));

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                    {
                        break;
                    }

                    var childObject = JsonSerializer.Deserialize(ref reader, bindingListType, converterOptions);

                    SetAssociationProperty(property, obj, childObject);

                    collection.Add(childObject);
                }

                RemoveMissingObjects(property.GetValue(obj) as IList, collection);
            }
            else if (typeof(DBObject).IsAssignableFrom(property.PropertyType))
            {
                var converterOptions = CreateDBObjectReferenceOptions(property.PropertyType);

                var childObject = JsonSerializer.Deserialize(ref reader, property.PropertyType, converterOptions);

                property.SetValue(obj, childObject);
            }
            else if (property.PropertyType.IsEnum)
            {
                var value = (Enum)Enum.Parse(property.PropertyType, reader.GetString());

                property.SetValue(obj, value);
            }
            else
            {
                var converterOptions = CreateOptions();
                var value = JsonSerializer.Deserialize(ref reader, property.PropertyType, converterOptions);

                property.SetValue(obj, value);
            }
        }

        private void SetAssociationProperty(PropertyInfo parentProperty, T parentObject, object childObject)
        {
            if (!parentProperty.PropertyType.IsGenericType || parentProperty.PropertyType.GetGenericTypeDefinition() != typeof(DBBindingList<>))
            {
                return;
            }

            Type bindingListType = parentProperty.PropertyType.GetGenericArguments().First();

            var childProperty = bindingListType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => p.PropertyType == typeof(T) && IsAssociationProperty(p, parentProperty));

            childProperty?.SetValue(childObject, parentObject);
        }

        private static void RemoveMissingObjects(IList existingCollection, IList importedObjects)
        {
            for (int j = existingCollection.Count - 1; j >= 0; j--)
            {
                var obj = (DBObject)existingCollection[j];

                if (!importedObjects.Contains(obj))
                {
                    existingCollection.Remove(obj);
                    obj.Delete();
                }
            }
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();

            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                if (HasIgnoreAttribute(property) || IsAssociationProperty(property, _parentProperty))
                {
                    continue;
                }

                writer.WritePropertyName(property.Name);

                WritePropertyValue(ref writer, property, property.GetValue(value));
            }

            writer.WriteEndObject();
        }

        private void WritePropertyValue(ref Utf8JsonWriter writer, PropertyInfo property, object value)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(DBBindingList<>))
            {
                Type bindingListType = property.PropertyType.GetGenericArguments().First();

                var converterOptions = CreateDBObjectOptions(bindingListType, property, null);

                writer.WriteStartArray();

                foreach (var obj in (IList)value)
                {
                    JsonSerializer.Serialize(writer, obj, converterOptions);
                }

                writer.WriteEndArray();
            }
            else if (typeof(DBObject).IsAssignableFrom(property.PropertyType))
            {
                var converterOptions = CreateDBObjectReferenceOptions(property.PropertyType);

                JsonSerializer.Serialize(writer, value, property.PropertyType, converterOptions);
            }
            else if (property.PropertyType.IsEnum)
            {
                writer.WriteStringValue(value.ToString());
            }
            else
            {
                var converterOptions = CreateOptions();
                JsonSerializer.Serialize(writer, value, property.PropertyType, converterOptions);
            }
        }

        private T FindObjectOrCreate(ref Utf8JsonReader reader)
        {
            var readerAtStart = reader;

            var identityProperties = GetIdentityProperties(typeof(T)).ToArray();

            var identities = new List<string>();

            int i = 0;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("Expected PropertyName token.");
                }

                string propertyName = reader.GetString();

                var property = Array.Find(identityProperties, p => p.Name == propertyName);

                if (property == null)
                {
                    reader.Skip();
                    continue;
                }

                // read value
                reader.Read();

                object value;

                if (typeof(DBObject).IsAssignableFrom(property.PropertyType))
                {
                    value = reader.GetString();
                }
                else if (property.PropertyType.IsEnum)
                {
                    value = (Enum)Enum.Parse(property.PropertyType, reader.GetString());
                }
                else
                {
                    var converterOptions = CreateOptions();
                    value = JsonSerializer.Deserialize(ref reader, property.PropertyType, converterOptions);
                }

                identities.AddRange(SplitIdentityValue(value?.ToString()));

                if (++i >= identityProperties.Length)
                {
                    break;
                }
            }

            reader = readerAtStart;

            var obj = FindObject(identities, _parentProperty);

            if (obj == null)
            {
                obj = Session.GetCollection<T>().CreateNewObject();
            }

            return obj;
        }

        private T FindObject(List<string> identityValue, PropertyInfo parentProperty)
        {
            if (_bindingList != null)
            {
                return FindObjectFromCollection(_bindingList, identityValue, parentProperty);
            }
            else
            {
                return FindObjectFromCollection(Session.GetCollection<T>().Binding, identityValue, parentProperty);
            }
        }

        private T FindObjectFromCollection(IList<T> collection, List<string> identityValues, PropertyInfo parentProperty)
        {
            var identityProperties = GetIdentityProperties(typeof(T));

            return collection.FirstOrDefault(entity =>
            {
                var identities = new List<string>();

                FindAllIdentities(parentProperty, typeof(T), identityProperties, entity, identities);
         
                return identityValues.SequenceEqual(identities);
            });
        }
    }

    public class DBObjectReferenceConverter<T> : DBObjectBaseConverter<T> where T : DBObject, new()
    {
        public DBObjectReferenceConverter(Session session) : base(session) { }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            var identityValue = reader.GetString();

            if (identityValue == null)
            {
                return null;
            }

            return GetObjectFromIdentity(SplitIdentityValue(identityValue).ToList());
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var identities = new List<string>();

            FindAllIdentities(null, typeof(T), GetIdentityProperties(typeof(T)), value, identities);

            writer.WriteStringValue(JoinIdentityValues(identities.ToArray()));
        }

        private T GetObjectFromIdentity(List<string> identityValues)
        {
            var identityProperties = GetIdentityProperties(typeof(T));

            var obj = Session.GetCollection<T>().Binding.FirstOrDefault(item =>
            {
                var identities = new List<string>();

                FindAllIdentities(null, typeof(T), identityProperties, item, identities);

                return identities.SequenceEqual(identityValues);
            });

            if (obj == null)
            {
                throw new JsonException($"Object not found for '{typeof(T).Name}' using values '{JoinIdentityValues(identityValues.ToArray())}'.");
            }

            return obj;
        }
    }

    public abstract class DBObjectBaseConverter<T> : JsonConverter<T>
    {
        protected Session Session { get; set; }

        private static readonly Dictionary<Type, JsonConverter> _dbObjectReferenceConverterCache = new();
        private static readonly Dictionary<Type, JsonConverter> _dbObjectConverterCache = new();

        public DBObjectBaseConverter(Session session)
        {
            Session = session;
        }

        protected void FindAllIdentities(PropertyInfo parentProperty, Type type, IEnumerable<PropertyInfo> identityProperties, object obj, List<string> identities)
        {
            foreach (var property in identityProperties)
            {
                if (property.PropertyType == type)
                {
                    continue; //prevent recursive loop
                }

                if (typeof(DBObject).IsAssignableFrom(property.PropertyType))
                {
                    if (IsAssociationProperty(property, parentProperty))
                    {
                        continue;
                    }

                    var childObject = property.GetValue(obj);

                    if (childObject == null)
                    {
                        // property hasn't been set so we can't compare against it
                        continue; 
                    }

                    FindAllIdentities(property, property.PropertyType, GetIdentityProperties(property.PropertyType), childObject, identities);
                }
                else
                {
                    var identity = property.GetValue(obj);

                    identities.Add(identity.ToString());
                }
            }
        }

        protected IEnumerable<PropertyInfo> GetIdentityProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(prop => Attribute.IsDefined(prop, typeof(IsIdentityAttribute), false));
        }

        protected bool HasIgnoreAttribute(PropertyInfo property)
        {
            return property.GetCustomAttribute<JsonIgnoreAttribute>() != null;
        }

        protected JsonSerializerOptions CreateOptions()
        {
            var converterOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            AddDefaultConverters(converterOptions);

            return converterOptions;
        }

        protected JsonSerializerOptions CreateDBObjectOptions(Type typeToConvert, PropertyInfo parent, object bindingList)
        {
            JsonConverter converter = null;

            if (bindingList == null && parent == null)
            {
                if (_dbObjectConverterCache.TryGetValue(typeToConvert, out JsonConverter value))
                {
                    converter = value;
                }
            }
            
            if (converter == null)
            {
                Type converterType = typeof(DBObjectConverter<>).MakeGenericType(typeToConvert);
                converter = (JsonConverter)Activator.CreateInstance(converterType, Session, parent, bindingList);

                if (bindingList == null && parent == null)
                {
                    _dbObjectConverterCache.Add(typeToConvert, converter);
                }
            }

            var converterOptions = new JsonSerializerOptions
            {
                Converters = { converter },
                PropertyNameCaseInsensitive = true
            };

            AddDefaultConverters(converterOptions);

            return converterOptions;
        }

        protected JsonSerializerOptions CreateDBObjectReferenceOptions(Type typeToConvert)
        {
            JsonConverter converter;

            if (_dbObjectReferenceConverterCache.TryGetValue(typeToConvert, out JsonConverter value))
            {
                converter = value;
            }
            else
            {
                Type converterType = typeof(DBObjectReferenceConverter<>).MakeGenericType(typeToConvert);
                converter = (JsonConverter)Activator.CreateInstance(converterType, Session);

                _dbObjectReferenceConverterCache.Add(typeToConvert, converter);
            }

            var converterOptions = new JsonSerializerOptions
            {
                Converters = { converter },
                PropertyNameCaseInsensitive = true
            };

            AddDefaultConverters(converterOptions);

            return converterOptions;
        }

        private static void AddDefaultConverters(JsonSerializerOptions options)
        {
            options.Converters.Add(new JsonStringEnumConverter());
        }

        protected bool IsAssociationProperty(PropertyInfo childProperty, PropertyInfo parentProperty)
        {
            if (childProperty == null || parentProperty == null)
            {
                return false;
            }

            var childAssociationValue = GetAssociationValueFromProperty(childProperty);
            var parentAssociationValue = GetAssociationValueFromProperty(parentProperty);

            if (parentProperty.PropertyType.IsGenericType && parentProperty.PropertyType.GetGenericTypeDefinition() == typeof(DBBindingList<>))
            {
                Type parentBindingListType = parentProperty.PropertyType.GetGenericArguments().First();

                if (parentAssociationValue == childAssociationValue && parentProperty.DeclaringType == childProperty.PropertyType && parentBindingListType == childProperty.DeclaringType)
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetAssociationValueFromProperty(PropertyInfo property)
        {
            if (property == null)
            {
                return null;
            }

            AssociationAttribute associationAttribute = property.GetCustomAttribute<AssociationAttribute>();

            if (associationAttribute == null)
            {
                return null;
            }

            return associationAttribute.Identity;
        }

        protected static string[] SplitIdentityValue(string value)
        {
            return value.Split('/');
        }

        protected string JoinIdentityValues(string[] values)
        {
            return string.Join('/', values);
        }
    }
}