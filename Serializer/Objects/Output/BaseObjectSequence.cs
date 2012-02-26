using System;
using System.Collections.Generic;
using json.Objects.TypeDefinitions;

namespace json.Objects
{
    public abstract class BaseObjectSequence : ObjectSequence
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

        public ObjectStructure CreateStructure()
        {
            return collectionDef.CreateStructureForItem();
        }

        public ObjectSequence CreateSequence()
        {
            return collectionDef.CreateSequenceForItem();
        }

        public bool CanCreateValue(object value)
        {
            return collectionDef.CanCreateValueForItem(value);
        }

        public ObjectValue CreateValue(object value)
        {
            return collectionDef.CreateValueForItem(value);
        }

        public void Add(ObjectOutput value)
        {
            Items.Add(value);
        }
    }
}