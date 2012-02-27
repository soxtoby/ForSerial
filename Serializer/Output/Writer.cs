namespace json
{
    public interface Writer
    {
        bool CanWrite(object value);
        void Write(object value);
        void BeginStructure();
        void SetType(string typeIdentifier);
        void EndStructure();
        void AddProperty(string name);
        void BeginSequence();
        void EndSequence();
        void WriteReference(int referenceIndex);
    }
}