using System;

namespace ForSerial.Objects
{
    internal class BaseOutput<T> : ObjectContainer
    {
        private T result;

        public TypeDefinition TypeDef { get { return TypeCache.GetTypeDefinition(typeof(T)); } }

        public void AssignToProperty(object obj, PropertyDefinition property) { }
        public void SetCurrentProperty(string name) { }

        public object GetTypedValue()
        {
            return result;
        }

        public void Add(ObjectOutput value)
        {
            result = (T)value.GetTypedValue();
        }

        public PreBuildInfo GetPreBuildInfo(Type readerType)
        {
            return null;
        }

        public ObjectContainer CreateStructure()
        {
            return TypeDef.CreateStructure();
        }

        public ObjectContainer CreateStructure(string typeIdentifier)
        {
            return TypeDef.CreateStructure(typeIdentifier);
        }

        public ObjectContainer CreateSequence()
        {
            return TypeDef.CreateSequence();
        }

        public bool CanCreateValue(object value)
        {
            return TypeDef.CanCreateValue(value);
        }

        public void WriteValue(object value)
        {
            result = (T)TypeDef.CreateValue(value).GetTypedValue();
        }
    }
}