using System;

namespace ForSerial.Objects.TypeDefinitions
{
    public class ValueTypeDefinition : DefaultStructureDefinition
    {
        private ValueTypeDefinition(Type type) : base(type) { }

        internal static ValueTypeDefinition CreateValueTypeDefinition(Type type)
        {
            return type.IsValueType
                ? new ValueTypeDefinition(type)
                : null;
        }

        public override ObjectContainer CreateStructure()
        {
            return new DefaultObjectStructure(this);
        }

        public override bool CanCreateValue(object value)
        {
            return value != null
                && value.GetType() == Type;
        }

        public override void Read(object input, ObjectReader reader, Writer writer, bool requestTypeIdentification)
        {
            if (writer.CanWrite(input))
                writer.Write(input);
            else
                base.Read(input, reader, writer, requestTypeIdentification);
        }

        public override ObjectOutput CreateValue(object value)
        {
            return new DefaultObjectValue(ConvertToCorrectType(value));
        }
    }
}