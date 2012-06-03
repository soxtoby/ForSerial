using System;
using System.Collections.Generic;
using System.Linq;
using ForSerial.Objects.TypeDefinitions;

namespace ForSerial.Objects
{
    public class DefaultObjectStructure : BaseObjectStructure
    {
        private object typedValue;
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

            IEnumerable<ConstructorDefinition> constructors = TypeDef.Constructors;
            ConstructorDefinition matchingConstructor = constructors.FirstOrDefault(ConstructorParametersMatchProperties);

            if (matchingConstructor == null)
                throw new NoMatchingConstructor(TypeDef.Type, Properties);

            PopulateConstructorParameters(matchingConstructor);

            if (typedValue != null) // If any constructor arguments have references back to this structure, it will already have been constructed while populating their properties
                return typedValue;

            typedValue = matchingConstructor.Construct(constructorArguments.ToArray());

            foreach (KeyValuePair<string, ObjectOutput> property in Properties)
            {
                CurrentProperty = property.Key;
                PropertyDefinition propDef;
                if (StructureDef.Properties.TryGetValue(property.Key, out propDef))
                    property.Value.AssignToProperty(typedValue, propDef);
            }

            return typedValue;
        }

        private void PopulateConstructorParameters(ConstructorDefinition constructor)
        {
            // Hold onto constructor arguments in case any args have back-references and we attempt to construct this more than once
            for (int i = constructorArguments.Count; i < constructor.Parameters.Count; i++)
            {
                constructorArguments.Add(GetParameterPropertyValue(constructor.Parameters[i]));
            }
        }

        private object GetParameterPropertyValue(ParameterDefinition parameter)
        {
            TypeDefinition typeDef = TypeCache.GetTypeDefinition(parameter.Type);
            return typeDef.ConvertToCorrectType(Properties.Get(parameter.Name).GetTypedValue());
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
            return Properties.ContainsKey(parameter.Name)
                && Properties[parameter.Name] != null
                && Properties[parameter.Name].TypeDef != NullTypeDefinition.Instance;
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
            return !parameter.Type.IsValueType;
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