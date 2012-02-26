using System;

namespace json.Objects.TypeDefinitions
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

        public override ObjectContainer CreateStructure()
        {
            return new DefaultObjectStructure(this);
        }

        public override bool CanCreateValue(object value)
        {
            return value != null
                && value.GetType() == Type;
        }

        public override ObjectValue CreateValue(object value)
        {
            return new DefaultObjectValue(value);
        }
    }
}