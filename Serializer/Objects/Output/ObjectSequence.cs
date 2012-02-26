namespace json.Objects
{
    public interface ObjectSequence : ObjectOutput
    {
        ObjectStructure CreateStructure();
        ObjectSequence CreateSequence();
        bool CanCreateValue(object value);
        ObjectValue CreateValue(object value);
        void Add(ObjectOutput value);
    }
}