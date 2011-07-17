using System;
using System.Collections.Generic;

namespace json.Json
{
    internal class JsonParser : Parser
    {
        private IEnumerator<Token> tokenEnumerator;
        private readonly List<ParseObject> objectReferences = new List<ParseObject>();

        private JsonParser(ParseValueFactory valueFactory) : base(valueFactory) { }

        public static ParseObject Parse(string json, ParseValueFactory valueFactory)
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

        private static ParseObject Parse(IEnumerable<Token> tokens, ParseValueFactory valueFactory)
        {
            JsonParser parser = new JsonParser(valueFactory);
            return parser.ParseTokens(tokens);
        }

        public override ParseObject ParseSubObject(ParseValueFactory subParseValueFactory)
        {
            MoveNextIfSymbol(",");
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
                            return ValueFactory.CreateBoolean(true);

                        case "false":
                            NextToken();
                            return ValueFactory.CreateBoolean(false);

                        case "null":
                            NextToken();
                            return ValueFactory.CreateNull();

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

            ParseNumber number = ValueFactory.CreateNumber(CurrentToken.NumericValue);
            NextToken();
            return number;
        }

        private ParseValue ParseString()
        {
            if (CurrentToken.TokenType != TokenType.String)
                throw new ParseException("Expected string.", CurrentToken);

            ParseString str = ValueFactory.CreateString(CurrentToken.StringValue);
            NextToken();
            return str;
        }

        private ParseObject ParseObject()
        {
            ExpectSymbol("{");

            ParseObject obj;

            if (IsSymbol("}"))
            {
                obj = ValueFactory.CreateObject();
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
            protected readonly JsonParser parser;

            /// <summary>
            /// The ParseObject to be returned.
            /// </summary>
            public ParseObject ParseObject { get; protected set; }

            /// <summary>
            /// Set to True if ParseObject should be returned immediately without parsing anymore properties.
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
                    ParseObject = parser.ReferenceObject();
                    NextPropertyParser = new IgnorePropertyParser(parser, ParseObject);
                    return;
                }

                ParseObject = parser.ValueFactory.CreateObject();

                if (name == "_type")
                {
                    if (parser.SetObjectType(ParseObject))
                        ReturnImmediately = true; // Object was pre-built
                    else
                        NextPropertyParser = new RegularPropertyParser(parser, ParseObject);
                }
                else
                {
                    NextPropertyParser = new RegularPropertyParser(parser, ParseObject);
                    NextPropertyParser.ParsePropertyValue(name);
                }

                parser.objectReferences.Add(ParseObject);
            }
        }

        private class IgnorePropertyParser : PropertyParser
        {
            public IgnorePropertyParser(JsonParser parser, ParseObject parseObject)
                : base(parser)
            {
                NextPropertyParser = this;
                ParseObject = parseObject;
            }

            public override void ParsePropertyValue(string name) { }
        }

        private class RegularPropertyParser : PropertyParser
        {
            public RegularPropertyParser(JsonParser parser, ParseObject parseObject)
                : base(parser)
            {
                NextPropertyParser = this;
                ParseObject = parseObject;
            }

            public override void ParsePropertyValue(string name)
            {
                parser.UsingObjectPropertyContext(ParseObject, name,
                    () => parser.ParseValue().AddToObject(ParseObject, name)
                );
            }
        }

        private ParseObject ReferenceObject()
        {
            int referenceId = Convert.ToInt32(GetNumber());
            return ValueFactory.CreateReference(objectReferences[referenceId]);
        }

        private bool SetObjectType(ParseObject obj)
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

        private ParseArray ParseArray()
        {
            ExpectSymbol("[");

            ParseArray array = ValueFactory.CreateArray();

            if (!IsSymbol("]"))
            {
                UsingArrayContext(array, () =>
                {
                    do
                    {
                        ParseValue().AddToArray(array);
                    } while (MoveNextIfSymbol(","));
                });
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

