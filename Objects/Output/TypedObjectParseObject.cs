namespace json.Objects
{
    public partial class TypedObjectBuilder
    {
        private interface TypedObjectParseObject : ParseObject
        {
            TypeDefinition TypeDef { get; }
            void AssignToProperty(object owner, PropertyDefinition property);
            object Object { get; }
        }
    }
}
