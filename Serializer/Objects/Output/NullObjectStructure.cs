namespace json.Objects
{
    internal class NullObjectStructure : ObjectStructure
    {
        public static readonly NullObjectStructure Instance = new NullObjectStructure();

        private NullObjectStructure() { }

        public void AssignToProperty(object obj, PropertyDefinition property) { }

        public object GetTypedValue()
        {
            return null;
        }

        public ObjectStructure CreateStructure(string property)
        {
            return Instance;
        }

        public ObjectSequence CreateSequence(string property)
        {
            return NullObjectSequence.Instance;
        }

        public void Add(string property, ObjectOutput value) { }

        public bool CanCreateValue(string property, object value)
        {
            return true;
        }

        public ObjectValue CreateValue(string property, object value)
        {
            return NullObjectValue.Instance;
        }

        public TypeDefinition TypeDef { get { return null; } }
    }
}