using System;
using System.Collections.Generic;
using System.Reflection;

namespace json.Objects
{
    public class CollectionDefinition : EnumerableDefinition
    {
        public TypeDefinition ItemTypeDef { get; private set; }
        private readonly MethodInfo adder;

        private CollectionDefinition(Type collectionType, Type itemType, MethodInfo addMethod)
            : base(collectionType)
        {
            ItemTypeDef = CurrentTypeHandler.GetTypeDefinition(itemType);
            adder = addMethod;
        }

        internal static EnumerableDefinition CreateCollectionDefinition(Type type)
        {
            Type itemType = type.GetGenericInterfaceType(typeof(ICollection<>));
            if (itemType != null)
            {
                var addMethod = type.GetMethod("Add", new[] { itemType });
                if (addMethod != null)
                {
                    return new CollectionDefinition(type, itemType, addMethod);
                }
            }
            return CreateEnumerableDefinition(type);
        }

        public void AddToCollection(object collection, object value)
        {
            if (adder != null)
                adder.Invoke(collection, new[] { ItemTypeDef.ConvertToCorrectType(value) });
        }

        public override bool PropertyCanBeSerialized(PropertyDefinition property)
        {
            // Collections can be populated in-place
            return property.CanGet;
        }
    }
}
