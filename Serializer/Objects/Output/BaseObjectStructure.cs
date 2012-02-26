using System;
using System.Collections.Generic;
using System.Linq;

namespace json.Objects
{
    public abstract class BaseObjectStructure : ObjectStructure
    {
        protected readonly Dictionary<string, TypedValue> Properties = new Dictionary<string, TypedValue>(StringComparer.OrdinalIgnoreCase);

        protected BaseObjectStructure(TypeDefinition typeDef)
        {
            TypeDef = typeDef;
        }

        public TypeDefinition TypeDef { get; private set; }
        public abstract void AssignToProperty(object obj, PropertyDefinition property);

        public ObjectStructure CreateStructure(string property)
        {
            return TypeDef.CreateStructureForProperty(property);
        }

        public ObjectSequence CreateSequence(string property)
        {
            return TypeDef.CreateSequenceForProperty(property);
        }

        public bool CanCreateValue(string property, object value)
        {
            return TypeDef.CanCreateValueForProperty(property, value);
        }

        public ObjectValue CreateValue(string property, object value)
        {
            return TypeDef.CreateValueForProperty(property, value);
        }

        public void Add(string property, TypedValue value)
        {
            Properties[property] = value;
        }

        public abstract object GetTypedValue();

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
            public NoMatchingConstructor(Type type, Dictionary<string, TypedValue> properties)
                : base("Could not find a matching constructor for type {0} with properties {1}"
                    .FormatWith(type.FullName, BuildParameterList(properties)))
            {
            }

            private static string BuildParameterList(Dictionary<string, TypedValue> properties)
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