using System;
using System.Collections.Generic;
using json.Objects.TypeDefinitions;

namespace json.Objects
{
    public abstract class BaseObjectSequence : ObjectContainer
    {
        private readonly SequenceDefinition collectionDef;
        protected readonly List<ObjectOutput> Items = new List<ObjectOutput>();

        protected BaseObjectSequence(SequenceDefinition collectionDef)
        {
            if (collectionDef == null) throw new ArgumentNullException("collectionDef");

            this.collectionDef = collectionDef;
        }

        public abstract void AssignToProperty(object obj, PropertyDefinition property);
        public abstract object GetTypedValue();

        public TypeDefinition TypeDef { get { return collectionDef; } }

        // TODO throw exception - can't have properties in a sequence
        public void SetCurrentProperty(string name) { }

        // TODO throw exception - can't change type of sequence (in JSON at least)

        public ObjectContainer CreateStructure()
        {
            return collectionDef.CreateStructureForItem();
        }

        public ObjectContainer CreateStructure(string typeIdentifier)
        {
            return CreateStructure();
        }

        public ObjectContainer CreateSequence()
        {
            return collectionDef.CreateSequenceForItem();
        }

        public bool CanCreateValue(object value)
        {
            return collectionDef.CanCreateValueForItem(value);
        }

        public void WriteValue(object value)
        {
            Add(collectionDef.CreateValueForItem(value));
        }

        public void Add(ObjectOutput value)
        {
            Items.Add(value);
        }

        public PreBuildInfo GetPreBuildInfo(Type readerType)
        {
            return TypeDef.GetPreBuildInfo(readerType);
        }

        protected object GetTypedValue(CollectionDefinition collectionDefinition)
        {
            object collection = collectionDefinition.ConstructNew();
            PopulateCollection(collectionDefinition, collection);
            return collection;
        }

        protected void PopulateCollection(CollectionDefinition collectionDefinition, object collection)
        {
            if (collection != null)
                foreach (ObjectOutput value in Items)
                    collectionDefinition.AddToCollection(collection, value.GetTypedValue());
        }
    }
}