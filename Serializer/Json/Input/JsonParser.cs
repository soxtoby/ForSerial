using System;

namespace json.Json
{
    public class JsonParser
    {
        private readonly Writer writer;
        private TokenReader reader;

        private JsonParser(Writer writer)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            this.writer = writer;
        }

        public static void Parse(string json, Writer writer)
        {
            try
            {
                Parse(new TokenReader(json), writer);
            }
            catch (ParseException e)
            {
                throw new ParseException(e, json);
            }
        }

        private static void Parse(TokenReader tokenReader, Writer writer)
        {
            JsonParser parser = new JsonParser(writer);
            parser.ParseTokens(tokenReader);
        }

        private void ParseTokens(TokenReader tokenReader)
        {
            reader = tokenReader;

            NextToken();

            if (!reader.EndOfFile)
                ParseValue();
        }

        private void ParseValue()
        {
            if (char.IsNumber(reader.CurrentChar))
            {
                ParseNumber();
            }
            else if (reader.CurrentChar == '"')
            {
                ParseString();
            }
            else if (char.IsLetter(reader.CurrentChar))
            {
                ParseWord();
            }
            else
                switch (reader.CurrentChar)
                {
                    case '{':
                        ParseObject();
                        return;
                    case '[':
                        ParseArray();
                        return;
                    default:
                        throw new ExpectedValue(reader.ExtractToken(), reader.CurrentLine, reader.CurrentPosition);
                }
        }

        private void ParseWord()
        {
            do
            {
                AssertNotEndOfFile();
                reader.KeepNextChar();

            } while (char.IsLetterOrDigit(reader.CurrentChar));

            string word = reader.ExtractToken();

            switch (word)
            {
                case "true":
                    writer.Write(true);
                    break;

                case "false":
                    writer.Write(false);
                    break;

                case "null":
                    writer.WriteNull();
                    break;

                default:
                    throw new ExpectedValue(word, reader.CurrentLine, reader.CurrentPosition);
            }

            NextToken();
        }

        private void ParseNumber()
        {
            writer.Write(GetNumber());
        }

        private void ParseString()
        {
            writer.Write(GetString());
            NextToken();
        }

        private void ParseObject()
        {
            ExpectSymbol('{');

            if (reader.CurrentChar == '}')
            {
                writer.BeginStructure(GetType());
            }
            else
            {
                PropertyParser propertyParser = new FirstPropertyParser(this);

                do
                {
                    string name = GetString();

                    ExpectSymbol(':');

                    propertyParser.ParsePropertyValue(name);

                    if (propertyParser.ReturnImmediately)
                        return;

                    propertyParser = propertyParser.NextPropertyParser;

                } while (MoveNextIfSymbol(','));
            }

            ExpectSymbol('}');

            writer.EndStructure();
        }

        private abstract class PropertyParser
        {
            protected readonly JsonParser parser;

            /// <summary>
            /// Set to True if OutputStructure should be returned immediately without parsing anymore properties.
            /// </summary>
            public bool ReturnImmediately { get; protected set; }

            public PropertyParser NextPropertyParser { get; protected set; }

            protected PropertyParser(JsonParser parser)
            {
                this.parser = parser;
            }

            /// <summary>
            /// Parse the property value for the property with the given name
            /// and return the next PropertyParser to use.
            /// </summary>
            public abstract void ParsePropertyValue(string name);
        }

        private class FirstPropertyParser : PropertyParser
        {
            public FirstPropertyParser(JsonParser parser)
                : base(parser)
            { }

            public override void ParsePropertyValue(string name)
            {
                if (name == "_ref")
                {
                    parser.ReferenceObject();
                    parser.ExpectSymbol('}');
                    ReturnImmediately = true;
                }
                else if (name == "_type")
                {
                    parser.BeginTypedStructure();
                    NextPropertyParser = new RegularPropertyParser(parser);
                }
                else
                {
                    parser.BeginStructure();
                    NextPropertyParser = new RegularPropertyParser(parser);
                    NextPropertyParser.ParsePropertyValue(name);
                }
            }
        }

        private class RegularPropertyParser : PropertyParser
        {
            public RegularPropertyParser(JsonParser parser)
                : base(parser)
            {
                NextPropertyParser = this;
            }

            public override void ParsePropertyValue(string name)
            {
                parser.writer.AddProperty(name);
                parser.ParseValue();
            }
        }

        private void ReferenceObject()
        {
            int referenceId = Convert.ToInt32(GetNumber());
            writer.WriteReference(referenceId);
        }

        private void BeginStructure()
        {
            writer.BeginStructure(GetType());
        }

        private void BeginTypedStructure()
        {
            string typeIdentifier = GetString();
            writer.BeginStructure(typeIdentifier, GetType());
        }

        private string GetString()
        {
            // Don't keep opening char
            reader.MoveNext();

            while (reader.CurrentChar != '"')
            {
                AssertNotEndOfFile();

                if (reader.CurrentChar == '\\')
                {
                    // Skip '\' and keep next char
                    reader.MoveNext();
                }

                reader.KeepNextChar();
            }

            // Don't keep closing char
            reader.MoveNext();

            string value = reader.ExtractToken();

            NextToken();

            return value;
        }

        private double GetNumber()
        {
            bool decimalSeparator = false;
            do
            {
                reader.KeepNextChar();
            } while (!reader.EndOfFile && (char.IsNumber(reader.CurrentChar) || !decimalSeparator && (decimalSeparator = reader.CurrentChar == '.')));

            string number = reader.ExtractToken();

            double value = double.Parse(number);

            NextToken();

            return value;
        }

        private void ParseArray()
        {
            ExpectSymbol('[');

            writer.BeginSequence();

            if (!(reader.CurrentChar == ']'))
            {
                do
                {
                    ParseValue();
                } while (MoveNextIfSymbol(','));
            }

            ExpectSymbol(']');

            writer.EndSequence();
        }

        private void ExpectSymbol(char value)
        {
            if (reader.CurrentChar != value)
                throw new ExpectedToken(value, reader.CurrentChar, reader.CurrentLine, reader.CurrentPosition);

            reader.MoveNext();
            NextToken();
        }

        private bool MoveNextIfSymbol(char value)
        {
            bool match = reader.CurrentChar == value;

            if (match)
            {
                reader.MoveNext();
                NextToken();
            }

            return match;
        }

        private void NextToken()
        {
            while (!reader.EndOfFile && Char.IsWhiteSpace(reader.CurrentChar))
                reader.MoveNext();
        }

        private void AssertNotEndOfFile()
        {
            if (reader.EndOfFile)
                throw new UnexpectedEndOfFile(reader.CurrentLine, reader.CurrentPosition);
        }

        internal class ExpectedToken : ParseException
        {
            public ExpectedToken(char expectedToken, char actualToken, int line, int position)
                : base("Expected {0}.".FormatWith(expectedToken), actualToken.ToString(), line, position)
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

