using System;
using json.Objects.TypeDefinitions;

namespace json.Objects
{
    public class NullObjectStructure : ObjectContainer
    {
        public static readonly NullObjectStructure Instance = new NullObjectStructure();

        private NullObjectStructure() { }

        public TypeDefinition TypeDef { get { return NullTypeDefinition.Instance; } }

        public object GetTypedValue()
        {
            return null;
        }

        public void AssignToProperty(object obj, PropertyDefinition property) { }

        public void SetCurrentProperty(string name) { }

        public ObjectContainer CreateStructure()
        {
            return Instance;
        }

        public ObjectContainer CreateStructure(string typeIdentifier)
        {
            return Instance;
        }

        public ObjectContainer CreateSequence()
        {
            return NullObjectSequence.Instance;
        }

        public void Add(ObjectOutput value) { }

        public PreBuildInfo GetPreBuildInfo(Type readerType)
        {
            return null;
        }

        public bool CanCreateValue(object value)
        {
            return true;
        }

        public void WriteValue(object value) { }
    }
}