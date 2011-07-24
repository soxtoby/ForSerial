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

        public override ParseValue ParseObject(object input, ParserValueFactory valueFactory)
        {
            ParseObject output = valueFactory.CreateObject(input);

            foreach (KeyValuePair<string, object> property in GetSerializableProperties(input, valueFactory.SerializeAllTypes))
            {
                valueFactory.ParseProperty(output, property.Key, property.Value);
            }

            return output;
        }
    }
}