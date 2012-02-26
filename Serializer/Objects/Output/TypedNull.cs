namespace json.Objects
{
    internal class TypedNull : ObjectValue
    {
        private TypedNull() { }

        private static TypedNull value;
        public static TypedNull Value
        {
            get { return value = value ?? new TypedNull(); }
        }

        public void AssignToProperty(object obj, PropertyDefinition property)
        {
            property.SetOn(obj, null);
        }

        public object GetTypedValue()
        {
            return null;
        }

        public TypeDefinition TypeDef { get { throw new System.NotImplementedException(); } }
    }
}
