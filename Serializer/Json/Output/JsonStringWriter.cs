using System;
using System.IO;
using System.Text.RegularExpressions;
using json.Objects;

namespace json.Json
{
    public class JsonStringWriter : NewWriter
    {
        private readonly TextWriter json;
        private bool suppressDelimiter = true;

        public JsonStringWriter(TextWriter textWriter)
        {
            if (textWriter == null) throw new ArgumentNullException("textWriter");

            json = textWriter;
        }

        public void WriteValue(object value)
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
                    WriteNumber(System.Convert.ToDouble(value));
                    break;
            }
        }

        private void WriteString(string value)
        {
            json.Write('"');
            json.Write(EscapeForJson(value));
            json.Write('"');
        }

        private void WriteNumber(double value)
        {
            json.Write(value);
        }

        private void WriteBoolean(bool value)
        {
            json.Write(value ? "true" : "false");
        }

        private void WriteNull()
        {
            json.Write("null");
        }

        public void BeginStructure()
        {
            json.Write('{');
            suppressDelimiter = true;
        }

        public void EndStructure()
        {
            json.Write('}');
            suppressDelimiter = false;
        }

        public void AddProperty(string name)
        {
            Delimit();
            WriteString(name);
            json.Write(':');
            suppressDelimiter = true;
        }

        public void BeginSequence()
        {
            json.Write('[');
            suppressDelimiter = true;
        }

        public void EndSequence()
        {
            json.Write(']');
            suppressDelimiter = false;
        }

        private void Delimit()
        {
            if (!suppressDelimiter)
                json.Write(',');
            suppressDelimiter = false;
        }

        // TODO escape control characters as well

        private static readonly Regex CharactersToEscape = new Regex(@"[""\\]", RegexOptions.Compiled);

        private static string EscapeForJson(string value)
        {
            return CharactersToEscape.Replace(value, @"\$0");
        }
    }
}