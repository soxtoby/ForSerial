using System;
using System.Collections;
using System.Collections.Generic;
using json.Objects.TypeDefinitions;

namespace json.Objects
{
    internal class TypedObjectTypedArray : SequenceOutputBase, TypedSequence
    {
        private readonly CollectionDefinition collectionDef;
        private readonly IEnumerable typedArray;
        private readonly Stack<Output> outputs = new Stack<Output>();

        public TypedObjectTypedArray(CollectionDefinition collectionDefinition)
        {
            this.collectionDef = collectionDefinition;
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

        public override OutputStructure AsStructure()
        {
            return new TypedObjectOutputStructure(typedArray);
        }

        public void AddItem(object item)
        {
            collectionDef.AddToCollection(typedArray, item);
        }

        public override Output CreateValue(Writer valueFactory, object value)
        {
            return collectionDef.ItemTypeDef.CreateValue(value);
        }

        public override OutputStructure BeginStructure(Writer valueFactory)
        {
            TypedObjectOutputStructure obj = new TypedObjectOutputStructure(collectionDef.ItemTypeDef);
            outputs.Push(obj);
            return obj;
        }

        public override SequenceOutput BeginSequence(Writer valueFactory)
        {
            TypedSequence array = collectionDef.ItemTypeDef.CreateSequence();
            outputs.Push(array);
            return array;
        }

        public override void EndStructure(Writer writer)
        {
            outputs.Pop();
        }

        public override void EndSequence(Writer writer)
        {
            outputs.Pop();
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

        public override void AddToStructure(OutputStructure structure, string name)
        {
            ((TypedObjectOutputStructure)structure).AddProperty(name, this);
        }

        public override void AddToSequence(SequenceOutput sequence)
        {
            ((TypedSequence)sequence).AddItem(typedArray);
        }
    }
}