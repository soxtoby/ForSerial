using System;
using System.Collections.Generic;
using System.Linq;

namespace json.Objects
{
    public class DefaultTypeDefinition : TypeDefinition
    {
        protected DefaultTypeDefinition(Type type) : base(type) { }

        internal static DefaultTypeDefinition CreateDefaultTypeDefinition(Type type)
        {
            return new DefaultTypeDefinition(type);
        }

        public override ParseValue ParseObject(object input, ParserValueFactory valueFactory)
        {
            ParseObject output = valueFactory.CreateObject(input);

            foreach (KeyValuePair<PropertyDefinition, object> propertyValue in GetSerializableProperties(input, valueFactory.SerializeAllTypes))
            {
                valueFactory.ParseProperty(output, propertyValue.Key.Name, propertyValue.Key.TypeDef, propertyValue.Value);
            }

            return output;
        }

        public override TypedObjectParseObject CreateObject()
        {
            return new TypedObjectRegularObject(this);
        }

        private IEnumerable<KeyValuePair<PropertyDefinition, object>> GetSerializableProperties(object obj, bool serializeAllTypes)
        {
            return Properties.Values
                .Where(p => serializeAllTypes || p.IsSerializable)
                .Select(p => new KeyValuePair<PropertyDefinition, object>(p, p.GetFrom(obj)))
                .Where(p => serializeAllTypes || ValueIsSerializable(p.Value));
        }
    }
}