using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace json.Objects.TypeDefinitions
{
    public class CollectionDefinition : SequenceDefinition
    {
        private readonly ActionMethod adder;

        private CollectionDefinition(Type collectionType, Type itemType, ActionMethod addMethod)
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
                    return new CollectionDefinition(type, itemType, ObjectInterfaceProvider.GetAction(addMethod));
                }
            }
            return null;
        }

        public override ObjectContainer CreateSequence()
        {
            return new CollectionSequence(this);
        }

        public void AddToCollection(object collection, object item)
        {
            if (adder != null)
                adder(collection, new[] { ItemTypeDef.ConvertToCorrectType(item) });
        }

        public object ConstructNew()
        {
            var defaultConstructor = Constructors.FirstOrDefault(c => c.Parameters.None());
            return defaultConstructor == null ? null
                : defaultConstructor.Construct(new object[] { });
        }
    }
}
