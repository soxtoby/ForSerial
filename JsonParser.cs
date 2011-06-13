using System.Collections.Generic;

namespace json
{
    internal class JsonParser : Parser
    {
        private IEnumerator<Token> tokenEnumerator;
        private ParseValueFactory valueFactory;

        private JsonParser(ParseValueFactory valueFactory)
        {
            this.valueFactory = valueFactory;
        }

        public static ParseObject Parse(string json, ParseValueFactory valueFactory)
        {
            return Parse(Scanner.Scan(json), valueFactory);
        }

        internal static ParseObject Parse(IEnumerable<Token> tokens, ParseValueFactory valueFactory)
        {
            JsonParser parser = new JsonParser(valueFactory);
            return parser.ParseTokens(tokens);
        }

        public ParseObject ParseSubObject(ParseValueFactory valueFactory)
        {
            return Parse(GetSubObjectTokens(), valueFactory);
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
    
                if (CurrentToken.TokenType == TokenType.EOF)
                    return null;
    
                return ParseObject();
            }
        }

        private ParseValue ParseValue()
        {
            switch (CurrentToken.TokenType)
            {
                case TokenType.Numeric:
                    ParseNumber number = valueFactory.CreateNumber(CurrentToken.NumericValue);
                    NextToken();
                    return number;

                case TokenType.String:
                    ParseString str = valueFactory.CreateString(CurrentToken.StringValue);
                    NextToken();
                    return str;

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

        private ParseObject ParseObject()
        {
            ExpectSymbol("{");

            ParseObject obj = valueFactory.CreateObject();

            if (!IsSymbol("}"))
            {
                do
                {
                    string name = ParseString();

                    ExpectSymbol(":");

                    if (name == "_type")
                    {
                        if (SetObjectType(obj))
                            return obj; // Object was pre-built
                    }
                    else
                    {
                        ParseValue().AddToObject(obj, name);
                    }

                } while (MoveNextIfSymbol(","));
            }

            ExpectSymbol("}");

            return obj;
        }

        private bool SetObjectType(ParseObject obj)
        {
            string typeIdentifier = ParseString();
            MoveNextIfSymbol(",");
            return obj.SetType(typeIdentifier, this);
        }

        private string ParseString()
        {
            if (CurrentToken.TokenType != TokenType.String)
                throw new ParseException("Expected string.", CurrentToken);

            string value = CurrentToken.StringValue;

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

