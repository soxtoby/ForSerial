using System;

namespace ForSerial.Objects.TypeDefinitions
{
    internal class EnumDefinition : TypeDefinition
    {
        private EnumDefinition(Type type) : base(type) { }

        internal static EnumDefinition CreateEnumDefinition(Type type)
        {
            return type.CanBeCastTo(typeof(Enum))
                ? new EnumDefinition(type)
                : null;
        }

        public override void Read(object input, ObjectReader reader, Writer writer, PartialOptions optionsOverride)
        {
            writer.Write((int)input);
        }

        public override ObjectOutput CreateValue(object value)
        {
            return new DefaultObjectValue(Enum.ToObject(Type, ConvertToCorrectType(value)));
        }
    }
}
