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

            foreach (KeyValuePair<PropertyDefinition, object> property in GetSerializableProperties(input, valueFactory.SerializeAllTypes))
            {
                valueFactory.ParseProperty(input, property.Key, output);
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
                .Where(p => serializeAllTypes || ShouldSerializeProperty(p))
                .Select(p => new KeyValuePair<PropertyDefinition, object>(p, p.GetFrom(obj)))
                .Where(p => serializeAllTypes || ValueIsSerializable(p.Value));
        }

        protected virtual bool ShouldSerializeProperty(PropertyDefinition property)
        {
            return property.IsSerializable;
        }
    }
}