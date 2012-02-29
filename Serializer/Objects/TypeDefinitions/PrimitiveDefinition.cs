using System;

namespace json.Objects.TypeDefinitions
{
    internal class PrimitiveDefinition : TypeDefinition
    {
        private PrimitiveDefinition(Type type) : base(type) { }

        internal static PrimitiveDefinition CreatePrimitiveTypeDefinition(Type type)
        {
            return IsPrimitiveType(type)
                ? new PrimitiveDefinition(type)
                : null;
        }

        private static bool IsPrimitiveType(Type type)
        {
            return type.IsPrimitive
                || type == typeof(string);  // string is considered a primitive for purposes of JSON serialization
        }

        public override void ReadObject(object input, ObjectReader reader, Writer writer, bool writeTypeIdentifier)
        {
            writer.Write(input);
        }

        public override ObjectValue CreateValue(object value)
        {
            return new DefaultObjectValue(value);
        }
    }
}