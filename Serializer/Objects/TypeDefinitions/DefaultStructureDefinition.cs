using System;

namespace ForSerial.Objects.TypeDefinitions
{
    public class DefaultStructureDefinition : StructureDefinition
    {
        protected DefaultStructureDefinition(Type type) : base(type) { }

        internal static DefaultStructureDefinition CreateDefaultStructureDefinition(Type type)
        {
            return new DefaultStructureDefinition(type);
        }

        public override void Read(object input, ObjectReader reader, Writer writer, bool requestTypeIdentification)
        {
            if (reader.ReferenceStructure(input))
                return;

            if (reader.ShouldWriteTypeIdentification(requestTypeIdentification))
                writer.BeginStructure(CurrentTypeResolver.GetTypeIdentifier(Type), reader.GetType());
            else
                writer.BeginStructure(Type);

            var filteredProperties = reader.PropertyFilter == PropertyFilter.PublicGetSet
                ? PublicGetSetProperties
                : AllSerializableProperties;

            for (int i = 0; i < filteredProperties.Length; i++)
            {
                PropertyDefinition property = filteredProperties[i];
                writer.AddProperty(property.Name);

                reader.PropertyStack.Push(property);

                object value = property.GetFrom(input);
                property.Read(value, reader, writer);

                reader.PropertyStack.Pop();
            }

            writer.EndStructure();
        }

        public override ObjectContainer CreateStructure()
        {
            return new DefaultObjectStructure(this);
        }
    }
}
