namespace json
{
    public interface NewWriter
    {
        void WriteValue(object value);
        void BeginStructure();
        void EndStructure();
        void AddProperty(string name);
        void BeginSequence();
        void EndSequence();
    }
}