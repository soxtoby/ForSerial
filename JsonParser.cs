using System.Collections.Generic;

namespace json
{
    internal class JsonParser
    {
        private IEnumerator<Token> tokenEnumerator;
        private ParseValueFactory valueFactory;

        private JsonParser(ParseValueFactory valueFactory)
        {
            this.valueFactory = valueFactory;
        }

        internal static ParseObject Parse(IEnumerable<Token> tokens, ParseValueFactory valueFactory)
        {
            JsonParser parser = new JsonParser(valueFactory);
            return parser.ParseTokens(tokens);
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
                        obj.SetTypeIdentifier(ParseString());
                    else
                        ParseValue().AddToObject(obj, name);

                } while (MoveNextIfSymbol(","));
            }

            ExpectSymbol("}");

            return obj;
        }

        private string ParseString()
        {
            if (CurrentToken.TokenType != TokenType.String)
                throw new ParseException("Expected name.", CurrentToken);

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

