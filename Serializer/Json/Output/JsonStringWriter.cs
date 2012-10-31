using System;
using System.IO;
using ForSerial.Objects;
using ForSerial.Objects.TypeDefinitions;

namespace ForSerial.Json
{
    public class JsonStringWriter : Writer
    {
        private const char QuoteChar = '"';
        private const char Comma = ',';
        private const char OpenBrace = '{';
        private const char CloseBrace = '}';
        private const char Colon = ':';
        private const char OpenBracket = '[';
        private const char CloseBracket = ']';
        private const string TypePropertyName = "_type";
        private const string ReferenceProperty = @"{""_ref"":";
        private const string True = "true";
        private const string False = "false";
        private const string Null = "null";

        protected readonly TextWriter Json;
        private bool suppressDelimiter = true;

        public JsonStringWriter(TextWriter textWriter)
        {
            if (textWriter == null) throw new ArgumentNullException("textWriter");

            Json = textWriter;
        }

        public bool CanWrite(object value)
        {
            return value.IsJsonPrimitiveType();
        }

        public void Write(object value)
        {
            if (value == null)
                WriteNull();
            else
                PrimitiveDefinition.GetWriterMethod(Type.GetTypeCode(value.GetType()))(value, this);
        }

        public void Write(bool value)
        {
            Delimit();
            Json.Write(value ? True : False);
        }

        public void Write(char value)
        {
            Delimit();
            Json.Write(value);
        }

        public void Write(decimal value)
        {
            Delimit();
            Json.Write(value);
        }

        public void Write(double value)
        {
            Delimit();
            Json.Write(value);
        }

        public void Write(float value)
        {
            Delimit();
            Json.Write(value);
        }

        public void Write(int value)
        {
            Delimit();
            Json.Write(value);
        }

        public void Write(long value)
        {
            Delimit();
            Json.Write(value);
        }

        public void Write(string value)
        {
            Delimit();
            Json.Write(QuoteChar);
            Json.Write(JsonStringEscaper.EscapeForJson(value));
            Json.Write(QuoteChar);
        }

        public void Write(uint value)
        {
            Delimit();
            Json.Write(value);
        }

        public void Write(ulong value)
        {
            Delimit();
            Json.Write(value);
        }

        public void WriteNull()
        {
            Delimit();
            Json.Write(Null);
        }

        private void WriteRawString(string value)
        {
            Json.Write(QuoteChar);
            Json.Write(value);
            Json.Write(QuoteChar);
        }

        public virtual void BeginStructure(Type readerType)
        {
            Delimit();
            Json.Write(OpenBrace);
            suppressDelimiter = true;
        }

        public virtual void BeginStructure(string typeIdentifier, Type readerType)
        {
            BeginStructure(readerType);
            AddProperty(TypePropertyName);
            Write(typeIdentifier);
        }

        public virtual void EndStructure()
        {
            Json.Write(CloseBrace);
            suppressDelimiter = false;
        }

        public virtual void AddProperty(string name)
        {
            Delimit();
            WriteRawString(name);
            Json.Write(Colon);
            suppressDelimiter = true;
        }

        public virtual void BeginSequence()
        {
            Delimit();
            Json.Write(OpenBracket);
            suppressDelimiter = true;
        }

        public virtual void EndSequence()
        {
            Json.Write(CloseBracket);
            suppressDelimiter = false;
        }

        public virtual void WriteReference(int referenceIndex)
        {
            Delimit();
            Json.Write(ReferenceProperty);
            Json.Write(referenceIndex);
            Json.Write(CloseBrace);
        }

        protected virtual void Delimit()
        {
            if (suppressDelimiter)
                suppressDelimiter = false;
            else
                Json.Write(Comma);
        }
    }
}
