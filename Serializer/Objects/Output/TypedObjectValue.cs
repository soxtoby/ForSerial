namespace json.Objects
{
    public interface TypedObjectValue
    {
        void AssignToProperty(object obj, PropertyDefinition property);
        object GetTypedValue();
    }
}