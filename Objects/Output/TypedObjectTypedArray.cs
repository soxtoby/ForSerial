using System;
using System.Collections;

namespace json.Objects
{
    internal class TypedObjectTypedArray : ParseArrayBase, TypedObjectArray
    {
        private readonly CollectionDefinition collectionDef;
        private readonly IEnumerable typedArray;

        public TypedObjectTypedArray(Type collectionType)
        {
            collectionDef = GetCollectionDefinition(collectionType);
            typedArray = (IEnumerable)Activator.CreateInstance(collectionType);
        }

        private static CollectionDefinition GetCollectionDefinition(Type collectionType)
        {
            CollectionDefinition collectionDef = TypeDefinition.GetTypeDefinition(collectionType) as CollectionDefinition;
            if (collectionDef == null)
                throw new InvalidCollectionType(collectionType);
            return collectionDef;
        }

        public IEnumerable GetTypedArray()
        {
            return typedArray;
        }

        public void PopulateCollection(object collection)
        {
            PopulateCollection(collectionDef, typedArray, () => collection);
        }

        public override ParseObject AsObject()
        {
            return new TypedObjectObject(typedArray);
        }

        public void AddItem(object item)
        {
            collectionDef.AddToCollection(typedArray, item);
        }

        public override ParseValue CreateValue(ParseValueFactory valueFactory, object value)
        {
            return collectionDef.ItemTypeDef.CreateValue(valueFactory, value);
        }

        public override ParseObject CreateObject(ParseValueFactory valueFactory)
        {
            return new TypedObjectObject(collectionDef.ItemTypeDef);
        }

        public override ParseArray CreateArray(ParseValueFactory valueFactory)
        {
            return new TypedObjectTypedArray(collectionDef.ItemTypeDef.Type);
        }

        private static IEnumerable PopulateCollection(TypeDefinition collectionType, IEnumerable items, Func<object> getCollection)
        {
            CollectionDefinition collectionDef = collectionType as CollectionDefinition;
            if (collectionDef == null) return null;

            IEnumerable collection = getCollection() as IEnumerable;

            if (collection != null)
            {
                foreach (object item in items)
                {
                    object itemToAdd = TypeInnerCollection(collectionDef.ItemTypeDef, item);
                    collectionDef.AddToCollection(collection, itemToAdd);
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