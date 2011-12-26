using System;
using System.Collections;

namespace json.Objects
{
    internal class TypedObjectTypedArray : ParseArrayBase, TypedObjectArray
    {
        private readonly JsonArrayDefinition jsonArrayDef;
        private readonly IEnumerable typedArray;

        public TypedObjectTypedArray(Type collectionType)
        {
            jsonArrayDef = GetCollectionDefinition(collectionType);
            typedArray = (IEnumerable)Activator.CreateInstance(collectionType);
        }

        private static JsonArrayDefinition GetCollectionDefinition(Type collectionType)
        {
            JsonArrayDefinition jsonArrayDef = CurrentTypeHandler.GetTypeDefinition(collectionType) as CollectionDefinition;
            if (jsonArrayDef == null)
                throw new InvalidCollectionType(collectionType);
            return jsonArrayDef;
        }

        public IEnumerable GetTypedArray()
        {
            return typedArray;
        }

        public void PopulateCollection(object collection)
        {
            PopulateCollection(jsonArrayDef, typedArray, () => collection);
        }

        public override ParseObject AsObject()
        {
            return new TypedObjectObject(typedArray);
        }

        public void AddItem(object item)
        {
            jsonArrayDef.AddToCollection(typedArray, item);
        }

        public override ParseValue CreateValue(ParseValueFactory valueFactory, object value)
        {
            return jsonArrayDef.ItemTypeDef.CreateValue(valueFactory, value);
        }

        public override ParseObject CreateObject(ParseValueFactory valueFactory)
        {
            return new TypedObjectObject(jsonArrayDef.ItemTypeDef);
        }

        public override ParseArray CreateArray(ParseValueFactory valueFactory)
        {
            return new TypedObjectTypedArray(jsonArrayDef.ItemTypeDef.Type);
        }

        private static IEnumerable PopulateCollection(TypeDefinition collectionType, IEnumerable items, Func<object> getCollection)
        {
            JsonArrayDefinition jsonArrayDef = collectionType as CollectionDefinition;
            if (jsonArrayDef == null) return null;

            IEnumerable collection = getCollection() as IEnumerable;

            if (collection != null)
            {
                foreach (object item in items)
                {
                    object itemToAdd = TypeInnerCollection(jsonArrayDef.ItemTypeDef, item);
                    jsonArrayDef.AddToCollection(collection, itemToAdd);
                }
            }

            return collection;
        }

        private static object TypeInnerCollection(TypeDefinition itemTypeDef, object item)
        {
            return itemTypeDef is CollectionDefinition
                ? PopulateCollection(itemTypeDef, (IEnumerable)item, () => Activator.CreateInstance(itemTypeDef.Type))
                : item;
        }

        private class InvalidCollectionType : Exception
        {
            public InvalidCollectionType(Type type) : base("Cannot create collection of type {0}.".FormatWith(type.FullName)) { }
        }

        public override void AddToObject(ParseObject obj, string name)
        {
            ((TypedObjectObject)obj).AddArray(name, this);
        }

        public override void AddToArray(ParseArray array)
        {
            ((TypedObjectArray)array).AddItem(typedArray);
        }
    }
}