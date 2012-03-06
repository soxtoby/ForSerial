using System;
using System.Collections.Generic;
using json.Objects.TypeDefinitions;

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

        private readonly Dictionary<string, TypeDefinition> knownTypesByIdentifier = new Dictionary<string, TypeDefinition>();
        private readonly Dictionary<Type, TypeDefinition> knownTypesByType = new Dictionary<Type, TypeDefinition>();

        private readonly List<Func<Type, TypeDefinition>> typeDefinitionFactories = new List<Func<Type, TypeDefinition>>
            {
                DefaultStructureDefinition.CreateDefaultStructureDefinition,
                EnumerableDefinition.CreateEnumerableDefinition,
                CollectionDefinition.CreateCollectionDefinition,
                JsonDictionaryDefinition.CreateDictionaryDefinition,
                ValueTypeDefinition.CreateValueTypeDefinition,
                PrimitiveDefinition.CreatePrimitiveTypeDefinition,
                NullableTypeDefinition.CreateNullableTypeDefinition,
                GuidDefinition.CreateGuidDefinition,
                DateTimeDefinition.CreateDateTimeDefinition,    // TODO why not just put these in the dictionary directly?
            };

        public TypeDefinition GetTypeDefinition(string assemblyQualifiedName)
        {
            return knownTypesByIdentifier.ContainsKey(assemblyQualifiedName)
                ? knownTypesByIdentifier[assemblyQualifiedName]
                : GetTypeDefinition(Type.GetType(assemblyQualifiedName));
        }

        public TypeDefinition GetTypeDefinition(Type type)
        {
            if (type == null) return NullTypeDefinition.Instance;

            TypeDefinition typeDef;
            if (!knownTypesByType.TryGetValue(type, out typeDef))
            {
                // Since this is where we automatically create a TypeDefinition, 
                // we need to register before we populate, in case the type contains itself.
                typeDef = CreateTypeDefinition(type);
                RegisterTypeDefinition(typeDef);
                typeDef.Populate();
            }

            return typeDef;
        }

        private void RegisterTypeDefinition(TypeDefinition typeDef)
        {
            knownTypesByIdentifier[GetTypeIdentifier(typeDef.Type)] = typeDef;
            knownTypesByType[typeDef.Type] = typeDef;
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