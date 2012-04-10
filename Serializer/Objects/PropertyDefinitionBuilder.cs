﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            PropertyDefinition propDef = new DefaultPropertyDefinition(
                GetTypeDefinition(property),
                GetName(property),
                GetGetter(property),
                GetSetter(property),
                GetDeclaringTypeName(property),
                HasPublicGetter(property),
                HasPublicSetter(property));

            IEnumerable<PropertyDefinitionAttribute> propertyDefinitionAttributes = property.GetCustomAttributes(false).OfType<PropertyDefinitionAttribute>();
            foreach (PropertyDefinitionAttribute attribute in propertyDefinitionAttributes)
            {
                attribute.InnerDefinition = propDef;
                propDef = attribute;
            }

            return propDef;
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