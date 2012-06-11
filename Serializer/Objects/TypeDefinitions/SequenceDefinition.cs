using System;
using System.Collections;

namespace ForSerial.Objects.TypeDefinitions
{
    public class SequenceDefinition : TypeDefinition
    {
        protected readonly TypeDefinition ItemTypeDef;

        protected SequenceDefinition(Type type, Type itemType)
            : base(type)
        {
            ItemTypeDef = TypeCache.GetTypeDefinition(itemType);
        }

        public Type ItemType { get { return ItemTypeDef.Type; } }

        public override void Read(object input, ObjectReader reader, Writer writer, PartialOptions optionsOverride)
        {
            IEnumerable inputArray = input as IEnumerable;
            if (inputArray == null) return;

            writer.BeginSequence();

            foreach (object item in inputArray)
                ItemTypeDef.ReadObject(item, reader, writer, optionsOverride);

            writer.EndSequence();
        }

        public ObjectContainer CreateStructureForItem()
        {
            return ItemTypeDef.CreateStructure();
        }

        public ObjectContainer CreateStructureForItem(string typeIdentifier)
        {
            return ItemTypeDef.CreateStructure(typeIdentifier);
        }

        public ObjectContainer CreateSequenceForItem()
        {
            return ItemTypeDef.CreateSequence();
        }

        public ObjectOutput CreateValueForItem(object value)
        {
            return ItemTypeDef.CreateValue(value);
        }

        public bool CanCreateValueForItem(object value)
        {
            return ItemTypeDef.CanCreateValue(value);
        }
    }
}