namespace json.Objects
{
    internal class BaseOutput<T> : ObjectContainer
    {
        private T result;

        public TypeDefinition TypeDef { get { return CurrentTypeHandler.GetTypeDefinition(typeof(T)); } }

        public void AssignToProperty(object obj, PropertyDefinition property) { }
        public void SetCurrentProperty(string name) { }
        public void SetType(string typeIdentifier) { }

        public object GetTypedValue()
        {
            return result;
        }

        public void Add(ObjectOutput value)
        {
            result = (T)value.GetTypedValue();
        }

        public ObjectContainer CreateStructure()
        {
            return TypeDef.CreateStructure();
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