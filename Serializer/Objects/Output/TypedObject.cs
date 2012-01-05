namespace json.Objects
{
    public interface TypedObject
    {
        TypeDefinition TypeDef { get; }
        void AssignToProperty(object owner, PropertyDefinition property);
        object Object { get; }
        void AddProperty(string name, TypedValue value);
        Output CreateValue(string name, object value);
        OutputStructure CreateStructure(string name);
        SequenceOutput CreateSequence(string name);
    }
}
