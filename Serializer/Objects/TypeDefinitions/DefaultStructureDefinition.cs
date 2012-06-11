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

        public override void Read(object input, ObjectReader reader, Writer writer, PartialOptions optionsOverride)
        {
            if (ReferenceStructure(input, reader, optionsOverride))
                return;

            if (ShouldWriteTypeIdentifier(reader.Options, optionsOverride))
                writer.BeginStructure(CurrentTypeResolver.GetTypeIdentifier(Type), reader.GetType());
            else
                writer.BeginStructure(Type);

            for (int i = 0; i < AllSerializableProperties.Length; i++)
            {
                PropertyDefinition property = AllSerializableProperties[i];

                if (property.MatchesPropertyFilter(reader.Options))
                {
                    writer.AddProperty(property.Name);

                    reader.PropertyStack.Push(property);

                    object value = property.GetFrom(input);
                    property.Read(value, reader, writer);

                    reader.PropertyStack.Pop();
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
