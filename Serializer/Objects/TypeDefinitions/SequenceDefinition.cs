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

        public override void ReadObject(object input, ObjectReader reader, Writer writer)
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

        public ObjectContainer CreateStructureForItem()
        {
            return ItemTypeDef.CreateStructure();
        }

        public ObjectContainer CreateSequenceForItem()
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