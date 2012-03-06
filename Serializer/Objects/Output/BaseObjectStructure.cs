using System;
using System.Collections.Generic;

namespace json.Objects
{
    public abstract class BaseObjectStructure : ObjectContainer
    {
        protected readonly StructureDefinition StructureDef;
        private string currentProperty;
        protected readonly Dictionary<string, ObjectOutput> Properties = new Dictionary<string, ObjectOutput>(StringComparer.OrdinalIgnoreCase);

        public TypeDefinition TypeDef { get { return StructureDef; } }

        protected BaseObjectStructure(StructureDefinition typeDef)
        {
            StructureDef = typeDef;
        }

        public abstract void AssignToProperty(object obj, PropertyDefinition property);

        public void SetCurrentProperty(string name)
        {
            currentProperty = name;
        }

        public ObjectContainer CreateStructure()
        {
            return StructureDef.CreateStructureForProperty(currentProperty);
        }

        public ObjectContainer CreateStructure(string typeIdentifier)
        {
            return StructureDef.CreateStructureForProperty(currentProperty, typeIdentifier);
        }

        public ObjectContainer CreateSequence()
        {
            return StructureDef.CreateSequenceForProperty(currentProperty);
        }

        public bool CanCreateValue(object value)
        {
            return StructureDef.CanCreateValueForProperty(currentProperty, value);
        }

        public void WriteValue(object value)
        {
            Add(StructureDef.CreateValueForProperty(currentProperty, value));
        }

        public void Add(ObjectOutput value)
        {
            Properties[currentProperty] = value;
        }

        public PreBuildInfo GetPreBuildInfo(Type readerType)
        {
            return TypeDef.GetPreBuildInfo(readerType);
        }

        public abstract object GetTypedValue();
    }
}