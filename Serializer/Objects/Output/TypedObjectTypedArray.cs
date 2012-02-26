using System;
using System.Collections;
using json.Objects.TypeDefinitions;

namespace json.Objects
{
    internal class TypedObjectTypedArray : ObjectSequence
    {
        private readonly CollectionDefinition collectionDef;
        private readonly IEnumerable typedArray;

        public TypedObjectTypedArray(CollectionDefinition collectionDefinition)
        {
            collectionDef = collectionDefinition;
            typedArray = (IEnumerable)Activator.CreateInstance(collectionDefinition.Type);
        }

        private void PopulateCollection(object collection)
        {
            PopulateCollection(collectionDef, typedArray, () => collection);
        }

        public void AssignToProperty(object obj, PropertyDefinition property)
        {
            if (property.CanSet)
                property.SetOn(obj, GetTypedValue());
            else if (property.CanGet)
                PopulateCollection(property.GetFrom(obj));
        }

        public object GetTypedValue()
        {
            return typedArray;
        }

        public TypeDefinition TypeDef { get { throw new NotImplementedException(); } }

        public void Add(TypedValue value)
        {
            collectionDef.AddToCollection(typedArray, value.GetTypedValue());
        }

        public bool CanCreateValue(object value)
        {
            throw new NotImplementedException();
        }

        public ObjectValue CreateValue(object value)
        {
            return collectionDef.ItemTypeDef.CreateValue(value);
        }

        public ObjectStructure CreateStructure()
        {
            return collectionDef.ItemTypeDef.CreateStructure();
        }

        public ObjectSequence CreateSequence()
        {
            return collectionDef.ItemTypeDef.CreateSequence();
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
    }
}