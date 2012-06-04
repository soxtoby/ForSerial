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
            if (ReferenceStructure(input, reader))
                return;

            if (reader.ShouldWriteTypeIdentification(requestTypeIdentification))
                writer.BeginStructure(CurrentTypeResolver.GetTypeIdentifier(Type), reader.GetType());
            else
                writer.BeginStructure(Type);

            for (int i = 0; i < AllSerializableProperties.Length; i++)
            {
                PropertyDefinition property = AllSerializableProperties[i];

                if (property.MatchesPropertyFilter(reader.MemberAccessibility, reader.MemberType))
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

        protected virtual bool ReferenceStructure(object input, ObjectReader reader)
        {
            return reader.ReferenceStructure(input);
        }

        public override ObjectContainer CreateStructure()
        {
            return new DefaultObjectStructure(this);
        }
    }
}
