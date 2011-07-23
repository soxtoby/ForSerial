using System;
using System.Collections;

namespace json.Objects
{
    public partial class TypedObjectBuilder
    {
        private class TypedObjectTypedArray : ParseArrayBase, TypedObjectArray
        {
            private readonly CollectionDefinition collectionDef;
            public IEnumerable Array { get; private set; }

            public TypedObjectTypedArray(Type collectionType)
            {
                collectionDef = PopulateCollectionDefinition(collectionType);
                Array = (IEnumerable)Activator.CreateInstance(collectionType);
            }

            private static CollectionDefinition PopulateCollectionDefinition(Type collectionType)
            {
                CollectionDefinition collectionDef = TypeDefinition.GetTypeDefinition(collectionType) as CollectionDefinition;
                if (collectionDef == null)
                    throw new InvalidCollectionType(collectionType);
                return collectionDef;
            }

            public IEnumerable GetTypedArray(Type type)
            {
                return Array;
            }

            public override ParseObject AsObject()
            {
                return new TypedObjectObject(Array);
            }

            public override void AddNull()
            {
                AddItem(null);
            }

            public override void AddBoolean(bool value)
            {
                AddItem(value);
            }

            public override void AddNumber(double value)
            {
                AddItem(value);
            }

            public override void AddString(string value)
            {
                AddItem(value);
            }

            public override void AddObject(ParseObject value)
            {
                TypedObjectObject obj = GetObjectAsTypedObjectObject(value);
                AddItem(obj.Object);
            }

            public override void AddArray(ParseArray value)
            {
                TypedObjectArray array = GetArrayAsTypedObjectArray(value);
                AddItem(array.GetTypedArray(collectionDef.ItemTypeDef.Type));
            }

            private void AddItem(object item)
            {
                collectionDef.AddToCollection(Array, item);
            }

            public override ParseObject CreateObject(ParseValueFactory valueFactory)
            {
                return new TypedObjectObject(collectionDef.ItemTypeDef);
            }

            public override ParseArray CreateArray(ParseValueFactory valueFactory)
            {
                return new TypedObjectTypedArray(collectionDef.ItemTypeDef.Type);
            }
        }

        private class InvalidCollectionType : Exception
        {
            public InvalidCollectionType(Type type) : base("Cannot create collection of type {0}.".FormatWith(type.FullName)) { }
        }
    }
}
