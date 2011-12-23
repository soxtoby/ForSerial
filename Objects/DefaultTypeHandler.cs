using System;
using System.Collections.Generic;

namespace json.Objects
{
    public class DefaultTypeHandler : TypeHandler
    {
        private DefaultTypeHandler() { }

        private static DefaultTypeHandler instance;
        public static DefaultTypeHandler Instance
        {
            get { return instance ?? (instance = new DefaultTypeHandler()); }
        }

        public string GetTypeIdentifier(Type type)
        {
            return type.AssemblyQualifiedName;
        }

        private readonly Dictionary<string, TypeDefinition> knownTypes = new Dictionary<string, TypeDefinition>();

        private readonly List<Func<Type, TypeDefinition>> typeDefinitionFactories = new List<Func<Type, TypeDefinition>>
            {
                DefaultTypeDefinition.CreateDefaultTypeDefinition,
                AnonymousTypeDefinition.CreateAnonymousTypeDefinition,
                CollectionDefinition.CreateCollectionDefinition,
                DictionaryDefinition.CreateDictionaryDefinition,
                JsonDictionaryDefinition.CreateDictionaryDefinition,
                ValueTypeDefinition.CreateValueTypeDefinition,
                PrimitiveTypeDefinition.CreatePrimitiveTypeDefinition,
                GuidDefinition.CreateGuidDefinition,
            };

        public TypeDefinition GetTypeDefinition(string assemblyQualifiedName)
        {
            return knownTypes.ContainsKey(assemblyQualifiedName)
                ? knownTypes[assemblyQualifiedName]
                : GetTypeDefinition(Type.GetType(assemblyQualifiedName));
        }

        public TypeDefinition GetTypeDefinition(Type type)
        {
            if (type == null) return null;

            if (!knownTypes.ContainsKey(GetTypeIdentifier(type)))
            {
                // Since this is where we automatically create a TypeDefinition, 
                // we need to register before we populate, in case the type contains itself.
                TypeDefinition typeDef = CreateTypeDefinition(type);
                RegisterTypeDefinition(typeDef);
                typeDef.Populate();
            }

            return knownTypes[GetTypeIdentifier(type)];
        }

        private void RegisterTypeDefinition(TypeDefinition typeDef)
        {
            knownTypes[GetTypeIdentifier(typeDef.Type)] = typeDef;
        }

        private TypeDefinition CreateTypeDefinition(Type type)
        {
            TypeDefinition typeDef = null;
            int i = typeDefinitionFactories.Count;
            while (typeDef == null && i >= 0)
                typeDef = typeDefinitionFactories[--i](type);
            return typeDef;
        }
    }
}