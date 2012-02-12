using System;
using System.Runtime.CompilerServices;

namespace json.Objects.TypeDefinitions
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

        public override bool IsDeserializable
        {
            get { return true; }
        }

        public override TypedObject CreateStructure()
        {
            return new ConstructorOnlyObject(this);
        }
    }
}