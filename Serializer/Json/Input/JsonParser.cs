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
            parser.Parse(json);
        }

        private void Parse(string json)
        {
            int i = 0;
            SkipWhitespace(json, ref i);
            if (i < json.Length)
                Parse(json, ref i);
        }

        private void Parse(string json, ref int i)
        {
            int start = i;

            SkipWhitespace(json, ref i);

            if (i == json.Length)
                throw new ExpectedValue(json.Substring(start), 0, 0);

            char c = json[i];

            if (c == OpenBrace)
            {
                ParseMap(json, ref i);
            }
            else if (c == OpenBracket)
            {
                ParseArray(json, ref i);
            }
            else if (char.IsDigit(c))
            {
                writer.Write(GetNextNumber(json, ref i));
            }
            else if (c == Quotes)
            {
                writer.Write(GetNextString(json, ref i));
            }
            else
            {
                string word = GetNextToken(json, ref i);
                switch (word)
                {
                    case True:
                        writer.Write(true);
                        break;
                    case False:
                        writer.Write(false);
                        break;
                    case Null:
                        writer.WriteNull();
                        break;
                    default:
                        throw new ExpectedValue(word, 0, 0);
                }
            }
        }

        private static string MatchBraces(char openBrace, char closeBrace, string json, ref int i)
        {
            int startIndex = i;
            i++;
            int braces = 1;
            for (; i < json.Length && braces > 0; i++)
            {
                char c = json[i];

                if (c == Quotes)
                    GetNextString(json, ref i);
                else if (c == openBrace)
                    braces++;
                else if (c == closeBrace)
                    braces--;
            }

            if (braces > 0)
                throw new UnmatchedBrace(openBrace, 0, 0);

            return json.Substring(startIndex, i - startIndex);
        }

        private void ParseMap(string json, ref int i)
        {
            Expect(OpenBrace, json, ref i);

            bool isFirstProperty = true;

            if (NextIs(CloseBrace, json, ref i))
            {
                writer.BeginStructure(typeof(JsonParser));
                writer.EndStructure();
            }
            else
            {
                do
                {
                    string propertyName = GetNextString(json, ref i);
                    Expect(Colon, json, ref i);

                    if (isFirstProperty)
                    {
                        if (propertyName == ReferencePropertyName)
                        {
                            writer.WriteReference(Convert.ToInt32(GetNextNumber(json, ref i)));
                            Expect(CloseBrace, json, ref i);
                            return;
                        }
                        if (propertyName == TypePropertyName)
                        {
                            writer.BeginStructure(GetNextString(json, ref i), typeof(JsonParser));
                        }
                        else
                        {
                            writer.BeginStructure(typeof(JsonParser));
                            writer.AddProperty(propertyName);
                            Parse(json, ref i);
                        }
                        isFirstProperty = false;
                    }
                    else
                    {
                        writer.AddProperty(propertyName);
                        Parse(json, ref i);
                    }

                } while (NextIs(Comma, json, ref i));

                Expect(CloseBrace, json, ref i);

                writer.EndStructure();
            }
        }

        private void ParseArray(string json, ref int i)
        {
            Expect(OpenBracket, json, ref i);

            writer.BeginSequence();

            if (NextIs(CloseBracket, json, ref i))
            {
                writer.EndSequence();
            }
            else
            {
                do
                {
                    Parse(json, ref i);
                } while (NextIs(Comma, json, ref i));

                Expect(CloseBracket, json, ref i);

                writer.EndSequence();
            }
        }

        private static string GetNextString(string json, ref int i)
        {
            Expect(Quotes, json, ref i);

            int stringStart = i;

            bool escaped = false;
            for (; i < json.Length; i++)
            {
                if (escaped)
                    escaped = false;
                else if (json[i] == Quotes)
                    break;
                else if (json[i] == Backslash)
                    escaped = true;
            }

            string stringValue = json.Substring(stringStart, i - stringStart);//TODO unescape
            i++;
            return stringValue;
        }

        private static double GetNextNumber(string json, ref int i)
        {
            string number = GetNextToken(json, ref i);
            return double.Parse(number);
        }

        private static string GetNextToken(string json, ref int i)
        {
            SkipWhitespace(json, ref i);
            int tokenStart = i;
            i++;
            for (; i < json.Length; i++)
            {
                char c = json[i];
                if (c == Comma
                    || c == CloseBrace
                    || c == CloseBracket
                    || c < whitespaceChars.Length && whitespaceChars[c])
                    break;
            }
            return json.Substring(tokenStart, i - tokenStart);
        }

        private static bool NextIs(char expected, string json, ref int i)
        {
            SkipWhitespace(json, ref i);
            if (i < json.Length && json[i] == expected)
            {
                i++;
                return true;
            }
            return false;
        }

        private static void Expect(char expected, string json, ref int i)
        {
            SkipWhitespace(json, ref i);
            if (i == json.Length || json[i] != expected)
                throw new ExpectedToken(expected, i == json.Length ? char.MinValue : json[i], 0, 0);
            i++;
        }

        private static readonly bool[] whitespaceChars;

        static JsonParser()
        {
            whitespaceChars = new bool[' ' + 1];
            whitespaceChars[' '] = true;
            whitespaceChars['\t'] = true;
            whitespaceChars['\r'] = true;
            whitespaceChars['\n'] = true;
        }

        private static void SkipWhitespace(string json, ref int i)
        {
            for (; i < json.Length; i++)
            {
                char c = json[i];
                if (c > whitespaceChars.Length || !whitespaceChars[c])
                    break;
            }
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

