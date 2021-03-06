using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ForSerial.Objects.TypeDefinitions
{
    public class CollectionDefinition : SequenceDefinition
    {
        private readonly ActionMethod adder;

        protected CollectionDefinition(Type collectionType, Type itemType, ActionMethod addMethod)
            : base(collectionType, itemType)
        {
            adder = addMethod;
        }

        internal static SequenceDefinition CreateCollectionDefinition(Type type)
        {
            // IEnumerable<T> with Add(T) method
            Type itemType = type.GetGenericInterfaceType(typeof(IEnumerable<>));
            if (itemType != null)
            {
                MethodInfo addMethod = type.GetMethod("Add", new[] { itemType });
                if (addMethod != null)
                {
                    return new CollectionDefinition(type, itemType, ObjectInterfaceProvider.GetAction(addMethod));
                }
            }

            // IEumerable with Add(object) method
            if (type.CanBeCastTo(typeof(IEnumerable)))
            {
                MethodInfo addMethod = type.GetMethod("Add", new[] { typeof(object) });
                if (addMethod != null)
                {
                    return new CollectionDefinition(type, typeof(object), ObjectInterfaceProvider.GetAction(addMethod));
                }
            }

            return null;
        }

        public bool CanAdd
        {
            get { return adder != null; }
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
            return ConstructDefault();
        }
    }
}
