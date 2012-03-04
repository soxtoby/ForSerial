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

        public override void ReadObject(object input, ObjectReader reader, Writer writer, bool writeTypeIdentifier)
        {
            reader.AddStructureReference(input);

            if (writeTypeIdentifier)
                writer.BeginStructure(CurrentTypeHandler.GetTypeIdentifier(Type), reader.GetType());
            else
                writer.BeginStructure(Type);

            foreach (PropertyDefinition property in Properties.Values)
            {
                if (property.CanGet)
                {
                    writer.AddProperty(property.Name);
                    object value = property.GetFrom(input);
                    reader.Read(value, property.ShouldWriteTypeIdentifier(value));
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
