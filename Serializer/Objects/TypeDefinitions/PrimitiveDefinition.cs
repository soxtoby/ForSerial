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

        public override bool IsDeserializable
        {
            get { return true; }
        }

        public override Output ReadObject(object input, ReaderWriter valueFactory)
        {
            return valueFactory.CreateValue(input);
        }

        public override Output CreateValue(object value)
        {
            switch (value.GetType().GetTypeCodeType())
            {
                case TypeCodeType.Object:
                    return new TypedObjectOutputStructure(value);

                case TypeCodeType.Boolean:
                    return (bool)value
                        ? TypedBoolean.True
                        : TypedBoolean.False;

                case TypeCodeType.String:
                    return new TypedString((string)value);

                case TypeCodeType.Number:
                    return new TypedNumber(System.Convert.ToDouble(value));

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}