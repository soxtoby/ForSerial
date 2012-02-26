using System;
using System.Collections.Generic;
using System.Linq;

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
                IEnumerable<ConstructorDefinition> constructors = TypeDef.Constructors;
                ConstructorDefinition matchingConstructor = constructors.FirstOrDefault(ConstructorParametersMatchProperties);

                if (matchingConstructor == null)
                    throw new NoMatchingConstructor(TypeDef.Type, properties);

                object[] parameters = matchingConstructor.Parameters
                    .Select(GetParameterPropertyValue)
                    .ToArray();
                return matchingConstructor.Construct(parameters);
            }
        }

        private object GetParameterPropertyValue(ParameterDefinition parameter)
        {
            TypeDefinition typeDef = CurrentTypeHandler.GetTypeDefinition(parameter.Type);
            return typeDef.ConvertToCorrectType(properties.Get(parameter.Name));
        }

        private bool ConstructorParametersMatchProperties(ConstructorDefinition constructor)
        {
            return constructor.Parameters
                .All(CanBeAssignedFromProperty);
        }

        private bool CanBeAssignedFromProperty(ParameterDefinition parameter)
        {
            return HavePropertyValue(parameter)
                ? ParameterTypeMatchesPropertyValue(parameter)
                : ParameterCanBeNull(parameter);
        }

        private bool HavePropertyValue(ParameterDefinition parameter)
        {
            return properties.ContainsKey(parameter.Name)
                && properties[parameter.Name] != null;
        }

        private bool ParameterTypeMatchesPropertyValue(ParameterDefinition parameter)
        {
            Type propertyValueType = properties[parameter.Name].GetType();
            TypeCodeType propertyValueTypeCodeType = propertyValueType.GetTypeCodeType();

            return propertyValueTypeCodeType == TypeCodeType.Object
                ? propertyValueType.CanBeCastTo(parameter.Type)
                : propertyValueTypeCodeType == parameter.Type.GetTypeCodeType();
        }

        private static bool ParameterCanBeNull(ParameterDefinition parameter)
        {
            return parameter.Type.IsByRef;
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
