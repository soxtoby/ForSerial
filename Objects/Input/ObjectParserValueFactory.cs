﻿namespace json.Objects
{
    public partial class ObjectParser
    {
        private class ObjectParserValueFactory : ParserValueFactory
        {
            private readonly ObjectParser parser;

            public ObjectParserValueFactory(ObjectParser parser)
            {
                this.parser = parser;
            }

            public bool SerializeAllTypes
            {
                get { return parser.options.SerializeAllTypes; }
            }

            public ParseValue Parse(object input)
            {
                return parser.ParseValue(input);
            }

            public void ParseProperty(ParseObject owner, string propertyName, object propertyValue)
            {
                parser.UsingObjectPropertyContext(owner, propertyName, () =>
                    {
                        ParseValue value = Parse(propertyValue);
                        value.AddToObject(owner, propertyName);
                    });
            }

            public void ParseArrayItem(ParseArray array, object item)
            {
                parser.UsingArrayContext(array, () =>
                    {
                        ParseValue value = parser.ParseValue(item);
                        value.AddToArray(array);
                    });
            }

            public ParseObject CreateObject(object input)
            {
                ParseObject obj = parser.ValueFactory.CreateObject();
                obj.SetType(parser.GetTypeIdentifier(input.GetType()), parser);
                parser.objectReferences[input] = obj;
                return obj;
            }

            public ParseValue CreateValue(object value)
            {
                return parser.ValueFactory.CreateValue(value);
            }

            public ParseObject CreateObject()
            {
                return parser.ValueFactory.CreateObject();
            }

            public ParseArray CreateArray()
            {
                return parser.ValueFactory.CreateArray();
            }

            public ParseObject CreateReference(ParseObject parseObject)
            {
                return parser.ValueFactory.CreateReference(parseObject);
            }
        }
    }
}