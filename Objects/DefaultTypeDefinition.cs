using System;
using System.Collections.Generic;

namespace json.Objects
{
    public class DefaultTypeDefinition : TypeDefinition
    {
        private DefaultTypeDefinition(Type type) : base(type) { }

        internal static DefaultTypeDefinition CreateDefaultTypeDefinition(Type type)
        {
            return new DefaultTypeDefinition(type);
        }

        public override ParseValue GetParseValue(ParseValueFactory valueFactory)
        {
            return valueFactory.CreateObject();
        }

        public override void ParseObject(object input, ParseValue output, ParserValueFactory valueFactory)
        {
            ParseObject obj = output as ParseObject;
            if (obj == null) return;    // should probably throw

            foreach (KeyValuePair<string, object> property in GetSerializableProperties(input, valueFactory.SerializeAllTypes))
            {
                ParseValue value = valueFactory.ParseProperty(obj, property.Key, property.Value);
                value.AddToObject(obj, property.Key);
            }
        }
    }
}