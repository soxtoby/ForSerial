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

        private JsonParser(Writer writer)
        {
            if (writer == null) throw new ArgumentNullException("writer");

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
            JsonParser parser = new JsonParser(writer);
            int i = 0;
            parser.Parse(json, ref i);
        }

        private void Parse(string json, ref int i)
        {
            int jsonLength = json.Length;
            char c;
            int braces = 1;
            bool inString = false;

            for (; i < json.Length; i++) { c = json[i]; if (c >= WhitespaceChars.Length || !WhitespaceChars[c])break; }

            int start = i;
            c = json[i];

            switch (c)
            {
                case Quotes:
                    ParseAndWriteString(json, ref i);
                    break;
                case OpenBrace:
                    while (++i < jsonLength && braces > 0)
                    {
                        c = json[i];
                        if (c == Quotes && json[i - 1] != Backslash)
                        {
                            inString = !inString;
                            continue;
                        }

                        if (inString)
                            continue;

                        if (c == OpenBrace)
                            braces++;
                        else if (c == CloseBrace)
                            braces--;
                    }
                    ParseMap(json.Substring(start, i - start));
                    break;
                case OpenBracket:
                    while (++i < jsonLength && braces > 0)
                    {
                        c = json[i];
                        if (c == Quotes && json[i - 1] != Backslash)
                        {
                            inString = !inString;
                            continue;
                        }

                        if (inString)
                            continue;

                        if (c == OpenBracket)
                            braces++;
                        else if (c == CloseBracket)
                            braces--;
                    }
                    ParseArray(json.Substring(start, i - start));
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

        private void ParseArray(string json)
        {
            writer.BeginSequence();
            int jsonLength = json.Length;
            int i = 1;
            while (i < jsonLength)
            {
                Parse(json, ref i);
                SkipComma(json, ref i);
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

        private void ParseMap(string json)
        {
            writer.BeginStructure(typeof(JsonParser));
            int jsonLength = json.Length;
            int i = 1;
            while (i < jsonLength)
            {
                string propertyName = ParseString(json, ref i);
                writer.AddProperty(propertyName);
                for (; i < json.Length; i++) { char c = json[i]; if (c >= WhitespaceChars.Length || !WhitespaceChars[c])break; }
                i++;
                Parse(json, ref i);
                SkipComma(json, ref i);
            }

            writer.EndStructure();
        }

        private void ParseAndWriteString(string json, ref int i)
        {
            string str = ParseString(json, ref i);
            writer.Write(str);
        }

        private string ParseString(string json, ref int i)
        {
            char c;
            for (; i < json.Length; i++) { c = json[i]; if (c >= WhitespaceChars.Length || !WhitespaceChars[c])break; }
            i++;
            int start = i;
            int jsonLength = json.Length;
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

        private void SkipColon(string json, ref int i)
        {
            for (; i < json.Length; i++) { char c = json[i]; if (c >= WhitespaceChars.Length || !WhitespaceChars[c])break; }
            if (i < json.Length && json[i] == Colon)
                i++;
            for (; i < json.Length; i++) { char c = json[i]; if (c >= WhitespaceChars.Length || !WhitespaceChars[c])break; }
        }

        private void SkipComma(string json, ref int i)
        {
            for (; i < json.Length; i++) { char c = json[i]; if (c >= WhitespaceChars.Length || !WhitespaceChars[c])break; }
            if (i < json.Length && (json[i] == Comma || json[i] == CloseBrace || json[i] == CloseBracket))
                i++;
            for (; i < json.Length; i++) { char c = json[i]; if (c >= WhitespaceChars.Length || !WhitespaceChars[c])break; }
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

