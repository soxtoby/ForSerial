using System;

namespace json.Objects.TypeDefinitions
{
    public class DefaultTypeDefinition : TypeDefinition
    {
        protected DefaultTypeDefinition(Type type) : base(type) { }

        internal static DefaultTypeDefinition CreateDefaultTypeDefinition(Type type)
        {
            return new DefaultTypeDefinition(type);
        }

        public override void Read(object input, ObjectReader reader, Writer writer, bool requestTypeIdentification)
        {
            if (reader.ReferenceStructure(input))
                return;

            if (reader.ShouldWriteTypeIdentification(requestTypeIdentification))
                writer.BeginStructure(CurrentTypeHandler.GetTypeIdentifier(Type), reader.GetType());
            else
                writer.BeginStructure(Type);

            foreach (PropertyDefinition property in Properties.Values)
            {
                if (property.CanGet)    // TODO Put serializable properties in an array in populate
                {
                    writer.AddProperty(property.Name);
                    object value = property.GetFrom(input);
                    property.Read(value, reader, writer);
                }
            }

            writer.EndStructure();
        }

        public override ObjectContainer CreateStructure()
        {
            return new DefaultObjectStructure(this);
        }
    }
}
