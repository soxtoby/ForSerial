
using System.Collections.Generic;

namespace json.Objects
{
    public partial class ObjectReader
    {
        private class ObjectReaderWriter : ReaderWriter
        {
            private readonly ObjectReader reader;
            private readonly bool setType;
            private readonly Stack<Output> outputs = new Stack<Output>();

            public ObjectReaderWriter(ObjectReader reader, bool setType = true)
            {
                this.reader = reader;
                this.setType = setType;
            }

            public bool SerializeAllTypes
            {
                get { return reader.options.SerializeAllTypes; }
            }

            public void ReadProperty(object source, PropertyDefinition property, OutputStructure target)
            {
                bool serializeTypeIdentifier = !property.TypeDef.IsSerializable || property.ForceTypeIdentifierSerialization;
                ReadProperty(target, property.Name, property.GetFrom(source), serializeTypeIdentifier);
            }

            public void ReadProperty(TypeDefinition propertyTypeDef, string propertyName, object propertyValue, OutputStructure target)
            {
                bool serializeTypeIdentifier = !propertyTypeDef.IsSerializable;
                ReadProperty(target, propertyName, propertyValue, serializeTypeIdentifier);
            }

            private void ReadProperty(OutputStructure target, string propertyName, object propertyValue, bool serializeTypeIdentifier)
            {
                using (reader.UseObjectPropertyContext(target, propertyName))
                using (reader.readerWriter.OverrideState(new ObjectReaderWriter(reader, serializeTypeIdentifier)))
                {
                    Output value = reader.ReadValue(propertyValue);
                    value.AddToStructure(target, propertyName);
                }
            }

            public void ReadArrayItem(SequenceOutput array, object item)
            {
                using (reader.UseArrayContext(array))
                {
                    Output value = reader.ReadValue(item);
                    value.AddToSequence(array);
                }
            }

            public OutputStructure CreateStructure(object input)
            {
                OutputStructure obj = reader.writer.Current.BeginStructure();
                if (reader.options.SerializeAllTypeInformation || setType)
                    obj.SetType(CurrentTypeHandler.GetTypeIdentifier(input.GetType()), reader);
                reader.objectReferences[input] = obj;
                return obj;
            }

            public Output CreateValue(object value)
            {
                return reader.writer.Current.CreateValue(value);
            }

            public OutputStructure BeginStructure()
            {
                return reader.writer.Current.BeginStructure();
            }

            public SequenceOutput BeginSequence()
            {
                return reader.writer.Current.BeginSequence();
            }

            public OutputStructure CreateReference(OutputStructure outputStructure)
            {
                return reader.writer.Current.CreateReference(outputStructure);
            }

            public void EndStructure()
            {
                reader.writer.Current.EndStructure();
            }

            public void EndSequence()
            {
                reader.writer.Current.EndSequence();
            }
        }
    }
}