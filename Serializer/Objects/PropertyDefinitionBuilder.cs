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
                GetShouldForceTypeIdentifierSerialization(property));
        }

        private static TypeDefinition GetTypeDefinition(PropertyInfo property)
        {
            return CurrentTypeHandler.GetTypeDefinition(property.PropertyType);
        }

        private static string GetName(PropertyInfo property)
        {
            return property.Name;
        }

        private GetMethod GetGetter(PropertyInfo property)
        {
            return interfaceProvider.GetPropertyGetter(property);
        }

        private SetMethod GetSetter(PropertyInfo property)
        {
            return interfaceProvider.GetPropertySetter(property);
        }

        private static bool GetShouldForceTypeIdentifierSerialization(PropertyInfo property)
        {
            bool forceTypeIdentifierSerialization = property.HasAttribute<SerializeTypeAttribute>();
            return forceTypeIdentifierSerialization;
        }
    }
}