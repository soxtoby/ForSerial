using System;
using System.Collections;
using System.Collections.Generic;

namespace json.Objects
{
    public class EnumerableDefinition : TypeDefinition
    {
        public TypeDefinition ItemTypeDef { get; private set; }

        protected EnumerableDefinition(Type type, Type itemType)
            : base(type)
        {
            ItemTypeDef = CurrentTypeHandler.GetTypeDefinition(itemType);
        }

        protected static EnumerableDefinition CreateEnumerableDefinition(Type type)
        {
            return type.CanBeCastTo(typeof(IEnumerable))
                ? new EnumerableDefinition(type, GetIEnumerableItemType(type))
                : null;
        }

        private static Type GetIEnumerableItemType(Type type)
        {
            return type.GetGenericInterfaceType(typeof(IEnumerable<>)) ?? typeof(object);
        }

        public override bool IsSerializable
        {
            get { return true; }// TODO this sort of thing should really be in a derived type
        }

        public override ParseValue ParseObject(object input, ParserValueFactory valueFactory)
        {
            IEnumerable inputArray = input as IEnumerable;
            if (inputArray == null) return null;

            ParseArray output = valueFactory.CreateArray();

            foreach (object item in inputArray)
            {
                valueFactory.ParseArrayItem(output, item);
            }

            return output;
        }

        public override TypedObjectArray CreateArray()
        {
            return new TypedObjectEnumerable(ItemTypeDef);
        }
    }
}