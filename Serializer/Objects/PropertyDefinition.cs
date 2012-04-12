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
        ObjectOutput CreateValue(object value);
        void Read(object value, ObjectReader reader, Writer writer);
        bool MatchesPropertyFilter(PropertyFilter filter);
    }
}