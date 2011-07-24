namespace json.Objects
{
    public interface TypedObjectParseObject
    {
        TypeDefinition TypeDef { get; }
        void AssignToProperty(object owner, PropertyDefinition property);
        object Object { get; }
        void AddProperty(string name, object value);
        void AddObject(string name, TypedObjectObject value);
        void AddArray(string name, TypedObjectArray array);
        ParseValue CreateValue(string name, ParseValueFactory valueFactory, object value);
        ParseObject CreateObject(string name, ParseValueFactory valueFactory);
        ParseArray CreateArray(string name, ParseValueFactory valueFactory);
    }
}
