using System;
using System.IO;
using System.Text.RegularExpressions;
using json.Objects;

namespace json.Json
{
    public class JsonStringWriter : Writer
    {
        private readonly TextWriter json;
        protected TextWriter Json { get { return json; } }
        private bool suppressDelimiter = true;

        public JsonStringWriter(TextWriter textWriter)
        {
            if (textWriter == null) throw new ArgumentNullException("textWriter");

            json = textWriter;
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

            switch (value.GetType().GetTypeCodeType())
            {
                case TypeCodeType.Boolean:
                    WriteBoolean((bool)value);
                    break;

                case TypeCodeType.String:
                    WriteString((string)value);
                    break;

                case TypeCodeType.Number:
                    WriteNumber(Convert.ToDouble(value));
                    break;
            }
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
            WriteString(name);
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
            BeginStructure(null); // FIXME readerType isn't being used in this class, but I'm not sure I like passing in null
            AddProperty("_ref");
            Write(referenceIndex);
            EndStructure();
        }

        protected virtual void Delimit()
        {
            if (!suppressDelimiter)
                Json.Write(',');
            suppressDelimiter = false;
        }

        private void WriteString(string value)
        {
            Json.Write('"');
            Json.Write(EscapeForJson(value));
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

        // TODO escape control characters as well
        private static readonly Regex CharactersToEscape = new Regex(@"[""\\]", RegexOptions.Compiled);

        private static string EscapeForJson(string value)
        {
            return CharactersToEscape.Replace(value, @"\$0");
        }
    }
}