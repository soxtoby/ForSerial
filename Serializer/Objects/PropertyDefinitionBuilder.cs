using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ForSerial.Objects
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
                GetAccessibility(property),
                MemberType.Property);

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

        private static MemberAccessibility GetAccessibility(PropertyInfo property)
        {
            if (property.GetGetMethod() != null)
            {
                if (property.GetSetMethod() != null)
                    return MemberAccessibility.PublicGetSet;
                return MemberAccessibility.PublicGet;
            }
            return MemberAccessibility.Private;
        }


        public PropertyDefinition Build(FieldInfo field)
        {
            PropertyDefinition propDef = new DefaultPropertyDefinition(
                GetTypeDefinition(field),
                GetName(field),
                GetGetter(field),
                GetSetter(field),
                GetDeclaringTypeName(field),
                GetAccessibility(field),
                MemberType.Field);

            return propDef;
        }

        private static TypeDefinition GetTypeDefinition(FieldInfo field)
        {
            return TypeCache.GetTypeDefinition(field.FieldType);
        }

        private static string GetName(FieldInfo field)
        {
            return field.Name;
        }

        private SetMethod GetSetter(FieldInfo field)
        {
            return interfaceProvider.GetFieldSetter(field);
        }

        private GetMethod GetGetter(FieldInfo field)
        {
            return interfaceProvider.GetFieldGetter(field);
        }

        private static string GetDeclaringTypeName(FieldInfo field)
        {
            return field.DeclaringType.FullName;
        }

        private static MemberAccessibility GetAccessibility(FieldInfo field)
        {
            if (field.IsPublic)
            {
                return field.IsInitOnly
                    ? MemberAccessibility.PublicGet
                    : MemberAccessibility.PublicGetSet;
            }
            return MemberAccessibility.Private;
        }
    }
}