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

        public override void ReadObject(object input, ObjectReader reader, Writer writer, bool writeTypeIdentifier)
        {
            reader.AddStructureReference(input);

            if (writeTypeIdentifier)
                writer.BeginStructure(CurrentTypeHandler.GetTypeIdentifier(Type), reader.GetType());
            else
                writer.BeginStructure(Type);

            foreach (KeyValuePair<PropertyDefinition, object> property in GetSerializableProperties(input))
            {
                writer.AddProperty(property.Key.Name);
                reader.Read(property.Value, property.Key.ShouldWriteTypeIdentifier(property.Value));
            }

            writer.EndStructure();
        }

        public override ObjectContainer CreateStructure()
        {
            return new DefaultObjectStructure(this);
        }

        private IEnumerable<KeyValuePair<PropertyDefinition, object>> GetSerializableProperties(object obj)
        {
            return Properties.Values
                .Where(p => p.CanGet)
                .Select(p => new KeyValuePair<PropertyDefinition, object>(p, p.GetFrom(obj)));
        }
    }
}
