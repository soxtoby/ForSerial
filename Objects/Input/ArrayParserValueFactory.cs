namespace json.Objects
{
    public partial class ObjectParser
    {
        private class ArrayParserValueFactory : ObjectParserValueFactory
        {
            private readonly ParseArray array;

            public ArrayParserValueFactory(ObjectParser parser, ParseArray array)
                : base(parser)
            {
                this.array = array;
            }

            public override ParseValue Parse(object input)
            {
                ParseValue value = null;
                parser.UsingArrayContext(array, () => value = parser.ParseValue(input));
                return value;
            }
        }
    }
}