using System.Collections.Generic;

namespace json.Json
{
    internal class JsonParser : Reader
    {
        private IEnumerator<Token> tokenEnumerator;
        private readonly List<OutputStructure> objectReferences = new List<OutputStructure>();

        private JsonParser(Writer writer) : base(writer) { }

        public static OutputStructure Parse(string json, Writer valueFactory)
        {
            try
            {
                return Parse(Scanner.Scan(json), valueFactory);
            }
            catch (ParseException e)
            {
                throw new ParseException(e, json);
            }
        }

        private static OutputStructure Parse(IEnumerable<Token> tokens, Writer valueFactory)
        {
            JsonParser parser = new JsonParser(valueFactory);
            return parser.ParseTokens(tokens);
        }

        public override OutputStructure ReadSubStructure(Writer subWriter)
        {
            MoveNextIfSymbol(",");
            return Parse(GetSubObjectTokens(), subWriter);
        }

        private IEnumerable<Token> GetSubObjectTokens()
        {
            yield return new Token("{", TokenType.Symbol, 0, 0);

            yield return tokenEnumerator.Current;

            while (tokenEnumerator.MoveNext())
                yield return tokenEnumerator.Current;
        }

        private OutputStructure ParseTokens(IEnumerable<Token> tokens)
        {
            using (tokenEnumerator = tokens.GetEnumerator())
            {
                NextToken();

                return CurrentToken.TokenType == TokenType.EOF
                    ? null
                    : ParseObject();
            }
        }

        private Output ParseValue()
        {
            switch (CurrentToken.TokenType)
            {
                case TokenType.Numeric:
                    return ParseNumber();

                case TokenType.String:
                    return ParseString();

                case TokenType.Word:
                    switch (CurrentToken.StringValue)
                    {
                        case "true":
                            NextToken();
                            return writer.Current.CreateValue(true);

                        case "false":
                            NextToken();
                            return writer.Current.CreateValue(false);

                        case "null":
                            NextToken();
                            return writer.Current.CreateValue(null);

                        default:
                            throw new ParseException("Expected value.", CurrentToken);
                    }

                case TokenType.Symbol:
                    switch (CurrentToken.StringValue)
                    {
                        case "{":
                            return ParseObject();
                        case "[":
                            return ParseArray();
                        default:
                            throw new ParseException("Expected value.", CurrentToken);
                    }

                default:
                    throw new ParseException("Expected value.", CurrentToken);
            }
        }

        private Output ParseNumber()
        {
            if (CurrentToken.TokenType != TokenType.Numeric)
                throw new ParseException("Expected number.", CurrentToken);

            Output number = writer.Current.CreateValue(CurrentToken.NumericValue);
            NextToken();
            return number;
        }

        private Output ParseString()
        {
            if (CurrentToken.TokenType != TokenType.String)
                throw new ParseException("Expected string.", CurrentToken);

            Output str = writer.Current.CreateValue(CurrentToken.StringValue);
            NextToken();
            return str;
        }

        private OutputStructure ParseObject()
        {
            ExpectSymbol("{");

            OutputStructure obj;

            if (IsSymbol("}"))
            {
                obj = writer.Current.CreateStructure();
            }
            else
            {
                PropertyParser propertyParser = new FirstPropertyParser(this);

                do
                {
                    string name = GetString();

                    ExpectSymbol(":");

                    propertyParser.ParsePropertyValue(name);

                    if (propertyParser.ReturnImmediately)
                        return propertyParser.OutputStructure;

                    propertyParser = propertyParser.NextPropertyParser;

                } while (MoveNextIfSymbol(","));

                obj = propertyParser.OutputStructure;
            }

            ExpectSymbol("}");

            return obj;
        }

        private abstract class PropertyParser
        {
            protected readonly JsonParser parser;

            /// <summary>
            /// The OutputStructure to be returned.
            /// </summary>
            public OutputStructure OutputStructure { get; protected set; }

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
                    OutputStructure = parser.ReferenceObject();
                    NextPropertyParser = new IgnorePropertyParser(parser, OutputStructure);
                    return;
                }

                OutputStructure = parser.writer.Current.CreateStructure();

                if (name == "_type")
                {
                    if (parser.SetObjectType(OutputStructure))
                        ReturnImmediately = true; // Object was pre-built
                    else
                        NextPropertyParser = new RegularPropertyParser(parser, OutputStructure);
                }
                else
                {
                    NextPropertyParser = new RegularPropertyParser(parser, OutputStructure);
                    NextPropertyParser.ParsePropertyValue(name);
                }

                parser.objectReferences.Add(OutputStructure);
            }
        }

        private class IgnorePropertyParser : PropertyParser
        {
            public IgnorePropertyParser(JsonParser parser, OutputStructure outputStructure)
                : base(parser)
            {
                NextPropertyParser = this;
                OutputStructure = outputStructure;
            }

            public override void ParsePropertyValue(string name) { }
        }

        private class RegularPropertyParser : PropertyParser
        {
            public RegularPropertyParser(JsonParser parser, OutputStructure outputStructure)
                : base(parser)
            {
                NextPropertyParser = this;
                OutputStructure = outputStructure;
            }

            public override void ParsePropertyValue(string name)
            {
                using (parser.UseObjectPropertyContext(OutputStructure, name))
                    parser.ParseValue().AddToStructure(OutputStructure, name);
            }
        }

        private OutputStructure ReferenceObject()
        {
            int referenceId = System.Convert.ToInt32(GetNumber());
            return writer.Current.CreateReference(objectReferences[referenceId]);
        }

        private bool SetObjectType(OutputStructure obj)
        {
            string typeIdentifier = GetString();
            return obj.SetType(typeIdentifier, this);
        }

        private string GetString()
        {
            if (CurrentToken.TokenType != TokenType.String)
                throw new ParseException("Expected string.", CurrentToken);

            string value = CurrentToken.StringValue;

            NextToken();

            return value;
        }

        private double GetNumber()
        {
            if (CurrentToken.TokenType != TokenType.Numeric)
                throw new ParseException("Expected number.", CurrentToken);

            double value = CurrentToken.NumericValue;

            NextToken();

            return value;
        }

        private SequenceOutput ParseArray()
        {
            ExpectSymbol("[");

            SequenceOutput array = writer.Current.CreateSequence();

            if (!IsSymbol("]"))
            {
                using (UseArrayContext(array))
                {
                    do
                    {
                        ParseValue().AddToSequence(array);
                    } while (MoveNextIfSymbol(","));
                }
            }

            ExpectSymbol("]");

            return array;
        }

        private void ExpectSymbol(string value)
        {
            if (!IsSymbol(value))
                throw new ParseException("Expected {0}.".FormatWith(value), CurrentToken);

            NextToken();
        }

        private bool MoveNextIfSymbol(string value)
        {
            bool match = IsSymbol(value);

            if (match)
                NextToken();

            return match;
        }

        private bool IsSymbol(string value)
        {
            return CurrentToken.TokenType == TokenType.Symbol && CurrentToken.StringValue == value;
        }

        private Token CurrentToken
        {
            get { return tokenEnumerator.Current; }
        }

        private void NextToken()
        {
            tokenEnumerator.MoveNext();
        }
    }
}

