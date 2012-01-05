using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace json.Objects
{
    internal class ConstructorOnlyObject : TypedObjectBase
    {
        private readonly Dictionary<string, object> properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public ConstructorOnlyObject(TypeDefinition typeDef)
        {
            TypeDef = typeDef;
        }

        public override object Object
        {
            get
            {
                ConstructorInfo[] constructors = TypeDef.Type.GetConstructors();
                ConstructorInfo matchingConstructor = constructors.FirstOrDefault(ConstructorParametersMatchProperties);

                if (matchingConstructor == null)
                    throw new NoMatchingConstructor(TypeDef.Type, properties);

                object[] parameters = matchingConstructor.GetParameters()
                    .Select(GetParameterPropertyValue)
                    .ToArray();
                return matchingConstructor.Invoke(parameters);
            }
        }

        private object GetParameterPropertyValue(ParameterInfo parameter)
        {
            TypeDefinition typeDef = CurrentTypeHandler.GetTypeDefinition(parameter.ParameterType);
            return typeDef.ConvertToCorrectType(properties.Get(parameter.Name));
        }

        private bool ConstructorParametersMatchProperties(ConstructorInfo constructor)
        {
            return constructor.GetParameters()
                .All(CanBeAssignedFromProperty);
        }

        private bool CanBeAssignedFromProperty(ParameterInfo parameter)
        {
            return HavePropertyValue(parameter)
                ? ParameterTypeMatchesPropertyValue(parameter)
                : ParameterCanBeNull(parameter);
        }

        private bool HavePropertyValue(ParameterInfo parameter)
        {
            return properties.ContainsKey(parameter.Name)
                && properties[parameter.Name] != null;
        }

        private bool ParameterTypeMatchesPropertyValue(ParameterInfo parameter)
        {
            Type propertyValueType = properties[parameter.Name].GetType();
            TypeCodeType propertyValueTypeCodeType = propertyValueType.GetTypeCodeType();

            return propertyValueTypeCodeType == TypeCodeType.Object
                ? propertyValueType.CanBeCastTo(parameter.ParameterType)
                : propertyValueTypeCodeType == parameter.ParameterType.GetTypeCodeType();
        }

        private static bool ParameterCanBeNull(ParameterInfo parameter)
        {
            return parameter.ParameterType.IsByRef;
        }

        public override void AddProperty(string name, TypedValue value)
        {
            properties[name] = value.GetTypedValue();
        }

        internal class NoMatchingConstructor : Exception
        {
            public NoMatchingConstructor(Type type, Dictionary<string, object> properties)
                : base("Could not find a matching constructor for type {0} with properties {1}"
                    .FormatWith(type.FullName, BuildParameterList(properties)))
            {
            }

            private static string BuildParameterList(Dictionary<string, object> properties)
            {
                return properties
                    .Select(kv => "{0} [{1}]".FormatWith(kv.Key, GetTypeName(kv.Value)))
                    .Join(", ");
            }

            private static string GetTypeName(object value)
            {
                return value == null ? "null" : value.GetType().FullName;
            }
        }
    }
}