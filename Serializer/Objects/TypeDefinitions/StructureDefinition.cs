using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace ForSerial.Objects.TypeDefinitions
{
    public abstract class StructureDefinition : TypeDefinition
    {
        protected PropertyDefinition[] AllSerializableProperties;

        private readonly PropertyCollection originalProperties = new PropertyCollection();
        private readonly Dictionary<string, SerializedPropertyCollection> propertiesByScenario = new Dictionary<string, SerializedPropertyCollection>();

        protected StructureDefinition(Type type)
            : base(type)
        {
        }

        private void PopulateProperties()
        {
            if (Type.IsInterface)
                return;

            PropertyDefinitionBuilder propBuilder = new PropertyDefinitionBuilder(ObjectInterfaceProvider);
            IEnumerable<PropertyDefinition> properties = Type.GetProperties(ReflectionHelper.InstanceMembers)
                .Where(NotMarkedWithIgnoreAttribute)
                .Select(propBuilder.Build)
                .Concat(Type.GetFields(ReflectionHelper.InstanceMembers)
                    .Select(propBuilder.Build));

            foreach (PropertyDefinition property in properties)
                if (!originalProperties.Contains(property.Name))
                    originalProperties.Add(property);

            AllSerializableProperties = originalProperties.ToArray();
        }

        private static bool NotMarkedWithIgnoreAttribute(PropertyInfo property)
        {
            object[] attributes = property.GetCustomAttributes(true);
            lock (IgnoreAttributes)
            {
                return attributes.None(a => IgnoreAttributes.Contains(a.GetType()));
            }
        }

        public virtual ObjectContainer CreateStructureForProperty(string name)
        {
            PropertyDefinition property;
            return Properties.TryGetValue(name, out property)
                ? property.CreateStructure()
                : NullObjectStructure.Instance;
        }

        public ObjectContainer CreateStructureForProperty(string name, string typeIdentifier)
        {
            PropertyDefinition property;
            return Properties.TryGetValue(name, out property)
                ? property.CreateStructure(typeIdentifier)
                : NullObjectStructure.Instance;
        }

        public virtual ObjectContainer CreateSequenceForProperty(string name)
        {
            PropertyDefinition property;
            return Properties.TryGetValue(name, out property)
                ? property.CreateSequence()
                : NullObjectStructure.Instance;
        }

        public bool CanCreateValueForProperty(string name, object value)
        {
            PropertyDefinition property;
            return Properties.TryGetValue(name, out property)
                && property.CanCreateValue(value);
        }

        public virtual ObjectOutput CreateValueForProperty(string name, object value)
        {
            PropertyDefinition property;
            return Properties.TryGetValue(name, out property)
                ? property.CreateValue(value)
                : NullObjectValue.Instance;
        }

        private PropertyCollection Properties
        {
            get
            {
                SerializedPropertyCollection properties;
                return SerializationScenario.Current == null ? originalProperties
                    : propertiesByScenario.TryGetValue(SerializationScenario.Current, out properties) ? properties
                    : propertiesByScenario[SerializationScenario.Current] = new SerializedPropertyCollection(originalProperties);
            }
        }

        internal override void Populate()
        {
            base.Populate();
            PopulateProperties();
        }

        protected virtual bool ReferenceStructure(object input, ObjectReader reader, PartialOptions optionsOverride)
        {
            return optionsOverride.MaintainReferences ?? reader.Options.MaintainReferences
                && reader.ReferenceStructure(input);
        }

        protected static bool ShouldWriteTypeIdentifier(ObjectParsingOptions readerOptions, PartialOptions optionsOverride)
        {
            return readerOptions.SerializeTypeInformation == TypeInformationLevel.All
                || readerOptions.SerializeTypeInformation == TypeInformationLevel.Minimal
                    && optionsOverride.SerializeTypeInformation > TypeInformationLevel.None;
        }

        public void AssignValueToProperty(object obj, string propertyName, ObjectOutput value)
        {
            PropertyDefinition propDef;
            if (Properties.TryGetValue(propertyName, out propDef))
                value.AssignToProperty(obj, propDef);
        }

        private class PropertyCollection : KeyedCollection<string, PropertyDefinition>
        {
            protected override string GetKeyForItem(PropertyDefinition item)
            {
                return item.Name;
            }

            public bool TryGetValue(string key, out PropertyDefinition property)
            {
                return Dictionary.TryGetValue(key, out property);
            }
        }

        private class SerializedPropertyCollection : PropertyCollection
        {
            public SerializedPropertyCollection(IEnumerable<PropertyDefinition> properties)
            {
                foreach (PropertyDefinition property in properties.Where(p => !Contains(GetKeyForItem(p))))
                    Add(property);
            }

            protected override string GetKeyForItem(PropertyDefinition item)
            {
                return item.SerializedName;
            }
        }
    }
}