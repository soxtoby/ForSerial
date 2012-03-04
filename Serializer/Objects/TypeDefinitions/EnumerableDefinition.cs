using System;
using System.Collections;
using System.Collections.Generic;

namespace json.Objects.TypeDefinitions
{
    public class EnumerableDefinition : SequenceDefinition
    {
        private EnumerableDefinition(Type type, Type itemType) : base(type, itemType) { }

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
            return new EnumerableSequence(this);
        }
    }
}