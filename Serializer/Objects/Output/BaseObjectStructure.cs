using System;
using System.Collections.Generic;

namespace json.Objects
{
    public abstract class BaseObjectStructure : ObjectContainer
    {
        private string currentProperty;
        protected readonly Dictionary<string, ObjectOutput> Properties = new Dictionary<string, ObjectOutput>(StringComparer.OrdinalIgnoreCase);

        public TypeDefinition TypeDef { get; private set; }

        protected BaseObjectStructure(TypeDefinition typeDef)
        {
            TypeDef = typeDef;
        }

        public abstract void AssignToProperty(object obj, PropertyDefinition property);

        public void SetCurrentProperty(string name)
        {
            currentProperty = name;
        }

        public ObjectContainer CreateStructure()
        {
            return TypeDef.CreateStructureForProperty(currentProperty);
        }

        public ObjectContainer CreateSequence()
        {
            return TypeDef.CreateSequenceForProperty(currentProperty);
        }

        public bool CanCreateValue(object value)
        {
            return TypeDef.CanCreateValueForProperty(currentProperty, value);
        }

        public void WriteValue(object value)
        {
            Add(TypeDef.CreateValueForProperty(currentProperty, value));
        }

        public void Add(ObjectOutput value)
        {
            Properties[currentProperty] = value;
        }

        public abstract object GetTypedValue();
    }
}