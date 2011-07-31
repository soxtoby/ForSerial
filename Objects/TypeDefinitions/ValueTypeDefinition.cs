using System;

namespace json.Objects
{
    public class ValueTypeDefinition : DefaultTypeDefinition
    {
        private ValueTypeDefinition(Type type) : base(type) { }

        internal static ValueTypeDefinition CreateValueTypeDefinition(Type type)
        {
            return type.IsValueType
                ? new ValueTypeDefinition(type)
                : null;
        }

        protected override bool DetermineIfDeserializable()
        {
            return true;
        }

        public override TypedObjectParseObject CreateObject()
        {
            return new TypedObjectConstructorOnlyObject(this);
        }

        public override ParseValue CreateValue(ParseValueFactory valueFactory, object value)
        {
            return value.GetType() == Type
                ? new TypedObjectObject(value)
                : base.CreateValue(valueFactory, value);
        }
    }
}