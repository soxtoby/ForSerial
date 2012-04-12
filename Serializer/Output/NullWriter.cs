using System;

namespace json
{
    public class NullWriter : Writer
    {
        private NullWriter() { }

        public static readonly NullWriter Instance = new NullWriter();

        public bool CanWrite(object value)
        {
            return true;
        }

        public void Write(object value)
        {
        }

        public void BeginStructure(Type readerType)
        {
        }

        public void BeginStructure(string typeIdentifier, Type readerType)
        {
        }

        public void EndStructure()
        {
        }

        public void AddProperty(string name)
        {
        }

        public void BeginSequence()
        {
        }

        public void EndSequence()
        {
        }

        public void WriteReference(int referenceIndex)
        {
        }

        public void Write(bool value)
        {
        }

        public void Write(char value)
        {
        }

        public void Write(decimal value)
        {
        }

        public void Write(double value)
        {
        }

        public void Write(float value)
        {
        }

        public void Write(int value)
        {
        }

        public void Write(long value)
        {
        }

        public void Write(string value)
        {
        }

        public void Write(uint value)
        {
        }

        public void Write(ulong value)
        {
        }

        public void WriteNull()
        {
        }
    }
}