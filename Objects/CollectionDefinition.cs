using System;
using System.Collections.Generic;
using System.Reflection;

namespace json.Objects
{
    public class CollectionDefinition
    {
        public TypeDefinition ItemTypeDef { get; private set; }
        private readonly MethodInfo adder;

        public bool IsCollection { get { return adder != null; } }

        private CollectionDefinition(Type collectionType)
        {
            ItemTypeDef = TypeDefinition.GetTypeDefinition(collectionType.GetGenericInterfaceType(typeof(ICollection<>)));

            if (ItemTypeDef != null)
            {
                adder = collectionType.GetMethod("Add", new[] { ItemTypeDef.Type });
            }
        }

        public void AddToCollection(object collection, object value)
        {
            if (adder != null)
            {
                adder.Invoke(collection, new[] { ItemTypeDef.ConvertToCorrectType(value) });
            }
        }

        // FIXME use a ConcurrentDictionary
        private static readonly Dictionary<string, CollectionDefinition> knownCollections = new Dictionary<string, CollectionDefinition>();

        public static CollectionDefinition GetCollectionDefinition(Type collectionType)
        {
            if (!knownCollections.ContainsKey(collectionType.AssemblyQualifiedName))
            {
                knownCollections[collectionType.AssemblyQualifiedName] = new CollectionDefinition(collectionType);
            }
            return knownCollections[collectionType.AssemblyQualifiedName];
        }
    }
}
