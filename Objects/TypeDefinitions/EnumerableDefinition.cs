using System;
using System.Collections;

namespace json.Objects
{
    public class EnumerableDefinition : TypeDefinition
    {
        protected EnumerableDefinition(Type type) : base(type) { }

        protected static EnumerableDefinition CreateEnumerableDefinition(Type type)
        {
            return type.CanBeCastTo(typeof(IEnumerable))
                ? new EnumerableDefinition(type)
                : null;
        }

        public override bool PropertyCanBeSerialized(PropertyDefinition property)
        {
            // No way to deserialize a plain IEnumerable
            return false;
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
    }
}