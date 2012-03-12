using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace json.Objects
{
    public abstract class StructureDefinition : TypeDefinition
    {
        public PropertyCollection Properties { get; private set; }// TODO use a KeyedCollection when moving out of TypeDefinition
        protected PropertyDefinition[] SerializableProperties;

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
            IEnumerable<PropertyDefinition> properties = Type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                .Where(NotMarkedWithIgnoreAttribute)
                .Select(propBuilder.Build);

            foreach (PropertyDefinition property in properties)
                Properties.Add(property);

            SerializableProperties = new PropertyDefinition[Properties.Count];
            for (int i = 0; i < SerializableProperties.Length; i++)
                SerializableProperties[i] = Properties[i];
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

        public virtual ObjectValue CreateValueForProperty(string name, object value)
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