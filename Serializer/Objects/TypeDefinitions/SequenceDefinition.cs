using System;
using System.Collections;

namespace json.Objects.TypeDefinitions
{
    public class SequenceDefinition : TypeDefinition
    {
        public TypeDefinition ItemTypeDef { get; private set; }

        protected SequenceDefinition(Type type, Type itemType)
            : base(type)
        {
            ItemTypeDef = CurrentTypeHandler.GetTypeDefinition(itemType);
        }

        public override Output ReadObject(object input, ReaderWriter valueFactory)
        {
            IEnumerable inputArray = input as IEnumerable;
            if (inputArray == null) return null;

            SequenceOutput output = valueFactory.CreateSequence();

            foreach (object item in inputArray)
            {
                valueFactory.ReadArrayItem(output, item);
            }

            return output;
        }
    }
}