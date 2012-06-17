using System;
using System.Collections.Generic;
using System.Linq;
using ForSerial.Objects.TypeDefinitions;

namespace ForSerial.Objects
{
    public delegate TypeDefinition FactoryMethod(Type type);

    internal static class TypeDefinitionFactory
    {

        private static readonly List<FactoryMethod> DefaultFactoryMethods = new List<FactoryMethod>
            {
                DefaultStructureDefinition.CreateDefaultStructureDefinition,
                EnumerableDefinition.CreateEnumerableDefinition,
                ArrayDefinition.CreateArrayDefinition,
                CollectionDefinition.CreateCollectionDefinition,
                JsonDictionaryDefinition.CreateDictionaryDefinition,
                ValueTypeDefinition.CreateValueTypeDefinition,
                PrimitiveDefinition.CreatePrimitiveTypeDefinition,
                EnumDefinition.CreateEnumDefinition,
                NullableTypeDefinition.CreateNullableTypeDefinition,
            };

        public static TypeDefinition CreateTypeDefinition(Type type)
        {
            List<FactoryMethod> factoryMethods = type.GetCustomAttributes(typeof(TypeDefinitionFilterAttribute), true)
                .Cast<TypeDefinitionFilterAttribute>()
                .Aggregate((IEnumerable<FactoryMethod>)DefaultFactoryMethods, (filteredMethods, filter) => filter.Filter(filteredMethods))
                .ToList();

            TypeDefinition typeDef = null;
            int i = factoryMethods.Count;
            while (typeDef == null && i >= 0)
                typeDef = factoryMethods[--i](type);
            return typeDef;
        }
    }
}