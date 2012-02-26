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

        public void SetCurrentProperty(string name) { } // TODO throw exception - can't have properties in a sequence

        public ObjectContainer CreateStructure()
        {
            return collectionDef.CreateStructureForItem();
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
    }
}