using System;
using System.Collections.Generic;
using System.Reflection;

namespace json.Objects
{
    public class CollectionDefinition
    {
        public Type ItemType { get; private set; }
        private readonly TypeCode itemTypeCode;
        private readonly MethodInfo adder;

        public bool IsCollection { get { return adder != null; } }

        private CollectionDefinition(Type collectionType)
        {
            ItemType = collectionType.GetGenericInterfaceType(typeof(ICollection<>));

            if (ItemType != null)
            {
                itemTypeCode = Type.GetTypeCode(ItemType);
                adder = collectionType.GetMethod("Add", new[] { ItemType });
            }
        }

        public void AddToCollection(object collection, object value)
        {
            if (adder != null)
            {
                if (itemTypeCode != TypeCode.Object)
                    value = Convert.ChangeType(value, itemTypeCode);

                adder.Invoke(collection, new[] { value });
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
