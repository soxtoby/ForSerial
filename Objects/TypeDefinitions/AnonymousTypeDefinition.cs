using System;
using System.Runtime.CompilerServices;

namespace json.Objects
{
    public class AnonymousTypeDefinition : DefaultTypeDefinition
    {
        public AnonymousTypeDefinition(Type type)
            : base(type)
        {
        }

        internal static AnonymousTypeDefinition CreateAnonymousTypeDefinition(Type type)
        {
            Boolean isAnonymousType = NameContainsAnonymousType(type) && HasCompilerGeneratedAttribute(type);
            return isAnonymousType
                ? new AnonymousTypeDefinition(type) 
                : null;
        }

        private static bool NameContainsAnonymousType(Type type)
        {
            return type.FullName.Contains("AnonymousType");
        }

        private static bool HasCompilerGeneratedAttribute(Type type)
        {
            return type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0;
        }

        protected override bool DetermineIfDeserializable()
        {
            return true;
        }

        public override TypedObjectParseObject CreateObject()
        {
            return new TypedObjectConstructorOnlyObject(this);
        }

        protected override bool ShouldSerializeProperty(PropertyDefinition property)
        {
            return true;
        }

        public override ParseValue CreateValue(ParseValueFactory valueFactory, object value)
        {
            return value.GetType() == Type
                ? new TypedObjectObject(value)
                : base.CreateValue(valueFactory, value);
        }
    }
}