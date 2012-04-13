using System;

namespace ForSerial.Objects.TypeDefinitions
{
    internal class NullableTypeDefinition : TypeDefinition
    {
        private readonly TypeDefinition underlyingTypeDef;

        private NullableTypeDefinition(Type type, Type underlyingType)
            : base(type)
        {
            if (underlyingType == null) throw new ArgumentNullException("underlyingType");

            underlyingTypeDef = TypeCache.GetTypeDefinition(underlyingType);
        }

        internal static NullableTypeDefinition CreateNullableTypeDefinition(Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType == null ? null
                : new NullableTypeDefinition(type, underlyingType);
        }

        public override void Read(object input, ObjectReader reader, Writer writer, bool requestTypeIdentification)
        {
            underlyingTypeDef.Read(input, reader, writer, requestTypeIdentification);
        }

        public override ObjectOutput CreateValue(object value)
        {
            return value == null
                ? new DefaultObjectValue(null)
                : underlyingTypeDef.CreateValue(value);
        }
    }
}