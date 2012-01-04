using System;
using System.Collections;

namespace json.Objects
{
    public class SequenceDefinition : TypeDefinition
    {
        public TypeDefinition ItemTypeDef { get; private set; }

        protected SequenceDefinition(Type type, Type itemType)
            : base(type)
        {
            ItemTypeDef = CurrentTypeHandler.GetTypeDefinition(itemType);
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