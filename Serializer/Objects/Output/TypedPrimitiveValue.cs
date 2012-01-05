namespace json.Objects
{
    public class TypedPrimitiveValue : TypedValue
    {
        private readonly object value;

        public TypedPrimitiveValue(object value)
        {
            this.value = value;
        }

        public void AssignToProperty(object obj, PropertyDefinition property)
        {
            property.SetOn(obj, value);
        }

        public object GetTypedValue()
        {
            return value;
        }
    }
}