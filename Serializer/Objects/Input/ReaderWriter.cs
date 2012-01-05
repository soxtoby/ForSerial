namespace json.Objects
{
    public interface ReaderWriter : Writer
    {
        bool SerializeAllTypes { get; }
        void ReadProperty(object source, PropertyDefinition property, OutputStructure target);
        void ReadProperty(TypeDefinition propertyTypeDef, string propertyName, object propertyValue, OutputStructure target);
        void ReadArrayItem(SequenceOutput array, object item);

        /// <summary>
        /// Creates an OutputStructure, sets its type to the type of the input, and adds to the collection of known references.
        /// </summary>
        OutputStructure CreateStructure(object input);
    }
}