namespace json.Objects
{
    public interface ObjectOutput
    {
        TypeDefinition TypeDef { get; }
        object GetTypedValue();
        void AssignToProperty(object obj, PropertyDefinition property);
    }
}