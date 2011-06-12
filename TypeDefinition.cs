using System;
using System.Collections.Generic;
using System.Linq;

namespace json
{
    public class TypeDefinition
    {
        public Type Type { get; private set; }
        public IDictionary<string, PropertyDefinition> Properties { get; private set; }

        private TypeDefinition(Type type)
        {
            Type = type;
            Properties = new Dictionary<string, PropertyDefinition>();
            PopulateProperties();
        }

        // FIXME Make this thread-safe - use a ConcurrentDictionary. This version of Mono doesn't appear to have it :(
        private static Dictionary<string, TypeDefinition> knownTypes = new Dictionary<string, TypeDefinition>();

        public static TypeDefinition GetTypeDefinition(Type type)
        {
            if (!knownTypes.ContainsKey(type.AssemblyQualifiedName))
                knownTypes[type.AssemblyQualifiedName] = new TypeDefinition(type);

            return knownTypes[type.AssemblyQualifiedName];
        }

        public static TypeDefinition GetTypeDefinition(string assemblyQualifiedName)
        {
            if (!knownTypes.ContainsKey(assemblyQualifiedName))
                knownTypes[assemblyQualifiedName] = new TypeDefinition(Type.GetType(assemblyQualifiedName));
            return knownTypes[assemblyQualifiedName];
        }

        private void PopulateProperties()
        {
            IEnumerable<PropertyDefinition> properties =
                Type.GetProperties().Select(p => new PropertyDefinition(p));

            foreach (PropertyDefinition property in properties.Where(p => p.IsSerializable))
            {
                Properties[property.Name] = property;
            }
        }
    }
}

