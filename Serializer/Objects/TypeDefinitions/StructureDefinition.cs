using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace ForSerial.Objects.TypeDefinitions
{
    public abstract class StructureDefinition : TypeDefinition
    {
        private const BindingFlags PublicInstanceMembers = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;

        public PropertyCollection Properties { get; private set; }
        protected PropertyDefinition[] AllSerializableProperties;

        protected StructureDefinition(Type type)
            : base(type)
        {
            Properties = new PropertyCollection();
        }

        private void PopulateProperties()
        {
            if (Type.IsInterface)
                return;

            PropertyDefinitionBuilder propBuilder = new PropertyDefinitionBuilder(ObjectInterfaceProvider);
            IEnumerable<PropertyDefinition> properties = Type.GetProperties(PublicInstanceMembers)
                .Where(NotMarkedWithIgnoreAttribute)
                .Select(propBuilder.Build)
                .Concat(Type.GetFields(PublicInstanceMembers)
                    .Select(propBuilder.Build));

            foreach (PropertyDefinition property in properties)
                Properties.Add(property);

            AllSerializableProperties = Properties.ToArray();
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
    }

    public class PropertyCollection : KeyedCollection<string, PropertyDefinition>
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

}