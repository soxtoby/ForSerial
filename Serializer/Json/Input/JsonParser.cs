using System;
using System.Linq;

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
            catch (IndexOutOfRangeException)
            {
                throw new UnexpectedEndOfFile();
            }
        }

        private static void DoParse(string json, Writer writer)
        {
            JsonParser parser = new JsonParser(json, writer);
            parser.ParseNextValue();
        }

        private void ParseNextValue()
        {
            SkipWhitespace();

            if (i >= jsonLength)
                return;

            char c = json[i];

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
                    ParseWord();
                    break;
            }
        }

        private void ParseArray()
        {
            writer.BeginSequence();

            i++; // [
            SkipWhitespace();

            if (json[i] == CloseBracket)
                i++; // ]
            else
                for (; i < json.Length; )
                {
                    ParseNextValue();
                    if (IsEndOfArray()) break;
                }

            writer.EndSequence();
        }

        private void ParseWord()
        {
            string word = GetWord();
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
                default:
                    if (char.IsDigit(word[0]))
                        writer.Write(double.Parse(word));
                    else
                        throw new ExpectedValue(word, CurrentLine, CurrentLinePosition);
                    break;
            }
        }

        private string GetWord()
        {
            int start = i;
            i++;
            for (; i < json.Length; i++)
            {
                char c = json[i];
                if (c == CloseBrace
                    || c == CloseBracket
                    || c == Comma
                    || c < WhitespaceChars.Length && WhitespaceChars[c])
                    break;
            }
            return json.Substring(start, i - start);
        }

        private void ParseMap()
        {
            i++; // {
            SkipWhitespace();

            if (json[i] == CloseBrace)
            {
                i++; // }
                writer.BeginStructure(typeof(JsonParser));
            }
            else
            {
                string propertyName = GetNextString();
                SkipWhitespace();
                if (json[i] != Colon)
                    throw new ExpectedToken(Colon, json[i], CurrentLine, CurrentLinePosition);
                i++; // :

                switch (propertyName)
                {
                    case ReferencePropertyName:
                        SkipWhitespace();
                        writer.WriteReference(int.Parse(GetWord()));
                        if (!IsEndOfMap())
                            throw new ExpectedToken(CloseBrace, json[i - 1] == Comma ? Comma : json[i], CurrentLine, CurrentLinePosition);
                        return;

                    case TypePropertyName:
                        string typeIdentifier = GetNextString();
                        writer.BeginStructure(typeIdentifier, typeof(JsonParser));
                        break;

                    default:
                        writer.BeginStructure(typeof(JsonParser));
                        writer.AddProperty(propertyName);
                        ParseNextValue();
                        break;
                }

                for (; i < json.Length; )
                {
                    if (IsEndOfMap()) break;

                    propertyName = GetNextString();
                    writer.AddProperty(propertyName);

                    SkipWhitespace();
                    i++; // :

                    ParseNextValue();
                }
            }

            writer.EndStructure();
        }

        private void ParseAndWriteString()
        {
            string str = GetNextString();
            writer.Write(str);
        }

        private string GetNextString()
        {
            SkipWhitespace();
            if (json[i] != Quotes)
                throw new ExpectedToken(Quotes, json[i], CurrentLine, CurrentLinePosition);
            i++; // "

            int start = i;
            bool containsBackslash = false;
            for (; i < json.Length; i++)
            {
                char c = json[i];

                if (c == Quotes)
                    break;

                if (c == Backslash)
                {
                    i++; // \
                    containsBackslash = true;
                }
            }

            string value = json.Substring(start, i - start);

            i++; // "

            return containsBackslash
                ? UnescapeString(value)
                : value;
        }

        private static string UnescapeString(string value)
        {
            return value;
        }

        private bool IsEndOfMap()
        {
            SkipWhitespace();
            bool close = false;
            if (json[i] == Comma || (close = json[i] == CloseBrace))
                i++; //, or }
            return close;
        }

        private bool IsEndOfArray()
        {
            SkipWhitespace();
            bool close = false;
            if (json[i] == Comma || (close = json[i] == CloseBracket))
                i++; // , or ]
            return close;
        }

        private void SkipWhitespace()
        {
            for (; i < json.Length; i++)
            {
                char c = json[i];
                if (c >= WhitespaceChars.Length || !WhitespaceChars[c])
                    break;
            }
        }

        private int CurrentLine
        {
            get { return Math.Max(0, json.Substring(0, i + 1).Count(c => c == '\n')); }
        }

        private int CurrentLinePosition
        {
            get { return Math.Max(0, json.Substring(0, i).LastIndexOf('\n')) - i; }
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
            public UnexpectedEndOfFile() : base("Unexpected end of file.", string.Empty, 0, 0) { }
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

