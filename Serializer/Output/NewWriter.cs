namespace json
{
    public interface NewWriter
    {
        bool CanWrite(object value);
        void Write(object value);
        void BeginStructure();
        void EndStructure();
        void AddProperty(string name);
        void BeginSequence();
        void EndSequence();
    }
}