using System;
using System.Collections;

namespace json.Objects
{
    public partial class TypedObjectBuilder : ParseValueFactory
    {
        private Type baseType;

        private TypedObjectBuilder() { }

        public TypedObjectBuilder(Type baseType)
        {
            this.baseType = baseType;
        }

        private static TypedObjectBuilder instance;
        public static TypedObjectBuilder GenericInstance
        {
            get { return instance ?? (instance = new TypedObjectBuilder()); }
        }

        public static T GetResult<T>(ParseValue value)
        {
            TypedObjectArray array = value as TypedObjectArray;
            if (array != null)
                return (T)array.GetTypedArray(typeof(T));

            TypedObjectObject obj = value.AsObject() as TypedObjectObject;
            if (obj == null)
                throw new InvalidResultObject();

            return (T)obj.Object;
        }

        public virtual ParseObject CreateObject()
        {
            if (baseType == null)
                return new TypedObjectObject();

            TypeDefinition typeDef = TypeDefinition.GetTypeDefinition(baseType);
            if (!typeDef.IsDeserializable)
                return new TypedObjectObject();

            TypedObjectObject obj = new TypedObjectObject(typeDef);
            baseType = null;    // Only needed for first object
            return obj;
        }

        public ParseArray CreateArray()
        {
            if (baseType == null)
                throw new UnknownRootArrayType();

            var array = new TypedObjectTypedArray(baseType);
            baseType = null;    // Only needed for first object
            return array;
        }

        public ParseNumber CreateNumber(double value)
        {
            return new TypedObjectNumber(value);
        }

        public ParseString CreateString(string value)
        {
            return new TypedObjectString(value);
        }

        public ParseBoolean CreateBoolean(bool value)
        {
            return value ? TypedObjectBoolean.True : TypedObjectBoolean.False;
        }

        public ParseNull CreateNull()
        {
            return TypedObjectNull.Value;
        }

        public ParseObject CreateReference(ParseObject parseObject)
        {
            return parseObject;
        }

        private static TypedObjectObject GetObjectAsTypedObjectObject(ParseObject value)
        {
            TypedObjectObject objectValue = value as TypedObjectObject;

            if (objectValue == null)
                throw new UnsupportedParseObject();

            return objectValue;
        }

        private static TypedObjectArray GetArrayAsTypedObjectArray(ParseArray value)
        {
            TypedObjectArray arrayValue = value as TypedObjectArray;

            if (arrayValue == null)
                throw new UnsupportedParseArray();

            return arrayValue;
        }

        private static IEnumerable PopulateCollection(Type collectionType, IEnumerable items, Func<object> getCollection)
        {
            CollectionDefinition collectionDef = CollectionDefinition.GetCollectionDefinition(collectionType);

            if (collectionDef.IsCollection)
            {
                IEnumerable collection = getCollection() as IEnumerable;

                if (collection != null)
                {
                    foreach (object item in items)
                    {
                        object itemToAdd = TypeInnerCollection(collectionDef.ItemTypeDef.Type, item);
                        collectionDef.AddToCollection(collection, itemToAdd);
                    }
                }

                return collection;
            }

            return null;
        }

        private static object TypeInnerCollection(Type itemType, object item)
        {
            CollectionDefinition collectionDef = CollectionDefinition.GetCollectionDefinition(itemType);
            return collectionDef.IsCollection
                ? PopulateCollection(itemType, (IEnumerable)item, () => Activator.CreateInstance(itemType))
                : item;
        }

        internal class UnsupportedParseObject : Exception
        {
            public UnsupportedParseObject() : base("Can only add ParseObjects that created by a TypedObjectBuilder.") { }
        }

        internal class UnsupportedParseArray : Exception
        {
            public UnsupportedParseArray() : base("Can only add ParseArrays of type TypedObjectArray.") { }
        }

        internal class InvalidResultObject : Exception
        {
            public InvalidResultObject() : base("Invalid ParseObject type. Object must be constructed using a TypedObjectBuilder.") { }
        }

        internal class UnknownRootArrayType : Exception
        {
            public UnknownRootArrayType() : base("Can't create array without a known type.") { }
        }
    }
}

