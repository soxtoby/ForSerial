using System;
using System.Collections.Generic;
using System.Linq;

namespace json.Objects.TypeDefinitions
{
    public class DefaultTypeDefinition : TypeDefinition
    {
        protected DefaultTypeDefinition(Type type) : base(type) { }

        internal static DefaultTypeDefinition CreateDefaultTypeDefinition(Type type)
        {
            return new DefaultTypeDefinition(type);
        }

        public override Output ReadObject(object input, ReaderWriter valueFactory)
        {
            OutputStructure output = valueFactory.CreateStructure(input);

            foreach (KeyValuePair<PropertyDefinition, object> property in GetSerializableProperties(input, valueFactory.SerializeAllTypes))
            {
                valueFactory.ReadProperty(input, property.Key, output);
            }

            valueFactory.EndStructure();

            return output;
        }

        public override TypedObject CreateStructure()
        {
            return new TypedRegularObject(this);
        }

        private IEnumerable<KeyValuePair<PropertyDefinition, object>> GetSerializableProperties(object obj, bool serializeAllTypes)
        {
            return Properties.Values
                .Where(p => p.CanGet)
                .Select(p => new KeyValuePair<PropertyDefinition, object>(p, p.GetFrom(obj)))
                .Where(p => serializeAllTypes || ValueIsSerializable(p.Value));
        }
    }
}