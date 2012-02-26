namespace json.Objects
{
    public interface ObjectContainer : ObjectOutput
    {
        void SetCurrentProperty(string name);
        void SetType(string typeIdentifier);
        ObjectContainer CreateStructure();
        ObjectContainer CreateSequence();
        bool CanCreateValue(object value);
        void WriteValue(object value);
        void Add(ObjectOutput value);
    }
}