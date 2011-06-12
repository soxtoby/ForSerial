using System.Collections.Generic;

namespace json
{
    internal class JsonParser
    {
        private IEnumerator<Token> tokenEnumerator;

        internal static JsonDictionary Parse(IEnumerable<Token> tokens)
        {
            JsonParser parser = new JsonParser();
            return parser.ParseTokens(tokens);
        }

        private JsonDictionary ParseTokens(IEnumerable<Token> tokens)
        {
            using (tokenEnumerator = tokens.GetEnumerator())
            {
                NextToken();

                if (CurrentToken.TokenType == TokenType.EOF)
                    return null;

                return ParseObject();
            }
        }

        private object ParseValue()
        {
            object value;
            switch (CurrentToken.TokenType)
            {
                case TokenType.Numeric:
                    value = CurrentToken.NumericValue;
                    NextToken();
                    return value;

                case TokenType.String:
                    value = CurrentToken.StringValue;
                    NextToken();
                    return value;

                case TokenType.Word:
                    switch (CurrentToken.StringValue)
                    {
                        case "true":
                            NextToken();
                            return true;

                        case "false":
                            NextToken();
                            return false;

                        case "null":
                            NextToken();
                            return null;

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

        private JsonDictionary ParseObject()
        {
            ExpectSymbol("{");

            JsonDictionary obj = new JsonDictionary();

            if (!IsSymbol("}"))
            {
                do
                {
                    string name = ParseName();

                    ExpectSymbol(":");

                    obj[name] = ParseValue();

                } while (MoveNextIfSymbol(","));
            }

            ExpectSymbol("}");

            return obj;
        }

        private string ParseName()
        {
            if (CurrentToken.TokenType != TokenType.String)
                throw new ParseException("Expected name.", CurrentToken);

            string value = CurrentToken.StringValue;

            NextToken();

            return value;
        }

        private List<object> ParseArray()
        {
            ExpectSymbol("[");

            List<object> array = new List<object>();

            if (!IsSymbol("]"))
            {
                do
                {
                    array.Insert(array.Count, ParseValue());
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

