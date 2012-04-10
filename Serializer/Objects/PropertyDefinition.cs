namespace json.Objects
{
    public interface PropertyDefinition
    {
        string Name { get; }
        bool CanGet { get; }
        bool CanSet { get; }
        TypeDefinition TypeDef { get; }
        string FullName { get; }
        object GetFrom(object source);
        void SetOn(object target, object value);
        ObjectContainer CreateStructure();
        ObjectContainer CreateStructure(string typeIdentifier);
        ObjectContainer CreateSequence();
        bool CanCreateValue(object value);
        ObjectValue CreateValue(object value);
        void Read(object value, ObjectReader reader, Writer writer);
        bool MatchesFilter(PropertyFilter filter);
    }
}