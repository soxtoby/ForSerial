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

            writer.BeginStructure();

            if (writeTypeIdentifier)
                writer.SetType(CurrentTypeHandler.GetTypeIdentifier(Type));

            foreach (KeyValuePair<PropertyDefinition, object> property in GetSerializableProperties(input, reader.SerializeAllTypes))
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

        private IEnumerable<KeyValuePair<PropertyDefinition, object>> GetSerializableProperties(object obj, bool serializeAllTypes)
        {
            return Properties.Values
                .Where(p => p.CanGet)
                .Select(p => new KeyValuePair<PropertyDefinition, object>(p, p.GetFrom(obj)))
                .Where(p => serializeAllTypes || ValueIsSerializable(p.Value));
        }
    }
}