using System;
using System.Reflection;

namespace json.Objects
{
    public class PropertyDefinitionBuilder
    {
        private readonly ObjectInterfaceProvider interfaceProvider;

        public PropertyDefinitionBuilder(ObjectInterfaceProvider interfaceProvider)
        {
            if (interfaceProvider == null) throw new ArgumentNullException("interfaceProvider");

            this.interfaceProvider = interfaceProvider;
        }

        public PropertyDefinition Build(PropertyInfo property)
        {
            return new PropertyDefinition(
                GetTypeDefinition(property),
                GetName(property),
                GetGetter(property),
                GetSetter(property),
                GetShouldForceTypeIdentifierSerialization(property),
                GetDeclaringTypeName(property),
                HasPublicGetter(property),
                HasPublicSetter(property));
        }

        private static TypeDefinition GetTypeDefinition(PropertyInfo property)
        {
            return TypeCache.GetTypeDefinition(property.PropertyType);
        }

        private static string GetName(PropertyInfo property)
        {
            return property.Name;
        }

        private GetMethod GetGetter(PropertyInfo property)
        {
            return property.GetGetMethod(true) == null ? null
                : interfaceProvider.GetPropertyGetter(property);
        }

        private SetMethod GetSetter(PropertyInfo property)
        {
            return property.GetSetMethod(true) == null ? null
                : interfaceProvider.GetPropertySetter(property);
        }

        private static bool GetShouldForceTypeIdentifierSerialization(PropertyInfo property)
        {
            bool forceTypeIdentifierSerialization = property.HasAttribute<SerializeTypeAttribute>();
            return forceTypeIdentifierSerialization;
        }

        private static string GetDeclaringTypeName(PropertyInfo property)
        {
            return property.DeclaringType.FullName;
        }

        private static bool HasPublicGetter(PropertyInfo property)
        {
            return property.GetGetMethod() != null;
        }

        private static bool HasPublicSetter(PropertyInfo property)
        {
            return property.GetSetMethod() != null;
        }
    }
}