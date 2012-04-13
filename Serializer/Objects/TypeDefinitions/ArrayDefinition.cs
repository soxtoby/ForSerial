using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ForSerial.Objects.TypeDefinitions
{
    public class ArrayDefinition : SequenceDefinition
    {
        private readonly Func<IEnumerable<object>, Array> buildArray;

        protected ArrayDefinition(Type type, Type itemType)
            : base(type, itemType)
        {
            buildArray = GetTypedArrayBuilderFunction(itemType);
        }

        private static Func<IEnumerable<object>, Array> GetTypedArrayBuilderFunction(Type itemType)
        {
            Type arrayBuilder = typeof(ArrayBuilder<>).MakeGenericType(itemType);
            MethodInfo buildArrayMethod = arrayBuilder.GetMethod("BuildArray", BindingFlags.Static | BindingFlags.Public);
            return (Func<IEnumerable<object>, Array>)Delegate.CreateDelegate(typeof(Func<IEnumerable<object>, Array>), buildArrayMethod);
        }

        internal static ArrayDefinition CreateArrayDefinition(Type type)
        {
            return type.IsArray
                ? new ArrayDefinition(type, type.GetElementType())
                : null;
        }

        public override ObjectContainer CreateSequence()
        {
            return new ArraySequence(this);
        }

        public object BuildArray(IEnumerable<object> items)
        {
            return buildArray(items);
        }

        private static class ArrayBuilder<T>
        {
            public static T[] BuildArray(IEnumerable<object> items)
            {
                return items.Cast<T>().ToArray();
            }
        }
    }
}