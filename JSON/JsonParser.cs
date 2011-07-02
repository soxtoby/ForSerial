using System;
using System.Collections.Generic;

namespace json.Json
{
    internal class JsonParser : Parser
    {
        private IEnumerator<Token> tokenEnumerator;
        private readonly ParseValueFactory valueFactory;
        private readonly List<ParseObject> objectReferences = new List<ParseObject>();

        private JsonParser(ParseValueFactory valueFactory)
        {
            this.valueFactory = valueFactory;
        }

        public static ParseObject Parse(string json, ParseValueFactory valueFactory)
        {
            return Parse(Scanner.Scan(json), valueFactory);
        }

        private static ParseObject Parse(IEnumerable<Token> tokens, ParseValueFactory valueFactory)
        {
            JsonParser parser = new JsonParser(valueFactory);
            return parser.ParseTokens(tokens);
        }

        public ParseObject ParseSubObject(ParseValueFactory subParseValueFactory)
        {
            return Parse(GetSubObjectTokens(), subParseValueFactory);
        }

        private IEnumerable<Token> GetSubObjectTokens()
        {
            yield return new Token("{", TokenType.Symbol, 0, 0);

            yield return tokenEnumerator.Current;

            while (tokenEnumerator.MoveNext())
                yield return tokenEnumerator.Current;
        }

        private ParseObject ParseTokens(IEnumerable<Token> tokens)
        {
            using (tokenEnumerator = tokens.GetEnumerator())
            {
                NextToken();

                return CurrentToken.TokenType == TokenType.EOF
                    ? null
                    : ParseObject();
            }
        }

        private ParseValue ParseValue()
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
                            return valueFactory.CreateBoolean(true);

                        case "false":
                            NextToken();
                            return valueFactory.CreateBoolean(false);

                        case "null":
                            NextToken();
                            return valueFactory.CreateNull();

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

        private ParseValue ParseNumber()
        {
            if (CurrentToken.TokenType != TokenType.Numeric)
                throw new ParseException("Expected number.", CurrentToken);

            ParseNumber number = valueFactory.CreateNumber(CurrentToken.NumericValue);
            NextToken();
            return number;
        }

        private ParseValue ParseString()
        {
            if (CurrentToken.TokenType != TokenType.String)
                throw new ParseException("Expected string.", CurrentToken);

            ParseString str = valueFactory.CreateString(CurrentToken.StringValue);
            NextToken();
            return str;
        }

        private ParseObject ParseObject()
        {
            ExpectSymbol("{");

            ParseObject obj = valueFactory.CreateObject();

            if (!IsSymbol("}"))
            {
                PropertyParser propertyParser = new FirstPropertyParser(this, obj);

                do
                {
                    string name = GetString();

                    ExpectSymbol(":");

                    propertyParser.ParsePropertyValue(name);

                    if (propertyParser.ReturnImmediately)
                        return propertyParser.ParseObject;

                    propertyParser = propertyParser.NextPropertyParser;

                } while (MoveNextIfSymbol(","));

                obj = propertyParser.ParseObject;
            }

            ExpectSymbol("}");

            return obj;
        }

        private abstract class PropertyParser
        {
            protected readonly JsonParser Parser;

            /// <summary>
            /// The ParseObject to be returned.
            /// </summary>
            public ParseObject ParseObject { get; protected set; }

            /// <summary>
            /// Set to True if ParseObject should be returned immediately without parsing anymore properties.
            /// </summary>
            public bool ReturnImmediately { get; protected set; }

            public PropertyParser NextPropertyParser { get; protected set; }

            protected PropertyParser(JsonParser parser, ParseObject parseObject)
            {
                Parser = parser;
                ParseObject = parseObject;
            }

            /// <summary>
            /// Parse the property value for the property with the given name
            /// and return the next PropertyParser to use.
            /// </summary>
            public abstract void ParsePropertyValue(string name);
        }

        private class FirstPropertyParser : PropertyParser
        {
            public FirstPropertyParser(JsonParser parser, ParseObject parseObject)
                : base(parser, parseObject)
            { }

            public override void ParsePropertyValue(string name)
            {
                switch (name)
                {
                    case "_ref":
                        ParseObject = Parser.ReferenceObject();
                        NextPropertyParser = new IgnorePropertyParser(Parser, ParseObject);
                        return; // Don't add reference to objectReferences

                    case "_type":
                        if (Parser.SetObjectType(ParseObject))
                            ReturnImmediately = true;   // Object was pre-built
                        break;

                    default:
                        NextPropertyParser = new RegularPropertyParser(Parser, ParseObject);
                        NextPropertyParser.ParsePropertyValue(name);
                        break;
                }

                Parser.objectReferences.Add(ParseObject);
            }
        }

        private class IgnorePropertyParser : PropertyParser
        {
            public IgnorePropertyParser(JsonParser parser, ParseObject parseObject)
                : base(parser, parseObject)
            {
                NextPropertyParser = this;
            }

            public override void ParsePropertyValue(string name) { }
        }

        private class RegularPropertyParser : PropertyParser
        {
            public RegularPropertyParser(JsonParser parser, ParseObject parseObject)
                : base(parser, parseObject)
            {
                NextPropertyParser = this;
            }

            public override void ParsePropertyValue(string name)
            {
                Parser.ParseValue().AddToObject(ParseObject, name);
            }
        }

        private ParseObject ReferenceObject()
        {
            int referenceId = Convert.ToInt32(GetNumber());
            return valueFactory.CreateReference(objectReferences[referenceId]);
        }

        private bool SetObjectType(ParseObject obj)
        {
            string typeIdentifier = GetString();
            MoveNextIfSymbol(",");
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

        private ParseArray ParseArray()
        {
            ExpectSymbol("[");

            ParseArray array = valueFactory.CreateArray();

            if (!IsSymbol("]"))
            {
                do
                {
                    ParseValue().AddToArray(array);
                } while (MoveNextIfSymbol(","));
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

