using System;
using System.Collections.Generic;
using System.Reflection;

namespace json.Objects
{
    public class CollectionDefinition : SequenceDefinition
    {
        private readonly MethodInfo adder;

        private CollectionDefinition(Type collectionType, Type itemType, MethodInfo addMethod)
            : base(collectionType, itemType)
        {
            adder = addMethod;
        }

        internal static SequenceDefinition CreateCollectionDefinition(Type type)
        {
            Type itemType = type.GetGenericInterfaceType(typeof(ICollection<>));
            if (itemType != null)
            {
                MethodInfo addMethod = type.GetMethod("Add", new[] { itemType });
                if (addMethod != null)
                {
                    return new CollectionDefinition(type, itemType, addMethod);
                }
            }
            return null;
        }

        public void AddToCollection(object collection, object item)
        {
            if (adder != null)
                adder.Invoke(collection, new[] { ItemTypeDef.ConvertToCorrectType(item) });
        }

        public override TypedSequence CreateSequence()
        {
            return new TypedObjectTypedArray(this);
        }
    }
}
