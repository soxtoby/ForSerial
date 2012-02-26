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

        public override void ReadObject(object input, ObjectReader reader, NewWriter writer)
        {
            IEnumerable inputArray = input as IEnumerable;
            if (inputArray == null) return;

            writer.BeginSequence();

            foreach (object item in inputArray)
            {
                reader.Read(item);
            }

            writer.EndSequence();
        }

        public ObjectStructure CreateStructureForItem()
        {
            return ItemTypeDef.CreateStructure();
        }

        public ObjectSequence CreateSequenceForItem()
        {
            return ItemTypeDef.CreateSequence();
        }

        public ObjectValue CreateValueForItem(object value)
        {
            return ItemTypeDef.CreateValue(value);
        }

        public bool CanCreateValueForItem(object value)
        {
            return ItemTypeDef.CanCreateValue(value);
        }
    }
}