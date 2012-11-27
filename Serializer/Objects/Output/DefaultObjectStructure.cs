using System;
using System.Collections.Generic;
using System.Linq;
using ForSerial.Objects.TypeDefinitions;

namespace ForSerial.Objects
{
    public class DefaultObjectStructure : BaseObjectStructure
    {
        private object typedValue;
        private ConstructorDefinition chosenConstructor;
        private readonly List<object> constructorArguments = new List<object>();

        public DefaultObjectStructure(StructureDefinition typeDef) : base(typeDef) { }

        public override void AssignToProperty(object obj, PropertyDefinition property)
        {
            property.SetOn(obj, GetTypedValue());
        }

        public override object GetTypedValue()
        {
            try
            {
                return TryGetTypedValue();
            }
            catch (Exception e)
            {
                throw new WriteException(TypeDef.Type.FullName, CurrentProperty, e);
            }
        }

        private object TryGetTypedValue()
        {
            if (typedValue != null)
                return typedValue;

            if (chosenConstructor == null)
            {
                IEnumerable<ConstructorDefinition> constructors = TypeDef.Constructors;
                chosenConstructor = constructors.FirstOrDefault(ConstructorParametersMatchProperties);

                if (chosenConstructor == null)
                    throw new NoMatchingConstructor(TypeDef.Type, Properties);
            }

            PopulateConstructorParameters();

            if (typedValue != null) // If any constructor arguments have references back to this structure, it will already have been constructed while populating their properties
                return typedValue;

            typedValue = chosenConstructor.Construct(constructorArguments.ToArray());

            foreach (KeyValuePair<string, ObjectOutput> property in Properties)
            {
                CurrentProperty = property.Key;
                StructureDef.AssignValueToProperty(typedValue, property.Key, property.Value);
            }

            return typedValue;
        }

        private void PopulateConstructorParameters()
        {
            // Hold onto constructor arguments in case any args have back-references and we attempt to construct this more than once
            for (int i = constructorArguments.Count; i < chosenConstructor.Parameters.Count; i++)
            {
                constructorArguments.Add(GetConstructorParameterPropertyValue(chosenConstructor.Parameters[i]));
            }
        }

        private object GetConstructorParameterPropertyValue(ParameterDefinition parameter)
        {
            string propertyName = AvailablePropertyName(parameter.Name);

            if (propertyName == null)
                return null;

            ObjectOutput property = Properties[propertyName];

            TypeDefinition typeDef = TypeCache.GetTypeDefinition(parameter.Type);
            object propertyValue = typeDef.ConvertToCorrectType(property.GetTypedValue());

            Properties.Remove(propertyName);    // Don't want to re-populate property that's being passed into the constructor

            return propertyValue;
        }

        private bool ConstructorParametersMatchProperties(ConstructorDefinition constructor)
        {
            return constructor.Parameters
                .All(CanBeAssignedFromProperty);
        }

        private bool CanBeAssignedFromProperty(ParameterDefinition parameter)
        {
            return HasPropertyValue(parameter)
                ? ParameterTypeMatchesPropertyValue(parameter)
                : ParameterCanBeNull(parameter);
        }

        private bool HasPropertyValue(ParameterDefinition parameter)
        {
            ObjectOutput value = GetProperty(parameter.Name);
            return value != null
                && value.TypeDef != NullTypeDefinition.Instance;
        }

        private bool ParameterTypeMatchesPropertyValue(ParameterDefinition parameter)
        {
            ObjectOutput property = GetProperty(parameter.Name);
            Type propertyValueType = property.TypeDef.Type;
            TypeCodeType propertyValueTypeCodeType = propertyValueType.GetTypeCodeType();

            return propertyValueTypeCodeType == TypeCodeType.Object
                ? propertyValueType.CanBeCastTo(parameter.Type)
                : propertyValueTypeCodeType == parameter.Type.GetTypeCodeType();
        }

        private static bool ParameterCanBeNull(ParameterDefinition parameter)
        {
            return !parameter.Type.IsValueType;
        }

        private ObjectOutput GetProperty(string propertyName)
        {
            string availablePropertyName = AvailablePropertyName(propertyName);
            return availablePropertyName == null ? null
                : Properties[availablePropertyName];
        }

        private string AvailablePropertyName(string propertyName)
        {
            return Properties.ContainsKey(propertyName) ? propertyName
                : Properties.ContainsKey("_" + propertyName) ? "_" + propertyName
                : null;
        }

        private class NoMatchingConstructor : Exception
        {
            public NoMatchingConstructor(Type type, Dictionary<string, ObjectOutput> properties)
                : base("Could not find a matching constructor for type {0} with {1}"
                    .FormatWith(type.FullName, BuildParameterList(properties)))
            {
            }

            private static string BuildParameterList(Dictionary<string, ObjectOutput> properties)
            {
                return properties.None() ? "no properties "
                    : properties
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