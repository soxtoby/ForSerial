using System;

namespace json
{
    public interface Writer
    {
        bool CanWrite(object value);
        void Write(object value);
        void BeginStructure(Type readerType);
        void BeginStructure(string typeIdentifier, Type readerType);
        void EndStructure();
        void AddProperty(string name);
        void BeginSequence();
        void EndSequence();
        void WriteReference(int referenceIndex);
    }
}