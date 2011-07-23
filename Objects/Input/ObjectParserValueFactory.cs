namespace json.Objects
{
    public partial class ObjectParser
    {
        private class ObjectParserValueFactory : ParserValueFactory
        {
            protected readonly ObjectParser parser;

            public ObjectParserValueFactory(ObjectParser parser)
            {
                this.parser = parser;
            }

            public bool SerializeAllTypes
            {
                get { return (parser.options & Options.SerializeAllTypes) != 0; }
            }

            public virtual ParseValue Parse(object input)
            {
                return parser.ParseValue(input);
            }

            public ParseValue ParseProperty(ParseObject owner, string propertyName, object propertyValue)
            {
                ParseValue value = null;
                parser.UsingObjectPropertyContext(owner, propertyName, () => value = Parse(propertyValue));
                return value;
            }

            public ParseObject CreateObject()
            {
                return parser.ValueFactory.CreateObject();
            }

            public ParseArray CreateArray()
            {
                return parser.ValueFactory.CreateArray();
            }

            public ParseNumber CreateNumber(double value)
            {
                return parser.ValueFactory.CreateNumber(value);
            }

            public ParseString CreateString(string value)
            {
                return parser.ValueFactory.CreateString(value);
            }

            public ParseBoolean CreateBoolean(bool value)
            {
                return parser.ValueFactory.CreateBoolean(value);
            }

            public ParseNull CreateNull()
            {
                return parser.ValueFactory.CreateNull();
            }

            public ParseObject CreateReference(ParseObject parseObject)
            {
                return parser.ValueFactory.CreateReference(parseObject);
            }
        }
    }
}