using System;
using System.Collections.Generic;

namespace json.Objects
{
    public abstract class BaseObjectStructure : ObjectContainer
    {
        protected readonly StructureDefinition StructureDef;
        protected string CurrentProperty;
        protected readonly Dictionary<string, ObjectOutput> Properties = new Dictionary<string, ObjectOutput>(StringComparer.OrdinalIgnoreCase);

        public TypeDefinition TypeDef { get { return StructureDef; } }

        protected BaseObjectStructure(StructureDefinition typeDef)
        {
            StructureDef = typeDef;
        }

        public abstract void AssignToProperty(object obj, PropertyDefinition property);

        public void SetCurrentProperty(string name)
        {
            CurrentProperty = name;
        }

        public ObjectContainer CreateStructure()
        {
            return StructureDef.CreateStructureForProperty(CurrentProperty);
        }

        public ObjectContainer CreateStructure(string typeIdentifier)
        {
            return StructureDef.CreateStructureForProperty(CurrentProperty, typeIdentifier);
        }

        public ObjectContainer CreateSequence()
        {
            return StructureDef.CreateSequenceForProperty(CurrentProperty);
        }

        public bool CanCreateValue(object value)
        {
            return StructureDef.CanCreateValueForProperty(CurrentProperty, value);
        }

        public void WriteValue(object value)
        {
            Add(StructureDef.CreateValueForProperty(CurrentProperty, value));
        }

        public void Add(ObjectOutput value)
        {
            Properties[CurrentProperty] = value;
        }

        public PreBuildInfo GetPreBuildInfo(Type readerType)
        {
            return TypeDef.GetPreBuildInfo(readerType);
        }

        public abstract object GetTypedValue();
    }
}
