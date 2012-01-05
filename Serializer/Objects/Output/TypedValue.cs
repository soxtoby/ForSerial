namespace json.Objects
{
    public interface TypedValue
    {
        void AssignToProperty(object obj, PropertyDefinition property);
        object GetTypedValue();
    }
}