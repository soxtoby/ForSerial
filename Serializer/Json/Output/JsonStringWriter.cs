using System;
using System.IO;
using json.Objects;

namespace json.Json
{
    public class JsonStringWriter : Writer
    {
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
            Delimit();

            if (value == null)
            {
                WriteNull();
                return;
            }

            Type valueType = value.GetType();
            if (valueType == typeof(bool))
                WriteBoolean((bool)value);
            else if (valueType == typeof(string))
                WriteString((string)value);
            else
                WriteNumber(Convert.ToDouble(value));
        }

        public virtual void BeginStructure(Type readerType)
        {
            Delimit();
            Json.Write('{');
            suppressDelimiter = true;
        }

        public virtual void BeginStructure(string typeIdentifier, Type readerType)
        {
            BeginStructure(readerType);
            AddProperty("_type");
            Write(typeIdentifier);
        }

        public virtual void EndStructure()
        {
            Json.Write('}');
            suppressDelimiter = false;
        }

        public virtual void AddProperty(string name)
        {
            Delimit();
            WriteRawString(name);
            Json.Write(':');
            suppressDelimiter = true;
        }

        public virtual void BeginSequence()
        {
            Delimit();
            Json.Write('[');
            suppressDelimiter = true;
        }

        public virtual void EndSequence()
        {
            Json.Write(']');
            suppressDelimiter = false;
        }

        public virtual void WriteReference(int referenceIndex)
        {
            Delimit();
            Json.Write(@"{""_ref"":");
            Json.Write(referenceIndex);
            Json.Write('}');
        }

        protected virtual void Delimit()
        {
            if (suppressDelimiter)
                suppressDelimiter = false;
            else
                Json.Write(',');
        }

        private void WriteString(string value)
        {
            Json.Write('"');
            Json.Write(EscapeForJson(value));
            Json.Write('"');
        }

        private void WriteRawString(string value)
        {
            Json.Write('"');
            Json.Write(value);
            Json.Write('"');
        }

        private void WriteNumber(double value)
        {
            Json.Write(value);
        }

        private void WriteBoolean(bool value)
        {
            Json.Write(value ? "true" : "false");
        }

        private void WriteNull()
        {
            Json.Write("null");
        }

        private static string EscapeForJson(string value)
        {
            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "")
                .Replace("\n", "\\n");
        }
    }
}
