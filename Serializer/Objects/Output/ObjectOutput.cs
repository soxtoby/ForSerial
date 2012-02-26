namespace json.Objects
{
    public interface ObjectOutput
    {
        void AssignToProperty(object obj, PropertyDefinition property);
        object GetTypedValue();
        TypeDefinition TypeDef { get; }
    }
}