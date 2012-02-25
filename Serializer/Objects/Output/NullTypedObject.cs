namespace json.Objects
{
    internal class NullTypedObject : TypedObject
    {
        private NullTypedObject() { }

        private static NullTypedObject instance;
        public static NullTypedObject Instance
        {
            get { return instance ?? (instance = new NullTypedObject()); }
        }

        public TypeDefinition TypeDef
        {
            get { return NullTypeDefinition.Instance; }
        }

        public void AssignToProperty(object owner, PropertyDefinition property)
        {
        }

        public object Object
        {
            get { return null; }
        }

        public void AddProperty(string name, TypedValue value)
        {
        }

        public Output CreateValue(string name, object value)
        {
            return TypedNull.Value;
        }

        public OutputStructure BeginStructure(string name)
        {
            return new TypedObjectOutputStructure(Instance);
        }

        public SequenceOutput BeginSequence(string name)
        {
            return TypedNullArray.Instance;
        }

        public void EndStructure()
        {
        }

        public void EndSequence()
        {
        }
    }
}