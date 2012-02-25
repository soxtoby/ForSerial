namespace json.Objects
{
    public interface TypedObject
    {
        TypeDefinition TypeDef { get; }
        void AssignToProperty(object owner, PropertyDefinition property);
        object Object { get; }
        void AddProperty(string name, TypedValue value);
        Output CreateValue(string name, object value);
        OutputStructure BeginStructure(string name);
        SequenceOutput BeginSequence(string name);
        void EndStructure();
        void EndSequence();
    }
}
