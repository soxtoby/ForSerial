using System;
using System.Collections.Generic;
using System.Linq;

namespace json.Objects
{
    public class DefaultObjectStructure : BaseObjectStructure
    {
        public DefaultObjectStructure(TypeDefinition typeDef) : base(typeDef) { }

        public override void AssignToProperty(object obj, PropertyDefinition property)
        {
            property.SetOn(obj, GetTypedValue());
        }

        public override object GetTypedValue()
        {
            IEnumerable<ConstructorDefinition> constructors = TypeDef.Constructors;
            ConstructorDefinition matchingConstructor = constructors.FirstOrDefault(ConstructorParametersMatchProperties);

            if (matchingConstructor == null)
                throw new NoMatchingConstructor(TypeDef.Type, Properties);

            object[] parameters = matchingConstructor.Parameters
                .Select(GetParameterPropertyValue)
                .ToArray();

            object obj = matchingConstructor.Construct(parameters);

            foreach (KeyValuePair<string, ObjectOutput> property in Properties)
            {
                PropertyDefinition propDef = TypeDef.Properties.Get(property.Key);
                if (propDef != null)
                    property.Value.AssignToProperty(obj, propDef);
            }

            return obj;
        }

        protected object GetParameterPropertyValue(ParameterDefinition parameter)
        {
            TypeDefinition typeDef = CurrentTypeHandler.GetTypeDefinition(parameter.Type);
            return typeDef.ConvertToCorrectType(Properties.Get(parameter.Name).GetTypedValue());
        }

        protected bool ConstructorParametersMatchProperties(ConstructorDefinition constructor)
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
            return Properties.ContainsKey(parameter.Name)
                && Properties[parameter.Name] != null;
        }

        private bool ParameterTypeMatchesPropertyValue(ParameterDefinition parameter)
        {
            Type propertyValueType = Properties[parameter.Name].TypeDef.Type;
            TypeCodeType propertyValueTypeCodeType = propertyValueType.GetTypeCodeType();

            return propertyValueTypeCodeType == TypeCodeType.Object
                ? propertyValueType.CanBeCastTo(parameter.Type)
                : propertyValueTypeCodeType == parameter.Type.GetTypeCodeType();
        }

        private static bool ParameterCanBeNull(ParameterDefinition parameter)
        {
            return parameter.Type.IsByRef;
        }

        internal class NoMatchingConstructor : Exception
        {
            public NoMatchingConstructor(Type type, Dictionary<string, ObjectOutput> properties)
                : base("Could not find a matching constructor for type {0} with properties {1}"
                    .FormatWith(type.FullName, BuildParameterList(properties)))
            {
            }

            private static string BuildParameterList(Dictionary<string, ObjectOutput> properties)
            {
                return properties
                    .Select(kv => "{0} [{1}]".FormatWith(kv.Key, GetTypeName(kv.Value.GetTypedValue())))
                    .Join(", ");
            }

            private static string GetTypeName(object value)
            {
                return value == null ? "null" : value.GetType().FullName;
            }
        }
    }
}