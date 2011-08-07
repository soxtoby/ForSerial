using System;

namespace json.Objects
{
    internal class PrimitiveTypeDefinition : TypeDefinition
    {
        private PrimitiveTypeDefinition(Type type) : base(type) { }

        internal static PrimitiveTypeDefinition CreatePrimitiveTypeDefinition(Type type)
        {
            return IsPrimitiveType(type)
                ? new PrimitiveTypeDefinition(type)
                : null;
        }

        public static bool IsPrimitiveType(Type type)
        {
            return type.IsPrimitive
                || type == typeof(string);  // string is considered a primitive for purposes of JSON serialization
        }

        protected override bool DetermineIfDeserializable()
        {
            return true;
        }

        public override ParseValue ParseObject(object input, ParserValueFactory valueFactory)
        {
            return valueFactory.CreateValue(input);
        }

        public override ParseValue CreateValue(ParseValueFactory valueFactory, object value)
        {
            return valueFactory.CreateValue(value);
        }
    }
}