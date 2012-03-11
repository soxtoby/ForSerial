using System;

namespace json.Json
{
    public class JsonParser
    {
        private const char Quotes = '"';
        private const char OpenBrace = '{';
        private const char OpenBracket = '[';
        private const string True = "true";
        private const string False = "false";
        private const string Null = "null";
        private const char Backslash = '\\';
        private const char CloseBrace = '}';
        private const char Comma = ',';
        private const char Colon = ':';
        private const char CloseBracket = ']';
        private const string ReferencePropertyName = "_ref";
        private const string TypePropertyName = "_type";
        private readonly Writer writer;
        private readonly string json;
        private readonly int jsonLength;
        private int i;

        private JsonParser(string json, Writer writer)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            this.json = json;
            jsonLength = json.Length;
            this.writer = writer;
        }

        public static void Parse(string json, Writer writer)
        {
            try
            {
                DoParse(json, writer);
            }
            catch (ParseException e)
            {
                throw new ParseException(e, json);
            }
        }

        private static void DoParse(string json, Writer writer)
        {
            JsonParser parser = new JsonParser(json, writer);
            parser.Parse();
        }

        private void Parse()
        {
            char c;
            int braces = 1;
            bool inString = false;

            for (; i < json.Length; i++) { c = json[i]; if (c >= WhitespaceChars.Length || !WhitespaceChars[c])break; }

            int start = i;
            c = json[i];

            switch (c)
            {
                case Quotes:
                    ParseAndWriteString();
                    break;
                case OpenBrace:
                    ParseMap();
                    break;
                case OpenBracket:
                    ParseArray();
                    break;
                default:
                    {
                        while (++i < jsonLength)
                        {
                            c = json[i];
                            if (c == CloseBrace
                                || c == CloseBracket
                                    || c == Comma
                                        || c < WhitespaceChars.Length && WhitespaceChars[c])
                                break;
                        }
                        string word = json.Substring(start, i - start);
                        ParseWord(word);
                    }
                    break;
            }
        }

        private void ParseArray()
        {
            writer.BeginSequence();
            i++;
            for (; i < json.Length; i++) { char c = json[i]; if (c >= WhitespaceChars.Length || !WhitespaceChars[c])break; }
            if (json[i] == CloseBracket)
                i++;
            else
                while (i < jsonLength)
                {
                    Parse();
                    if (SkipComma()) break;
                }
            writer.EndSequence();
        }

        private void ParseWord(string word)
        {
            switch (word)
            {
                case True:
                    writer.Write(true);
                    return;
                case False:
                    writer.Write(false);
                    return;
                case Null:
                    writer.WriteNull();
                    return;
            }
            writer.Write(double.Parse(word));
        }

        private void ParseMap()
        {
            writer.BeginStructure(typeof(JsonParser));
            i++;
            for (; i < json.Length; i++) { char c = json[i]; if (c >= WhitespaceChars.Length || !WhitespaceChars[c])break; }
            if (json[i] == CloseBrace)
                i++;
            else
                while (i < jsonLength)
                {
                    string propertyName = ParseString();
                    writer.AddProperty(propertyName);
                    for (; i < json.Length; i++) { char c = json[i]; if (c >= WhitespaceChars.Length || !WhitespaceChars[c])break; }
                    i++;
                    Parse();
                    if (SkipComma()) break;
                }

            writer.EndStructure();
        }

        private void ParseAndWriteString()
        {
            string str = ParseString();
            writer.Write(str);
        }

        private string ParseString()
        {
            char c;
            for (; i < json.Length; i++) { c = json[i]; if (c >= WhitespaceChars.Length || !WhitespaceChars[c])break; }
            i++;
            int start = i;
            while (++i < jsonLength)
            {
                c = json[i];
                if (c == Quotes && json[i - 1] != Backslash)
                    break;
            }
            string value = json.Substring(start, i - start);
            i++;
            return value;
        }

        private bool SkipComma()
        {
            for (; i < json.Length; i++) { char c = json[i]; if (c >= WhitespaceChars.Length || !WhitespaceChars[c])break; }
            bool close = false;
            if (i < jsonLength && (json[i] == Comma || (close = json[i] == CloseBrace) || (close = json[i] == CloseBracket)))
                i++;
            for (; i < json.Length; i++) { char c = json[i]; if (c >= WhitespaceChars.Length || !WhitespaceChars[c])break; }
            return close;
        }

        private static readonly bool[] WhitespaceChars = new bool[' ' + 1];

        static JsonParser()
        {
            WhitespaceChars[' '] = true;
            WhitespaceChars['\n'] = true;
            WhitespaceChars['\t'] = true;
            WhitespaceChars['\r'] = true;
        }


        internal class ExpectedToken : ParseException
        {
            public ExpectedToken(char expectedToken, char actualToken, int line, int position)
                : base("Expected {0}.".FormatWith(expectedToken), actualToken.ToString(), line, position)
            {
            }
        }

        internal class UnmatchedBrace : ParseException
        {
            public UnmatchedBrace(char brace, int line, int position)
                : base("Brace is unmatched.", brace.ToString(), line, position)
            {
            }
        }

        internal class UnexpectedEndOfFile : ParseException
        {
            public UnexpectedEndOfFile(int line, int position) : base("Unexpected end of file.", string.Empty, line, position) { }
        }

        internal class ExpectedValue : ParseException
        {
            protected internal ExpectedValue(string tokenString, int line, int position)
                : base("Expected value. ", tokenString, line, position)
            {
            }
        }
    }
}

