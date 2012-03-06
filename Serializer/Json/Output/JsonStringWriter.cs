using System;
using System.IO;
using json.Objects;

namespace json.Json
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
        private const string Backslash = "\\";
        private const string EscapedBackslash = "\\\\";
        private const string QuoteString = "\"";
        private const string EscapedQuote = "\\\"";
        private const string Return = "\r";
        private const string NewLine = "\n";
        private const string EscapedNewLine = "\\n";

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

        private void WriteString(string value)
        {
            Json.Write(QuoteChar);
            Json.Write(EscapeForJson(value));
            Json.Write(QuoteChar);
        }

        private void WriteRawString(string value)
        {
            Json.Write(QuoteChar);
            Json.Write(value);
            Json.Write(QuoteChar);
        }

        private void WriteNumber(double value)
        {
            Json.Write(value);
        }

        private void WriteBoolean(bool value)
        {
            Json.Write(value ? True : False);
        }

        private void WriteNull()
        {
            Json.Write(Null);
        }

        private static string EscapeForJson(string value)
        {
            return value
                .Replace(Backslash, EscapedBackslash)
                .Replace(QuoteString, EscapedQuote)
                .Replace(Return, string.Empty)
                .Replace(NewLine, EscapedNewLine);
        }
    }
}
