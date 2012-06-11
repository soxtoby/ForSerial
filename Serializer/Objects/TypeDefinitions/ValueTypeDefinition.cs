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

        protected override bool ReferenceStructure(object input, ObjectReader reader, PartialOptions optionsOverride)
        {
            return false;   // Can't reference value types
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

        public override void Read(object input, ObjectReader reader, Writer writer, PartialOptions optionsOverride)
        {
            if (writer.CanWrite(input))
                writer.Write(input);
            else
                base.Read(input, reader, writer, optionsOverride);
        }

        public override ObjectOutput CreateValue(object value)
        {
            return new DefaultObjectValue(ConvertToCorrectType(value));
        }
    }
}