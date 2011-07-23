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

        public override ParseValue GetParseValue(ParseValueFactory valueFactory)
        {
            return valueFactory.CreateArray();
        }

        public override void ParseObject(object input, ParseValue output, ParserValueFactory valueFactory)
        {
            IEnumerable inputArray = input as IEnumerable;
            ParseArray outputArray = output as ParseArray;
            if (inputArray == null || outputArray == null) return;

            foreach (object item in inputArray)
            {
                ParseValue value = valueFactory.Parse(item);
                value.AddToArray(outputArray);
            }
        }
    }
}