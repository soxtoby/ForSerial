namespace json.Objects
{
    public interface ObjectStructure : ObjectOutput
    {
        ObjectStructure CreateStructure(string property);
        ObjectSequence CreateSequence(string property);
        bool CanCreateValue(string property, object value);
        ObjectValue CreateValue(string property, object value);
        void Add(string property, ObjectOutput value);
    }
}