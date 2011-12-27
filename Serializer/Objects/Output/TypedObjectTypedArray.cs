using System;
using System.Collections;

namespace json.Objects
{
    internal class TypedObjectTypedArray : ParseArrayBase, TypedObjectArray
    {
        private readonly CollectionDefinition collectionDef;
        private readonly IEnumerable typedArray;

        public TypedObjectTypedArray(CollectionDefinition collectionDefinition)
        {
            this.collectionDef = collectionDefinition;
            typedArray = (IEnumerable)Activator.CreateInstance(collectionDefinition.Type);
        }

        public IEnumerable GetTypedArray()
        {
            return typedArray;
        }

        private void PopulateCollection(object collection)
        {
            PopulateCollection(collectionDef, typedArray, () => collection);
        }

        public void AssignToProperty(object obj, PropertyDefinition property)
        {
            if (property.CanSet)
                property.SetOn(obj, GetTypedArray());
            else if (property.CanGet)
                PopulateCollection(property.GetFrom(obj));
        }

        public object GetTypedValue()
        {
            return GetTypedArray();
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
            return collectionDef.ItemTypeDef.CreateArray();
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
            ((TypedObjectObject)obj).AddProperty(name, this);
        }

        public override void AddToArray(ParseArray array)
        {
            ((TypedObjectArray)array).AddItem(typedArray);
        }
    }
}