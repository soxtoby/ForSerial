using System;

namespace ForSerial
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
        void Write(bool value);
        void Write(char value);
        void Write(decimal value);
        void Write(double value);
        void Write(float value);
        void Write(int value);
        void Write(long value);
        void Write(string value);
        void Write(uint value);
        void Write(ulong value);
        void WriteNull();
    }
}