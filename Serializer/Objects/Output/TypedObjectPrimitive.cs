namespace json.Objects
{
    public class TypedObjectPrimitive : TypedObjectValue
    {
        private readonly object value;

        public TypedObjectPrimitive(object value)
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