
namespace json.Objects
{
    public partial class ObjectParser
    {
        private class ObjectParserValueFactory : ParserValueFactory
        {
            private readonly ObjectParser parser;
            private readonly bool setType;

            public ObjectParserValueFactory(ObjectParser parser, bool setType = true)
            {
                this.parser = parser;
                this.setType = setType;
            }

            public bool SerializeAllTypes
            {
                get { return parser.options.SerializeAllTypes; }
            }

            public void ParseProperty(object source, PropertyDefinition property, ParseObject target)
            {
                bool serializeTypeIdentifier = !property.TypeDef.IsSerializable || property.ForceTypeIdentifierSerialization;
                ParseProperty(target, property.Name, property.GetFrom(source), serializeTypeIdentifier);
            }

            public void ParseProperty(TypeDefinition propertyTypeDef, string propertyName, object propertyValue, ParseObject target)
            {
                bool serializeTypeIdentifier = !propertyTypeDef.IsSerializable;
                ParseProperty(target, propertyName, propertyValue, serializeTypeIdentifier);
            }

            private void ParseProperty(ParseObject target, string propertyName, object propertyValue, bool serializeTypeIdentifier)
            {
                using (parser.UseObjectPropertyContext(target, propertyName))
                using (parser.parserValueFactory.OverrideState(new ObjectParserValueFactory(parser, serializeTypeIdentifier)))
                {
                    ParseValue value = parser.ParseValue(propertyValue);
                    value.AddToObject(target, propertyName);
                }
            }

            public void ParseArrayItem(ParseArray array, object item)
            {
                using (parser.UseArrayContext(array))
                {
                    ParseValue value = parser.ParseValue(item);
                    value.AddToArray(array);
                }
            }

            public ParseObject CreateObject(object input)
            {
                ParseObject obj = parser.valueFactory.Current.CreateObject();
                if (parser.options.SerializeAllTypeInformation || setType)
                    obj.SetType(CurrentTypeHandler.GetTypeIdentifier(input.GetType()), parser);
                parser.objectReferences[input] = obj;
                return obj;
            }

            public ParseValue CreateValue(object value)
            {
                return parser.valueFactory.Current.CreateValue(value);
            }

            public ParseObject CreateObject()
            {
                return parser.valueFactory.Current.CreateObject();
            }

            public ParseArray CreateArray()
            {
                return parser.valueFactory.Current.CreateArray();
            }

            public ParseObject CreateReference(ParseObject parseObject)
            {
                return parser.valueFactory.Current.CreateReference(parseObject);
            }
        }
    }
}