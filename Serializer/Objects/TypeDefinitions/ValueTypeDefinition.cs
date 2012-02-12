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

        public override bool IsDeserializable
        {
            get { return true; }
        }

        public override TypedObject CreateStructure()
        {
            return new ConstructorOnlyObject(this);
        }

        public override Output CreateValue(object value)
        {
            return new TypedObjectOutputStructure(value);
        }
    }
}