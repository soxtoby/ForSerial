using System;
using System.Collections.Generic;

namespace json.Json
{
    internal class JsonParser
    {
        private readonly Writer writer;
        private IEnumerator<Token> tokenEnumerator;

        private JsonParser(Writer writer)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            this.writer = writer;
        }

        public static void Parse(string json, Writer writer)
        {
            try
            {
                Parse(Scanner.Scan(json), writer);
            }
            catch (ParseException e)
            {
                throw new ParseException(e, json);
            }
        }

        private static void Parse(IEnumerable<Token> tokens, Writer writer)
        {
            JsonParser parser = new JsonParser(writer);
            parser.ParseTokens(tokens);
        }

        public void ReadSubStructure(Writer subWriter)
        {
            MoveNextIfSymbol(",");
            Parse(GetSubObjectTokens(), subWriter);
        }

        private IEnumerable<Token> GetSubObjectTokens()
        {
            yield return new Token("{", TokenType.Symbol, 0, 0);

            yield return tokenEnumerator.Current;

            while (tokenEnumerator.MoveNext())
                yield return tokenEnumerator.Current;
        }

        private void ParseTokens(IEnumerable<Token> tokens)
        {
            using (tokenEnumerator = tokens.GetEnumerator())
            {
                NextToken();

                if (CurrentToken.TokenType != TokenType.EOF)
                    ParseValue();
            }
        }

        private void ParseValue()
        {
            switch (CurrentToken.TokenType)
            {
                case TokenType.Numeric:
                    ParseNumber();
                    return;

                case TokenType.String:
                    ParseString();
                    return;

                case TokenType.Word:
                    switch (CurrentToken.StringValue)
                    {
                        case "true":
                            NextToken();
                            writer.Write(true);
                            return;

                        case "false":
                            NextToken();
                            writer.Write(false);
                            return;

                        case "null":
                            NextToken();
                            writer.Write(null);
                            return;

                        default:
                            throw new ParseException("Expected value.", CurrentToken);
                    }

                case TokenType.Symbol:
                    switch (CurrentToken.StringValue)
                    {
                        case "{":
                            ParseObject();
                            return;
                        case "[":
                            ParseArray();
                            return;
                        default:
                            throw new ParseException("Expected value.", CurrentToken);
                    }

                default:
                    throw new ParseException("Expected value.", CurrentToken);
            }
        }

        private void ParseNumber()
        {
            if (CurrentToken.TokenType != TokenType.Numeric)
                throw new ParseException("Expected number.", CurrentToken);

            writer.Write(CurrentToken.NumericValue);
            NextToken();
        }

        private void ParseString()
        {
            if (CurrentToken.TokenType != TokenType.String)
                throw new ParseException("Expected string.", CurrentToken);

            writer.Write(CurrentToken.StringValue);
            NextToken();
        }

        private void ParseObject()
        {
            ExpectSymbol("{");

            if (IsSymbol("}"))
            {
                writer.BeginStructure();
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
                        return;

                    propertyParser = propertyParser.NextPropertyParser;

                } while (MoveNextIfSymbol(","));
            }

            ExpectSymbol("}");

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
                    NextPropertyParser = new IgnorePropertyParser(parser);
                    return;
                }

                parser.writer.BeginStructure();

                if (name == "_type")
                {
                    parser.SetObjectType();
                    //ReturnImmediately = true; // Object was pre-built // TODO reimplement prebuild
                    NextPropertyParser = new RegularPropertyParser(parser);
                }
                else
                {
                    NextPropertyParser = new RegularPropertyParser(parser);
                    NextPropertyParser.ParsePropertyValue(name);
                }

                //parser.objectReferences.Add(OutputStructure);//TODO reimplement object references
            }
        }

        private class IgnorePropertyParser : PropertyParser
        {
            public IgnorePropertyParser(JsonParser parser)
                : base(parser)
            {
                NextPropertyParser = this;
            }

            public override void ParsePropertyValue(string name) { }
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
            //int referenceId = Convert.ToInt32(GetNumber());
            //return writer.CreateReference(objectReferences[referenceId]);
        }

        private void SetObjectType()
        {
            string typeIdentifier = GetString();
            writer.SetType(typeIdentifier);
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

        private void ParseArray()
        {
            ExpectSymbol("[");

            writer.BeginSequence();

            if (!IsSymbol("]"))
            {
                do
                {
                    ParseValue();
                } while (MoveNextIfSymbol(","));
            }

            ExpectSymbol("]");

            writer.EndSequence();
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

