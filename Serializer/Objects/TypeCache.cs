using System;
using System.Collections.Generic;
using ForSerial.Objects.TypeDefinitions;

namespace ForSerial.Objects
{
    public static class TypeCache
    {
        static TypeCache()
        {
            RegisterTypeDefinition(ObjectDefinition.Instance);
            RegisterTypeDefinition(GuidDefinition.Instance);
            RegisterTypeDefinition(DateTimeDefinition.Instance);
        }

        private static readonly Dictionary<string, TypeDefinition> KnownTypesByIdentifier = new Dictionary<string, TypeDefinition>();
        private static readonly Dictionary<Type, TypeDefinition> KnownTypesByType = new Dictionary<Type, TypeDefinition>();

        public static TypeDefinition GetTypeDefinition(string assemblyQualifiedName)
        {
            return KnownTypesByIdentifier.ContainsKey(assemblyQualifiedName)
                ? KnownTypesByIdentifier[assemblyQualifiedName]
                : GetTypeDefinition(CurrentTypeResolver.GetType(assemblyQualifiedName));
        }

        public static TypeDefinition GetTypeDefinition(object value)
        {
            return GetTypeDefinition(value == null ? null : value.GetType());
        }

        public static TypeDefinition GetTypeDefinition(Type type)
        {
            if (type == null) return NullTypeDefinition.Instance;

            TypeDefinition typeDef;
            if (!KnownTypesByType.TryGetValue(type, out typeDef))
            {
                // Since this is where we automatically create a TypeDefinition, 
                // we need to register before we populate, in case the type contains itself.
                typeDef = TypeDefinitionFactory.CreateTypeDefinition(type);
                RegisterTypeDefinition(typeDef);
                typeDef.Populate();
            }

            return typeDef;
        }

        public static void RegisterTypeDefinition(TypeDefinition typeDef)
        {
            KnownTypesByIdentifier[CurrentTypeResolver.GetTypeIdentifier(typeDef.Type)] = typeDef;
            KnownTypesByType[typeDef.Type] = typeDef;
        }
    }
}