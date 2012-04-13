using System;
using System.Collections;
using System.Collections.Generic;

namespace ForSerial.Objects.TypeDefinitions
{
    public class EnumerableDefinition : SequenceDefinition
    {
        private readonly Func<ObjectContainer> sequenceCreate;

        private EnumerableDefinition(Type type, Type itemType)
            : base(type, itemType)
        {
            sequenceCreate = GetGenericListType().CanBeCastTo(type)
                ? (Func<ObjectContainer>)CreateEnumerableSequence
                : CreateNullSequence;
        }

        public Type GetGenericListType()
        {
            return typeof(List<>).MakeGenericType(ItemType);
        }

        internal static SequenceDefinition CreateEnumerableDefinition(Type type)
        {
            return type.CanBeCastTo(typeof(IEnumerable))
                       ? new EnumerableDefinition(type, GetIEnumerableItemType(type))
                       : null;
        }

        private static Type GetIEnumerableItemType(Type type)
        {
            return type.GetGenericInterfaceType(typeof(IEnumerable<>)) ?? typeof(object);
        }

        public override ObjectContainer CreateSequence()
        {
            return sequenceCreate();
        }

        private ObjectContainer CreateEnumerableSequence()
        {
            return new EnumerableSequence(this);
        }

        private static ObjectContainer CreateNullSequence()
        {
            return NullObjectSequence.Instance;
        }
    }
}