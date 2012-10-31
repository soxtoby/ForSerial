using System;
using System.Linq;

namespace ForSerial.Json
{
    public class JsonReader
    {
        private const char Quotes = '"';
        private const char OpenBrace = '{';
        private const char OpenBracket = '[';
        private const char CloseBrace = '}';
        private const char CloseBracket = ']';
        private const string True = "true";
        private const string False = "false";
        private const string Null = "null";
        private const char Backslash = '\\';
        private const char Comma = ',';
        private const char Colon = ':';
        private const string ReferencePropertyName = "_ref";
        private const string TypePropertyName = "_type";

        private readonly Writer writer;
        private readonly string json;
        private readonly int jsonLength;

        private int i;

        private JsonReader(string json, Writer writer)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            this.json = json;
            jsonLength = json.Length;
            this.writer = writer;
        }

        public static void Read(string json, Writer writer)
        {
            if (string.IsNullOrEmpty(json))
                return;

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
            JsonReader reader = new JsonReader(json, writer);
            reader.ParseNextValue();
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
                while (i < jsonLength)
                {
                    ParseNextValue();
                    if (IsEndOfArray()) break;
                }

            writer.EndSequence();
        }

        private bool IsEndOfArray()
        {
            SkipWhitespace();
            bool close = false;
            if (i < jsonLength && (json[i] == Comma || (close = json[i] == CloseBracket)))
                i++; // , or ]
            return close;
        }

        private void ParseMap()
        {
            int mapStartIndex = i;

            i++; // {
            SkipWhitespace();

            if (json[i] == CloseBrace)
            {
                i++; // }
                writer.BeginStructure(typeof(JsonReader));
            }
            else
            {
                if (ParseFirstProperty())
                    return; // reference object
                ParseRestOfProperties(mapStartIndex);
            }

            writer.EndStructure();
        }

        private bool ParseFirstProperty()
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
                        throw new ExpectedToken(CloseBrace, json[i - 1] == Comma
                            ? Comma
                            : json[i], CurrentLine, CurrentLinePosition);
                    return true;

                case TypePropertyName:
                    string typeIdentifier = GetNextString();
                    writer.BeginStructure(typeIdentifier, typeof(JsonReader));
                    return false;

                default:
                    writer.BeginStructure(typeof(JsonReader));
                    writer.AddProperty(propertyName);
                    ParseNextValue();
                    return false;
            }
        }

        private void ParseRestOfProperties(int mapStartIndex)
        {
            while (i < jsonLength)
            {
                if (IsEndOfMap()) return;

                string propertyName = GetNextString();
                writer.AddProperty(propertyName);

                SkipWhitespace();
                i++; // :

                ParseNextValue();
            }

            // End of file without finding end of map
            i = mapStartIndex;
            throw new UnmatchedBrace('{', CurrentLine, CurrentLinePosition);
        }

        private bool IsEndOfMap()
        {
            SkipWhitespace();
            bool close = false;
            if (i < jsonLength && (json[i] == Comma || (close = json[i] == CloseBrace)))
                i++; //, or }
            return close;
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

            int start = i + 1;
            bool containsBackslash = false;
            while (++i < jsonLength)
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
                ? JsonStringEscaper.UnescapeString(value)
                : value;
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
                    double number;
                    if (double.TryParse(word, out number))
                        writer.Write(number);
                    else
                        throw new ExpectedValue(word, CurrentLine, CurrentLinePosition);
                    break;
            }
        }

        private string GetWord()
        {
            int start = i;
            while (++i < jsonLength)
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

        private void SkipWhitespace()
        {
            do
            {
                char c = json[i];
                if (c >= WhitespaceChars.Length || !WhitespaceChars[c])
                    break;
            } while (++i < jsonLength);
        }

        private int CurrentLine
        {
            get { return Math.Max(0, json.Substring(0, i).Count(c => c == '\n')); }
        }

        private int CurrentLinePosition
        {
            get { return Math.Max(0, json.Substring(0, i).LastIndexOf('\n') - i); }
        }

        private static readonly bool[] WhitespaceChars = new bool[' ' + 1];

        static JsonReader()
        {
            WhitespaceChars[' '] = true;
            WhitespaceChars['\n'] = true;
            WhitespaceChars['\t'] = true;
            WhitespaceChars['\r'] = true;
        }


        private class ExpectedToken : ParseException
        {
            public ExpectedToken(char expectedToken, char actualToken, int line, int position)
                : base("Expected {0}.".FormatWith(expectedToken), actualToken.ToString(), line, position)
            {
            }
        }

        private class UnmatchedBrace : ParseException
        {
            public UnmatchedBrace(char brace, int line, int position)
                : base("Brace is unmatched.", brace.ToString(), line, position)
            {
            }
        }

        private class UnexpectedEndOfFile : ParseException
        {
            public UnexpectedEndOfFile() : base("Unexpected end of file.", string.Empty, 0, 0) { }
        }

        private class ExpectedValue : ParseException
        {
            protected internal ExpectedValue(string tokenString, int line, int position)
                : base("Expected value. ", tokenString, line, position)
            {
            }
        }
    }
}

