using System;
using System.Collections;

namespace json.Objects.TypeDefinitions
{
    public class SequenceDefinition : TypeDefinition
    {
        protected readonly TypeDefinition ItemTypeDef;

        protected SequenceDefinition(Type type, Type itemType)
            : base(type)
        {
            ItemTypeDef = CurrentTypeHandler.GetTypeDefinition(itemType);
        }

        public Type ItemType { get { return ItemTypeDef.Type; } }

        public override void Read(object input, ObjectReader reader, Writer writer, bool requestTypeIdentification)
        {
            IEnumerable inputArray = input as IEnumerable;
            if (inputArray == null) return;

            writer.BeginSequence();

            foreach (object item in inputArray)
                ItemTypeDef.ReadObject(item, reader, writer, requestTypeIdentification);

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